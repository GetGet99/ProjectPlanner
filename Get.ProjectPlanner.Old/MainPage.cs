using Get.ProjectPlanner.DiskWrapper;
using Get.ProjectPlanner.UIControls;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Get.ProjectPlanner;

class MainPage : Page
{
    public FolderAsyncCollection<ProjectRoot> ProjectsProperty { get; }
    public MainPage()
    {
        BackdropMaterial.SetApplyToRootOrPageBackground(this, true);
        var localFolder = ApplicationData.Current.LocalFolder;
        ProjectsProperty = new(localFolder.GetFolder("projects"), x => new(new(() => x)));
        Content = InitContent();
        //KeyDown += OnKeyDown;
    }
    //// Hot Reload
    //private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    //{
    //    if (e.Key is VirtualKey.R && Window.Current.CoreWindow.GetKeyState(VirtualKey.Control) is not CoreVirtualKeyStates.None)
    //        Content = InitContent();
    //}

    UIElement InitContent()
    {
        return new ProjectRootListDisplay(ProjectsProperty);
    }
}