using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace K.Extensions.FileCheck
{
    /// <summary>
    /// Provides extension methods for archive file checks.
    /// </summary>
    public static class ArchiveExtensions
    {
        /// <summary>
        /// Dictionary of archive file signatures.
        /// </summary>
        private static readonly Dictionary<string, string[]> ArchiveSignatures = new Dictionary<string, string[]>
        {
            { "zip", new string[] { "50", "4B", "03", "04" } },
            { "rar", new string[] { "52", "61", "72", "21", "1A", "07", "00" } },
            { "gzip", new string[] { "1F", "8B", "08" } }
        };

        /// <summary>
        /// Checks if the given byte array represents an archive file.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents an archive file, false otherwise.</returns>
        public static bool IsArchive(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return false;

            return ArchiveSignatures.Values.Any(signature => SharedExtensions.CheckSignature(bytes, signature));
        }

        /// <summary>
        /// Checks if the given stream represents an archive file.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if the stream represents an archive file, false otherwise.</returns>
        public static bool IsArchive(this Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long originalPosition = stream.CanSeek ? stream.Position : 0;

            try
            {
                if (ArchiveSignatures.Values.Any(signature => CheckSignature(stream, signature)))
                {
                    return true;
                }
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }

            return false;
        }

        /// <summary>
        /// Checks if the given stream matches the given archive file signature.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <param name="signature">The archive file signature to match.</param>
        /// <returns>True if the stream matches the archive file signature, false otherwise.</returns>
        private static bool CheckSignature(Stream stream, string[] signature)
        {
            byte[] buffer = new byte[signature.Length];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead < signature.Length)
                return false;

            return SharedExtensions.CheckSignature(buffer, signature);
        }
    }
}
