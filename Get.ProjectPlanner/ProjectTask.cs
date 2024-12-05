using Get.ProjectPlanner.DiskWrapper;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Get.ProjectPlanner;

class ProjectTask(AsyncLazy<StorageFolder> folder)
{
    public IAsyncProperty<bool> IsCompletedProperty { get; } =
        new FileTextAsyncProperty<bool>(folder.GetFile("iscompleted.bool"), x => x ? "true" : "false", x => x is "true");
    public IAsyncProperty<string> TitleProperty { get; } =
        new FileTextAsyncProperty(folder.GetFile("title.str"));
    public FolderAsyncCollection<ProjectTask> Children { get; } =
        new FolderAsyncCollection<ProjectTask>(folder.GetFolder("children"), x => new(new(() => x)));
}
class ProjectRoot(AsyncLazy<StorageFolder> folder)
{
    public IAsyncProperty<string> TitleProperty { get; } =
        new FileTextAsyncProperty(folder.GetFile("title.str"));
    public ProjectTask RootTask { get; } = new(folder.GetFolder("root"));
}
