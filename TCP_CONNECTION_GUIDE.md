# TCP-Based Remote Provider Connection

This implementation replaces the NamedPipe-based communication with TCP sockets, enabling remote connections between the Provider Client and Server.

## Changes Made

### 1. Server Side (`PlatynUI.Provider.Server`)
- **ProviderServerBase.cs**: Replaced `NamedPipeServerStream` with `TcpListener`
  - Listens on a TCP port instead of a named pipe
  - Port is calculated as: `BasePort + (ProcessId % 10000)` (default BasePort: 50000)
  - Automatically registers the server in a local registry file
  - Supports multiple concurrent client connections

### 2. Client Side (`PlatynUI.Extension.Provider.Client`)
- **ProcessProvider.cs**: Replaced `NamedPipeClientStream` with `TcpClient`
  - Connects to TCP endpoint (host:port) instead of named pipe
  - Supports both local and remote connections
  
- **NodeProvider.cs**: Updated discovery mechanism
  - Discovers local servers from the registry file
  - Discovers remote servers from configuration file
  - Automatically attempts connections to all discovered servers

### 3. Core Infrastructure (`PlatynUI.Provider.Core`)
- **PipeHelper.cs → ConnectionHelper.cs**: Renamed and redesigned
  - Manages TCP port allocation
  - Maintains a JSON-based registry of active servers
  - Provides server registration/unregistration
  - Auto-cleans stale entries (older than 1 minute)

- **ConnectionConfig.cs**: New configuration system
  - Allows defining remote hosts to connect to
  - Stored as JSON in temp directory

## Usage

### Local Connections (Automatic)
Local servers are discovered automatically via the registry file. No configuration needed.

### Remote Connections
To connect to remote providers, create/edit the configuration file:

**Location**: `{TempPath}/PlatynUI.Provider_{UserId}_config.json`

**Example**:
```json
{
  "BasePort": 50000,
  "RemoteHosts": [
    {
      "HostName": "192.168.1.100",
      "Port": 52345,
      "Description": "Remote NGx Application"
    },
    {
      "HostName": "remote-machine.local",
      "Port": 51000,
      "Description": "Test Environment"
    }
  ]
}
```

### Port Calculation
Each server process automatically gets assigned a port:
```
Port = BasePort + (ProcessId % 10000)
```

For process ID 2345 with BasePort 50000:
```
Port = 50000 + 2345 = 52345
```

### Registry File
Active servers are registered in:
**Location**: `{TempPath}/PlatynUI.Provider_{UserId}_registry.json`

**Example**:
```json
{
  "2345": {
    "ProcessId": 2345,
    "Port": 52345,
    "Timestamp": "2025-11-28T10:30:00Z",
    "HostName": "localhost"
  }
}
```

## Network Requirements

### Firewall Configuration
When running remote connections, ensure:
1. TCP ports in range 50000-60000 are open on the server machine
2. Client machine can reach the server on these ports

### Security Considerations
⚠️ **Important**: This implementation does NOT include authentication or encryption.

For production use, consider:
- Using VPN or SSH tunneling
- Implementing authentication in the JSON-RPC layer
- Adding TLS/SSL encryption
- Restricting port access with firewall rules

## Troubleshooting

### Connection Issues
1. **Check registry file**: Verify the server is registered
2. **Check port availability**: `netstat -an | grep {port}`
3. **Firewall**: Ensure port is open
4. **Network**: Verify connectivity with `ping` or `telnet`

### Debug Output
Enable debug tracing to see connection attempts:
- Check Debug.WriteLine output in the application logs
- Look for messages like: "Connecting to process X at host:port"

## Migration Notes

### Breaking Changes
- Configuration files now use different format (JSON instead of named pipe names)
- Client code that manually constructed pipe names will need updates
- Discovery mechanism changed from mutex-based to registry-based

### Backwards Compatibility
This change is **not** backwards compatible with the NamedPipe implementation. All components (server and client) must be updated together.

## Example Scenarios

### Scenario 1: Local Testing
No configuration needed. Server and client run on same machine, automatic discovery works.

### Scenario 2: Remote Provider
1. Run Provider.Server on Machine A (automatically gets port 52345 for process 2345)
2. On Machine B, create config file with Machine A's hostname and port
3. Client automatically discovers and connects to remote provider

### Scenario 3: Multiple Remote Providers
Add multiple entries to `RemoteHosts` array in configuration file. Client connects to all of them.
