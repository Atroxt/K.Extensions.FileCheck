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
        internal static bool IsZipFile(byte[] bytesIterated)
        {
            string[] zipSignature = new string[] { "50", "4B", "03", "04" }; // ZIP file signature as well for docx and odt
            return CheckSignature(bytesIterated, zipSignature);
        }

        /// <summary>
        /// Checks if the given byte array matches the given archive file signature.
        /// </summary>
        /// <param name="bytes">The byte array to check.</param>
        /// <param name="signature">The archive file signature to match.</param>
        /// <returns>True if the byte array matches the archive file signature, false otherwise.</returns>
        internal static bool CheckSignature(byte[] bytes, string[] signature)
        {
            if (bytes.Length < signature.Length)
                return false;

            for (int i = 0; i < signature.Length; i++)
            {
                if (bytes[i] != Convert.ToByte(signature[i], 16))
                    return false;
            }

            return true;
        }
    }
}
