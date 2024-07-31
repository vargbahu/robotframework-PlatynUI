using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace PlatynUI.Provider.Avalonia;

public static class AppBuilderExtensions
{
    //
    // Summary:
    //     Initializes ReactiveUI framework to use with Avalonia. Registers Avalonia scheduler,
    //     an activation for view fetcher, a template binding hook. Remember to call this
    //     method if you are using ReactiveUI in your application.
    public static AppBuilder EnablePlatynUIServer(this AppBuilder builder)
    {
        return builder.AfterSetup(_ =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.Exit += (sender, e) => Server.Stop();
                Server.Start();
            }
        });
    }
}
