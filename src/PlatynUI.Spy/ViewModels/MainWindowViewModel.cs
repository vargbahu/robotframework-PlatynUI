using System;
using Avalonia.Metadata;
using PlatynUI.Runtime;
using ReactiveUI;

namespace PlatynUI.Spy.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    bool _isTheThingEnabled = true;

    bool IsRefreshEnabled
    {
        get { return _isTheThingEnabled; }
        set { this.RaiseAndSetIfChanged(ref _isTheThingEnabled, value); }
    }

    public void Refresh(object parameter)
    {
        Console.WriteLine("Refresh");
    }

    [DependsOn(nameof(IsRefreshEnabled))]
    public bool CanRefresh(object parameter)
    {
        return IsRefreshEnabled;
    }

    public void AlwaysOnTop()
    {
        Console.WriteLine("AlwaysOnTop");

        IsRefreshEnabled = !IsRefreshEnabled;
    }

    public TreeNode[] Root { get; } = [new TreeNode(null, new Desktop()) { IsExpanded = true, IsSelected = true }];

    private TreeNode? _selectedNode;
    public TreeNode? SelectedNode
    {
        get => _selectedNode;
        set { this.RaiseAndSetIfChanged(ref _selectedNode, value); }
    }
}
