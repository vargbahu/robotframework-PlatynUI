// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;
using Tmds.DBus.Protocol;
using Tmds.DBus.SourceGenerator;
using Attribute = PlatynUI.Runtime.Core.Attribute;

[assembly: PlatynUiExtension(supportedPlatforms: [RuntimePlatform.Linux])]

namespace PlatynUI.Extension.Linux.AtSpi2;

struct ElementReference(string service, ObjectPath path)
{
    public string Service = service;
    public ObjectPath Path = path;

    public override readonly string ToString()
    {
        return $"{Service}:{Path}";
    }

    public static bool operator ==(ElementReference left, ElementReference right)
    {
        return left.Service == right.Service && left.Path == right.Path;
    }

    public static bool operator !=(ElementReference left, ElementReference right)
    {
        return !(left.Service == right.Service && left.Path == right.Path);
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is ElementReference reference && this == reference;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Service, Path);
    }
}

class Adapter(Connection connection, INode? parent, ElementReference elementReference) : INode, IAdapter
{
    private List<INode>? _children;
    private IDictionary<string, IAttribute>? _attributes;
    private string[]? _interfaces = null;

    public INode? Parent => parent;

    public IList<INode> Children => _children ??= GetChildren();

    private List<INode> GetChildren()
    {
        var result = new List<INode>();

        var children = Element.GetChildrenAsync().GetAwaiter().GetResult();
        foreach (var child in children)
        {
            var acc = new OrgA11yAtspiAccessible(connection, child.Item1, child.Item2);
            var interfaces = acc.GetInterfacesAsync().GetAwaiter().GetResult();

            if (interfaces.Contains("org.a11y.atspi.Component"))
            {
                result.Add(new ComponentAdapter(Connection, this, new ElementReference(child.Item1, child.Item2)));
                continue;
            }

            if (interfaces.Contains("org.a11y.atspi.Application"))
            {
                var childCount = acc.GetChildCountPropertyAsync().GetAwaiter().GetResult();
                if (childCount == 0)
                {
                    continue;
                }
                result.Add(new ApplicationAdapter(Connection, this, new ElementReference(child.Item1, child.Item2)));

                continue;
            }

            result.Add(new Adapter(Connection, this, new ElementReference(child.Item1, child.Item2)));
        }
        return result;
    }

    static string ConvertToCamelCase(string input)
    {
        string[] words = input.Trim().Split(' ', '\t');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length == 0)
            {
                continue;
            }
            words[i] = char.ToUpper(words[i][0]) + words[i][1..];
        }
        return string.Join("", words);
    }

    public string LocalName
    {
        get
        {
            try
            {
                return ConvertToCamelCase(Element.GetRoleNameAsync().GetAwaiter().GetResult());
            }
            catch (Exception) { }
            return "Unknown";
        }
    }

    public virtual string NamespaceURI => Namespaces.Raw;

    public IDictionary<string, IAttribute> Attributes => _attributes ??= GetAttributes();

    private Dictionary<string, string> GetAttributesDictionary()
    {
        try
        {
            return Element.GetAttributesAsync().GetAwaiter().GetResult();
        }
        catch (Exception) { }
        return [];
    }

    protected virtual List<IAttribute> GetAttributesList()
    {
        return
        [
            .. GetAttributesDictionary().Select(x => new Attribute("Native." + x.Key, () => x.Value)),
            new Attribute("Name", () => Element.GetNamePropertyAsync().GetAwaiter().GetResult()),
            new Attribute("Description", () => Element.GetDescriptionPropertyAsync().GetAwaiter().GetResult()),
            new Attribute("Role", () => LocalName),
            new Attribute("RoleId", () => Element.GetRoleAsync().GetAwaiter().GetResult()),
            new Attribute("LocalizedRoleName", () => Element.GetLocalizedRoleNameAsync().GetAwaiter().GetResult()),
            new Attribute("AccessibleId", () => Element.GetAccessibleIdPropertyAsync().GetAwaiter().GetResult()),
            new Attribute("Locale", () => Element.GetLocalePropertyAsync().GetAwaiter().GetResult()),
            //new Attribute("HelpText", () => Element.GetHelpTextPropertyAsync().GetAwaiter().GetResult().ToString()),
            new Attribute("State", () => Element.GetStateAsync().GetAwaiter().GetResult()),
            new Attribute("GetRelationSetAsync", () => Element.GetRelationSetAsync().GetAwaiter().GetResult()),
            new Attribute("Interfaces", () => Interfaces),
        ];
    }

    private Dictionary<string, IAttribute> GetAttributes()
    {
        return GetAttributesList().OrderBy(x => x.Name).ToDictionary(x => x.Name);
    }

    string[] Interfaces => _interfaces ??= Element.GetInterfacesAsync().GetAwaiter().GetResult();

    public Connection Connection => connection;
    public ElementReference ElementReference { get; } = elementReference;
    public OrgA11yAtspiAccessible Element { get; } = new(connection, elementReference.Service, elementReference.Path);

    public string Id => "";

    public string Name => Element.GetNamePropertyAsync().GetAwaiter().GetResult();

    public string Role => LocalName;

    public string ClassName => "";

    public string[] SupportedRoles => ["Adapter"];

    public string Type => "element";

    public string[] SupportedTypes => ["element"];

    public string FrameworkId => "AtSpi2";

    public string RuntimeId => ElementReference.ToString();

    public INode Clone()
    {
        return new Adapter(Connection, Parent, ElementReference);
    }

    public bool IsSamePosition(INode other)
    {
        if (other is Adapter adapter)
        {
            return ElementReference == adapter.ElementReference;
        }

        return false;
    }

    public void Refresh()
    {
        _children = null;
        _attributes = null;
    }
}

