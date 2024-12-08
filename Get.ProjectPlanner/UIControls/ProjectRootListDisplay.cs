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
using System.Threading.Tasks;
using System.Diagnostics;
using System;
namespace Get.ProjectPlanner.UIControls;

partial class ProjectRootListDisplay(FolderAsyncCollection<ProjectRoot> col) : TemplateControl<NavigationView>
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
        // apparently we have to add the button first?
        // otherwise InvalidCastException
        var count = await col.GetCountAsync();
        //count = 0;
        var projList = new List<ProjectRoot>(count);
        for (int i = 0; i < count; i++)
        {
            var ele = await col.GetAsync(i);
            projList.Add(ele);
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
                    projList[nav.MenuItems.IndexOf(nav.SelectedItem)].Children
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
                var projRoot = await col.CreateNewAsync(null);
                await projRoot.TitleProperty.SetAsync("New Project");
                // create an empty task
                await projRoot.Children.CreateNewAsync(null);
                projList.Add(projRoot);
                var nvi = new NavigationViewItem { Content = CreateTB(projRoot.TitleProperty) };
                nav.MenuItems.Add(nvi);
                // select it
                nav.SelectedItem = nvi;
            }
        };
    }
}
static class Extension
{
    public static T ForceWait<T>(this Task<T> task)
    {
        task.Wait();
        return task.Result;
    }
}