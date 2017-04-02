using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;
using YandexDisk.Client.Tests.Polyfils;

namespace YandexDisk.Client.Tests
{
    public class MetaInfoClientTests
    {
        [Test]
        public async Task GetDiskInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + "", 
                httpStatusCode: HttpStatusCode.OK, 
                result: @"
{
  ""trash_size"": 4631577437,
  ""total_space"": 319975063552,
  ""used_space"": 26157681270,
  ""system_folders"":
  {
    ""applications"": ""disk:/Приложения"",
    ""downloads"": ""disk:/Загрузки/""
  }
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Disk result = await diskClient.MetaInfo.GetDiskInfoAsync(CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.AreEqual(319975063552L, result.TotalSpace);
            Assert.AreEqual(4631577437L, result.TrashSize);
            Assert.AreEqual(26157681270L, result.UsedSpace);

            Assert.NotNull(result.SystemFolders);
            Assert.AreEqual("disk:/Приложения", result.SystemFolders.Applications);
            Assert.AreEqual("disk:/Загрузки/", result.SystemFolders.Downloads);
        }

        [Test]
        public async Task GetInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + UriPf.EscapePath("resources?sort=name&path=/&limit=20&offset=0"), 
                httpStatusCode: HttpStatusCode.OK,
                result: @"
{
  ""public_key"": ""HQsmHLoeyBlJf8Eu1jlmzuU+ZaLkjPkgcvmokRUCIo8="",
  ""_embedded"": {
    ""sort"": """",
    ""path"": ""disk:/foo"",
    ""items"": [
      {
        ""path"": ""disk:/foo/bar"",
        ""type"": ""dir"",
        ""name"": ""bar"",
        ""modified"": ""2014-04-22T10:32:49+04:00"",
        ""created"": ""2014-04-22T10:32:49+04:00""
      },
      {
        ""name"": ""photo.png"",
        ""preview"": ""https://downloader.disk.yandex.ru/preview/..."",
        ""created"": ""2014-04-21T14:57:13+04:00"",
        ""modified"": ""2014-04-21T14:57:14+04:00"",
        ""path"": ""disk:/foo/photo.png"",
        ""md5"": ""4334dc6379c8f95ddf11b9508cfea271"",
        ""type"": ""file"",
        ""mime_type"": ""image/png"",
        ""size"": 34567
      }
    ],
    ""limit"": 20,
    ""offset"": 0
  },
  ""name"": ""foo"",
  ""created"": ""2014-04-21T14:54:42+04:00"",
  ""custom_properties"": {""foo"":""1"", ""bar"":""2""},
  ""public_url"": ""https://yadi.sk/d/2AEJCiNTZGiYX"",
  ""modified"": ""2014-04-22T10:32:49+04:00"",
  ""path"": ""disk:/foo"",
  ""type"": ""dir""
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Resource result = await diskClient.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = "/",
                Limit = 20,
                Offset = 0,
                Sort = "name"
            }, CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.AreEqual("HQsmHLoeyBlJf8Eu1jlmzuU+ZaLkjPkgcvmokRUCIo8=", result.PublicKey);
            Assert.AreEqual("disk:/foo", result.Path);
            Assert.NotNull(result.Embedded);
            Assert.AreEqual("", result.Embedded.Sort);
            Assert.AreEqual("disk:/foo", result.Embedded.Path);
            Assert.IsNotEmpty(result.Embedded.Items);
            Assert.AreEqual(2, result.Embedded.Items.Count);

            Resource firstItem = result.Embedded.Items[0];
            Assert.NotNull(firstItem);
            Assert.AreEqual("disk:/foo/bar", firstItem.Path);
            Assert.AreEqual(ResourceType.Dir, firstItem.Type);
            Assert.AreEqual("bar", firstItem.Name);
            Assert.AreEqual(new DateTime(2014, 04, 22, 6, 32, 49), firstItem.Created);
            Assert.AreEqual(new DateTime(2014, 04, 22, 6, 32, 49), firstItem.Modified);

            Resource secondItem = result.Embedded.Items[1];
            Assert.NotNull(secondItem);
            Assert.AreEqual("photo.png", secondItem.Name);
            Assert.AreEqual("disk:/foo/photo.png", secondItem.Path);
            Assert.AreEqual("https://downloader.disk.yandex.ru/preview/...", secondItem.Preview);
            Assert.AreEqual(ResourceType.File, secondItem.Type);
            Assert.AreEqual("4334dc6379c8f95ddf11b9508cfea271", secondItem.Md5);
            Assert.AreEqual("image/png", secondItem.MimeType);
            Assert.AreEqual(34567, secondItem.Size);
            Assert.AreEqual(new DateTime(2014, 04, 21, 10, 57, 13), secondItem.Created.ToUniversalTime());
            Assert.AreEqual(new DateTime(2014, 04, 21, 10, 57, 14), secondItem.Modified.ToUniversalTime());

            Assert.AreEqual("foo", result.Name);
            //Assert.AreEqual("custom_properties", result.CustomProperties);
            Assert.AreEqual(new DateTime(2014, 04, 21, 10, 54, 42), result.Created.ToUniversalTime());
            Assert.AreEqual(new DateTime(2014, 04, 22, 6, 32, 49), result.Modified.ToUniversalTime());
            Assert.AreEqual("disk:/foo", result.Path);
            Assert.AreEqual(ResourceType.Dir, result.Type);

            //ToDo: Check undefined properties
        }

        [Test]
        public async Task GetTrashInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET",
                url: TestHttpClient.BaseUrl + UriPf.EscapePath("trash/resources?path=/foo/cat.png&limit=30&offset=50"),
                httpStatusCode: HttpStatusCode.OK,
                result: @"
{
  ""preview"": ""https://downloader.disk.yandex.ru/preview/..."",
  ""name"": ""cat.png"",
  ""created"": ""2014-07-16T13:07:45+04:00"",
  ""custom_properties"": {""foo"":""1"", ""bar"":""2""},
  ""origin_path"": ""disk:/foo/cat.png"",
  ""modified"": ""2014-07-16T13:07:45+04:00"",
  ""path"": ""trash:/cat.png"",
  ""md5"": ""02bab05c02537e53dedd408261e0aadf"",
  ""type"": ""file"",
  ""mime_type"": ""image/png"",
  ""size"": 903337
},
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Resource result = await diskClient.MetaInfo.GetTrashInfoAsync(new ResourceRequest
            {
                Path = "/foo/cat.png",
                Limit = 30,
                Offset = 50
            }, CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.AreEqual("cat.png", result.Name);
            Assert.AreEqual("trash:/cat.png", result.Path);
            Assert.AreEqual("https://downloader.disk.yandex.ru/preview/...", result.Preview);
            //Assert.AreEqual("custom_properties", result.CustomProperties);
            Assert.AreEqual("disk:/foo/cat.png", result.OriginPath);
            Assert.AreEqual(ResourceType.File, result.Type);
            Assert.AreEqual("02bab05c02537e53dedd408261e0aadf", result.Md5);
            Assert.AreEqual("image/png", result.MimeType);
            Assert.AreEqual(903337, result.Size);
            Assert.AreEqual(new DateTime(2014, 07, 16, 9, 07, 45), result.Created);
            Assert.AreEqual(new DateTime(2014, 07, 16, 9, 07, 45), result.Modified);
        }

        [Test]
        public async Task GetFilesInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + UriPf.EscapePath(@"resources/files?media_type=""audio,compressed""&limit=30&offset=50"),
                httpStatusCode: HttpStatusCode.OK,
                result: @"
{
  ""items"": [
    {
      ""name"": ""photo2.png"",
      ""preview"": ""https://downloader.disk.yandex.ru/preview/..."",
      ""created"": ""2014-04-22T14:57:13+04:00"",
      ""modified"": ""2014-04-22T14:57:14+04:00"",
      ""path"": ""disk:/foo/photo2.png"",
      ""md5"": ""53f4dc6379c8f95ddf11b9508cfea271"",
      ""type"": ""file"",
      ""mime_type"": ""image/png"",
      ""size"": 54321
    },
    {
      ""name"": ""photo1.png"",
      ""preview"": ""https://downloader.disk.yandex.ru/preview/..."",
      ""created"": ""2014-04-21T14:57:13+04:00"",
      ""modified"": ""2014-04-21T14:57:14+04:00"",
      ""path"": ""disk:/foo/photo1.png"",
      ""md5"": ""4334dc6379c8f95ddf11b9508cfea271"",
      ""type"": ""file"",
      ""mime_type"": ""image/png"",
      ""size"": 34567
    }
  ],
  ""limit"": 20,
  ""offset"": 10
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            FilesResourceList result = await diskClient.MetaInfo.GetFilesInfoAsync(new FilesResourceRequest
            {
                Limit = 30,
                Offset = 50,
                MediaType = new[] { MediaType.Audio, MediaType.Compressed}
            }, CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.AreEqual(20, result.Limit);
            Assert.AreEqual(10, result.Offset);
            Assert.IsNotEmpty(result.Items);
            Assert.AreEqual(2, result.Items.Count);

            var firstItem = result.Items[0];
            Assert.AreEqual("photo2.png", firstItem.Name);
            Assert.AreEqual("https://downloader.disk.yandex.ru/preview/...", firstItem.Preview);
            Assert.AreEqual("disk:/foo/photo2.png", firstItem.Path);
            Assert.AreEqual(ResourceType.File, firstItem.Type);
            Assert.AreEqual("53f4dc6379c8f95ddf11b9508cfea271", firstItem.Md5);
            Assert.AreEqual("image/png", firstItem.MimeType);
            Assert.AreEqual(54321, firstItem.Size);
            Assert.AreEqual(new DateTime(2014, 04, 22, 10, 57, 13), firstItem.Created);
            Assert.AreEqual(new DateTime(2014, 04, 22, 10, 57, 14), firstItem.Modified);

            var secondItem = result.Items[1];
            Assert.AreEqual("photo1.png", secondItem.Name);
            Assert.AreEqual("https://downloader.disk.yandex.ru/preview/...", secondItem.Preview);
            Assert.AreEqual("disk:/foo/photo1.png", secondItem.Path);
            Assert.AreEqual(ResourceType.File, secondItem.Type);
            Assert.AreEqual("4334dc6379c8f95ddf11b9508cfea271", secondItem.Md5);
            Assert.AreEqual("image/png", secondItem.MimeType);
            Assert.AreEqual(34567, secondItem.Size);
            Assert.AreEqual(new DateTime(2014, 04, 21, 10, 57, 13), secondItem.Created);
            Assert.AreEqual(new DateTime(2014, 04, 21, 10, 57, 14), secondItem.Modified);
        }

        [Test]
        public async Task GetLastUploadedInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + UriPf.EscapePath(@"resources/last-uploaded?media_type=""audio,executable""&limit=20"), 
                httpStatusCode: HttpStatusCode.OK,
                result: @"
{
  ""items"": [
      {
        ""name"": ""photo2.png"",
        ""preview"": ""https://downloader.disk.yandex.ru/preview/..."",
        ""created"": ""2014-04-22T14:57:13+04:00"",
        ""modified"": ""2014-04-22T14:57:14+04:00"",
        ""path"": ""disk:/foo/photo2.png"",
        ""md5"": ""53f4dc6379c8f95ddf11b9508cfea271"",
        ""type"": ""file"",
        ""mime_type"": ""image/png"",
        ""size"": 54321
      },
      {
        ""name"": ""photo1.png"",
        ""preview"": ""https://downloader.disk.yandex.ru/preview/..."",
        ""created"": ""2014-04-21T14:57:13+04:00"",
        ""modified"": ""2014-04-21T14:57:14+04:00"",
        ""path"": ""disk:/foo/photo1.png"",
        ""md5"": ""4334dc6379c8f95ddf11b9508cfea271"",
        ""type"": ""file"",
        ""mime_type"": ""image/png"",
        ""size"": 34567
      }
    ],
    ""limit"": 20,
  }
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            LastUploadedResourceList result = await diskClient.MetaInfo.GetLastUploadedInfoAsync(new LastUploadedResourceRequest
            {
                Limit = 20,
                MediaType = new[] { MediaType.Audio, MediaType.Executable }
            }, CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.AreEqual(20, result.Limit);
            Assert.IsNotEmpty(result.Items);
            Assert.AreEqual(2, result.Items.Count);

            var firstItem = result.Items[0];
            Assert.AreEqual("photo2.png", firstItem.Name);
            Assert.AreEqual("https://downloader.disk.yandex.ru/preview/...", firstItem.Preview);
            Assert.AreEqual("disk:/foo/photo2.png", firstItem.Path);
            Assert.AreEqual(ResourceType.File, firstItem.Type);
            Assert.AreEqual("53f4dc6379c8f95ddf11b9508cfea271", firstItem.Md5);
            Assert.AreEqual("image/png", firstItem.MimeType);
            Assert.AreEqual(54321, firstItem.Size);
            Assert.AreEqual(new DateTime(2014, 04, 22, 10, 57, 13), firstItem.Created);
            Assert.AreEqual(new DateTime(2014, 04, 22, 10, 57, 14), firstItem.Modified);

            var secondItem = result.Items[1];
            Assert.AreEqual("photo1.png", secondItem.Name);
            Assert.AreEqual("https://downloader.disk.yandex.ru/preview/...", secondItem.Preview);
            Assert.AreEqual("disk:/foo/photo1.png", secondItem.Path);
            Assert.AreEqual(ResourceType.File, secondItem.Type);
            Assert.AreEqual("4334dc6379c8f95ddf11b9508cfea271", secondItem.Md5);
            Assert.AreEqual("image/png", secondItem.MimeType);
            Assert.AreEqual(34567, secondItem.Size);
            Assert.AreEqual(new DateTime(2014, 04, 21, 10, 57, 13), secondItem.Created);
            Assert.AreEqual(new DateTime(2014, 04, 21, 10, 57, 14), secondItem.Modified);
        }


        [Test]
        public async Task AppendCustomPropertiesTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "PATCH",
                url: TestHttpClient.BaseUrl + UriPf.EscapePath(@"resources?path=/foo"),
                httpStatusCode: HttpStatusCode.OK,
                request: @"{""custom_properties"":{""foo"":""1"",""bar"":""2""}}",
                result: @"
{
  ""public_key"": ""HQsmHLoeyBlJf8Eu1jlmzuU+ZaLkjPkgcvmokRUCIo8="",
  ""_embedded"": {
    ""sort"": """",
    ""path"": ""disk:/foo"",
    ""items"": [
      {
        ""path"": ""disk:/foo/bar"",
        ""type"": ""dir"",
        ""name"": ""bar"",
        ""modified"": ""2014-04-22T10:32:49+04:00"",
        ""created"": ""2014-04-22T10:32:49+04:00""
      },
      {
        ""name"": ""photo.png"",
        ""preview"": ""https://downloader.disk.yandex.ru/preview/..."",
        ""created"": ""2014-04-21T14:57:13+04:00"",
        ""modified"": ""2014-04-21T14:57:14+04:00"",
        ""path"": ""disk:/foo/photo.png"",
        ""md5"": ""4334dc6379c8f95ddf11b9508cfea271"",
        ""type"": ""file"",
        ""mime_type"": ""image/png"",
        ""size"": 34567
      }
    ],
    ""limit"": 20,
    ""offset"": 0
  },
  ""name"": ""foo"",
  ""created"": ""2014-04-21T14:54:42+04:00"",
  ""custom_properties"": {""foo"":""1"", ""bar"":""2""},
  ""public_url"": ""https://yadi.sk/d/2AEJCiNTZGiYX"",
  ""modified"": ""2014-04-22T10:32:49+04:00"",
  ""path"": ""disk:/foo"",
  ""type"": ""dir""
}
");

            var diskClient = new DiskHttpApi(TestHttpClient.BaseUrl,
                                             TestHttpClient.ApiKey,
                                             logSaver: null,
                                             httpClient: httpClientTest);

            Resource result = await diskClient.MetaInfo.AppendCustomProperties("/foo", new Dictionary<string, string>  {
                { "foo", "1" },
                { "bar", "2" }
            }, CancellationToken.None).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.IsNotEmpty(result.CustomProperties);
            Assert.AreEqual("1", result.CustomProperties["foo"]);
            Assert.AreEqual("2", result.CustomProperties["bar"]);
        }
    }
}
