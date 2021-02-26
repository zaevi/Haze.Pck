using System;
using System.IO;

namespace Haze.Pck
{
    public static class PckFile
    {
        /// <summary>
        /// Opens a PckArchive on the specified path for reading.
        /// </summary>
        /// <param name="pckFileName">The path to pck file</param>
        public static PckArchive Open(string pckFileName)
        {
            if (pckFileName is null)
                throw new ArgumentNullException(nameof(pckFileName));

            var fs = File.OpenRead(pckFileName);
            return new PckArchive(fs, leaveOpen: false);
        }

        /// <summary>
        /// Creates a PckPacker to the path.
        /// </summary>
        /// <param name="pckFileName">The pck path to write</param>
        public static PckPacker Create(string pckFileName)
        {
            if (pckFileName is null)
                throw new ArgumentNullException(nameof(pckFileName));

            var fs = File.Create(pckFileName);
            return new PckPacker(fs, leaveOpen: false);
        }
    }
}
