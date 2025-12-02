// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;

namespace PlatynUI.Provider.Core;

/// <summary>
/// Configuration for remote provider connections
/// </summary>
public class ConnectionConfig
{
    private static readonly string ConfigPath = Path.Combine(
        Path.GetTempPath(),
        $"PlatynUI.Provider_{UserInfo.GetUserId()}_config.json"
    );

    public List<RemoteHost> RemoteHosts { get; set; } = new();
    public int BasePort { get; set; } = 50000;

    public static ConnectionConfig Load()
    {
        if (!File.Exists(ConfigPath))
        {
            return new ConnectionConfig();
        }

        try
        {
            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<ConnectionConfig>(json) ?? new ConnectionConfig();
        }
        catch
        {
            return new ConnectionConfig();
        }
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(ConfigPath, json);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to save configuration: {ex.Message}");
        }
    }

    public class RemoteHost
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 50000;
        public string? Description { get; set; }
    }
}
