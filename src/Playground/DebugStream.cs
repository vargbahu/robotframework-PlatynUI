public class DebugStream : Stream
{
    private readonly Stream _innerStream;

    public DebugStream(Stream innerStream)
    {
        _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        var data = System.Text.Encoding.UTF8.GetString(buffer, offset, count);
        Console.Write(data);
        _innerStream.Write(buffer, offset, count);
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        var data = System.Text.Encoding.UTF8.GetString(buffer, offset, count);
        Console.Write(data);
        await _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override void Flush() => _innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

    public override void SetLength(long value) => _innerStream.SetLength(value);

    public override bool CanRead => _innerStream.CanRead;

    public override bool CanSeek => _innerStream.CanSeek;

    public override bool CanWrite => _innerStream.CanWrite;

    public override long Length => _innerStream.Length;

    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }
}
