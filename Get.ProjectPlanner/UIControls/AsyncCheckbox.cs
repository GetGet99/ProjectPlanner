using Get.UI.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Get.ProjectPlanner.UIControls;

partial class AsyncCheckBox(IAsyncProperty<bool> prop) : TemplateControl<CheckBox>
{
    protected override async void Initialize(CheckBox rootElement)
    {
        rootElement.MinWidth = 0;
        rootElement.VerticalAlignment = VerticalAlignment.Center;
        rootElement.IsEnabled = false;
        rootElement.IsChecked = await prop.GetAsync();
        rootElement.LostFocus += async delegate
        {
            rootElement.IsEnabled = false;
            await prop.SetAsync(rootElement.IsChecked ?? false);
            rootElement.IsEnabled = true;
        };
        rootElement.IsEnabled = true;
    }
}