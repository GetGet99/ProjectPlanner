using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Get.ProjectPlanner;

static class Extension
{
    public static AsyncLazy<StorageFolder> GetFolder(this AsyncLazy<StorageFolder> cur, string path)
    {
        return new(async () => await (await cur).CreateFolderAsync(path, CreationCollisionOption.OpenIfExists));
    }
    public static AsyncLazy<StorageFile> GetFile(this AsyncLazy<StorageFolder> cur, string path)
    {
        return new(async () => await (await cur).CreateFileAsync(path, CreationCollisionOption.OpenIfExists));
    }
    public static AsyncLazy<StorageFolder> GetFolder(this Task<StorageFolder> cur, string path)
    {
        return new(async () => await (await cur).CreateFolderAsync(path, CreationCollisionOption.OpenIfExists));
    }
    public static AsyncLazy<StorageFile> GetFile(this Task<StorageFolder> cur, string path)
    {
        return new(async () => await (await cur).CreateFileAsync(path, CreationCollisionOption.OpenIfExists));
    }
    public static AsyncLazy<StorageFolder> GetFolder(this StorageFolder cur, string path)
    {
        return new(async () => await cur.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists));
    }
    public static AsyncLazy<StorageFile> GetFile(this StorageFolder cur, string path)
    {
        return new(async () => await cur.CreateFileAsync(path, CreationCollisionOption.OpenIfExists));
    }
}