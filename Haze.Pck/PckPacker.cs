using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Haze.Pck
{
    public class PckPacker : IDisposable
    {
        Stream _stream;
        bool _leaveOpen;
        bool _disposed;

        (int, int, int)? _version;
        Dictionary<string, PckPackerEntry> _entries;

        /// <summary>
        /// Gets or sets the Godot version. The default is <see cref="PckArchive.DefaultVersion"/>
        /// </summary>
        public (int Major, int Minor, int Patch) Version 
        { 
            get => _version ?? PckArchive.DefaultVersion; 
            set => _version = value; 
        }

        /// <summary>
        /// Gets or sets whether MD5 is calculated when packing
        /// </summary>
        public bool ComputeMD5 { get; set; }

        /// <summary>
        /// Creates PckPacker to the specified stream.
        /// </summary>
        public PckPacker(Stream stream, bool leaveOpen)
        {
            _entries = new Dictionary<string, PckPackerEntry>();
            _stream = stream;
            _leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Adds an entry to packer.
        /// </summary>
        public void Add(PckPackerEntry entry)
        {
            if (entry is null)
                throw new ArgumentNullException(nameof(entry));
            _entries.Add(entry.Path, entry);
        }

        /// <summary>
        /// Pack the PckPacker.
        /// </summary>
        public void Pack()
        {
            using var writer = new BinaryWriter(_stream, Encoding.UTF8, _leaveOpen);

            // header
            writer.Write(PckArchive.Magic);
            writer.Write((Int32)1); // format
            writer.Write(Version.Major);
            writer.Write(Version.Minor);
            writer.Write(Version.Patch);
            writer.Write(new byte[64]); // reserved

            // files' header
            writer.Write(_entries.Count);
            foreach (var (path, entry) in _entries)
            {
                var pathBytes = Encoding.UTF8.GetBytes(path);
                writer.Write(pathBytes.Length); // path length
                writer.Write(pathBytes);        // path bytes
                entry.OffsetPosition = _stream.Position;
                writer.Write(default(Int64));   // offset
                writer.Write(default(Int64));   // size
                writer.Write(default(UInt64));  
                writer.Write(default(UInt64));  // md5
            }

            // contents 
            foreach (var (_, entry) in _entries)
            {
                entry.Offset = _stream.Position;
                entry.Pack(_stream);
                entry.Size = _stream.Position - entry.Offset;
            }

            // go back
            foreach (var (_, entry) in _entries)
            {
                _stream.Seek(entry.OffsetPosition, SeekOrigin.Begin);
                writer.Write(entry.Offset);
                writer.Write(entry.Size);
            }

            // compute MD5 hash
            if(ComputeMD5)
            {
                foreach (var (_, entry) in _entries)
                {
                    entry.MD5 = MD5.Create().ComputeHash(new PckEntryStream(_stream, entry.Offset, entry.Size));
                }
                foreach (var (_, entry) in _entries)
                {
                    _stream.Seek(entry.OffsetPosition + 16, SeekOrigin.Begin);
                    _stream.Write(entry.MD5, 0, 16);
                }
            }

            _stream.Flush();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (!_leaveOpen)
                {
                    _stream.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
