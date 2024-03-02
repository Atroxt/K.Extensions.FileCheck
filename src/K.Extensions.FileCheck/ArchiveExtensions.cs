using System;
using System.Collections.Generic;
using System.IO;

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
            foreach (var signature in ArchiveSignatures.Values)
            {
                if (CheckSignature(bytes, signature))
                    return true;
            }
            return false;
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

            foreach (var signature in ArchiveSignatures.Values)
            {
                if (IsArchive(stream, signature))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the given stream matches the given archive file signature.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <param name="signature">The archive file signature to match.</param>
        /// <returns>True if the stream matches the archive file signature, false otherwise.</returns>
        private static bool IsArchive(Stream stream, string[] signature)
        {
            if (stream == null || !stream.CanRead || signature.Length == 0)
                return false;

            List<byte> bytesIterated = new List<byte>();

            for (int i = 0; i < signature.Length; i++)
            {
                int bit = stream.ReadByte();
                if (bit == -1) return false; // End of stream reached
                bytesIterated.Add((byte)bit);
            }
            return CheckSignature(bytesIterated.ToArray(), signature);
        }

        /// <summary>
        /// Checks if the given byte array matches the given archive file signature.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <param name="signature">The archive file signature to match.</param>
        /// <returns>True if the byte array matches the archive file signature, false otherwise.</returns>
        private static bool CheckSignature(byte[] bytes, string[] signature)
        {
            if (bytes == null || signature.Length == 0 || bytes.Length < signature.Length)
                return false;

            for (int i = 0; i < signature.Length; i++)
            {
                if (bytes[i] != Convert.ToByte(signature[i], 16))
                    return false;
            }

            return true;
        }
    }
}
