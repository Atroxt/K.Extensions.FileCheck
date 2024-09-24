using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace K.Extensions.FileCheck
{
    /// <summary>
    /// Provides extension methods for Excel document checks.
    /// </summary>
    public static class ExcelExtensions
    {
        /// <summary>
        /// Checks if the given byte array represents an Excel document.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents an Excel document, false otherwise.</returns>

        public static bool IsExcelDocument(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
                return false;

            byte[] bytesIterated = new byte[4];
            Array.Copy(bytes, bytesIterated, 4);

            // Check if it's a ZIP file (possibly an XLSX)
            if (SharedExtensions.IsZipFile(bytesIterated))
            {
                using var memoryStream = new MemoryStream(bytes);
                return IsXlsxExcelDocument(memoryStream);
            }
            else
            {
                // Check for older XLS format
                return CheckExcelDocumentType(bytesIterated);
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
                byte[] buffer = new byte[4];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead < 4)
                    return false;

                return SharedExtensions.IsZipFile(buffer) ? IsXlsxExcelDocument(stream) : CheckExcelDocumentType(buffer);
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Checks if the given byte list matches the byte pattern of an Excel document.
        /// </summary>
        /// <param name="bytesIterated">The byte list to check.</param>
        /// <returns>True if the byte list matches the byte pattern of an Excel document, false otherwise.</returns>
        private static bool CheckExcelDocumentType(byte[] bytesIterated)
        {
            // Define byte patterns for different Excel document file types
            Dictionary<string, string[]> documentTypes = new Dictionary<string, string[]>
            {
                { "xls", new string[] { "D0", "CF", "11", "E0" } } // XLS signature
            };

            return documentTypes.Values.Any(pattern => SharedExtensions.CheckSignature(bytesIterated, pattern));
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
