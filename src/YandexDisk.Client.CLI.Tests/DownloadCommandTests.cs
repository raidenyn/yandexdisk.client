using NUnit.Framework;
using System;
using System.IO;
using YandexDisk.Client.Tests;

namespace YandexDisk.Client.CLI.Tests
{
    public class DownloadCommandTests
    {
        [Test]
        public void DownloadZippedFolderTest()
        {
            var program = new TestYandexDiskCliProgram(
                new TestHttpClient((request) => {
                    if (request.RequestUri.ToString() == "https://downloader.dst.yandex.ru/disk/abc")
                    {
                        return new ResponseContent { Text = "Test folder content" };
                    }
                    
                    if (request.RequestUri.ToString().Contains("download")) {
                        return new ResponseContent { Text = @"
                        {
                          ""href"": ""https://downloader.dst.yandex.ru/disk/abc"",
                          ""method"": ""GET"",
                          ""templated"": false
                        }
                        " };
                    }

                    if (request.RequestUri.ToString().Contains("resource"))
                    {
                        return new ResponseContent { Text = @"{
                          ""name"": ""foo"",
                          ""type"": ""dir""
                        }" };
                    }

                    throw new Exception("Unknown function call");
                })
            );

            var tempFolder = Path.GetTempPath();
            var file = "/foo";
            var target = tempFolder + file + ".zip";

            int result = program.Run(new[] {
                "download",
                "-t", "access-tocken",
                file,
                tempFolder
            });

            Assert.AreEqual(0, result);
            Assert.IsTrue(File.Exists(target), "Temp file should exist");

            File.Delete(target);
        }

        [Test]
        public void DownloadUnzippedFolderTest()
        {
            var program = new TestYandexDiskCliProgram(
                new TestHttpClient((request) => {
                    if (request.RequestUri.ToString() == "https://downloader.dst.yandex.ru/disk/abc")
                    {
                        return new ResponseContent { Bites = TestFiles.folder };
                    }

                    if (request.RequestUri.ToString().Contains("download"))
                    {
                        return new ResponseContent { Text = @"
                        {
                          ""href"": ""https://downloader.dst.yandex.ru/disk/abc"",
                          ""method"": ""GET"",
                          ""templated"": false
                        }
                        " };
                    }

                    if (request.RequestUri.ToString().Contains("resource"))
                    {
                        return new ResponseContent { Text = @"{
                          ""name"": ""test"",
                          ""type"": ""dir""
                        }" };
                    }

                    throw new Exception("Unknown function call");
                })
            );

            var tempFolder = Path.GetTempPath();
            var folder = "/test";
            var target = tempFolder + folder;

            int result = program.Run(new[] {
                "download",
                "-t", "access-tocken",
                "-u",
                folder,
                tempFolder
            });

            Assert.AreEqual(0, result);
            Assert.IsTrue(Directory.Exists(target), "New folder should exist");
            Assert.IsFalse(File.Exists(target + ".zip"), "Temp file should not exist");

            Directory.Delete(target, recursive: true);
        }

        [Test]
        public void DownloadFileTest()
        {
            var program = new TestYandexDiskCliProgram(
                new TestHttpClient((request) => {
                    if (request.RequestUri.ToString() == "https://downloader.dst.yandex.ru/disk/abc")
                    {
                        return new ResponseContent { Text = @"Test file content" };
                    }

                    if (request.RequestUri.ToString().Contains("download"))
                    {
                        return new ResponseContent { Text = @"
                        {
                          ""href"": ""https://downloader.dst.yandex.ru/disk/abc"",
                          ""method"": ""GET"",
                          ""templated"": false
                        }
                        " };
                    }

                    if (request.RequestUri.ToString().Contains("resource"))
                    {
                        return new ResponseContent { Text = @"{
                          ""name"": ""foo"",
                          ""type"": ""file""
                        }" };
                    }

                    throw new Exception("Unknown function call");
                })
            );

            var tempFolder = Path.GetTempPath();
            var file = "/foo";
            var target = tempFolder + file;

            int result = program.Run(new[] {
                "download",
                "-t", "access-tocken",
                file,
                tempFolder
            });

            Assert.AreEqual(0, result);
            Assert.IsTrue(File.Exists(target), "Temp file should exist");

            File.Delete(target);
        }

        [Test]
        public void ErrorDownloadTest()
        {
            var program = new TestYandexDiskCliProgram(
                new TestHttpClient((request) => {
                    throw new Exception("Emulating exception on request sending.");
                })
            );

            var tempFolder = Path.GetTempPath();
            var file = "/foo";
            var target = tempFolder + file + ".zip";

            int result = program.Run(new[] {
                "download",
                "-t", "access-tocken",
                file,
                tempFolder
            });

            Assert.AreEqual(-1, result);
            Assert.IsFalse(File.Exists(target), "Temp file should not exist");
        }
    }
}
