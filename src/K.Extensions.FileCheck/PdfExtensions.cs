using System.IO;

namespace K.Extensions.FileCheck
{
    public static class PdfExtensions
    {
        public static bool IsPdf(this byte[] bytes)
        {
            if (bytes == null || bytes.Length < 5)
                return false;

            return bytes[0] == 0x25 &&  // %
                   bytes[1] == 0x50 &&  // P
                   bytes[2] == 0x44 &&  // D
                   bytes[3] == 0x46 &&  // F
                   bytes[4] == 0x2D;    // -
        }

        public static bool IsPdf(this Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            byte[] header = new byte[5];
            int bytesRead = stream.Read(header, 0, 5);

            if (bytesRead < 5)
                return false;

            return header[0] == 0x25 &&  // %
                   header[1] == 0x50 &&  // P
                   header[2] == 0x44 &&  // D
                   header[3] == 0x46 &&  // F
                   header[4] == 0x2D;    // -
        }
    }
}
