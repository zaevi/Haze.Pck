using System;
using System.IO;

namespace Haze.Pck
{
    public class PckArchiveEntry
    {
        public PckArchive Archive { get; internal set; }

        /// <summary>
        /// The path to entry. It would start with res://
        /// </summary>
        public string Path { get; internal set; }

        public Int64 Offset { get; internal set; }

        public Int64 Size { get; internal set; }

        public byte[] MD5 { get; internal set; }

        /// <summary>
        /// Opens this entry.
        /// </summary>
        public Stream Open()
        {
            return new PckEntryStream(Archive.Stream, Offset, Size);
        }
    }
}
