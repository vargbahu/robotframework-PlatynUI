// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO.Pipes;
using Microsoft.VisualStudio.Threading;
using PlatynUI.Provider.Core;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;
using StreamJsonRpc;
using Attribute = PlatynUI.Runtime.Core.Attribute;

[assembly: PlatynUiExtension(supportedPlatforms: [RuntimePlatform.Any])]

namespace PlatynUI.Provider.Client;

class ApplicationNode(INode? parent, ProcessProvider provider) : INode
{
    public INode? Parent => parent;

    IList<INode>? _children = null;
    public IList<INode> Children => _children ??= provider.GetNodes(this).ToList();

    public string LocalName => provider.Process.ProcessName;

    public string NamespaceURI => Namespaces.App;

    IDictionary<string, IAttribute>? _attributes = null;
    public IDictionary<string, IAttribute> Attributes => _attributes ??= GetAttributes();

    public Dictionary<string, IAttribute> GetAttributes()
    {
        var result = new List<IAttribute>()
        {
            new Attribute("Technology", ThreadHelper.JoinableTaskFactory.Run(provider.ApplicationInfo.GetTechnology)),
            new Attribute("Name", provider.Process.ProcessName),
            new Attribute("ProcessId", provider.Process.Id),
            new Attribute("SessionId", provider.Process.SessionId),
            new Attribute("MainWindowHandle", provider.Process.MainWindowHandle),
            new Attribute("MainWindowTitle", provider.Process.MainWindowTitle),
            new Attribute("MainModule.FileName", provider.Process.MainModule?.FileName),
            new Attribute("MainModule.ModuleName", provider.Process.MainModule?.ModuleName),
            new Attribute(
                "FileVersionInfo.FileDescription",
                provider.Process.MainModule?.FileVersionInfo.FileDescription
            ),
            new Attribute("FileVersionInfo.ProductName", provider.Process.MainModule?.FileVersionInfo.ProductName),
            new Attribute("FileVersionInfo.InternalName", provider.Process.MainModule?.FileVersionInfo.InternalName),
            new Attribute("FileVersionInfo.CompanyName", provider.Process.MainModule?.FileVersionInfo.CompanyName),
            new Attribute("FileVersionInfo.Comments", provider.Process.MainModule?.FileVersionInfo.Comments),
            new Attribute("FileVersionInfo.FileVersion", provider.Process.MainModule?.FileVersionInfo.FileVersion),
            new Attribute(
                "FileVersionInfo.ProductVersion",
                provider.Process.MainModule?.FileVersionInfo.ProductVersion
            ),
            new Attribute("FileVersionInfo.SpecialBuild", provider.Process.MainModule?.FileVersionInfo.SpecialBuild),
            new Attribute("FileVersionInfo.IsDebug", provider.Process.MainModule?.FileVersionInfo.IsDebug)
        };

        return result.OrderBy(x => x.Name).ToDictionary(x => x.Name);
    }

    public ProcessProvider Provider => provider;

    public INode Clone()
    {
        return new ApplicationNode(parent, provider);
    }

