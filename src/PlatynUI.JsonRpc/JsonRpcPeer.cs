using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.Threading;

namespace PlatynUI.JsonRpc;

public enum JsonRpcErrorCode
{
    ParseError = -32700,
    InvalidRequest = -32600,
    MethodNotFound = -32601,
    InvalidParams = -32602,
    InternalError = -32603,
}

public abstract record JsonRpcMessage
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; init; } = "2.0";
}

public record JsonRpcRequest : JsonRpcMessage
{
    [JsonPropertyName("method")]
    public required string Method { get; init; }

    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }

    [JsonPropertyName("id")]
    public required JsonElement? Id { get; set; }
}

public record JsonRpcNotification : JsonRpcMessage
{
    [JsonPropertyName("method")]
    public required string Method { get; init; }

    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }
}

public abstract record JsonResponseBase : JsonRpcMessage { }

public record JsonRpcResponse : JsonResponseBase
{
    [JsonPropertyName("result")]
    public object? Result { get; set; }

    [JsonPropertyName("id")]
    public required JsonElement Id { get; set; }
}

public class JsonRpcError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}

public record JsonRpcErrorResponse : JsonResponseBase
{
    [JsonPropertyName("error")]
    public required JsonRpcError Error { get; set; }

    [JsonPropertyName("id")]
    public required JsonElement? Id { get; set; }
}

public class JsonRpcPeer(Stream readerStream, Stream writerStream)
{
    public readonly Stream ReaderStream = readerStream;
    public readonly Stream WriterStream = writerStream;

    public JoinableTaskFactory? JoinableTaskFactory { get; set; }

    private readonly ConcurrentDictionary<
        string,
        Func<JsonElement?, JsonSerializerOptions, Task<object?>>
    > requestHandlers = new();
    private readonly ConcurrentDictionary<
        string,
        Func<JsonElement?, JsonSerializerOptions, Task>
    > notificationHandlers = new();

    private readonly ConcurrentDictionary<string, TaskCompletionSource<JsonRpcResponse>> pendingRequests = new();
    private int idCounter = 1;
    private Encoding encoding = Encoding.UTF8;
    private Task? messageLoopTask;
    private CancellationTokenSource? cancellationTokenSource;

    public JsonSerializerOptions JsonSerializerOptions { get; set; } =
        new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    private readonly TaskCompletionSource<bool> completionSource = new();

    public Task<bool> Completion
    {
        get { return completionSource.Task; }
    }

    public void Start()
    {
        if (messageLoopTask == null)
        {
            cancellationTokenSource = new CancellationTokenSource();
            messageLoopTask = Task.Run(() => MessageLoopAsync(cancellationTokenSource.Token));
        }
    }

