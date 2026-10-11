using PerspectiveV.Web.S3Storage.Interface;

namespace PerspectiveV.Web.S3Storage.Implementation;

public class ProgressStream : Stream
{
    private readonly Stream _inner;
    private readonly IUploadProgress _progress;
    private readonly long? _total;
    private long _transferred;

    public ProgressStream(Stream inner, IUploadProgress progress, long? total)
    {
        _inner = inner;
        _progress = progress;
        _total = total;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _inner.Write(buffer, offset, count);

        _transferred += count;
        _progress.Report(_transferred, _total);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await _inner.WriteAsync(buffer, cancellationToken);

        _transferred += buffer.Length;
        _progress.Report(_transferred, _total);
    }

    // --- passthrough ---
    public override bool CanRead => _inner.CanRead;
    public override bool CanSeek => _inner.CanSeek;
    public override bool CanWrite => _inner.CanWrite;
    public override long Length => _inner.Length;

    public override long Position
    {
        get => _inner.Position;
        set => _inner.Position = value;
    }

    public override void Flush() => _inner.Flush();
    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
    public override void SetLength(long value) => _inner.SetLength(value);
}