using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Haze.Pck
{
    public class PckArchive : IDisposable
    {
        /// <summary>
        /// Header magic (GDPC)
        /// </summary>
        public const Int32 Magic = 0x43504447;

        /// <summary>
        /// Godot default version (currently 3.2.4)
        /// </summary>
        public static readonly (int Major, int Minor, int Patch) DefaultVersion = (3, 2, 4);
        
        readonly Stream _stream;
        readonly bool _leaveOpen;
        bool _disposed;

        readonly List<PckArchiveEntry> _entries;
        readonly Dictionary<string, PckArchiveEntry> _entriesDictionary;

        internal Stream Stream => _stream;

        /// <summary>
        /// Godot version
        /// </summary>
        public (int Major, int Minor, int Patch) Version { get; }

        /// <summary>
        /// Gets the entries in this archive
        /// </summary>
        public ReadOnlyCollection<PckArchiveEntry> Entries { get; }

        /// <summary>
        /// Opens a PckArchive on the specified stream.
        /// </summary>
        public PckArchive(Stream stream) : this(stream, false) { }

        /// <summary>
        /// Opens a PckArchive on the specified stream.
        /// </summary>
        public PckArchive(Stream stream, bool leaveOpen)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            _stream = stream;
            _leaveOpen = leaveOpen;
            _entries = new List<PckArchiveEntry>();
            _entriesDictionary = new Dictionary<string, PckArchiveEntry>();

            Entries = new ReadOnlyCollection<PckArchiveEntry>(_entries);

            // start read
            using var reader = new BinaryReader(_stream, Encoding.UTF8, true);

            if (reader.ReadInt32() != Magic)
                throw new NotSupportedException("wrong pack header magic");

            if (reader.ReadInt32() != 1)
                throw new NotSupportedException("wrong pack format");

            Version = (reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

            reader.ReadBytes(64); // reserved

            var fileCount = reader.ReadInt32();
            for (var i = 0; i < fileCount; i++)
            {
                var pathLen = reader.ReadInt32();
                var pathBytes = reader.ReadBytes(pathLen);
                var offset = reader.ReadInt64();
                var size = reader.ReadInt64();
                var md5 = reader.ReadBytes(16);

                var path = Encoding.UTF8.GetString(pathBytes).TrimEnd('\0');
                var entry = new PckArchiveEntry { Archive = this, Path = path, Offset = offset, Size = size, MD5 = md5 };
                _entries.Add(entry);
                _entriesDictionary.Add(entry.Path, entry);
            }
        }

        /// <summary>
        /// Get entry by resPath
        /// </summary>
        /// <param name="resPath">Path to entry. It should start with res://</param>
        /// <returns>Returns <code>PckArchiveEntry</code> if exists, <code>null</code> otherwise</returns>
        public PckArchiveEntry GetEntry(string resPath)
        {
            if (resPath is null)
                throw new ArgumentNullException(nameof(resPath));

            return _entriesDictionary.GetValueOrDefault(resPath);
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
