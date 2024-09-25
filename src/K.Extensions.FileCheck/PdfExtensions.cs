using System;
using System.IO;

namespace K.Extensions.FileCheck
{
    /// <summary>
    /// Provides extension methods for PDF file checks.
    /// </summary>
    public static class PdfExtensions
    {
        private const int MinimumByteArrayLength = 5;

        /// <summary>
        /// Checks if the given byte array represents a PDF file.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents a PDF file, false otherwise.</returns>
        public static bool IsPdf(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < MinimumByteArrayLength)
                return false;

            return CheckPdfPattern(bytes.AsSpan());
        }

        /// <summary>
        /// Checks if the given stream represents a PDF file.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if the stream represents a PDF file, false otherwise.</returns>
        public static bool IsPdf(this Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long originalPosition = stream.CanSeek ? stream.Position : 0;

            try
            {
                Span<byte> header = stackalloc byte[MinimumByteArrayLength];
                int bytesRead = stream.Read(header);

                if (bytesRead < MinimumByteArrayLength)
                    return false;

                return CheckPdfPattern(header);
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Checks if the given byte array matches the byte pattern of a PDF file.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array matches the byte pattern of a PDF file, false otherwise.</returns>
        private static bool CheckPdfPattern(ReadOnlySpan<byte> bytes)
        {
            return bytes.Length >= MinimumByteArrayLength &&
                   bytes[0] == 0x25 &&  // %
                   bytes[1] == 0x50 &&  // P
                   bytes[2] == 0x44 &&  // D
                   bytes[3] == 0x46 &&  // F
                   bytes[4] == 0x2D;    // -
        }
    }
}