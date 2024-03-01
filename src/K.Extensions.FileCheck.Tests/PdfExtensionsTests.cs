namespace K.Extensions.FileCheck.Tests
{
    [TestClass]
    public class PdfExtensionsTests
    {
        private readonly string _testDataPath = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Data\\";

        [TestMethod]
        [DataRow("Document.pdf", true)]
        [DataRow("Document.docx", false)]
        [DataRow("", false)]
        public void IsPdf_WithByteArray(string document, bool expected)
        {
            byte[] pdfBytes = string.IsNullOrEmpty(document) ? Array.Empty<byte>() : File.ReadAllBytes($"{_testDataPath}{document}");
            bool result = pdfBytes.IsPdf();
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        [DataRow("Document.pdf", true)]
        [DataRow("Document.docx", false)]
        public void IsPdf_WithPdfFileStream(string document, bool expected)
        {
            using (var stream = File.OpenRead($"{_testDataPath}{document}"))
            {
                bool result = stream.IsPdf();
                Assert.AreEqual(expected, result);
            }
        }
        [TestMethod]
        public void IsPdf_WithEmptyStream()
        {
            using (var stream = new MemoryStream(new byte[1],true))
            {
                // close stream then u can't read the Stream
                stream.Close();
                bool result = stream.IsPdf();
                Assert.AreEqual(false, result);
            }
        }
        [TestMethod]
        public void IsPdf_WithStreamLowerThen5()
        {
            using (var stream = new MemoryStream(new byte[4], true))
            {
                bool result = stream.IsPdf();
                Assert.AreEqual(false, result);
            }
        }

    }
}
