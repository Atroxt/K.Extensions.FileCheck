using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace K.Extensions.FileCheck
{
    public static class ImageExtensions
    {
        public static bool IsImage(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < 8)
                throw new ArgumentException("Byte array is null or too short to determine image type");

            List<byte> bytesIterated = bytes.Take(8).ToList();

            return CheckImageType(bytesIterated);
        }
        public static bool IsImage(this Stream stream)
        {
            if (stream == null || !stream.CanRead)
                throw new ArgumentException("Stream is null or not readable");

            List<byte> bytesIterated = new List<byte>();

            for (int i = 0; i < 8; i++)
            {
                int bit = stream.ReadByte();
                if (bit == -1) break; // End of stream reached
                bytesIterated.Add((byte)bit);
            }

            return CheckImageType(bytesIterated);
        }
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
