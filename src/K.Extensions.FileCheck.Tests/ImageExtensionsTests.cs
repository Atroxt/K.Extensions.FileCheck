namespace K.Extensions.FileCheck.Tests
{
    [TestClass]
    public class ImageExtensionsTests
    {
        private readonly string _testDataPath = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Data\\";

        [TestMethod]
        [DataRow("Image.jpg", true)]
        [DataRow("Image.gif", true)]
        [DataRow("Image.png", true)]
        [DataRow("Image.bmp", true)]
        [DataRow("Document.docx", false)]
        public void IsImage_WithByteArray(string document, bool expected)
        {
            byte[] imageBytes = File.ReadAllBytes($"{_testDataPath}{document}");
            bool result = imageBytes.IsImage();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("Image.jpg", true)]
        [DataRow("Image.gif", true)]
        [DataRow("Image.png", true)]
        [DataRow("Image.bmp", true)]
        [DataRow("Document.docx", false)]
        public void IsImage_WithImageFileStream(string document, bool expected)
        {
            using (var stream = File.OpenRead($"{_testDataPath}{document}"))
            {
                bool result = stream.IsImage();
                Assert.AreEqual(expected, result);
            }
        }
    }
}