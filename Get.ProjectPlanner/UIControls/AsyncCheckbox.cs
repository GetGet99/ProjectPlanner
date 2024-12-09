using Get.UI.Data;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Get.ProjectPlanner.UIControls;

partial class AsyncCheckBox(IAsyncProperty<bool> prop) : TemplateControl<CheckBox>
{
    CheckBox rootElement;
    protected override async void Initialize(CheckBox rootElement)
    {
        this.rootElement = rootElement;
        rootElement.MinWidth = 0;
        rootElement.VerticalAlignment = VerticalAlignment.Center;
        tcs = new();
        rootElement.IsEnabled = false;
        rootElement.IsChecked = await prop.GetAsync();
        rootElement.Checked += (_, _) => Set(true);
        rootElement.Unchecked += (_, _) => Set(false);
        rootElement.IsEnabled = true;
        tcs.SetResult();
        tcs = null;
    }
    public void Toggle()
    {
        // Checked and Unchecked event will handle this
        rootElement.IsChecked = !(rootElement.IsChecked ?? false);
    }
    TaskCompletionSource? tcs;
    async void Set(bool value)
    {
        if (tcs is not null)
            await tcs.Task;
        tcs = new();
        rootElement.IsEnabled = false;
        await prop.SetAsync(value);
        rootElement.IsEnabled = true;
        tcs.SetResult();
        tcs = null;
    }
}