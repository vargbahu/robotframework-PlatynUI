using System.Text.Json.Serialization;
using PlatynUI.JsonRpc;

namespace Playground;

record Student
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    public required int Age { get; init; }

    public string[] Subjects { get; init; } = [];
}

record School(string Name, int Year, Student[] Students);

[JsonRpcEndpoint]
interface IDemoApi
{
    [JsonRpcRequest("Add")]
    double Add(double a, double b);

    [JsonRpcRequest("Echo")]
    string Echo(string message);

    [JsonRpcRequest("AddAsync")]
    Task<double> AddAsync(double a, double b, bool negative = false);

    [JsonRpcRequest("EchoAsync")]
    Task<string> EchoAsync(string message, string? format = null, int abc = 0);

    [JsonRpcNotification]
    void Notify(string message);

    [JsonRpcRequest]
    School GetSchool(string name, int year, Student[] students)
    {
        return new School(name, year, students);
    }
}

[JsonRpcEndpoint("textDocument")]
interface IDocumentApi
{
    [JsonRpcRequest("getText")]
    Task<string> GetTextAsync(string uri, CancellationToken token);

    [JsonRpcRequest("Add")]
    double Add(double a, double b, double c);
}

partial class DemoEndpoint : IDocumentApi, IDemoApi
{
    double IDemoApi.Add(double a, double b)
    {
        return a + b;
    }

    public double Add(double a, double b, double c)
    {
        return a + b + c;
    }

    string IDemoApi.Echo(string message)
    {
        return message;
    }

    Task<double> IDemoApi.AddAsync(double a, double b, bool negative)
    {
        return Task.FromResult(a + b);
    }

    public Task<string> EchoAsync(string message, string? format, int abc)
    {
        return Task.FromResult($"{message} (abc: {abc}) {format}");
    }

    public void Notify(string message)
    {
        Console.Error.WriteLine($"Notification received {message}");
    }

    public Task<string> GetTextAsync(string uri, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
