# Yandex Disk API Client for .NET

[Yandex Disk Rest API](https://tech.yandex.ru/disk/rest/) client library for .NET 4.6

[![yandexdisk-client MyGet Build Status](https://www.myget.org/BuildSource/Badge/yandexdisk-client?identifier=8a8dfc85-9089-404c-a42e-93dabef1cb25)](https://www.myget.org/)

## How to install

From [NuGet](https://www.nuget.org/packages/YandexDisk.Client/) or
[MyGet](https://www.myget.org/feed/yandexdisk-client)
```cmd
PM> Install-Package YandexDisk.Client
```


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
    IEnumerable<Resource> allFilesInFolder = fooResourceDescription.Embedded.Items.Where(item => item.Type == ResourceType.File);

    //Path to local folder for downloading files
    string localFolder = @"C:\foo";

    //Run all downloadings in parallel. DiskApi is thread safe.
    IEnumerable<Task> downloadingTasks =
        allFilesInFolder.Select(file => diskApi.Files.DownloadFileAsync(file.Path, System.IO.Path.Combine(localFolder, file.Name)));

    //Wait all done
    await Task.WhenAll(downloadingTasks);
}
```

## How to build
Open solution src/YandexDisk.Client.sln in Visual Studio 2015 (support C# 6 is required). Make sure that NuGet restoring option is turn on. Run build solution.
