using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Get.ProjectPlanner.DiskWrapper;

class FolderAsyncCollection<T>(AsyncLazy<StorageFolder> folder, Func<StorageFolder, T> creator)
{
    readonly FolderAsyncCollection collection = new(folder);
    public Task<int> GetCountAsync() => collection.GetCountAsync();
    public async Task<T> GetAsync(int index) => creator(await collection.GetAsync(index));
    public async Task<T> CreateNewAsync(int? index) => creator(await collection.CreateNewAsync(index));
    public async Task DeleteAsync(int index) => await collection.DeleteAsync(index);
}