using System;
using System.IO;

namespace Haze.Pck
{
    internal class PckEntryStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _startPosition;
        private readonly long _size;

        bool _canRead = true;

        long _position;
        long _endPosition;

        public override bool CanRead => _baseStream.CanRead && _canRead;

        public override bool CanSeek => _baseStream.CanSeek;

        public override bool CanWrite => false;

        public override long Length => _size;

        public override long Position { get => _position - _startPosition; set => Seek(value, SeekOrigin.Begin); }

        public PckEntryStream(Stream baseStream, Int64 startPosition, Int64 size)
        {
            _baseStream = baseStream;
            _startPosition = startPosition;
            _size = size;

            _position = _startPosition;
            _endPosition = _startPosition + _size;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_baseStream.Position != _position)
                _baseStream.Seek(_position, SeekOrigin.Begin);

            if (_position + count > _endPosition)
                count = (int)(_endPosition - _position);

            var ret = _baseStream.Read(buffer, offset, count);

            _position += ret;

            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var position = origin switch
            {
                SeekOrigin.Begin => _startPosition + offset,
                SeekOrigin.Current => _position + offset,
                SeekOrigin.End => _endPosition + offset,
                _ => throw new ArgumentException(nameof(origin)),
            };

            _position = Math.Clamp(position, _startPosition, _endPosition);
            return _position;
        }

        public override void Flush() => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
