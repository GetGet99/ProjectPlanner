using Get.ProjectPlanner.DiskWrapper;
using Get.UI.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using ProgressRing = Microsoft.UI.Xaml.Controls.ProgressRing;
using System.Threading.Tasks;
using System;
using Windows.UI.WebUI;
namespace Get.ProjectPlanner.UIControls;

partial class ProjectTaskListDisplay(FolderAsyncCollection<ProjectTask> col, bool autoFocus = false) : TemplateControl<StackPanel>
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
            CreateAndAddTaskUI(t, autoFocus: autoFocus && i is 0);
        }
        rootElement.Children.Remove(progress);
        if (count is 0)
        {
            Visibility = Visibility.Collapsed;
        }
        //var newBtn = new Button {
        //    Content = "+ New Task",
        //    Margin = new(INDENT, count is 0 ? -16 : 0, 0, 0)
        //};
        //rootElement.Children.Add(
        //    newBtn
        //);
    }
    static int? NullIfNegative(int? val)
    {
        if (val is null) return null;
        if (val.Value < 0) return null;
        return val;
    }
    int? IndexOfChild(ProjectTaskDisplay? sender)
    {
        var val = sender is null ? null : NullIfNegative(childSP?.Children.IndexOf(sender));
        return val;
    }
    int? IndexOfChildP1(ProjectTaskDisplay? sender)
    {
        var val = IndexOfChild(sender);
        return val is null ? null : val.Value + 1;
    }
    public void FocusNavigate(bool direction, ProjectTaskDisplay? sender = null)
    {
        if (childSP is null) return;
        var childIdxNull = IndexOfChild(sender);
        if (childIdxNull is not int childIdx) return;
        childIdx = direction ? childIdx + 1 : childIdx - 1;
        FocusNavigate(current: direction, focusIdx: childIdx);
    }
    public void FocusNavigate(bool current, int focusIdx)
    {
        if (childSP is null) return;
        if (focusIdx < 0)
        {
            Navigate?.Invoke(false);
            return;
        }
        else if (focusIdx >= childSP.Children.Count)
        {
            Navigate?.Invoke(true);
            return;
        }
        else
        {
            if (childSP.Children[focusIdx] is ProjectTaskDisplay ptd)
                ptd.FocusNavigate(current: current);
        }
    }
    public void CreateTask(ProjectTaskDisplay? sender = null, bool autoFocus = false)
    {
        CreateTask(index: IndexOfChildP1(sender), autoFocus: autoFocus);
    }
    public async void CreateTask(int? index = null, bool autoFocus = false)
    {
        //newBtn.Margin = newBtn.Margin with { Top = 0 };
        var task = await col.CreateNewAsync(index);
        CreateAndAddTaskUI(task, idx: index, autoFocus: autoFocus);
    }
    public void CreateAndAddTaskUI(ProjectTask t, int? idx = null, bool autoFocus = false)
    {
        // we will eventually create it
        if (childSP is null) return;
        var td = new ProjectTaskDisplay(this, t, autoFocus: autoFocus)
        {
            Margin = new(INDENT, 0, 0, 0)
        };
        Visibility = Visibility.Visible;
        if (idx == null)
            childSP.Children.Add(td);
        else
            childSP.Children.Insert(idx.Value, td);
    }
    public async Task DeleteAsync(ProjectTaskDisplay childItem)
    {
        if (childSP is null) throw new InvalidOperationException("parent is not even initialized?");
        var idx = childSP.Children.IndexOf(childItem);
        childSP.Children.Remove(childItem);
        await col.DeleteAsync(idx);
        if (childSP.Children.Count > 0)
        {
            var toSelect = (ProjectTaskDisplay)(idx < childSP.Children.Count ? childSP.Children[idx] : childSP.Children[^1]);
            toSelect.FocusNavigate(current: true);
        } else
        {
            // let parent handle this
            Navigate?.Invoke(true);
        }
    }
    public event Action<bool>? Navigate;
}