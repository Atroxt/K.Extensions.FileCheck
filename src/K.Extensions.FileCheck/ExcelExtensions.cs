using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace K.Extensions.FileCheck
{
    /// <summary>
    /// Provides extension methods for Excel document checks.
    /// </summary>
    public static class ExcelExtensions
    {
        private const int MinimumByteArrayLength = 4;
        private static readonly Dictionary<string, ReadOnlyMemory<byte>> DocumentTypes = new Dictionary<string, ReadOnlyMemory<byte>>
        {
            { "xls", new byte[] { 0xD0, 0xCF, 0x11, 0xE0 } }
        };
        /// <summary>
        /// Checks if the given byte array represents an Excel document.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents an Excel document, false otherwise.</returns>

        public static bool IsExcelDocument(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < MinimumByteArrayLength)
                return false;

            ReadOnlySpan<byte> bytesSpan = bytes.AsSpan(0, MinimumByteArrayLength);
            
            // Check if it's a ZIP file (possibly an XLSX)
            if (SharedExtensions.IsZipFile(bytesSpan))
            {
                using var memoryStream = new MemoryStream(bytes);
                return IsXlsxExcelDocument(memoryStream);
            }
            else
            {
                // Check for older XLS format
                return CheckExcelDocumentType(bytesSpan);
            }
        }

        /// <summary>
        /// Checks if the given stream represents an Excel document.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if the stream represents an Excel document, false otherwise.</returns>
        public static bool IsExcelDocument(this Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long originalPosition = stream.Position;

            try
            {
                Span<byte> buffer = stackalloc byte[MinimumByteArrayLength];
                int bytesRead = stream.Read(buffer);

                if (bytesRead < MinimumByteArrayLength)
                    return false;

                return SharedExtensions.IsZipFile(buffer) ? 
                    IsXlsxExcelDocument(stream) : CheckExcelDocumentType(buffer);
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Checks if the given byte list matches the byte pattern of an Excel document.
        /// </summary>
        /// <param name="bytes">The byte list to check.</param>
        /// <returns>True if the byte list matches the byte pattern of an Excel document, false otherwise.</returns>
        private static bool CheckExcelDocumentType(ReadOnlySpan<byte> bytes)
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
        /// Checks if the stream is an XLSX Excel document by checking the ZIP file structure.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>True if it's an XLSX Excel document, false otherwise.</returns>
        private static bool IsXlsxExcelDocument(Stream stream)
        {
            try
            {
                using ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read, true);
                // Check if the ZIP archive contains the typical XLSX structure
                ZipArchiveEntry? entry = archive.GetEntry("xl/workbook.xml");
                return entry != null; // XLSX if workbook.xml is present
            }
            catch (InvalidDataException)
            {
                return false; // If the file is not a valid ZIP archive
            }
        }
    }
}
