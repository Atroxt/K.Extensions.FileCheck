using System;
using System.Collections.Generic;
using System.IO;

namespace K.Extensions.FileCheck
{
    /// <summary>
    /// Provides extension methods for image file checks.
    /// </summary>
    public static class ImageExtensions
    {
        private const int MinimumByteArrayLength = 8;
        private static readonly Dictionary<string, ReadOnlyMemory<byte>> ImageTypes = new Dictionary<string, ReadOnlyMemory<byte>>
            {
                { "jpg|jpeg", new byte[] { 0xFF, 0xD8 } }, // JPEG signature
                { "bmp", new byte[] { 0x42, 0x4D } },     // BMP signature
                { "gif", new byte[] { 0x47, 0x49, 0x46 } }, // GIF signature
                { "png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } // PNG signature
            };

        /// <summary>
        /// Checks if the given byte array represents an image file.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents an image file, false otherwise.</returns>
        public static bool IsImage(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < MinimumByteArrayLength)
                return false;

            ReadOnlySpan<byte> bytesSpan = bytes.AsSpan(0, Math.Min(MinimumByteArrayLength, bytes.Length));
            return CheckImageType(bytesSpan);
        }

        /// <summary>
        /// Checks if the given stream represents an image file.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if the stream represents an image file, false otherwise.</returns>
        public static bool IsImage(this Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long originalPosition = stream.CanSeek ? stream.Position : 0;

            try
            {
                Span<byte> buffer = stackalloc byte[MinimumByteArrayLength];
                int bytesRead = stream.Read(buffer);

                if (bytesRead < MinimumByteArrayLength)
                    buffer = buffer.Slice(0, bytesRead); // Resize if less than 8 bytes were read

                return CheckImageType(buffer);
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Checks if the given byte list matches the byte pattern of an image file.
        /// </summary>
        /// <param name="bytes">The byte list to check.</param>
        /// <returns>True if the byte list matches the byte pattern of an image file, false otherwise.</returns>
        private static bool CheckImageType(ReadOnlySpan<byte> bytes)
        {
            foreach (var pattern in ImageTypes.Values)
            {
                if (SharedExtensions.CheckSignature(bytes, pattern.Span))
                {
                    return true;
                }
            }

            return false;
        }
    }
}