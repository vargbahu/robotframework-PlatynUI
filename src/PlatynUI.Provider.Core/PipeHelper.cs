using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace PlatynUI.Provider.Core;

public static class ConnectionHelper
{
    private static readonly string RegistryPath = Path.Combine(
        Path.GetTempPath(),
        $"PlatynUI.Provider_{UserInfo.GetUserId()}_registry.json"
    );
    
    private static readonly int BasePort = 50000;
    private static readonly ConcurrentDictionary<int, int> ProcessPorts = new();

    public static int GetPortForProcess(int processId)
    {
        // Generate a deterministic port based on process ID
        // This ensures the same process always gets the same port
        return BasePort + (processId % 10000);
    }

    public static string GetLocalHostName()
    {
        try
        {
            // Try to get the machine's network hostname
            string hostName = Dns.GetHostName();
            
            // Try to get the first non-loopback IPv4 address
            var hostEntry = Dns.GetHostEntry(hostName);
            var ipAddress = hostEntry.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip));
            
            if (ipAddress != null)
            {
                return ipAddress.ToString();
            }
            
            // Fallback to hostname if no suitable IP found
            return hostName;
        }
        catch
        {
            // Ultimate fallback
            return "localhost";
        }
    }

    public static void RegisterServer(int processId, int port, string? hostName = null)
    {
        try
        {
            var registry = LoadRegistry();
            registry[processId.ToString()] = new ServerInfo
            {
                ProcessId = processId,
                Port = port,
                HostName = hostName ?? GetLocalHostName(),
                Timestamp = DateTime.UtcNow
            };
            SaveRegistry(registry);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to register server: {ex.Message}");
        }
    }

    public static void UnregisterServer(int processId)
    {
        try
        {
            var registry = LoadRegistry();
            registry.Remove(processId.ToString());
            SaveRegistry(registry);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to unregister server: {ex.Message}");
        }
    }

    public static Dictionary<int, ServerInfo> GetRegisteredServers()
    {
        try
        {
            var registry = LoadRegistry();
            // Clean up stale entries (older than 1 minute)
            var cutoff = DateTime.UtcNow.AddMinutes(-1);
            var validEntries = registry
                .Where(kvp => kvp.Value.Timestamp > cutoff)
                .ToDictionary(kvp => int.Parse(kvp.Key), kvp => kvp.Value);
            
            if (validEntries.Count != registry.Count)
            {
                SaveRegistry(validEntries.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value));
            }
            
            return validEntries;
        }
        catch
        {
            return new Dictionary<int, ServerInfo>();
        }
    }

    private static Dictionary<string, ServerInfo> LoadRegistry()
    {
        if (!File.Exists(RegistryPath))
        {
            return new Dictionary<string, ServerInfo>();
        }

        try
        {
            var json = File.ReadAllText(RegistryPath);
            return JsonSerializer.Deserialize<Dictionary<string, ServerInfo>>(json) 
                ?? new Dictionary<string, ServerInfo>();
        }
        catch
        {
            return new Dictionary<string, ServerInfo>();
        }
    }

    private static void SaveRegistry(Dictionary<string, ServerInfo> registry)
    {
        var json = JsonSerializer.Serialize(registry, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        File.WriteAllText(RegistryPath, json);
    }

    public class ServerInfo
    {
        public int ProcessId { get; set; }
        public int Port { get; set; }
        public DateTime Timestamp { get; set; }
        public string? HostName { get; set; } = "localhost";
    }
}
