using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Get.ProjectPlanner.DiskWrapper;

class FileTextAsyncProperty<T>(AsyncLazy<StorageFile> file, Func<T, string> saveConvert, Func<string, T> loadConvert) : IAsyncProperty<T>
{
    public async Task<T> GetAsync()
    {
        return loadConvert(await FileIO.ReadTextAsync(await file));
    }
    public async Task SetAsync(T title)
    {
        await FileIO.WriteTextAsync(await file, saveConvert(title));
    }
}

class FileTextAsyncProperty(AsyncLazy<StorageFile> file) : FileTextAsyncProperty<string>(file, x => x, x => x)
{

}

class FileIntTextAsyncProperty(AsyncLazy<StorageFile> file, int defaultValue) :
    FileTextAsyncProperty<int>(file, x => x.ToString(), x => x is "" ? defaultValue : int.Parse(x))
{

}
