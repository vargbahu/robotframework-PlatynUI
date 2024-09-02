from PlatynUI.ui.runtime.core import dotnet_interface


print(dotnet_interface.DotNetInterface.finder().FindSingleNode(None, "/*", False))

for e in dotnet_interface.DotNetInterface.finder().FindNodes(None, "/*", False):
    print(e.GetAttributeValue("Name"))
