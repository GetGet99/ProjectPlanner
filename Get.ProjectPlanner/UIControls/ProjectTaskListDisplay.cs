using Get.ProjectPlanner.DiskWrapper;
using Get.UI.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using ProgressRing = Microsoft.UI.Xaml.Controls.ProgressRing;
using System.Threading.Tasks;
namespace Get.ProjectPlanner.UIControls;

partial class ProjectTaskListDisplay(FolderAsyncCollection<ProjectTask> col) : TemplateControl<StackPanel>
{
    const int INDENT = 24;
    StackPanel? childSP;
    protected override async void Initialize(StackPanel rootElement)
    {
        rootElement.Spacing = 16;
        childSP = new StackPanel() { Spacing = 16 };
        rootElement.Children.Add(childSP);
        var count = await col.GetCountAsync();
        var progress = new ProgressRing
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            IsActive = true
        };
        rootElement.Children.Add(progress);
        for (int i = 0; i < count; i++)
        {
            var t = await col.GetAsync(i);
            CreateAndAddTaskUI(t);
        }
        rootElement.Children.Remove(progress);
        //var newBtn = new Button {
        //    Content = "+ New Task",
        //    Margin = new(INDENT, count is 0 ? -16 : 0, 0, 0)
        //};
        //rootElement.Children.Add(
        //    newBtn
        //);
    }
    public async void CreateTask(bool autoFocus = false)
    {
        //newBtn.Margin = newBtn.Margin with { Top = 0 };
        var task = await col.CreateNewAsync(null);
        CreateAndAddTaskUI(task, autoFocus);
    }
    public void CreateAndAddTaskUI(ProjectTask t, bool autoFocus = false)
    {
        // we will eventually create it
        if (childSP is null) return;
        var td = new ProjectTaskDisplay(this, t, autoFocus: autoFocus)
        {
            Margin = new(INDENT, 0, 0, 0)
        };
        childSP.Children.Add(td);
    }
    public async Task DeleteAsync(ProjectTaskDisplay childItem)
    {
        var idx = childSP.Children.IndexOf(childItem);
        childSP.Children.Remove(childItem);
        await col.DeleteAsync(idx);
    }
}