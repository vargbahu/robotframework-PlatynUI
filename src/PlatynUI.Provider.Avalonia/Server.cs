using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.VisualStudio.Threading;
using PlatynUI.Provider.Core;
using PlatynUI.Provider.Server;

namespace PlatynUI.Provider.Avalonia;

public static class Server
{
    static readonly JoinableTaskContext JoinableTaskContext = new();
    static readonly JoinableTaskFactory JoinableTaskFactory = new(JoinableTaskContext);

    public static void Start(IClassicDesktopStyleApplicationLifetime lifetime)
    {
        new Thread(() =>
        {
            JoinableTaskFactory.Run(() => RunAsync(lifetime));
        }).Start();
    }

    public static void Stop()
    {
        cts.Cancel();
    }

    private static readonly CancellationTokenSource cts = new();

    public static async Task RunAsync(IClassicDesktopStyleApplicationLifetime lifetime)
    {

        if (lifetime != null)
        {
            Console.WriteLine("Lifetime is AvaloniaObject");
            var server = new ProviderServer(new ApplicationInfo(), NodeInfo.GetNodeInfo(lifetime));

            await server.RunAsync(cts.Token);
        }
    }
}