    public void Stop()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = null;
            messageLoopTask = null;
        }
    }

    public void RegisterRequestHandler<T>(string methodName, Func<JsonElement?, JsonSerializerOptions, Task<T>> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

        if (requestHandlers.ContainsKey(methodName))
        {
            throw new ArgumentException(
                $"Request handler for method '{methodName}' already exists",
                nameof(methodName)
            );
        }
        requestHandlers[methodName] = async (parameters, options) => await handler(parameters, options);
    }

    public void RegisterNotificationHandler(string methodName, Func<JsonElement?, JsonSerializerOptions, Task> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);
        if (notificationHandlers.ContainsKey(methodName))
        {
            throw new ArgumentException(
                $"Notification handler for method '{methodName}' already exists",
                nameof(methodName)
            );
        }
        notificationHandlers[methodName] = handler;
    }

    private async Task SendMessageAsync(string json)
    {
        var bodyBytes = Encoding.UTF8.GetBytes(json + "\r\n");
        var header = "Content-Length: " + bodyBytes.Length + "\r\n\r\n";
        var headerBytes = Encoding.ASCII.GetBytes(header);
        await WriterStream.WriteAsync(headerBytes);
        await WriterStream.WriteAsync(bodyBytes);
        await WriterStream.FlushAsync();
    }

    public void SendNotification(string method, object @params)
    {
        if (JoinableTaskFactory != null)
        {
            JoinableTaskFactory.Run(async () => await SendNotificationAsync(method, @params));
        }
        else
        {
            Task.Run(async () => await SendNotificationAsync(method, @params)).GetAwaiter().GetResult();
        }
    }

    public async Task SendNotificationAsync(string method, object @params)
    {
        var message = new JsonRpcNotification
        {
            Method = method,
            Params = @params != null ? JsonSerializer.SerializeToElement(@params, JsonSerializerOptions) : null,
        };
        var json = JsonSerializer.Serialize(message, JsonSerializerOptions);
        await SendMessageAsync(json);
    }

    public void SendRequest(string method)
    {
        SendRequest(method, null);
    }

    public void SendRequest(string method, object? @params)
    {
        SendRequest<object>(method, @params);
    }

    public T SendRequest<T>(string method)
    {
        return SendRequest<T>(method, null);
    }

    public T SendRequest<T>(string method, object? @params)
    {
        if (JoinableTaskFactory != null)
        {
            return JoinableTaskFactory.Run(async () => await SendRequestAsync<T>(method, @params));
        }
        else
        {
            return Task.Run(async () => await SendRequestAsync<T>(method, @params)).GetAwaiter().GetResult();
        }
    }

    public async Task SendRequestAsync(string method)
    {
        await SendRequestAsync<object>(method, null);
    }

    public async Task SendRequestAsync(string method, object? @params)
    {
        await SendRequestAsync<object>(method, @params);
    }

    public async Task<T> SendRequestAsync<T>(string method)
    {
        return await SendRequestAsync<T>(method, null);
    }

    public async Task<T> SendRequestAsync<T>(string method, object? @params)
    {
        var id = Interlocked.Increment(ref idCounter).ToString();
        var message = new JsonRpcRequest
        {
            Method = method,
            Params = @params != null ? JsonSerializer.SerializeToElement(@params, JsonSerializerOptions) : null,
            Id = JsonSerializer.SerializeToElement(id),
        };
        var json = JsonSerializer.Serialize(message, JsonSerializerOptions);

        var tcs = new TaskCompletionSource<JsonRpcResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
        pendingRequests[id] = tcs;

        await SendMessageAsync(json);

        var response = await tcs.Task;

        if (response.Result is JsonElement jsonElement)
        {
            T? result = jsonElement.Deserialize<T>(JsonSerializerOptions);

            if (result == null && !IsNullable(typeof(T)))
            {
                throw new InvalidOperationException(
                    $"Deserialization resulted in null for non-nullable type {typeof(T).FullName}"
                );
            }

            return result!;
        }

        return (T)default!;
    }

    private static bool IsNullable(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null || !type.IsValueType;
    }

    protected async Task<string?> ReadMessageAsync()
    {
        string? line;
        int contentLength = 0;
        encoding = Encoding.UTF8;

        while (!string.IsNullOrEmpty(line = await ReadLineAsync(ReaderStream, cancellationTokenSource!.Token)))
        {
            if (line.StartsWith("Content-Length: "))
                contentLength = int.Parse(line["Content-Length: ".Length..]);
            else if (line.StartsWith("Content-Type: "))
            {
                var contentType = line["Content-Type: ".Length..];
                var charsetToken = contentType
                    .Split(';')
                    .FirstOrDefault(s => s.Trim().StartsWith("charset=", StringComparison.OrdinalIgnoreCase));
                var charset = charsetToken?.Split('=')[1].Trim();
                encoding = charset?.ToLower() switch
                {
                    "utf-8" => Encoding.UTF8,
                    "ascii" => Encoding.ASCII,
                    "utf-16" => Encoding.Unicode,
                    _ => Encoding.UTF8,
                };
            }
        }

        if (contentLength > 0)
        {
            var buffer = new byte[contentLength];
            await ReaderStream.ReadExactlyAsync(buffer.AsMemory(0, contentLength));
            return encoding.GetString(buffer);
        }

        return null;
    }

    protected static async Task<string?> ReadLineAsync(Stream stream, CancellationToken ca = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (ca.IsCancellationRequested)
            return null;

        var sb = new StringBuilder();
        var buffer = new byte[1];
        int prev = 0;

        while (await stream.ReadAsync(buffer.AsMemory(0, 1), ca) > 0)
        {
            int curr = buffer[0];
            if (prev == '\r' && curr == '\n')
                break;
            if (prev != 0)
                sb.Append((char)prev);
            prev = curr;
        }

        return sb.Length > 0 ? sb.ToString() : null;
    }

    protected async Task MessageLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var json = await ReadMessageAsync();
                if (json == null)
                    break;

                await HandleIncomingMessageAsync(json);
            }
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            completionSource.TrySetException(e);
            return;
        }

        completionSource.TrySetResult(!cancellationToken.IsCancellationRequested);
    }

    protected async Task HandleIncomingMessageAsync(string json)
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json, JsonSerializerOptions);

            if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                var responses = new List<object>();

                foreach (var element in jsonElement.EnumerateArray())
                {
                    var response = await ProcessSingleMessage(element);
                    if (response != null)
                    {
                        responses.Add(response);
                    }
                }

                if (responses.Count > 0)
                {
                    var batchResponse = JsonSerializer.Serialize<object?>(responses, JsonSerializerOptions);
                    await SendMessageAsync(batchResponse);
                }
            }
            else
            {
                var response = await ProcessSingleMessage(jsonElement);
                if (response != null)
                {
                    var singleResponse = JsonSerializer.Serialize<object?>(response, JsonSerializerOptions);
                    await SendMessageAsync(singleResponse);
                }
            }
        }
        catch (JsonException ex)
        {
            await SendMessageAsync(
                JsonSerializer.Serialize(
                    new JsonRpcErrorResponse
                    {
                        Error = new JsonRpcError
                        {
                            Code = (int)JsonRpcErrorCode.ParseError,
                            Message = ex.Message,
                            Data = ex.StackTrace,
                        },
                        Id = null,
                    },
                    JsonSerializerOptions
                )
            );
        }
        catch (Exception ex)
        {
            await SendMessageAsync(
                JsonSerializer.Serialize(
                    new JsonRpcErrorResponse
                    {
                        Error = new JsonRpcError
                        {
                            Code = (int)JsonRpcErrorCode.InvalidRequest,
                            Message = ex.Message,
                            Data = ex.StackTrace,
                        },
                        Id = null,
                    },
                    JsonSerializerOptions
                )
            );
        }
    }

    protected async Task<JsonResponseBase?> ProcessSingleMessage(JsonElement jsonElement)
    {
        if (jsonElement.TryGetProperty("id", out var idProperty) && !jsonElement.TryGetProperty("method", out _))
        {
            if (
                idProperty.ValueKind != JsonValueKind.Null
                && pendingRequests.TryRemove(idProperty.ToString(), out var tcs)
            )
            {
                if (jsonElement.TryGetProperty("error", out var errorProperty))
                {
                    var error = errorProperty.Deserialize<JsonRpcError>(JsonSerializerOptions);
                    tcs.SetException(new Exception($"RPC Error {error?.Code}: {error?.Message} ({error?.Data})"));
                }
                else if (jsonElement.TryGetProperty("result", out var resultProperty))
                {
                    var response = new JsonRpcResponse { Id = idProperty, Result = resultProperty };
                    tcs.SetResult(response);
                }
                else
                {
                    tcs.SetException(new Exception("RPC Error: Invalid response format"));
                }
                return null;
            }
            else if (
                idProperty.ValueKind == JsonValueKind.Null
                && jsonElement.TryGetProperty("error", out var errorProperty)
            )
            {
                var error = errorProperty.Deserialize<JsonRpcError>(JsonSerializerOptions);
                Console.Error.WriteLine(
                    $"Received error with null ID: {error?.Code}: {error?.Message} ({error?.Data})"
                );

                return null;
            }
        }
        else if (jsonElement.TryGetProperty("method", out var methodProperty))
        {
            if (
                jsonElement.TryGetProperty("id", out var requestIdProperty)
                && requestIdProperty.ValueKind != JsonValueKind.Null
            )
            {
                var message =
                    jsonElement.Deserialize<JsonRpcRequest>(JsonSerializerOptions)
                    ?? throw new JsonException("Invalid request format");
                return await ProcessRequestAsync(message, requestIdProperty);
            }
            else
            {
                var message =
                    jsonElement.Deserialize<JsonRpcNotification>(JsonSerializerOptions)
                    ?? throw new JsonException("Invalid notification format");
                return await ProcessNotificationAsync(message);
            }
        }

        return null;
    }

    protected async Task<JsonResponseBase> ProcessRequestAsync(JsonRpcRequest message, JsonElement requestId)
    {
        if (requestHandlers.TryGetValue(message.Method, out var handler))
        {
            try
            {
                var result = await handler(message.Params, JsonSerializerOptions);
                return new JsonRpcResponse { Id = requestId, Result = result };
            }
            catch (Exception ex)
            {
                return new JsonRpcErrorResponse
                {
                    Id = requestId,
                    Error = new JsonRpcError
                    {
                        Code = (int)JsonRpcErrorCode.InternalError,
                        Message = ex.Message,
                        Data = ex.StackTrace,
                    },
                };
            }
        }
        else
        {
            return new JsonRpcErrorResponse
            {
                Id = requestId,
                Error = new JsonRpcError
                {
                    Code = (int)JsonRpcErrorCode.MethodNotFound,
                    Message = $"Method for request {message?.Method} not found",
                },
            };
        }
    }

    protected async Task<JsonResponseBase?> ProcessNotificationAsync(JsonRpcNotification message)
    {
        if (notificationHandlers.TryGetValue(message.Method, out var handler))
        {
            try
            {
                await handler(message.Params, JsonSerializerOptions);
                return null;
            }
            catch (Exception ex)
            {
                return new JsonRpcErrorResponse
                {
                    Id = null,
                    Error = new JsonRpcError
                    {
                        Code = (int)JsonRpcErrorCode.InternalError,
                        Message = ex.Message,
                        Data = ex.StackTrace,
                    },
                };
            }
        }
        else
        {
            return new JsonRpcErrorResponse
            {
                Id = null,
                Error = new JsonRpcError
                {
                    Code = (int)JsonRpcErrorCode.MethodNotFound,
                    Message = $"Method for notification {message?.Method} not found",
                },
            };
        }
    }
}
