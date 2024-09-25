using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        private static readonly Dictionary<string, ReadOnlyMemory<byte>> ArchiveSignatures = new Dictionary<string, ReadOnlyMemory<byte>>
        {
            { "zip", new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
            { "rar", new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 } },
            { "gzip", new byte[] { 0x1F, 0x8B, 0x08 } },
            { "7z", new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C } },
            { "bz2", new byte[] { 0x42, 0x5A, 0x68 } },
            { "xz", new byte[] { 0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00 } },
            { "iso", new byte[] { 0x43, 0x44, 0x30, 0x30, 0x31 } },
            { "cab", new byte[] { 0x4D, 0x53, 0x43, 0x46 } }
        };

        /// <summary>
        /// Checks if the given byte array represents an archive file.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents an archive file, false otherwise.</returns>
        public static bool IsArchive(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return false;

            ReadOnlySpan<byte> bytesSpan = bytes.AsSpan();
            foreach (var signature in ArchiveSignatures.Values)
            {
                if (SharedExtensions.CheckSignature(bytesSpan, signature.Span))
                {
                    return true;
                }
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
            return IsArchiveInternal(stream, false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously checks if the given stream represents an archive file.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if the stream represents an archive file, false otherwise.</returns>
        public static ValueTask<bool> IsArchiveAsync(this Stream stream)
        {
            return IsArchiveInternal(stream, true);
        }

        private static async ValueTask<bool> IsArchiveInternal(Stream stream, bool isAsync)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long originalPosition = stream.CanSeek ? stream.Position : 0;

            try
            {
                Memory<byte> buffer = new byte[ArchiveSignatures.Values.Max(s => s.Length)];
                int bytesRead = isAsync
                    ? await stream.ReadAsync(buffer)
                    : stream.Read(buffer.Span);

                if (bytesRead == 0)
                    return false;

                foreach (var signature in ArchiveSignatures.Values)
                {
                    if (SharedExtensions.CheckSignature(buffer.Span.Slice(0, bytesRead), signature.Span))
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }
        }
    }
}
