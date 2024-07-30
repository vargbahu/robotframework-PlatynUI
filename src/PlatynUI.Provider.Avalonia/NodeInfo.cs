using System.Runtime.CompilerServices;
using PlatynUI.Provider.Core;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace PlatynUI.Provider.Avalonia;

public class NodeInfo(object node, NodeReference reference) : INodeInfoAsync
{
    public NodeReference NodeReference { get; } = reference;
    public WeakReference<object> Node { get; } = new(node);

    private static readonly ConditionalWeakTable<object, NodeInfo> _nodes = [];

    public static NodeInfo GetNodeInfo(object node)
    {
        lock (_nodes)
        {
            if (!_nodes.TryGetValue(node, out NodeInfo? nodeInfo))
            {
                var reference = new NodeReference([80, 108, node.GetHashCode()]);

                nodeInfo = new NodeInfo(node, reference);

                _nodes.Add(node, nodeInfo);
            }

            return nodeInfo;
        }
    }

    public static NodeInfo? GetNodeInfo(NodeReference reference)
    {
        return _nodes.FirstOrDefault(pair => pair.Value.NodeReference == reference).Value;
    }

    public async Task<bool> IsValid(NodeReference node)
    {
        var nodeInfo = GetNodeInfo(node);
        if (nodeInfo is null)
        {
            return false;
        }

        return true;
    }

    public async Task<IList<NodeReference>> GetChildren(NodeReference parent)
    {
        return [];
    }

    public async Task<string[]> GetAttributeNames(NodeReference node)
    {
        return ["RuntimeId", "Name", "Role"];
    }

    public async Task<object?> GetAttributeValue(NodeReference node, string attributeName)
    {
        var nodeInfo = GetNodeInfo(node);
        if (nodeInfo is null)
        {
            return null;
        }

        if (attributeName == "RuntimeId")
        {
            return nodeInfo.NodeReference.RuntimeId;
        }
        if (nodeInfo.Node.TryGetTarget(out var target))
        {
            switch (attributeName)
            {
                case "Name":
                    return "TODO";
                case "Role":
                    return target.GetType().Name;
            }
        }
        return null;
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