class ApplicationAdapter(Connection connection, INode? parent, ElementReference elementReference)
    : Adapter(connection, parent, elementReference)
{
    OrgA11yAtspiApplication Application { get; } = new(connection, elementReference.Service, elementReference.Path);

    public override string NamespaceURI => Namespaces.App;

    protected override List<IAttribute> GetAttributesList()
    {
        return
        [
            .. base.GetAttributesList(),
            new Attribute("AtspiVersion", () => Application.GetAtspiVersionPropertyAsync().GetAwaiter().GetResult()),
            new Attribute("Id", () => Application.GetIdPropertyAsync().GetAwaiter().GetResult()),
            new Attribute("Version", () => Application.GetVersionPropertyAsync().GetAwaiter().GetResult()),
            new Attribute("ToolkitName", () => Application.GetToolkitNamePropertyAsync().GetAwaiter().GetResult()),
        ];
    }
}

class ComponentAdapter(Connection connection, INode? parent, ElementReference elementReference)
    : Adapter(connection, parent, elementReference),
        IElement
{
    OrgA11yAtspiComponent Component { get; } = new(connection, elementReference.Service, elementReference.Path);

    public bool IsEnabled => Element.GetStateAsync().GetAwaiter().GetResult().Any(c => c == 8);

    public bool IsVisible => Element.GetStateAsync().GetAwaiter().GetResult().Any(c => c == 25);

    public bool IsInView => true; // TODO

    public bool TopLevelParentIsActive => true; // TODO

    public Rect BoundingRectangle
    {
        get
        {
            var (X, Y) = Component.GetPositionAsync(0).GetAwaiter().GetResult();
            var (Width, Height) = Component.GetSizeAsync().GetAwaiter().GetResult();

            return new Rect(X, Y, Width, Height);
        }
    }

    public Rect VisibleRectangle => BoundingRectangle;

    public Point DefaultClickPosition
    {
        get
        {
            var b = BoundingRectangle;
            return new Point(b.X + b.Width / 2, b.Y + b.Height / 2);
        }
    }

    protected override List<IAttribute> GetAttributesList()
    {
        return
        [
            .. base.GetAttributesList(),
            new Attribute("IsEnabled", () => IsEnabled),
            new Attribute("IsVisible", () => IsVisible),
            new Attribute("IsInView", () => IsInView),
            new Attribute("TopLevelParentIsActive", () => TopLevelParentIsActive),
            new Attribute("BoundingRectangle", () => BoundingRectangle),
            new Attribute("VisibleRectangle", () => VisibleRectangle),
            new Attribute("DefaultClickPosition", () => DefaultClickPosition),
        ];
    }
}

[Export(typeof(INodeProvider))]
class NodeProvider : INodeProvider
{
    private Connection? _connection;
    private Adapter? _root;

    Connection Connection => _connection ??= CreateConnection();

    private static Connection CreateConnection()
    {
        var bus = new OrgA11yBus(Connection.Session, "org.a11y.Bus", "/org/a11y/bus");
        var address = bus.GetAddressAsync().GetAwaiter().GetResult();

        return new Connection(new ClientConnectionOptions(address) { AutoConnect = true });
    }

    public IEnumerable<INode> GetNodes(INode parent)
    {
        Root.Refresh();
        return Root.Children;
    }

    public Adapter Root =>
        _root ??= new Adapter(
            Connection,
            null,
            new ElementReference("org.a11y.atspi.Registry", "/org/a11y/atspi/accessible/root")
        );
}
