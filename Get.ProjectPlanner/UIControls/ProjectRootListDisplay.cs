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
using Get.Data.Helpers;
using System.Globalization;
using Get.Symbols;
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
        NavigationViewItem CreateNVI(List<ProjectRoot> projList, ProjectRoot root)
        {
            NavigationViewItem? nvi = null!;
            return nvi = new NavigationViewItem
            {
                Content = CreateTB(root.TitleProperty),
                ContextFlyout = new MenuFlyout
                {
                    Items =
                    {
                        new MenuFlyoutItem { Text = "Delete" }
                        .WithCustomCode(x => x.Click += async delegate {
                            var idx = nav.MenuItems.IndexOf(nvi);
                            
                            if ((NavigationViewItem)nav.SelectedItem == nvi)
                            {
                                nav.SelectedItem = null;
                                nav.MenuItems.RemoveAt(idx);
                                //if (nav.MenuItems.Count > 0)
                                //    nav.SelectedItem = idx < nav.MenuItems.Count ? nav.MenuItems[idx] : nav.MenuItems[^1];
                            } else
                            {
                                nav.MenuItems.RemoveAt(idx);
                            }
                            projList.RemoveAt(idx);
                            nvi.IsEnabled = false;
                            await col.DeleteAsync(idx);
                        })
                    }
                }
            };
        }
        var mainView = new Border();
        nav.Content = new Grid
        {
            RowDefinitions =
            {
                new(),
                new() { Height = GridLength.Auto }
            },
            Children =
            {
                new ScrollViewer { Content = mainView },
                new HStack(spacing: 16)
                {
                    Margin = new(16),
                    Children =
                    {
#if DEBUG
                        HotKey("Hot Reload", Key("Ctrl"), Key("R")),
#endif
                        HotKey("Toggle Check", Key("Ctrl"), Key(SymbolEx.UnderscoreSpace)),
                        HotKey("New Task", Key("Ctrl"), Key(SymbolEx.ReturnKey)),
                        HotKey("New Child Task", Key("Ctrl"), Key(SymbolEx.UpArrowShiftKey), Key(SymbolEx.ReturnKey)),
                        HotKey("Delete Task", Key("Ctrl"), Key("Del")),
                        HotKey("Previous", Key(SymbolEx.UpArrowShiftKey), Key("Tab")),
                        HotKey("Next", Key("Tab")),
                    }
                }.WithCustomCode(x => Grid.SetRow(x, 1))
            }
        };
        var welcome = new TextBlock
        {
            FontSize = 60,
            Text = "Welcome!\nClick \"New Project\" to get started!",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };
        mainView.Child = welcome;
        // apparently we have to add the button first?
        // otherwise InvalidCastException
        var count = await col.GetCountAsync();
        //count = 0;
        var projList = new List<ProjectRoot>(count);
        for (int i = 0; i < count; i++)
        {
            var ele = await col.GetAsync(i);
            projList.Add(ele);
            nav.MenuItems.Add(CreateNVI(projList, ele));
        }
        var newBtn = new NavigationViewItem() { SelectsOnInvoked = false, Icon = new SymbolIcon(Symbol.Add), Content = "New Project" };
        nav.FooterMenuItems.Add(
            newBtn
        );
        nav.SelectionChanged += delegate
        {
            if (nav.SelectedItem == null)
                mainView.Child = welcome;
            else
                mainView.Child = new ProjectTaskListDisplay(
                    projList[nav.MenuItems.IndexOf(nav.SelectedItem)].Children,
                    autoFocus: true
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
                var nvi = CreateNVI(projList, projRoot);
                nav.MenuItems.Add(nvi);
                // select it
                nav.SelectedItem = nvi;
            }
        };
    }
    static UIElement HotKey(string action, params IEnumerable<UIElement> keys)
    {
        var hs = new HStack(spacing: 4);
        foreach (var k in keys)
            hs.Children.Add(k);
        return new HStack(spacing: 8)
        {
            hs,
            new TextBlock
            {
                Text = action,
                TextLineBounds = TextLineBounds.Tight,
                VerticalAlignment = VerticalAlignment.Center,
            }
        };
    }
    static UIElement Key(string hotkey)
    {
        return new Border
        {
            CornerRadius = new(4),
            BorderBrush = Solid(Colors.White),
            Padding = new(4),
            BorderThickness = new(1),
            Height = 25,
            VerticalAlignment = VerticalAlignment.Center,
            Child = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Text = hotkey,
                FontSize = 15,
                TextLineBounds = TextLineBounds.Tight,
            }
        };
    }
    static UIElement Key(SymbolEx hotkey)
    {
        return new Border
        {
            CornerRadius = new(4),
            BorderBrush = Solid(Colors.White),
            Padding = new(4),
            BorderThickness = new(1),
            VerticalAlignment = VerticalAlignment.Center,
            Child = new SymbolExIcon
            {
                SymbolEx = hotkey,
                FontSize = 15
            }
        };
    }
}