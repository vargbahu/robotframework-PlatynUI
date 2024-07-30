using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using PlatynUI.Provider.Core;

namespace PlatynUI.Provider.Avalonia;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

public class ApplicationInfo : IApplicationInfoAsync
{
    public async Task<string> GetTechnology()
    {
        return "Avalonia";
    }

    public async Task<IList<NodeReference>> GetChildren()
    {
        var children = new List<NodeReference>();

        if (Application.Current is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            foreach (var window in lifetime.Windows)
            {
                children.Add(NodeInfo.GetNodeInfo(window).NodeReference);
            }
        }

        return [];
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
