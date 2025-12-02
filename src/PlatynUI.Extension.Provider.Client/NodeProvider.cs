// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.Composition;
using System.Diagnostics;
using PlatynUI.Provider.Core;
using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;

[assembly: PlatynUiExtension(supportedPlatforms: [RuntimePlatform.Any])]

namespace PlatynUI.Extension.Provider.Client;

[Export(typeof(INodeProvider))]
class NodeProvider : INodeProvider
{
    Dictionary<int, ProcessProvider> _providerProcesses = [];

    public IEnumerable<INode> GetNodes(INode parent)
    {
        ThreadHelper.JoinableTaskFactory.Run(async () =>
        {
            var tasks = new List<Task<ProcessProvider?>>();
            
            // 1. Get all registered LOCAL servers from the connection registry
            var registeredServers = ConnectionHelper.GetRegisteredServers();
            
            foreach (var serverEntry in registeredServers)
            {
                int processId = serverEntry.Key;
                var serverInfo = serverEntry.Value;
                
                if (_providerProcesses.ContainsKey(processId))
                {
                    continue;
                }

                Process? process = null;
                try
                {
                    process = Process.GetProcessById(processId);
                }
                catch (ArgumentException)
                {
                    // Process doesn't exist anymore
                    Debug.WriteLine($"Process {processId} not found, skipping");
                    continue;
                }

                Debug.WriteLine($"Found registered server for process {process.Id} with name {process.ProcessName} on port {serverInfo.Port}");

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
                        var provider = new ProcessProvider(
                            process, 
                            serverInfo.Port, 
                            serverInfo.HostName ?? "localhost", 
                            parent
                        );
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
            
            // 2. Also check for REMOTE hosts from configuration
            var config = ConnectionConfig.Load();
            foreach (var remoteHost in config.RemoteHosts)
            {
                // Use negative process ID for remote connections to avoid conflicts
                int virtualProcessId = -1 - remoteHost.Port;
                
                if (_providerProcesses.ContainsKey(virtualProcessId))
                {
                    continue;
                }
                
                Debug.WriteLine($"Attempting to connect to remote host {remoteHost.HostName}:{remoteHost.Port}");
                
                tasks.Add(
                    Task.Run(async () =>
                    {
                        // Create a dummy process for remote connections
                        var dummyProcess = Process.GetCurrentProcess();
                        var provider = new ProcessProvider(
                            dummyProcess,
                            remoteHost.Port,
                            remoteHost.HostName,
                            parent
                        );
                        try
                        {
                            await provider.ConnectAsync();
                            return provider;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to connect to remote host {remoteHost.HostName}:{remoteHost.Port}: {ex.Message}");
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

        return _providerProcesses.Values.Select(x => x.GetRootNode()).Where(x => x != null).Cast<INode>().ToList();
    }
}
