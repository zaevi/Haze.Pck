using System;
using System.IO;

namespace Haze.Pck
{
    /// <summary>
    /// The base class of packer entries.
    /// </summary>
    public abstract class PckPackerEntry
    {
        internal string Path { get; set; }

        internal Int64 OffsetPosition { get; set; }

        internal Int64 Offset { get; set; }

        internal Int64 Size { get; set; }
        
        internal byte[] MD5 { get; set; }

        public PckPackerEntry(string resPath)
        {
            if (resPath is null)
                throw new ArgumentNullException(nameof(resPath));
            Path = resPath;
        }

        public abstract void Pack(Stream dest);
    }

    public class PckPackerEntryEntry : PckPackerEntry
    {
        public PckArchiveEntry Entry { get; }

        public PckPackerEntryEntry(string resPath, PckArchiveEntry entry) : base(resPath)
        {
            if (entry is null)
                throw new ArgumentNullException(nameof(entry));
            Entry = entry;
        }

        public override void Pack(Stream dest)
        {
            using var stream = Entry.Open();
            stream.CopyTo(dest);
        }
    }

    public class PckPackerEntryBytes : PckPackerEntry
    {
        public byte[] Bytes { get; }

        public PckPackerEntryBytes(string resPath, byte[] bytes) : base(resPath)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));
            Bytes = bytes;
        }

        public override void Pack(Stream dest)
        {
            dest.Write(Bytes.AsSpan());
        }
    }

    public class PckPackerEntryStream : PckPackerEntry
    {
        public Stream Stream { get; }

        public PckPackerEntryStream(string resPath, Stream stream) : base(resPath)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            Stream = stream;
        }

        public override void Pack(Stream dest)
        {
            Stream.CopyTo(dest);
        }
    }

    public class PckPackerEntryFile : PckPackerEntry
    {
        public string FilePath { get; }

        public PckPackerEntryFile(string resPath, string filePath) : base(resPath)
        {
            if (filePath is null)
                throw new ArgumentNullException(nameof(filePath));
            FilePath = filePath;
        }

        public override void Pack(Stream dest)
        {
            using var stream = File.OpenRead(FilePath);
            stream.CopyTo(dest);
        }
    }
}
