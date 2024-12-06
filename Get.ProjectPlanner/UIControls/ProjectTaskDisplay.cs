using Get.UI.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System;
using Get.Data.Helpers;
namespace Get.ProjectPlanner.UIControls;

class ProjectTaskDisplay(ProjectTaskListDisplay parent, ProjectTask pt, bool autoFocus = false) : TemplateControl<StackPanel>
{
    protected override void Initialize(StackPanel rootElement)
    {
        rootElement.Spacing = 16;
        rootElement.Children.Add(new StackPanel {
            Orientation = Orientation.Horizontal,
            Children = {
                new AsyncCheckBox(pt.IsCompletedProperty),
                new AsyncTextBox(pt.TitleProperty, placeholder: "Do something! (Ctrl + Enter: New Task, Ctrl + Shift + Enter: New Child Task)", autoFocus: autoFocus)
                {
                    VerticalAlignment = VerticalAlignment.Center
                }.AssignTo(out var tb)
            }
        });
        rootElement.Children.Add(new ProjectTaskListDisplay(pt.Children).AssignTo(out var childrenView));
        tb.CtrlEnter += () => parent.CreateTask(autoFocus: true);
        tb.CtrlShiftEnter += () => childrenView.CreateTask(autoFocus: true);
        tb.CtrlDel += () => _ = parent.DeleteAsync(this);
    }
}