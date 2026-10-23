using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevenZip;

namespace ME3TweaksModManager.Tests
{
    [TestClass]
    public class NativeArchiveTests
    {
        // Deliberately independent of GlobalTest.Init(), game data and online services.
        [TestMethod]
        public void NativeLibraryExtractsAndCreatesModArchives()
        {
            var root = Path.Combine(Path.GetTempPath(), "M3ArchiveTest-" + Guid.NewGuid());
            Directory.CreateDirectory(root);
            try
            {
                var zipPath = Path.Combine(root, "mod.zip");
                var expected = Encoding.UTF8.GetBytes("synthetic mod content");
                using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                using (var entry = zip.CreateEntry("folder/content.txt").Open())
                    entry.Write(expected);

                using (var archive = new SevenZipExtractor(zipPath))
                {
                    Assert.AreEqual(1u, archive.FilesCount);
                    using var content = new MemoryStream();
                    archive.ExtractFile(0, content);
                    CollectionAssert.AreEqual(expected, content.ToArray());
                    archive.ExtractArchive(Path.Combine(root, "extracted"));
                }
                CollectionAssert.AreEqual(expected,
                    File.ReadAllBytes(Path.Combine(root, "extracted", "folder", "content.txt")));

                var sevenZipPath = Path.Combine(root, "mod.7z");
                new SevenZipCompressor().CompressFiles(sevenZipPath,
                    Path.Combine(root, "extracted", "folder", "content.txt"));
                using (var archive = new SevenZipExtractor(sevenZipPath))
                using (var content = new MemoryStream())
                {
                    archive.ExtractFile(0, content);
                    CollectionAssert.AreEqual(expected, content.ToArray());
                }
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}
