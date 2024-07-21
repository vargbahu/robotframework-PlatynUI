// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;
using PlatynUI.Technology.UiAutomation.Core;

[assembly: PlatynUiExtension(supportedPlatforms: [Platform.Windows])]

namespace PlatynUI.Technology.UiAutomation;

[Export(typeof(INodeProvider))]
class NodeProvider : INodeProvider
{
    public IEnumerable<Runtime.Core.INode> GetNodes(Runtime.Core.INode parent)
    {
        foreach (var e in Automation.RootElement.EnumerateChildren(Automation.RawViewWalker, true))
        {
            yield return new UiAutomationNode(parent, e);
        }
        ;
    }
}
