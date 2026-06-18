using System.Windows;
using System.Windows.Controls;

namespace Library_Project
{
    internal static class WpfInputHelper
    {
        public static string? Prompt(Window owner, string title, string message, string defaultText = "")
        {
            var textBox = new TextBox
            {
                Text = defaultText,
                Margin = new Thickness(0, 8, 0, 0),
                MinWidth = 420,
            };

            var panel = new StackPanel { Margin = new Thickness(12) };
            panel.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap });
            panel.Children.Add(textBox);

            var dialog = new Window
            {
                Title = title,
                Content = panel,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner,
                ResizeMode = ResizeMode.NoResize,
            };

            var buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 12, 0, 0),
            };

            string? result = null;
            var ok = new Button { Content = "OK", Width = 80, Margin = new Thickness(0, 0, 8, 0), IsDefault = true };
            ok.Click += (_, _) => { result = textBox.Text; dialog.DialogResult = true; };
            var cancel = new Button { Content = "Отмена", Width = 80, IsCancel = true };
            cancel.Click += (_, _) => { dialog.DialogResult = false; };
            buttons.Children.Add(ok);
            buttons.Children.Add(cancel);
            panel.Children.Add(buttons);

            return dialog.ShowDialog() == true ? result : null;
        }
    }
}
