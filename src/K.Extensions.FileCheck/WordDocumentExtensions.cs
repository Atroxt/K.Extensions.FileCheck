using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace K.Extensions.FileCheck
{
    /// <summary>
    /// Provides extension methods for Word document checks.
    /// </summary>
    public static class WordDocumentExtensions
    {
        private const int MinimumByteArrayLength = 4;
        private static readonly Dictionary<string, ReadOnlyMemory<byte>> DocumentTypes = new Dictionary<string, ReadOnlyMemory<byte>>
            {
                { "doc", new byte[] { 0xD0, 0xCF, 0x11, 0xE0 } }, // DOC signature
                { "odt", new byte[] { 0x3C, 0x6F, 0x66, 0x66, 0x69, 0x63, 0x65, 0x3A, 0x64, 0x6F, 0x63, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x2D, 0x63, 0x6F, 0x6E, 0x74, 0x65, 0x6E, 0x74 } } // ODT signature
            };

        /// <summary>
        /// Checks if the given byte array represents a Word document.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents a Word document, false otherwise.</returns>
        public static bool IsWordDocument(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < MinimumByteArrayLength)
                return false;

            ReadOnlySpan<byte> bytesSpan = bytes.AsSpan(0, Math.Min(MinimumByteArrayLength, bytes.Length));

            if (SharedExtensions.IsZipFile(bytesSpan))
            {
                using var memoryStream = new MemoryStream(bytes);
                return IsDocxWordDocument(memoryStream);
            }
            else
            {
                return CheckWordDocumentType(bytesSpan);
            }
        }

        /// <summary>
        /// Checks if the given stream represents a Word document.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if the stream represents a Word document, false otherwise.</returns>
        public static bool IsWordDocument(this Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long originalPosition = stream.Position;

            try
            {
                Span<byte> buffer = stackalloc byte[MinimumByteArrayLength];
                int bytesRead = stream.Read(buffer);

                if (bytesRead < MinimumByteArrayLength)
                    buffer = buffer.Slice(0, bytesRead); // Resize if less than 4 bytes were read

                return SharedExtensions.IsZipFile(buffer) ?
                    IsDocxWordDocument(stream) : CheckWordDocumentType(buffer);
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Checks if the given byte list matches the byte pattern of a Word document.
        /// </summary>
        /// <param name="bytes">The byte list to check.</param>
        /// <returns>True if the byte list matches the byte pattern of a Word document, false otherwise.</returns>
        private static bool CheckWordDocumentType(ReadOnlySpan<byte> bytes)
        {
            foreach (var pattern in DocumentTypes.Values)
            {
                if (SharedExtensions.CheckSignature(bytes, pattern.Span))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the stream is a DOCX Word document by checking the ZIP file structure.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if it's a DOCX Word document, false otherwise.</returns>
        private static bool IsDocxWordDocument(Stream stream)
        {
            try
            {
                using ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read, true);
                // Check if the Word document structure exists inside the ZIP
                ZipArchiveEntry? entry = archive.GetEntry("word/document.xml");
                if (entry != null)
                {
                    return true; // It's a valid DOCX Word document
                }

                ZipArchiveEntry? odtEntry = archive.GetEntry("content.xml");
                if (odtEntry != null)
                {
                    return true; // It's a valid ODT Word document
                }
            }
            catch (InvalidDataException)
            {
                // If the file is not a valid ZIP archive, return false
                return false;
            }
            return false;
        }
    }
}