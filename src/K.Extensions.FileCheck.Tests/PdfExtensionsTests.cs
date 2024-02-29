namespace K.Extensions.FileCheck.Tests
{
    [TestClass]
    public class PdfExtensionsTests
    {
        private readonly string _testDataPath = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Data\\";

        [TestMethod]
        [DataRow("Document.pdf", true)]
        [DataRow("Document.docx", false)]
        public void IsPdf_WithByteArray(string document, bool expected)
        {
            byte[] pdfBytes = File.ReadAllBytes($"{_testDataPath}{document}");
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

    }
}
