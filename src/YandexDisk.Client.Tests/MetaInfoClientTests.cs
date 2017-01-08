using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;
using Xunit;

namespace YandexDisk.Client.Tests
{
    public class MetaInfoClientTests
    {
        [Fact]
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
            Assert.Equal(319975063552L, result.TotalSpace);
            Assert.Equal(4631577437L, result.TrashSize);
            Assert.Equal(26157681270L, result.UsedSpace);

            Assert.NotNull(result.SystemFolders);
            Assert.Equal("disk:/Приложения", result.SystemFolders.Applications);
            Assert.Equal("disk:/Загрузки/", result.SystemFolders.Downloads);
        }

        [Fact]
        public async Task GetInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + "resources?sort=name&path=%2F&limit=20&offset=0", 
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
            Assert.Equal("HQsmHLoeyBlJf8Eu1jlmzuU+ZaLkjPkgcvmokRUCIo8=", result.PublicKey);
            Assert.Equal("disk:/foo", result.Path);
            Assert.NotNull(result.Embedded);
            Assert.Equal("", result.Embedded.Sort);
            Assert.Equal("disk:/foo", result.Embedded.Path);
            Assert.NotEmpty(result.Embedded.Items);
            Assert.Equal(2, result.Embedded.Items.Count);

            Resource firstItem = result.Embedded.Items[0];
            Assert.NotNull(firstItem);
            Assert.Equal("disk:/foo/bar", firstItem.Path);
            Assert.Equal(ResourceType.Dir, firstItem.Type);
            Assert.Equal("bar", firstItem.Name);
            Assert.Equal(new DateTime(2014, 04, 22, 10, 32, 49, DateTimeKind.Local), firstItem.Created);
            Assert.Equal(new DateTime(2014, 04, 22, 10, 32, 49, DateTimeKind.Local), firstItem.Modified);

            Resource secondItem = result.Embedded.Items[1];
            Assert.NotNull(secondItem);
            Assert.Equal("photo.png", secondItem.Name);
            Assert.Equal("disk:/foo/photo.png", secondItem.Path);
            Assert.Equal("https://downloader.disk.yandex.ru/preview/...", secondItem.Preview);
            Assert.Equal(ResourceType.File, secondItem.Type);
            Assert.Equal("4334dc6379c8f95ddf11b9508cfea271", secondItem.Md5);
            Assert.Equal("image/png", secondItem.MimeType);
            Assert.Equal(34567, secondItem.Size);
            Assert.Equal(new DateTime(2014, 04, 21, 14, 57, 13, DateTimeKind.Local), secondItem.Created);
            Assert.Equal(new DateTime(2014, 04, 21, 14, 57, 14, DateTimeKind.Local), secondItem.Modified);

            Assert.Equal("foo", result.Name);
            //Assert.Equal("custom_properties", result.CustomProperties);
            Assert.Equal(new DateTime(2014, 04, 21, 14, 54, 42, DateTimeKind.Local), result.Created);
            Assert.Equal(new DateTime(2014, 04, 22, 10, 32, 49, DateTimeKind.Local), result.Modified);
            Assert.Equal("disk:/foo", result.Path);
            Assert.Equal(ResourceType.Dir, result.Type);

