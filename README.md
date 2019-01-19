# Yandex Disk API Client for .NET

[Yandex Disk Rest API](https://tech.yandex.ru/disk/rest/) client library for .NET Standard

[![Build status](https://ci.appveyor.com/api/projects/status/tiranp5ojj9ivfeb/branch/master?svg=true)](https://ci.appveyor.com/project/raidenyn/yandexdisk-client/branch/master)

## How to install

From [NuGet](https://www.nuget.org/packages/YandexDisk.Client/) or
[MyGet](https://www.myget.org/feed/yandexdisk-client)
```cmd
PM> Install-Package YandexDisk.Client
```

## Changes in version 1.3
- Now the library is supporting .NET Standard 2.0 and available for .NET Core.
- Removed supporting of .NET 4.0 and 4.5


## How to use

Example of uploading file to Yandex Disk

```C#
async Task UploadSample()
{
  //You should have oauth token from Yandex Passport.
  //See https://tech.yandex.ru/oauth/
  string oauthToken = "<token hear>"

  // Create a client instance
  IDiskApi diskApi = new DiskHttpApi(oauthToken);

  //Upload file from local
  await diskApi.Files.UploadFileAsync(path: "/foo/myfile.txt",
                                      overwrite: false,
                                      localFile: @"C:\myfile.txt",
                                      cancellationToken: CancellationToken.None);
}
```


Example of downloading files from Yandex Disk

```C#
async Task DownloadAllFilesInFolder(IDiskApi diskApi)
{
    //Getting information about folder /foo and all files in it
    Resource fooResourceDescription = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest
                                            {
                                                Path = "/foo", //Folder on Yandex Disk
                                            }, CancellationToken.None);

    //Getting all files from response
    IEnumerable<Resource> allFilesInFolder =
        fooResourceDescription.Embedded.Items.Where(item => item.Type == ResourceType.File);

    //Path to local folder for downloading files
    string localFolder = @"C:\foo";

    //Run all downloadings in parallel. DiskApi is thread safe.
    IEnumerable<Task> downloadingTasks =
        allFilesInFolder.Select(file =>
          diskApi.Files.DownloadFileAsync(path: file.Path,
                                          localPath: System.IO.Path.Combine(localFolder, file.Name)));

    //Wait all done
    await Task.WhenAll(downloadingTasks);
}
```

## How to build
Open solution src/YandexDisk.Client.sln in Visual Studio 2017 (support C# 7.3 is required). Run build solution.
