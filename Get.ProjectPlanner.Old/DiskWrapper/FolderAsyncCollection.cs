using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Get.ProjectPlanner.DiskWrapper;

class FolderAsyncCollection(AsyncLazy<StorageFolder> folder)
{
    IAsyncProperty<List<string>> IndexProperty { get; } = new FileTextAsyncProperty<List<string>>(
        folder.GetFile("index.idx"),
        x => string.Join('\n', x),
        x => x is "" ? new() : [.. x.Split('\n')]
    );
    public async Task<int> GetCountAsync()
    {
        var l = await IndexProperty.GetAsync();
        return l.Count;
    }
    public async Task<StorageFolder> CreateNewAsync(int? idx = null)
    {
        var index = await IndexProperty.GetAsync();
        var f = await folder;
        var newFolder = await f.CreateFolderAsync("f", CreationCollisionOption.GenerateUniqueName);
        if (idx.HasValue)
            index.Insert(idx.Value, newFolder.Name);
        else
            index.Add(newFolder.Name);
        await IndexProperty.SetAsync(index);
        return newFolder;
    }

    public async Task DeleteAsync(int idx)
    {
        var index = await IndexProperty.GetAsync();
        var fn = index[idx];
        await (await (await folder).GetFolderAsync(fn)).DeleteAsync();
        index.Remove(fn);
        await IndexProperty.SetAsync(index);
    }

    public async Task<StorageFolder> GetAsync(int idx)
    {
        var index = await IndexProperty.GetAsync();
        var fn = index[idx];
        return await (await folder).GetFolderAsync(fn);
    }
}