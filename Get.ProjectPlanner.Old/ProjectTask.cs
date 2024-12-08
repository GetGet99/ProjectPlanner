using Get.ProjectPlanner.DiskWrapper;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Get.ProjectPlanner;

class ProjectTask
{
    public ProjectTask? Parent { get; }
    public IAsyncProperty<bool> IsCompletedProperty { get; }
    public IAsyncProperty<string> TitleProperty { get; }
    public FolderAsyncCollection<ProjectTask> Children { get; }
    public ProjectTask(AsyncLazy<StorageFolder> folder, ProjectTask? parent)
    {
        Parent = parent;
        IsCompletedProperty = new FileTextAsyncProperty<bool>(folder.GetFile("iscompleted.bool"), x => x ? "true" : "false", x => x is "true");
        TitleProperty = new FileTextAsyncProperty(folder.GetFile("title.str"));
        Children = new FolderAsyncCollection<ProjectTask>(folder.GetFolder("children"), x => new(new(() => x), this));
    }
}
class ProjectRoot(AsyncLazy<StorageFolder> folder)
{
    public IAsyncProperty<string> TitleProperty { get; } =
        new FileTextAsyncProperty(folder.GetFile("title.str"));
    public FolderAsyncCollection<ProjectTask> Children { get; }
        = new FolderAsyncCollection<ProjectTask>(folder.GetFolder("tasks"), x => new(new(() => x), null));
}
