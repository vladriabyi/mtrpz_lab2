using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Program.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void TestParseCommandLine_DefaultFormat()
        {
            // Arrange
            string[] args = new string[] { };

            // Act
            string result = Program.ParseCommandLine(args);

            // Assert
            Assert.AreEqual("html", result);
        }

        [TestMethod]
        public void TestParseCommandLine_CustomFormat()
        {
            // Arrange
            string[] args = new string[] { "--format=pdf" };

            // Act
            string result = Program.ParseCommandLine(args);

            // Assert
            Assert.AreEqual("pdf", result);
        }

        [TestMethod]
        public void TestCreateHTML()
        {
            // Arrange
            string text = "Test text";
            string tempPath = Path.GetTempPath(); // Використовуємо тимчасову директорію
            string name = "test";
            string expectedFilePath = Path.Combine(tempPath, name + ".html");

            // Act
            Program.CreateHTML(text, tempPath, name);

            // Assert
            Assert.IsTrue(File.Exists(expectedFilePath));

            // Clean up
            if (File.Exists(expectedFilePath))
            {
                File.Delete(expectedFilePath);
            }
        }

        [TestMethod]
        public void TestCheckItalic()
        {
            // Arrange
            string text = "_italic_";
            int index = 0;

            // Act
            Program.CheckItalic(ref text, index);

            // Assert
            Assert.AreEqual("<i>italic</i>", text);
        }

        [TestMethod]
        public void TestCheckBold()
        {
            // Arrange
            string text = "**bold**";
            int index = 0;

            // Act
            Program.CheckBold(ref text, index);

            // Assert
            Assert.AreEqual("<b>bold</b>", text);
        }

        [TestMethod]
        public void TestCheckMono()
        {
            // Arrange
            string text = "`monospace`";
            int index = 0;

            // Act
            Program.CheckMono(ref text, index);

            // Assert
            Assert.AreEqual("<tt>monospace</tt>", text);
        }

        [TestMethod]
        public void TestCheckPref()
        {
            // Arrange
            string text = "```code```";
            int index = 0;

            // Act
            Program.CheckPref(ref text, ref index);

            // Assert
            Assert.AreEqual("<pre>code</pre>", text);
        }

        [TestMethod]
        public void TestCheckParag()
        {
            // Arrange
            string text = "Paragraph\nText\n";
            string expectedText = "<p>Paragraph</p><p>Text</p>";

            // Act
            Program.CheckParag(ref text);

            // Assert
            Assert.AreEqual(expectedText, text);
        }

        [TestMethod]
        public void TestAddHTMLStructure()
        {
            // Arrange
            string text = "Test text";
            string expectedText = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
</head>
<body>
Test text
</body>
</html>";

            // Act
            Program.AddHTMLStructure(ref text);

            // Assert
            Assert.AreEqual(expectedText, text);
        }
    }
}
