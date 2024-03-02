using System;
using System.Collections.Generic;
using System.IO;

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

            List<byte> bytesIterated = new List<byte>(bytes).GetRange(0, 4);

            return CheckExcelDocumentType(bytesIterated);
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

            List<byte> bytesIterated = new List<byte>();

            for (int i = 0; i < 4; i++)
            {
                int bit = stream.ReadByte();
                if (bit == -1) break; // End of stream reached
                bytesIterated.Add((byte)bit);
            }

            return CheckExcelDocumentType(bytesIterated);
        }

        /// <summary>
        /// Checks if the given byte list matches the byte pattern of an Excel document.
        /// </summary>
        /// <param name="bytesIterated">The byte list to check.</param>
        /// <returns>True if the byte list matches the byte pattern of an Excel document, false otherwise.</returns>
        private static bool CheckExcelDocumentType(List<byte> bytesIterated)
        {
            // Define byte patterns for different Excel document file types
            Dictionary<string, string[]> documentTypes = new Dictionary<string, string[]>
            {
                { "xlsx", new string[] { "50", "4B", "03", "04" } }, // XLSX signature
                { "xls", new string[] { "D0", "CF", "11", "E0" } } // XLS signature
            };

            foreach (var documentType in documentTypes)
            {
                if (IsDocumentType(bytesIterated.ToArray(), documentType.Value))
                    return true;
            }

            return false;
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
    }
}