    public bool IsSamePosition(INode other)
    {
        if (other is ApplicationNode node)
        {
            try
            {
                return node.Provider.PipeName == provider.PipeName
                    && node.Provider.Process.Id == provider.Process.Id
                    && provider.Process.ProcessName == node.Provider.Process.ProcessName
                    && provider.Process.StartTime == node.Provider.Process.StartTime;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    public void Refresh()
    {
        _children = null;
        _attributes = null;
    }
}

class ProcessProvider(Process process, string pipeName, INode? parent) : IDisposable
{
    ~ProcessProvider()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _disposed = false;

    public void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Stream?.Dispose();
            JsonRpc?.Dispose();
        }

        _disposed = true;
        Stream = null;
        JsonRpc = null;
    }

    public Process Process { get; } = process;
    public INode? Parent { get; } = parent;

    ApplicationNode? _applicationNode = null;
    public ApplicationNode ApplicationNode => _applicationNode ??= new ApplicationNode(Parent, this);

    NamedPipeClientStream? Stream = null;
    JsonRpc? JsonRpc = null;

    private IApplicationInfoAsync? _applicationInfo = null;
    public IApplicationInfoAsync ApplicationInfo =>
        _applicationInfo ?? throw new InvalidOperationException("Not connected");

    private INodeInfoAsync? _nodeInfo = null;
    private INodeInfoAsync NodeInfo => _nodeInfo ?? throw new InvalidOperationException("Not connected");

    public string Technology = "";

    public string PipeName => pipeName;

    public async Task ConnectAsync()
    {
        if (Stream != null)
        {
            return;
        }

        Stream = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        try
        {
            Debug.WriteLine($"Connecting to process {Process.Id} with name {Process.ProcessName} and {PipeName}");
            await Stream.ConnectAsync(2000);
        }
        catch
        {
            Debug.WriteLine(
                $"Failed to connect to process {Process.Id} with name {Process.ProcessName} and {PipeName}"
            );
            Stream = null;
            throw;
        }

        try
        {
            JsonRpc = new JsonRpc(Stream);
            _applicationInfo = JsonRpc.Attach<IApplicationInfoAsync>();
            _nodeInfo = JsonRpc.Attach<INodeInfoAsync>();
            JsonRpc.JoinableTaskFactory = ThreadHelper.JoinableTaskFactory;
            JsonRpc.StartListening();
        }
        catch (Exception e)
        {
            Stream = null;
            JsonRpc = null;
            _applicationInfo = null;
            _nodeInfo = null;

            Debug.WriteLine(
                $"Failed to attach to process {Process.Id} with name {Process.ProcessName} and {PipeName}: {e}"
            );
            throw;
        }
        Debug.WriteLine($"Connected to process {Process.Id} with name {Process.ProcessName}");
    }

    public IEnumerable<INode> GetNodes(INode parent)
    {
        return [];
    }
}

class ThreadHelper
{
    public static readonly JoinableTaskContext JoinableTaskContext = new();
    public static readonly JoinableTaskFactory JoinableTaskFactory = new(JoinableTaskContext);
}

[Export(typeof(INodeProvider))]
class NodeProvider : INodeProvider
{
    Dictionary<int, ProcessProvider> _providerProcesses = [];

    public IEnumerable<INode> GetNodes(INode parent)
    {
        ThreadHelper.JoinableTaskFactory.Run(async () =>
        {
            var tasks = new List<Task<ProcessProvider?>>();
            foreach (var process in Process.GetProcesses())
            {
                if (_providerProcesses.ContainsKey(process.Id))
                {
                    continue;
                }

                var pipeName = PipeHelper.BuildPipeName(process.Id);

                if (Mutex.TryOpenExisting(pipeName, out var mutex))
                {
                    Debug.WriteLine($"Found mutex for process {process.Id} with name {process.ProcessName}");
                }
                else
                {
                    continue;
                }

                process.EnableRaisingEvents = true;

                process.Exited += (sender, e) =>
                {
                    if (_providerProcesses.TryGetValue(process.Id, out ProcessProvider? value))
                    {
                        value.Dispose();
                        _providerProcesses.Remove(process.Id);
                    }
                };
                tasks.Add(
                    Task.Run(async () =>
                    {
                        var provider = new ProcessProvider(process, pipeName, parent);
                        try
                        {
                            await provider.ConnectAsync();
                            return provider;
                        }
                        catch
                        {
                            provider.Dispose();
                            return null;
                        }
                    })
                );
            }
            var connected = (await Task.WhenAll(tasks) ?? []).Where(x => x != null);
            foreach (var provider in connected)
            {
                if (provider != null)
                {
                    _providerProcesses[provider.Process.Id] = provider;
                }
            }
        });

        return _providerProcesses.Values.Select(x => x.ApplicationNode);
    }
}
