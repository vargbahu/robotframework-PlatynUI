using System.Net.Sockets;
using PlatynUI.JsonRpc;
using PlatynUI.JsonRpc.Endpoints;

var tcpClient = new TcpClient() { NoDelay = true };

await tcpClient.ConnectAsync("localhost", 7721);

var stream = tcpClient.GetStream();

var peer = new JsonRpcPeer(stream, stream);

var displaydevice = IDisplayDeviceEndpoint.Attach(peer);
peer.Start();

var rect = displaydevice.GetBoundingRectangle();
Console.WriteLine($"Rect: {rect.X}, {rect.Y}, {rect.Width}, {rect.Height}");

for (var i = 0; i < 1000; i++)
{
    displaydevice.HighlightRect(i, i, 200, 200);
}

Thread.Sleep(3000);
