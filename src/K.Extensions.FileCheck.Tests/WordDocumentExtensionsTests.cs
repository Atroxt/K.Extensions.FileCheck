namespace K.Extensions.FileCheck.Tests
{
    [TestClass]
    public class WordDocumentExtensionsTests
    {
        private readonly string _testDataPath = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Data\\";

        [TestMethod]
        [DataRow("Document.pdf", false)]
        [DataRow("Document.docx", true)]
        [DataRow("Document.doc", true)]
        [DataRow("Document.odt", true)]
        [DataRow("", false)]
        public void IsWordDocument_WithByteArray(string document, bool expected)
        {
            byte[] imageBytes = string.IsNullOrEmpty(document) ? Array.Empty<byte>() : File.ReadAllBytes($"{_testDataPath}{document}");
            bool result = imageBytes.IsWordDocument();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("Image.bmp", false)]
        [DataRow("Document.docx", true)]
        [DataRow("Document.doc", true)]
        [DataRow("Document.odt", true)]
        public void IsWordDocument_WithImageFileStream(string document, bool expected)
        {
            using (var stream = File.OpenRead($"{_testDataPath}{document}"))
            {
                bool result = stream.IsWordDocument();
                Assert.AreEqual(expected, result);
            }
        }
        [TestMethod]
        public void IsWordDocument_WithEmptyStream()
        {
            using (var stream = new MemoryStream(new byte[1], true))
            {
                // close stream then u can't read the Stream
                stream.Close();
                bool result = stream.IsWordDocument();
                Assert.AreEqual(false, result);
            }
        }
    }
}