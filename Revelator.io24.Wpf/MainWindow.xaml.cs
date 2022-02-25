using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Revelator.io24.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
#if DEBUG
            Environment.Exit(0);
#endif
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox)
            {
                textBox
                    .GetBindingExpression(TextBox.TextProperty)
                    .UpdateSource();
            }
        }
    }
}
