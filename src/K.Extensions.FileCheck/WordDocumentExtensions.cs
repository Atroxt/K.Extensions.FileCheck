using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace K.Extensions.FileCheck
{
    /// <summary>
    /// Provides extension methods for Word document checks.
    /// </summary>
    public static class WordDocumentExtensions
    {
        /// <summary>
        /// Checks if the given byte array represents a Word document.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents a Word document, false otherwise.</returns>
        public static bool IsWordDocument(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
                return false;

            var bytesIterated = bytes.Take(4).ToArray();

            if (IsZipFile(bytesIterated))
            {
                using var memoryStream = new MemoryStream(bytes);
                return IsDocxWordDocument(memoryStream);
            }
            else
            {
                return CheckWordDocumentType(bytesIterated);
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
                byte[] buffer = new byte[4];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead < 4)
                    Array.Resize(ref buffer, bytesRead); // Resize if less than 8 bytes were read

                return IsZipFile(buffer) ?
                    IsDocxWordDocument(stream) : CheckWordDocumentType(buffer);
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }
            return false;
        }

        /// <summary>
        /// Checks if the given byte list matches the byte pattern of a Word document.
        /// </summary>
        /// <param name="bytesIterated">The byte list to check.</param>
        /// <returns>True if the byte list matches the byte pattern of a Word document, false otherwise.</returns>
        private static bool CheckWordDocumentType(byte[] bytesIterated)
        {
            // Define byte patterns for different Word document file types
            Dictionary<string, string[]> documentTypes = new Dictionary<string, string[]>
            {
                { "doc", new string[] { "D0", "CF", "11", "E0" } }, // DOC signature
                { "odt", new string[] { "3C", "6F", "66", "66", "69", "63", "65", "3A", "64", "6F", "63", "75", "6D", "65", "6E", "74", "2D", "63", "6F", "6E", "74", "65", "6E", "74" } } // ODT signature
            };

            return documentTypes.Values.Any(pattern => IsDocumentType(bytesIterated, pattern));
        }

        /// <summary>
        /// Checks if the given byte array matches the given byte pattern.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <param name="pattern">The byte pattern to match.</param>
        /// <returns>True if the byte array matches the byte pattern, false otherwise.</returns>
        private static bool IsDocumentType(byte[] bytes, string[] pattern)
        {
            if (bytes.Length < pattern.Length)
                return false;

            for (int i = 0; i < pattern.Length; i++)
            {
                if (bytes[i] != Convert.ToByte(pattern[i], 16))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the byte array matches the ZIP signature.
        /// </summary>
        /// <param name="bytesIterated">The byte array to check.</param>
        /// <returns>True if it is a ZIP file (possibly a DOCX file), false otherwise.</returns>
        private static bool IsZipFile(byte[] bytesIterated)
        {
            string[] zipSignature = new string[] { "50", "4B", "03", "04" }; // ZIP file signature as well for docx and odt
            return IsDocumentType(bytesIterated, zipSignature);
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