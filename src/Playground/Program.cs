using System.Diagnostics;
using System.Net.Sockets;
using PlatynUI.JsonRpc;
using PlatynUI.JsonRpc.Endpoints;
using PlatynUI.Runtime.Core;

// var tcpClient = new TcpClient()
// {
//     NoDelay = true,
//     SendTimeout = 1000,
//     ReceiveTimeout = 1000,
// };

// await tcpClient.ConnectAsync("192.168.178.33", 7721);
// //await tcpClient.ConnectAsync("localhost", 7721);

// var stream = tcpClient.GetStream();

//var peer = new JsonRpcPeer(stream);
Console.WriteLine(Directory.GetCurrentDirectory());

var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "dotnet",
        ArgumentList = { "run" },
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        CreateNoWindow = true,
        WorkingDirectory = "../../../../src/PlatynUI.Server",
    },
};

process.Start();
var peer = new JsonRpcPeer(process.StandardOutput.BaseStream, process.StandardInput.BaseStream);

try
{
    var displaydevice = IDisplayDeviceEndpoint.Attach(peer);
    var mouseDevice = IMouseDeviceEndpoint.Attach(peer);
    peer.Start();

    var rect = displaydevice.GetBoundingRectangle();
    Console.WriteLine($"Rect: {rect.X}, {rect.Y}, {rect.Width}, {rect.Height}");
    Console.WriteLine("MousePosition: " + mouseDevice.GetPosition());
    Console.WriteLine("DoubleClickTime: " + mouseDevice.GetDoubleClickTime());
    Console.WriteLine("DoubleClickSize: " + mouseDevice.GetDoubleClickSize());

    // while (true)
    // {
    //     Console.WriteLine("MousePosition: " + mouseDevice.GetPosition());
    // }

    for (var i = 10; i < 200; i++)
    {
        //displaydevice.HighlightRect(i, i, 200, 200);
        mouseDevice.Move(i, i, options: new MouseOptions { AfterMoveDelay = 5, MoveType = MouseMoveType.Direct });

        //Console.WriteLine("MousePosition: " + mouseDevice.GetPosition());
    }

    Console.WriteLine(mouseDevice.Move(0, 0, options: new MouseOptions { MoveType = MouseMoveType.Shaky }));
    Console.WriteLine(
        mouseDevice.Move(rect.TopLeft.X, rect.TopLeft.Y, options: new MouseOptions { MoveType = MouseMoveType.Shaky })
    );
    mouseDevice.Move(rect.BottomRight.X, rect.BottomRight.Y);
    mouseDevice.Move(rect.TopRight.X, rect.TopRight.Y);
    mouseDevice.Move(rect.BottomLeft.X, rect.BottomLeft.Y);
    mouseDevice.Move(rect.Center.X, rect.Center.Y);

    mouseDevice.Move(27, 12);
    mouseDevice.Click(MouseButton.Left);
    mouseDevice.Click(MouseButton.Left);

    Console.WriteLine("MousePosition: " + mouseDevice.GetPosition());

    Thread.Sleep(3000);
}
finally
{
    process.Kill();
    process.Dispose();
    //tcpClient.Close();
}
