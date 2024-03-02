namespace K.Extensions.FileCheck.Tests
{
    [TestClass]
    public class ExcelExtensionsTests
    {
        private readonly string _testDataPath = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}Data\\";

        [TestMethod]
        [DataRow("Document.pdf", false)]
        [DataRow("Excel.xlsx", true)]
        [DataRow("Excel97.xls", true)]
       [DataRow("", false)]
        public void IsExcelDocument_WithByteArray(string document, bool expected)
        {
            byte[] imageBytes = string.IsNullOrEmpty(document) ? Array.Empty<byte>() : File.ReadAllBytes($"{_testDataPath}{document}");
            bool result = imageBytes.IsExcelDocument();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("Excel.xlsx", true)]
        [DataRow("Excel97.xls", true)]
        public void IsExcelDocument_WithImageFileStream(string document, bool expected)
        {
            using (var stream = File.OpenRead($"{_testDataPath}{document}"))
            {
                bool result = stream.IsExcelDocument();
                Assert.AreEqual(expected, result);
            }
        }
        [TestMethod]
        public void IsExcelDocument_WithEmptyStream()
        {
            using (var stream = new MemoryStream(new byte[1], true))
            {
                // close stream then u can't read the Stream
                stream.Close();
                bool result = stream.IsExcelDocument();
                Assert.AreEqual(false, result);
            }
        }
    }
}