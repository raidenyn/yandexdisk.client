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
                        return @"Test file content";
                    }
                    
                    if (request.RequestUri.ToString().Contains("download")) {
                        return @"
                        {
                          ""href"": ""https://downloader.dst.yandex.ru/disk/abc"",
                          ""method"": ""GET"",
                          ""templated"": false
                        }
                        ";
                    }

                    if (request.RequestUri.ToString().Contains("resource"))
                    {
                        return @"{
                          ""name"": ""foo"",
                          ""type"": ""dir""
                        }";
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
        public void DownloadFileTest()
        {
            var program = new TestYandexDiskCliProgram(
                new TestHttpClient((request) => {
                    if (request.RequestUri.ToString() == "https://downloader.dst.yandex.ru/disk/abc")
                    {
                        return @"Test file content";
                    }

                    if (request.RequestUri.ToString().Contains("download"))
                    {
                        return @"
                        {
                          ""href"": ""https://downloader.dst.yandex.ru/disk/abc"",
                          ""method"": ""GET"",
                          ""templated"": false
                        }
                        ";
                    }

                    if (request.RequestUri.ToString().Contains("resource"))
                    {
                        return @"{
                          ""name"": ""foo"",
                          ""type"": ""file""
                        }";
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