            //ToDo: Check undefined properties
        }

        [Fact]
        public async Task GetTrashInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + "trash/resources?path=%2Ffoo%2Fcat.png&limit=30&offset=50", 
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
            Assert.Equal("cat.png", result.Name);
            Assert.Equal("trash:/cat.png", result.Path);
            Assert.Equal("https://downloader.disk.yandex.ru/preview/...", result.Preview);
            //Assert.Equal("custom_properties", result.CustomProperties);
            Assert.Equal("disk:/foo/cat.png", result.OriginPath);
            Assert.Equal(ResourceType.File, result.Type);
            Assert.Equal("02bab05c02537e53dedd408261e0aadf", result.Md5);
            Assert.Equal("image/png", result.MimeType);
            Assert.Equal(903337, result.Size);
            Assert.Equal(new DateTime(2014, 07, 16, 13, 07, 45, DateTimeKind.Local), result.Created);
            Assert.Equal(new DateTime(2014, 07, 16, 13, 07, 45, DateTimeKind.Local), result.Modified);
        }

        [Fact]
        public async Task GetFilesInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + @"resources/files?media_type=""audio%2Ccompressed""&limit=30&offset=50",
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
            Assert.Equal(20, result.Limit);
            Assert.Equal(10, result.Offset);
            Assert.NotEmpty(result.Items);
            Assert.Equal(2, result.Items.Count);

            var firstItem = result.Items[0];
            Assert.Equal("photo2.png", firstItem.Name);
            Assert.Equal("https://downloader.disk.yandex.ru/preview/...", firstItem.Preview);
            Assert.Equal("disk:/foo/photo2.png", firstItem.Path);
            Assert.Equal(ResourceType.File, firstItem.Type);
            Assert.Equal("53f4dc6379c8f95ddf11b9508cfea271", firstItem.Md5);
            Assert.Equal("image/png", firstItem.MimeType);
            Assert.Equal(54321, firstItem.Size);
            Assert.Equal(new DateTime(2014, 04, 22, 14, 57, 13, DateTimeKind.Local), firstItem.Created);
            Assert.Equal(new DateTime(2014, 04, 22, 14, 57, 14, DateTimeKind.Local), firstItem.Modified);

            var secondItem = result.Items[1];
            Assert.Equal("photo1.png", secondItem.Name);
            Assert.Equal("https://downloader.disk.yandex.ru/preview/...", secondItem.Preview);
            Assert.Equal("disk:/foo/photo1.png", secondItem.Path);
            Assert.Equal(ResourceType.File, secondItem.Type);
            Assert.Equal("4334dc6379c8f95ddf11b9508cfea271", secondItem.Md5);
            Assert.Equal("image/png", secondItem.MimeType);
            Assert.Equal(34567, secondItem.Size);
            Assert.Equal(new DateTime(2014, 04, 21, 14, 57, 13, DateTimeKind.Local), secondItem.Created);
            Assert.Equal(new DateTime(2014, 04, 21, 14, 57, 14, DateTimeKind.Local), secondItem.Modified);
        }

        [Fact]
        public async Task GetLastUploadedInfoTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "GET", 
                url: TestHttpClient.BaseUrl + @"resources/last-uploaded?media_type=""audio%2Cexecutable""&limit=20", 
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
            Assert.Equal(20, result.Limit);
            Assert.NotEmpty(result.Items);
            Assert.Equal(2, result.Items.Count);

            var firstItem = result.Items[0];
            Assert.Equal("photo2.png", firstItem.Name);
            Assert.Equal("https://downloader.disk.yandex.ru/preview/...", firstItem.Preview);
            Assert.Equal("disk:/foo/photo2.png", firstItem.Path);
            Assert.Equal(ResourceType.File, firstItem.Type);
            Assert.Equal("53f4dc6379c8f95ddf11b9508cfea271", firstItem.Md5);
            Assert.Equal("image/png", firstItem.MimeType);
            Assert.Equal(54321, firstItem.Size);
            Assert.Equal(new DateTime(2014, 04, 22, 14, 57, 13, DateTimeKind.Local), firstItem.Created);
            Assert.Equal(new DateTime(2014, 04, 22, 14, 57, 14, DateTimeKind.Local), firstItem.Modified);

            var secondItem = result.Items[1];
            Assert.Equal("photo1.png", secondItem.Name);
            Assert.Equal("https://downloader.disk.yandex.ru/preview/...", secondItem.Preview);
            Assert.Equal("disk:/foo/photo1.png", secondItem.Path);
            Assert.Equal(ResourceType.File, secondItem.Type);
            Assert.Equal("4334dc6379c8f95ddf11b9508cfea271", secondItem.Md5);
            Assert.Equal("image/png", secondItem.MimeType);
            Assert.Equal(34567, secondItem.Size);
            Assert.Equal(new DateTime(2014, 04, 21, 14, 57, 13, DateTimeKind.Local), secondItem.Created);
            Assert.Equal(new DateTime(2014, 04, 21, 14, 57, 14, DateTimeKind.Local), secondItem.Modified);
        }


        [Fact]
        public async Task AppendCustomPropertiesTest()
        {
            var httpClientTest = new TestHttpClient(
                methodName: "PATCH",
                url: TestHttpClient.BaseUrl + @"resources?path=%2Ffoo",
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
            Assert.NotEmpty(result.CustomProperties);
            Assert.Equal("1", result.CustomProperties["foo"]);
            Assert.Equal("2", result.CustomProperties["bar"]);
        }
    }
}
