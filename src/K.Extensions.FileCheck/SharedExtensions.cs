using System;

namespace K.Extensions.FileCheck
{
    internal static class SharedExtensions
    {
        /// <summary>
        /// Checks if the byte array matches the ZIP signature.
        /// </summary>
        /// <param name="bytesIterated">The byte array to check.</param>
        /// <returns>True if it is a ZIP file (possibly a DOCX file), false otherwise.</returns>
        internal static bool IsZipFile(ReadOnlySpan<byte> bytesIterated)
        {
            var zipSignature = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // ZIP file signature as well for docx and odt
            return CheckSignature(bytesIterated, zipSignature.AsSpan());
        }

        /// <summary>
        /// Checks if the given byte array matches the given archive file signature.
        /// </summary>
        /// <returns>True if the byte array matches the archive file signature, false otherwise.</returns>
        internal static bool CheckSignature(ReadOnlySpan<byte> bytes, ReadOnlySpan<byte> signature)
        {
            if (bytes.Length < signature.Length)
                return false;

            for (int i = 0; i < signature.Length; i++)
            {
                if (bytes[i] != signature[i])
                    return false;
            }

            return true;
        }
    }
}
