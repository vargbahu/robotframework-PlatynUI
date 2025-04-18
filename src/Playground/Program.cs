using System.Text.Json;
using System.Text.Json.Serialization;

using PlatynUI.JsonRpc;
using Playground;

var peer1 = new JsonRpcPeer(Console.OpenStandardInput(), Console.OpenStandardOutput());

DemoEndpoint.Attach(peer1);

// Methode bei peer1 registrieren
peer1.RegisterRequestHandler(
    "AddOld",
    (parameters, options) =>
    {
        if (!parameters.HasValue)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (parameters is not JsonElement element)
        {
            throw new ArgumentException("Invalid parameters");
        }
        if (element.ValueKind == JsonValueKind.Object)
        {
            // Deserialize the parameters from a JSON object
            var addParams = element.Deserialize<AddParams>() ?? throw new ArgumentException("Invalid parameters");

            return Task.FromResult(new[] { addParams.A + addParams.B, addParams.C });
        }
        var numbers = parameters?.Deserialize<double[]>();
        if (numbers == null || numbers.Length != 2)
        {
            throw new ArgumentException("Invalid parameters");
        }
        return Task.FromResult(new[] { numbers[0] + numbers[1] });
    }
);
peer1.RegisterRequestHandler(
    "Huhu",
    (parameters, options) =>
    {
        return Task.FromResult("Hallo");
    }
);
peer1.RegisterNotificationHandler(
    "Hello",
    (parameters, options) =>
    {
        Console.Error.WriteLine($"Notification received {parameters}");
        return Task.CompletedTask;
    }
);

peer1.Start();
Console.WriteLine(await peer1.Completion);

record AddParams
{
    public required double A { get; init; }
    public required double B { get; init; }

    public double C { get; init; } = -1;
};
