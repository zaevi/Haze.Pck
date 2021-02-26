using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Haze.Pck
{
    public static partial class PckExtensions
    {
        #region [PckPacker Extensions]

        /// <summary>
        /// Adds entry from PckArchiveEntry with the same resPath.
        /// </summary>
        public static void Add(this PckPacker source, PckArchiveEntry entry)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (entry is null)
                throw new ArgumentNullException(nameof(entry));
            source.Add(new PckPackerEntryEntry(entry.Path, entry));
        }

        /// <summary>
        /// Adds entry from PckArchiveEntry with a new resPath.
        /// </summary>
        /// <param name="resPath">The resPath to entry. It should start with res://</param>
        public static void Add(this PckPacker source, string resPath, PckArchiveEntry entry)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            source.Add(new PckPackerEntryEntry(resPath, entry));
        }

        /// <summary>
        /// Adds entry from bytes.
        /// </summary>
        /// <param name="resPath">The resPath to entry. It should start with res://</param>
        public static void Add(this PckPacker source, string resPath, byte[] bytes)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            source.Add(new PckPackerEntryBytes(resPath, bytes));
        }

        /// <summary>
        /// Adds entry from specified stream.
        /// </summary>
        /// <param name="resPath">The resPath to entry. It should start with res://</param>
        public static void Add(this PckPacker source, string resPath, Stream stream)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            source.Add(new PckPackerEntryStream(resPath, stream));
        }

        /// <summary>
        /// Adds entry from specified text.
        /// </summary>
        /// <param name="resPath">The resPath to entry. It should start with res://</param>
        public static void Add(this PckPacker source, string resPath, string text, Encoding encoding)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (text is null)
                throw new ArgumentNullException(nameof(source));
            if (encoding is null)
                throw new ArgumentNullException(nameof(source));
            var bytes = encoding.GetBytes(text);
            source.Add(new PckPackerEntryBytes(resPath, bytes));
        }

        /// <summary>
        /// Adds entry from specified file.
        /// </summary>
        /// <param name="resPath">The resPath to entry. It should start with res://</param>
        public static void Add(this PckPacker source, string resPath, string filePath)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            source.Add(new PckPackerEntryFile(resPath, filePath));
        }

        #endregion

        #region [PckArchiveEntry Extensions]

        public static string GetMD5String(this PckArchiveEntry source)
        {
            if(source is null)
                throw new ArgumentNullException(nameof(source));

            return string.Concat(source.MD5.Select(b => b.ToString("x2")));
        }

        public static void ExtractToFile(this PckArchiveEntry source, string destFileName, bool overwrite = false)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (destFileName is null)
                throw new ArgumentNullException(nameof(destFileName));

            using var fs = File.Open(destFileName, overwrite ? FileMode.Create : FileMode.CreateNew);
            using var es = source.Open();
            es.CopyTo(fs);
        }

        public static void ExtractRelativeToDirectory(this PckArchiveEntry source, string destDirectoryName, bool overwrite = false)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (destDirectoryName is null)
                throw new ArgumentNullException(nameof(destDirectoryName));

            var relPath = source.Path.Replace("res://", "");
            var destPath = Path.Combine(destDirectoryName, relPath);

            Directory.CreateDirectory(Path.GetDirectoryName(destPath));
            source.ExtractToFile(destPath, overwrite);
        }

        #endregion

        #region [PckArchive Extensions]

        public static void ExtractToDirectory(this PckArchive source, string destDirectoryName, bool overwrite = false)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (destDirectoryName is null)
                throw new ArgumentNullException(nameof(destDirectoryName));

            foreach(var entry in source.Entries)
            {
                entry.ExtractRelativeToDirectory(destDirectoryName, overwrite);
            }
        }

        #endregion
    }
}
