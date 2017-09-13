using NUnit.Framework;
using System;
using System.Net.Http;
using System.Reflection;
using YandexDisk.Client.Tests;

namespace YandexDisk.Client.CLI.Tests
{
    public class UploadCommandTests
    {
        [Test]
        public void SimpleUploadTest()
        {
            var program = new TestYandexDiskCliProgram(
                new TestHttpClient((request) => {
                    if (request.Method == HttpMethod.Get) {
                        return @"
                        {
                          ""href"": ""https://uploader1d.dst.yandex.net:443/upload-target/..."",
                          ""method"": ""PUT"",
                          ""templated"": false
                        }
                        ";
                    }

                    if (request.Method == HttpMethod.Put)
                    {
                        return @"";
                    }

                    throw new Exception("Unknown function call");
                })
            );

            int result = program.Run(new[] {
                "upload",
                "-t", "access-tocken",
                Assembly.GetExecutingAssembly().Location,
                "https://uploader1d.dst.yandex.net/upload-target/"
            });

            Assert.AreEqual(0, result);
        }

        [Test]
        public void ErrorUploadTest()
        {
            var program = new TestYandexDiskCliProgram(
                new TestHttpClient((request) => {
                    throw new Exception("Emulating exception on request sending.");
                })
            );

            int result = program.Run(new[] {
                "upload",
                "-t", "access-tocken",
                Assembly.GetExecutingAssembly().Location,
                "https://uploader1d.dst.yandex.net/upload-target/"
            });

            Assert.AreEqual(-1, result);
        }
    }
}
