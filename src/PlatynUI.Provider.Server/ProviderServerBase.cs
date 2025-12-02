using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using PlatynUI.Provider.Core;
using StreamJsonRpc;

namespace PlatynUI.Provider.Server
{
    public class ProviderServer(IApplicationInfoAsync applicationInfo, INodeInfoAsync nodeInfo)
    {
        IApplicationInfoAsync ApplicationInfo { get; } = applicationInfo;
        INodeInfoAsync NodeInfo { get; } = nodeInfo;

        async Task RespondToRpcRequestsAsync(Stream stream, int clientId)
        {
            Debug.WriteLine(
                $"Connection request #{clientId} received. Spinning off an async Task to cater to requests."
            );

            var jsonRpc = new JsonRpc(stream);
            jsonRpc.AddLocalRpcTarget(ApplicationInfo);
            jsonRpc.AddLocalRpcTarget(NodeInfo);

            jsonRpc.StartListening();

            Debug.WriteLine($"JSON-RPC listener attached to #{clientId}. Waiting for requests...");

            await jsonRpc.Completion;

            Debug.WriteLine($"Connection #{clientId} terminated.");
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            int port = ConnectionHelper.GetPortForProcess(Environment.ProcessId);
            var endpoint = new IPEndPoint(IPAddress.Any, port);
            var listener = new TcpListener(endpoint);
            
            try
            {
                listener.Start();
                Debug.WriteLine($"TCP server started on port {port} for process {Environment.ProcessId}");
                
                // Register this server in the connection registry with actual hostname
                string hostName = ConnectionHelper.GetLocalHostName();
                ConnectionHelper.RegisterServer(Environment.ProcessId, port, hostName);

                int clientId = 0;
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("Cancellation requested. Exiting the loop.");
                        break;
                    }

                    Debug.WriteLine("Waiting for client to make a connection...");

                    try
                    {
                        var client = await listener.AcceptTcpClientAsync(cancellationToken);
                        var stream = client.GetStream();
                        _ = RespondToRpcRequestsAsync(stream, ++clientId);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine($"Error while waiting for connection: {e}");
                    }
                }
            }
            finally
            {
                listener.Stop();
                ConnectionHelper.UnregisterServer(Environment.ProcessId);
                Debug.WriteLine($"TCP server stopped on port {port}");
            }
        }
    }
}
