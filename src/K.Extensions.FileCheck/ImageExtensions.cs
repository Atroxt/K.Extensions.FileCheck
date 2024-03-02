using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace K.Extensions.FileCheck
{
    /// <summary>
    /// Provides extension methods for image file checks.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Checks if the given byte array represents an image file.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <returns>True if the byte array represents an image file, false otherwise.</returns>
        public static bool IsImage(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < 8)
                return false;

            List<byte> bytesIterated = bytes.Take(8).ToList();

            return CheckImageType(bytesIterated);
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

            List<byte> bytesIterated = new List<byte>();

            for (int i = 0; i < 8; i++)
            {
                int bit = stream.ReadByte();
                if (bit == -1) break; // End of stream reached
                bytesIterated.Add((byte)bit);
            }

            return CheckImageType(bytesIterated);
        }

        /// <summary>
        /// Checks if the given byte list matches the byte pattern of an image file.
        /// </summary>
        /// <param name="bytesIterated">The byte list to check.</param>
        /// <returns>True if the byte list matches the byte pattern of an image file, false otherwise.</returns>
        private static bool CheckImageType(List<byte> bytesIterated)
        {
            // Define byte patterns for different image file types
            Dictionary<string, string[]> imageTypes = new Dictionary<string, string[]>
            {
                { "jpg", new string[] { "FF", "D8" } },
                { "bmp", new string[] { "42", "4D" } },
                { "gif", new string[] { "47", "49", "46" } },
                { "png", new string[] { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" } }
            };

            foreach (var imageType in imageTypes)
            {
                if (IsImageType(bytesIterated.ToArray(), imageType.Value))
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
        private static bool IsImageType(byte[] bytes, string[] pattern)
        {
            if (bytes.Length < pattern.Length) return false;

            for (int i = 0; i < pattern.Length; i++)
            {
                if (bytes[i] != Convert.ToByte(pattern[i], 16))
                    return false;
            }

            return true;
        }
    }
}