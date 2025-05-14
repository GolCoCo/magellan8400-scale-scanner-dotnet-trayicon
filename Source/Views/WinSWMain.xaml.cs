using Syncfusion.Windows.Shared;
using System;
using System.ComponentModel;
using Magellan8400ReaderTray.Models;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.IO.Ports;
using Syncfusion.SfSkinManager;
using System.Windows.Input;
using Magellan8400ReaderTray.Controllers;
using System.Windows.Forms;


namespace Magellan8400ReaderTray.Views
{
    /// <summary>
    /// Interaction logic for WinSWMain.xaml
    /// </summary>
    public partial class WinSWMain : ChromelessWindow
    {
        public string AppTitle = "MAGELLAN 8400 READER";

        public Brush TitleBarBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));
        public Brush TitleBarForeground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private SettingMain _settingMain;
        public ICommand OpenCommand { get; }
        public ICommand ExitCommand { get; }

        public ScannerController _Scanner;
        public ScaleController _Scale;

        public WinSWMain()
        {
            InitializeComponent();
            OpenCommand = new RelayCommand(Open);
            ExitCommand = new RelayCommand(Exit);

            this.DataContext = this;

            _settingMain = SettingMain.Load<SettingMain>();
            if (_settingMain == null)
            {
                _settingMain = new SettingMain();
            }

            _Scanner = new ScannerController(lbxLogs);
            _Scale = new ScaleController(lbxScaleLogs);
            tbxScaleOuputPath.Text = _settingMain._FolderPathScanner;
            tbxOuputPath.Text = _settingMain._FolderPathScale;
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }



        private void ChromelessWindow_Closing(object sender, CancelEventArgs e)
        {
            _settingMain.Save();
        }

        private void ChromelessWindow_StateChanged(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            _settingMain.Save();
            System.Windows.Application.Current.Shutdown();
        }

        private void btnHideReader_Click(object sender, RoutedEventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Hide();
            TrayIcon.Visibility = Visibility.Visible;
        }
        private void Open()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
            TrayIcon.Visibility = Visibility.Collapsed;
        }

        private void Exit()
        {
            TrayIcon.Dispose();
            _settingMain.Save();
            App.Current.Shutdown();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            bool ret = _Scanner.Start();
            if (ret)
            {
                tbxConneted.Background = Brushes.DarkGreen;
                tbxConneted.Text = "Connected";
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
            }
            else
            {
                tbxConneted.Background = Brushes.DarkRed;
                tbxConneted.Text = "Disconnect";
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;

            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            bool ret = _Scanner.Stop();
            if (ret)
            {
                tbxConneted.Background = Brushes.DarkRed;
                tbxConneted.Text = "Disconnect";
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;

            }
            else
            {
                tbxConneted.Background = Brushes.DarkRed;
                tbxConneted.Text = "Disconnect";
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
  
            }
        }

        private void btnOuputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            // If the user selects a folder (not canceled), print the selected folder path
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settingMain._FolderPathScale = folderDialog.SelectedPath;
                tbxOuputPath.Text = _settingMain._FolderPathScale;
                _settingMain.Save();
            }
        }

        private void btnScaleStart_Click(object sender, RoutedEventArgs e)
        {
            bool ret = _Scale.Start();
            if (ret)
            {
                tbxScaleConneted.Background = Brushes.DarkGreen;
                tbxScaleConneted.Text = "Connected";
                btnScaleStart.IsEnabled = false;
                btnScaleStop.IsEnabled = true;
            }
            else
            {
                tbxScaleConneted.Background = Brushes.DarkRed;
                tbxScaleConneted.Text = "Disconnect";
                btnScaleStart.IsEnabled = true;
                btnScaleStop.IsEnabled = false;
            }
        }

        private void btnScaleStop_Click(object sender, RoutedEventArgs e)
        {
            bool ret = _Scale.Stop();
            if (ret)
            {
                tbxScaleConneted.Background = Brushes.DarkRed;
                tbxScaleConneted.Text = "Disconnect";
                btnScaleStart.IsEnabled = true;
                btnScaleStop.IsEnabled = false;

            }
            else
            {
                tbxScaleConneted.Background = Brushes.DarkRed;
                tbxScaleConneted.Text = "Disconnect";
                btnScaleStart.IsEnabled = true;
                btnScaleStop.IsEnabled = false;

            }
        }

        private void btnScaleOuputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            // If the user selects a folder (not canceled), print the selected folder path
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settingMain._FolderPathScanner = folderDialog.SelectedPath;
                tbxScaleOuputPath.Text = _settingMain._FolderPathScale;
                _settingMain.Save();
            }
        }
    }

}
