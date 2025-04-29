using Syncfusion.Windows.Shared;
using System;
using System.ComponentModel;
using Magellan8400ReaderTray.Models;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.IO.Ports;
using Syncfusion.SfSkinManager;


namespace Magellan8400ReaderTray.Views
{
    /// <summary>
    /// Interaction logic for WinMain.xaml
    /// </summary>
    public partial class WinMain : ChromelessWindow
    {
        public string AppTitle = "MAGELLAN 8400";

        public Brush TitleBarBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));
        public Brush TitleBarForeground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public WinMain()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }



        private void ChromelessWindow_Closing(object sender, CancelEventArgs e)
        {
            
        }

        private void ChromelessWindow_StateChanged(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnStartScale_Click(object sender, RoutedEventArgs e)
        {
            WinScale form = new WinScale();
            SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
            form.Show();
        }

        private void btnStartScanner_Click(object sender, RoutedEventArgs e)
        {
            WinScanner form = new WinScanner();
            SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
            form.ShowDialog();
        }
    }

}
