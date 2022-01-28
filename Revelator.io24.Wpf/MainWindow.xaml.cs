using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void routing_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button is null)
                return;

            var route = NameToRoute(button.Name);

            //Name:
            App.UpdateService.ToggleMute(route, (route, state) => {
                Dispatcher.Invoke(() => {
                    var name = RouteToName(route);
                    var button = this.FindName(name) as Button;
                    if (button is null)
                        return;

                    button.Background = state
                        ? Brushes.Green
                        : Brushes.Red;
                });
            });
        }

        private void headphones_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button is null)
                return;

            App.UpdateService.SetHeadphonesToMix(button.Name);
            //if (button == this.main_phones)
            //{
            //    this.main_phones.Background = Brushes.Green;
            //    this.mixA_phones.Background = Brushes.Red;
            //    this.mixB_phones.Background = Brushes.Red;
            //}
            //else if (button == this.mixA_phones)
            //{
            //    this.main_phones.Background = Brushes.Red;
            //    this.mixA_phones.Background = Brushes.Green;
            //    this.mixB_phones.Background = Brushes.Red;
            //}
            //else if (button == this.mixB_phones)
            //{
            //    this.main_phones.Background = Brushes.Red;
            //    this.mixA_phones.Background = Brushes.Red;
            //    this.mixB_phones.Background = Brushes.Green;
            //}
        }

        private string NameToRoute(string name)
            => name.Replace("_", "/");

        private string RouteToName(string route)
            => route.Replace("/", "_");

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
#if DEBUG
            Environment.Exit(0);
#endif
        }
    }
}
