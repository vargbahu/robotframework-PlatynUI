using System.Collections.ObjectModel;
using PlatynUI.Runtime.Core;
using ReactiveUI;

namespace PlatynUI.Spy.ViewModels;

public class TreeNode(TreeNode? parent, INode node) : ViewModelBase
{
    public INode Node { get; } = node;

    public string NodeName
    {
        get { return $"{Node.LocalName}"; }
    }

    string? _description;
    public string Description
    {
        get
        {
            if (_description == null)
            {
                var result = "";
                if (Node.Attributes.TryGetValue("Name", out var name))
                {
                    result = $"\"{name.Value}\"";
                }
                else if (Node.Attributes.TryGetValue("AutomationId", out var automationId))
                {
                    result = $"\"{automationId.Value}\"";
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = $"\"{result}\"";
                }
                _description = result;
            }
            return _description;
        }
    }

    public TreeNode? Parent { get; } = parent;

    ObservableCollection<TreeNode>? _children;
    public ObservableCollection<TreeNode> Children => _children ??= GetChildren();

    private ObservableCollection<TreeNode> GetChildren()
    {
        var children = new ObservableCollection<TreeNode>();

        foreach (var child in Node.Children)
        {
            children.Add(new TreeNode(this, child));
        }

        return children;
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
    }

    private bool _isExpanded;
    public bool IsExpanded
    {
        get { return _isExpanded; }
        set { this.RaiseAndSetIfChanged(ref _isExpanded, value); }
    }
}
