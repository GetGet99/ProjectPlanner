using Get.UI.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System;
using Get.Data.Helpers;
using static Get.UI.Data.QuickCreate;
namespace Get.ProjectPlanner.UIControls;

partial class ProjectTaskDisplay(ProjectTaskListDisplay parent, ProjectTask pt, bool autoFocus = false) : TemplateControl<OrientedStack>
{
    AsyncTextBox? tb;
    ProjectTaskListDisplay? childrenView;
    protected override void Initialize(OrientedStack rootElement)
    {
        rootElement.Orientation = Orientation.Vertical;
        rootElement.Spacing = 16;
        rootElement.Children.Add(new OrientedStack(Orientation.Horizontal, spacing: 4) {
            HorizontalAlignment = HorizontalAlignment.Left,
            Children = {
                new AsyncCheckBox(pt.IsCompletedProperty)
                {
                    VerticalAlignment = VerticalAlignment.Center
                }
                .AssignTo(out var cb),
                new AsyncTextBox(pt.TitleProperty, placeholder: "Do something!", autoFocus: autoFocus)
                {
                    VerticalAlignment = VerticalAlignment.Center
                }.AssignTo(out var tb)
            }
        });
        this.tb = tb;
        rootElement.Children.Add(new ProjectTaskListDisplay(pt.Children).AssignTo(out var childrenView));
        this.childrenView = childrenView;
        tb.Navigate += x =>
        {
            if (x is true) // next
                childrenView.FocusNavigate(true, 0);
            else // previous, go back to parent
                parent.FocusNavigate(false, this);
        };
        tb.CtrlSpace += cb.Toggle;
        tb.CtrlEnter += () => parent.CreateTask(this, autoFocus: true);
        tb.CtrlShiftEnter += () => childrenView.CreateTask(0, autoFocus: true);
        tb.CtrlDel += () => _ = parent.DeleteAsync(this);
        childrenView.Navigate += x =>
        {
            if (x is false) // previous
                FocusNavigate(true); // basically current
            else // next, go to parent
                parent.FocusNavigate(true, this);
        };
    }
    public async void FocusNavigate(bool current)
    {
        if (current is true)
            tb?.FocusNavigate();
        else
            childrenView?.FocusNavigate(current: false, await pt.Children.GetCountAsync() - 1);
    }
}