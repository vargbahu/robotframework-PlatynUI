// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using PlatynUI.Runtime.Core;
using Attribute = PlatynUI.Runtime.Core.Attribute;

namespace PlatynUI.Runtime;

public class Desktop : INode
{
    private Desktop()
    {
        PlatynUiExtensions.ComposeParts(this);
    }

    [ImportMany]
    IEnumerable<INodeProvider>? Providers { get; set; }

    public INode? Parent => null;

    private IList<INode>? _children;
    public IList<INode> Children => _children ??= GetChildren();

    protected IList<INode> GetChildren()
    {
        var children = new List<INode>();

        if (Providers == null)
        {
            return children;
        }

        foreach (var item in Providers)
        {
            children.AddRange(item.GetNodes(this));
        }
        return children;
    }

    public string LocalName => "Desktop";

    public string NamespaceURI => Namespaces.Raw;

    Dictionary<string, IAttribute>? _attributes;
    public IDictionary<string, IAttribute> Attributes =>
        _attributes ??= new List<IAttribute>()
        {
            new Attribute("Name", Environment.MachineName),
            new Attribute("Platform", Environment.OSVersion.Platform.ToString()),
            new Attribute("Version", Environment.OSVersion.VersionString),
            new Attribute("Role", "Desktop"),
            new Attribute("BoundingRectangle", DisplayDevice.GetBoundingRectangle())
        }
            .OrderBy(x => x.Name)
            .ToDictionary(x => x.Name);

    private static Desktop? _instance;
    public static Desktop GetInstance() => _instance ??= new Desktop();

    public INode Clone()
    {
        return new Desktop();
    }

    public bool IsSamePosition(INode other)
    {
        return this == other;
    }

    public void Refresh()
    {
        _children = null;
    }
}
