// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Reflection;
using PlatynUI.Runtime.Core;
using Attribute = PlatynUI.Runtime.Core.Attribute;

namespace PlatynUI.Runtime;

public class Desktop : INode
{
    [ImportMany]
    IEnumerable<INodeProvider>? Providers { get; set; }

    public INode? Parent => null;

    private bool _providerImported = false;

    protected void ImportProviders()
    {
        if (_providerImported)
        {
            return;
        }

        _providerImported = true;

        AggregateCatalog catalog = PlatynUiExtensions.GetPlatynUIExtensionCatalog();

        var container = new CompositionContainer(catalog);

        container.ComposeParts(this);
    }

    public IList<INode> Children
    {
        get
        {
            ImportProviders();

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
            new Attribute("Role", "Desktop")
        }
            .OrderBy(x => x.Name)
            .ToDictionary(x => x.Name);

    public INode Clone()
    {
        return new Desktop();
    }

    public bool IsSamePosition(INode other)
    {
        return this == other;
    }
}
