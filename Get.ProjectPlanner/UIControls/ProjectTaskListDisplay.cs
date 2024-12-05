using Get.ProjectPlanner.DiskWrapper;
using Get.UI.Data;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using ProgressRing = Microsoft.UI.Xaml.Controls.ProgressRing;
namespace Get.ProjectPlanner.UIControls;

class ProjectTaskListDisplay(FolderAsyncCollection<ProjectTask> col, bool padding = false) : TemplateControl<StackPanel>
{
    const int INDENT = 24;
    protected override async void Initialize(StackPanel rootElement)
    {
        rootElement.Spacing = 16;
        var childSP = new StackPanel() { Spacing = 16 };
        rootElement.Children.Add(childSP);
        var count = await col.GetCountAsync();
        var progress = new ProgressRing
        {
            IsActive = true
        };
        rootElement.Children.Add(progress);
        for (int i = 0; i < count; i++)
        {
            var ele = await col.GetAsync(i);
            childSP.Children.Add(new ProjectTaskDisplay(ele)
            {
                Margin = new(INDENT, 0, 0, 0)
            });
        }
        rootElement.Children.Remove(progress);
        var newBtn = new Button {
            Content = "+ New Task",
            Margin = new(INDENT, count is 0 ? -16 : 0, 0, 0)
        };
        rootElement.Children.Add(
            newBtn
        );
        newBtn.Click += async delegate
        {
            newBtn.Margin = newBtn.Margin with { Top = 0 };
            var ele = await col.CreateNewAsync(null);
            childSP.Children.Add(new ProjectTaskDisplay(ele)
            {
                Margin = new(INDENT, 0, 0, 0)
            });
        };
    }
}