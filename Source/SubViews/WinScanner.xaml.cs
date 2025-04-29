using Syncfusion.Windows.Shared;
using System;
using System.ComponentModel;
using Magellan8400ReaderTray.Models;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.IO.Ports;

// For more convenient shorthand.
using OPOSConstants = OPOSCONSTANTSLib.OPOS_Constants;
using OPOSScaleConstants = OPOSCONSTANTSLib.OPOSScaleConstants;
using OposScale_CCO;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Collections.Generic;
using OposScanner_CCO;

namespace Magellan8400ReaderTray.Views
{
    /// <summary>
    /// Interaction logic for WinMain.xaml
    /// </summary>
    public partial class WinScanner : ChromelessWindow
    {
        public string AppTitle = "MAGELLAN 8400 SCANNER";
        public Brush TitleBarBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));
        public Brush TitleBarForeground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private OPOSScannerClass _OposScanner = null;
        private SettingMain _settingMain;

        private List<List<string>> _Data;

        private string _DeviceName = "MagellanSC";

        public WinScanner()
        {
            InitializeComponent();
            this.DataContext = this;
            this._Data = new List<List<string>>();
            _settingMain = SettingMain.Load<SettingMain>();
            if(_settingMain == null)
            {
                _settingMain = new SettingMain();
            }
        }

        private void ChromelessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _OposScanner = new OPOSScannerClass();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            tbxOuputPath.Text = _settingMain._FolderPathScanner;
        }

        private void AppendLog(string data)
        {
            Dispatcher.Invoke(() =>
            {
                lbxLogs.Items.Add(data);
                string csdt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                _Data.Add(new List<string> { csdt, data });
            });
        }


        private void ChromelessWindow_Closing(object sender, CancelEventArgs e)
        {
            
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _OposScanner.Open(_DeviceName);
                _OposScanner.ClaimDevice(1000);
                if (_OposScanner.Claimed)
                {
                    tbxConneted.Background = Brushes.DarkGreen;
                    tbxConneted.Text = "Connected";
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;

                    // Subscribe to the delegate.
                    _OposScanner.DataEvent += DataEvent;

                    // Enable the scanner and decoding events.
                    _OposScanner.DeviceEnabled = true;
                    _OposScanner.DataEventEnabled = true;
                    _OposScanner.DecodeData = true;

                    GoodBeep(ref _OposScanner, _DeviceName);

                }
                else
                {
                    _OposScanner.Close();
                    tbxConneted.Background = Brushes.DarkRed;
                    tbxConneted.Text = "Disconnect";
                    btnStart.IsEnabled = true;
                    btnStop.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                UtilMethods.ShowMessageBox($"{ex.Message}");
            }
        }

        private void GoodBeep(ref OPOSScannerClass scanner, string profileName)
        {
            string command = string.Empty;

                //command = "30 00 04";

                //command = "42";

            command = "33 33 34"; 


            if (command != string.Empty)
            {
                int fodder = 0;
                scanner.DirectIO(-1, ref fodder, ref command);
            }
        }

        private void DataEvent(int Status)
        {
            AppendLog("Data: " + _OposScanner.ScanDataLabel);
            _OposScanner.DataEventEnabled = true;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(_OposScanner == null)
                {
                    return;
                }
                _OposScanner.DataEvent -= DataEvent;

                // Disable, release and close the scanner.
                _OposScanner.DataEventEnabled = false;
                _OposScanner.ReleaseDevice();
                _OposScanner.Close();
                tbxConneted.Background = Brushes.DarkRed;
                tbxConneted.Text = "Disconnect";
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
                _OposScanner = null;
            }
            catch (Exception ex) { 
                ex.ToString();
            }


        }

        private void btnOutputText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = Path.Combine(_settingMain._FolderPathScanner, $"{DateTime.Now.ToString("yy-MM-dd-HH-mm-scanner-data")}.csv");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"DATETIME,WEIGHT");
                    foreach (List<string> item in _Data)
                    {
                        writer.WriteLine($"{item[0]},{item[1]}");
                    }
                }
                UtilMethods.ShowMessageBox($"Successful Data Saved. \n{filePath}");
            }
            catch (Exception ex)
            {
                UtilMethods.ShowMessageBox($"Error: {ex.Message}");
            }
        }

        private void btnOuputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            // If the user selects a folder (not canceled), print the selected folder path
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settingMain._FolderPathScanner = folderDialog.SelectedPath;
                tbxOuputPath.Text = _settingMain._FolderPathScanner;
                _settingMain.Save();
            }
        }
    }

}
