using Get.UI.Data;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

using static Get.UI.Data.QuickCreate;
using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
namespace Get.ProjectPlanner.UIControls;

class AsyncTextBox(IAsyncProperty<string> prop, string placeholder = "", bool asTextBlock = true, bool autoFocus = false) : TemplateControl<TextBox>
{
    protected override async void Initialize(TextBox rootElement)
    {
        if (asTextBlock)
        {
            rootElement.Margin = new(-4, 0, 0, 0);
            rootElement.Padding = new(4, 0, 4, 0);
            rootElement.MinHeight = 0;
            rootElement.Background = Solid(Colors.Transparent);
            rootElement.BorderBrush = Solid(Colors.Transparent);
            rootElement.HorizontalAlignment = HorizontalAlignment.Left;
        }
        rootElement.PlaceholderText = placeholder;
        rootElement.IsEnabled = false;
        rootElement.Text = await prop.GetAsync();
        rootElement.LostFocus += async delegate
        {
            rootElement.IsEnabled = false;
            await prop.SetAsync(rootElement.Text);
            rootElement.IsEnabled = true;
        };
        rootElement.IsEnabled = true;
        rootElement.AddHandler(PreviewKeyDownEvent, new KeyEventHandler((_, e) =>
        {
            if (e.Key is VirtualKey.Enter)
            {
                if (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control) is not CoreVirtualKeyStates.None)
                {
                    if (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Shift) is not CoreVirtualKeyStates.None)
                        CtrlShiftEnter?.Invoke();
                    else
                        CtrlEnter?.Invoke();
                }
            }
            else if (e.Key is VirtualKey.Delete)
            {
                if (Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Control) is not CoreVirtualKeyStates.None)
                {
                    CtrlDel?.Invoke();
                }
            }
        }), true);
        if (autoFocus)
        {
            rootElement.Focus(FocusState.Keyboard);
        }
    }


    public event Action? CtrlDel;
    public event Action? CtrlEnter;
    public event Action? CtrlShiftEnter;
}