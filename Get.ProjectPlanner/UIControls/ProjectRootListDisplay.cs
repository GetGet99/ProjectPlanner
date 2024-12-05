using Get.ProjectPlanner.DiskWrapper;
using Get.UI.Data;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using ProgressRing = Microsoft.UI.Xaml.Controls.ProgressRing;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using static Get.UI.Data.QuickCreate;
using NavigationViewBackButtonVisible = Microsoft.UI.Xaml.Controls.NavigationViewBackButtonVisible;
namespace Get.ProjectPlanner.UIControls;

class ProjectRootListDisplay(FolderAsyncCollection<ProjectRoot> col) : TemplateControl<NavigationView>
{
    protected override async void Initialize(NavigationView nav)
    {
        nav.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        nav.IsPaneToggleButtonVisible = false;
        nav.IsSettingsVisible = false;
        static AsyncTextBox CreateTB(IAsyncProperty<string> title)
        {
            return new AsyncTextBox(title, placeholder: "Project Title", asTextBlock: true);
        }
        var mainView = new Border();
        nav.Content = new ScrollViewer { Content = mainView };
        mainView.Child = new TextBlock
        {
            FontSize = 60,
            Text = "Welcome!\nClick \"New Project\" to get started!",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };
        var count = await col.GetCountAsync();
        var list = new List<ProjectRoot>(count);
        for (int i = 0; i < count; i++)
        {
            var ele = await col.GetAsync(i);
            list.Add(ele);
            nav.MenuItems.Add(new NavigationViewItem { Content = CreateTB(ele.TitleProperty) });
        }
        var newBtn = new NavigationViewItem() { SelectsOnInvoked = false, Icon = new SymbolIcon(Symbol.Add), Content = "New Project" };
        nav.FooterMenuItems.Add(
            newBtn
        );
        nav.SelectionChanged += delegate
        {
            if (nav.SelectedItem == null)
                mainView.Child = new TextBlock
                {
                    Text = "Welcome! Click \"New Project\" to get started!"
                };
            else
                mainView.Child = new ProjectTaskListDisplay(
                    list[nav.MenuItems.IndexOf(nav.SelectedItem)].RootTask.Children,
                    padding: true
                )
                {
                    Margin = new(0, 16, 0, 16)
                };
        };
        if (count >= 1)
        {
            nav.SelectedItem = nav.MenuItems[0];
        }
        nav.ItemInvoked += async (_, e) =>
        {
            if (e.InvokedItemContainer == newBtn)
            {
                var ele = await col.CreateNewAsync(null);
                await ele.TitleProperty.SetAsync("New Project");
                list.Add(ele);
                var nvi = new NavigationViewItem { Content = CreateTB(ele.TitleProperty) };
                nav.MenuItems.Add(nvi);
                // select it
                nav.SelectedItem = nvi;
            }
        };
    }
}