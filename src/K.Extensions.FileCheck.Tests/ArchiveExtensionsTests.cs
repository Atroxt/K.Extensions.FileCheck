namespace K.Extensions.FileCheck.Tests
{
    [TestClass]
    public class ArchiveExtensionsTests
    {
        private readonly string _testDataPath = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Data\\";

        [TestMethod]
        [DataRow("Image.jpg", false)]
        [DataRow("Zip.zip", true)]
        [DataRow("", false)]
        public void IsArchive_WithByteArray(string document, bool expected)
        {
            byte[] imageBytes = string.IsNullOrEmpty(document) ? Array.Empty<byte>() : File.ReadAllBytes($"{_testDataPath}{document}");
            bool result = imageBytes.IsArchive();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("Image.jpg", false)]
        [DataRow("Zip.zip", true)]
        [DataRow("Document.docx", true)]
        public void IsArchive_WithImageFileStream(string document, bool expected)
        {
            using (var stream = File.OpenRead($"{_testDataPath}{document}"))
            {
                bool result = stream.IsArchive();
                Assert.AreEqual(expected, result);
            }
        }
        [TestMethod]
        public void IsArchive_WithEmptyStream()
        {
            using (var stream = new MemoryStream(new byte[1], true))
            {
                // close stream then u can't read the Stream
                stream.Close();
                bool result = stream.IsArchive();
                Assert.AreEqual(false, result);
            }
        }
    }
}