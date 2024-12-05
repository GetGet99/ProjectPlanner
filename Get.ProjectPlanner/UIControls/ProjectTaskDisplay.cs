using Get.UI.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
namespace Get.ProjectPlanner.UIControls;

class ProjectTaskDisplay(ProjectTask pt) : TemplateControl<StackPanel>
{
    protected override void Initialize(StackPanel rootElement)
    {
        rootElement.Spacing = 16;
        rootElement.Children.Add(new StackPanel {
            Orientation = Orientation.Horizontal,
            Children = {
                new AsyncCheckBox(pt.IsCompletedProperty),
                new AsyncTextBox(pt.TitleProperty, placeholder: "Do something!")
                {
                    VerticalAlignment = VerticalAlignment.Center
                }
            }
        });
        rootElement.Children.Add(new ProjectTaskListDisplay(pt.Children));
    }
}