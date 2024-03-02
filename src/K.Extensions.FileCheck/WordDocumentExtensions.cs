using System;
using System.Collections.Generic;
using System.IO;
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

            List<byte> bytesIterated = bytes.Take(4).ToList();

            return CheckWordDocumentType(bytesIterated);
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

            List<byte> bytesIterated = new List<byte>();

            for (int i = 0; i < 4; i++)
            {
                int bit = stream.ReadByte();
                if (bit == -1) break; // End of stream reached
                bytesIterated.Add((byte)bit);
            }

            return CheckWordDocumentType(bytesIterated);
        }

        /// <summary>
        /// Checks if the given byte list matches the byte pattern of a Word document.
        /// </summary>
        /// <param name="bytesIterated">The byte list to check.</param>
        /// <returns>True if the byte list matches the byte pattern of a Word document, false otherwise.</returns>
        private static bool CheckWordDocumentType(List<byte> bytesIterated)
        {
            // Define byte patterns for different Word document file types
            Dictionary<string, string[]> documentTypes = new Dictionary<string, string[]>
            {
                { "docx", new string[] { "50", "4B", "03", "04" } }, // DOCX signature
                { "doc", new string[] { "D0", "CF", "11", "E0" } }, // DOC signature
                { "odt", new string[] { "3C", "6F", "66", "66", "69", "63", "65", "3A", "64", "6F", "63", "75", "6D", "65", "6E", "74", "2D", "63", "6F", "6E", "74", "65", "6E", "74" } } // ODT signature
            };

            foreach (var documentType in documentTypes)
            {
                if (IsDocumentType(bytesIterated.ToArray(), documentType.Value, documentType.Key))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the given byte array matches the given byte pattern.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <param name="pattern">The byte pattern to match.</param>
        /// <param name="documentTypeKey">The document type key.</param>
        /// <returns>True if the byte array matches the byte pattern, false otherwise.</returns>
        private static bool IsDocumentType(byte[] bytes, string[] pattern, string documentTypeKey)
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
    }
}