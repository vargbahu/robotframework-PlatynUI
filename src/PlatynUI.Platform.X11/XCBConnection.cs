// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0


using PlatynUI.Platform.X11.Interop.XCB;
using PlatynUI.Runtime;

namespace PlatynUI.Platform.X11;

[Serializable]
public class X11Exception : Exception
{
    public X11Exception() { }

    public X11Exception(string? message)
        : base(message) { }

    public X11Exception(string? message, Exception? innerException)
        : base(message, innerException) { }
}

public unsafe class XCBConnection : IDisposable
{
    private static XCBConnection? _instance = null;
    private xcb_connection_t* _connection = null;

    private XCBConnection() { }

    ~XCBConnection()
    {
        Disconnect();
    }

    public static XCBConnection Instance => _instance ??= new XCBConnection();

    private void Disconnect()
    {
        if (_connection != null)
        {
            XCB.disconnect(_connection);
        }
        _connection = null;
    }

    public xcb_connection_t* Connection
    {
        get
        {
            if (_connection == null)
            {
                _connection = Connect();
            }
            return _connection;
        }
    }

    private static xcb_connection_t* Connect()
    {
        var result = XCB.connect(null, null);
        if (result == null)
        {
            throw new X11Exception("Can't connect to Display");
        }
        return result;
    }

    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Disconnect();
        }

        _disposed = true;
    }

    public uint RootWindow
    {
        get { return XCB.setup_roots_iterator(XCB.get_setup(Connection)).data->root; }
    }

    public Size ScreenSize
    {
        get
        {
            var data = XCB.setup_roots_iterator(XCB.get_setup(Connection)).data;
            return new Size(data->width_in_pixels, data->height_in_pixels);
        }
    }
}
