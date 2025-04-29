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

namespace Magellan8400ReaderTray.Views
{
    /// <summary>
    /// Interaction logic for WinMain.xaml
    /// </summary>
    public partial class WinScale : ChromelessWindow
    {
        public string AppTitle = "MAGELLAN 8400 SCALE";
        public Brush TitleBarBackground = new SolidColorBrush(Color.FromRgb(43, 87, 154));
        public Brush TitleBarForeground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private static OPOSScaleClass _OposScale = null;
        private SettingMain _settingMain;
        private static String[] names =
        {
            "USBScale",
            "RS232Scale",
            "SCRS232Scale"
        };

        private List<List<string>> _Data;

        private string _DeviceName = "MagellanSC";

        public WinScale()
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
            _OposScale = new OPOSScaleClass();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            tbxOuputPath.Text = _settingMain._FolderPathScale;
        }

        private void StatusUpdateEvent(int value)
        {
            int status = (int)_OposScale.ResultCode;

            if (value == (int)OPOSScaleConstants.SCAL_SUE_STABLE_WEIGHT)
            {
                AppendLog(WeightFormat(_OposScale.ScaleLiveWeight));
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNSTABLE)
            {
                AppendLog("-> Scale weight unstable");
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_ZERO)
            {
                AppendLog(WeightFormat(_OposScale.ScaleLiveWeight));
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_OVERWEIGHT)
            {
                AppendLog("-> Weight limit exceeded.");
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_NOT_READY)
            {
                AppendLog("-> Scale not ready.");
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNDER_ZERO)
            {
                AppendLog("-> Scale under zero weight.");
            }
            else
            {
                AppendLog($"-> Unknown status {value}");
            }
        }

        private void AppendLog(string data)
        {
            Dispatcher.Invoke(() =>
            {
                lbxLogs.Items.Add(data);
            });
        }

        private string WeightFormat(int weight)
        {
            string weightStr = string.Empty;

            string units = UnitAbbreviation(_OposScale.WeightUnits);
            if (units == string.Empty)
            {
                weightStr = string.Format("Unknown weight unit");
            }
            else
            {
                double dWeight = 0.001 * (double)weight;
                weightStr = string.Format("{0:0.000} {1}", dWeight, units);
                string csdt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                weightStr = $"{csdt} - Weight: {weightStr}";
                _Data.Add(new List<string> { csdt, weightStr });
            }

            return weightStr;
        }

        private string UnitAbbreviation(int units)
        {
            string unitStr = string.Empty;

            switch ((OPOSScaleConstants)units)
            {
                case OPOSScaleConstants.SCAL_WU_GRAM: unitStr = "gr."; break;
                case OPOSScaleConstants.SCAL_WU_KILOGRAM: unitStr = "Kg."; break;
                case OPOSScaleConstants.SCAL_WU_OUNCE: unitStr = "oz."; break;
                case OPOSScaleConstants.SCAL_WU_POUND: unitStr = "lb."; break;
            }

            return unitStr;
        }


        private void ChromelessWindow_Closing(object sender, CancelEventArgs e)
        {

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _OposScale.Open(_DeviceName);
                _OposScale.ClaimDevice(1000);
                if (_OposScale.Claimed)
                {
                    tbxConneted.Background = Brushes.DarkGreen;
                    tbxConneted.Text = "Connected";
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;

                    _OposScale.StatusNotify = (int)OPOSScaleConstants.SCAL_SN_ENABLED;
                    if (_OposScale.ResultCode == (int)OPOSConstants.OPOS_SUCCESS)
                    {
                        // Subscribe to the delegate.
                        _OposScale.StatusUpdateEvent += StatusUpdateEvent;

                        // Enable scale events.
                        _OposScale.DeviceEnabled = true;
                        if (_OposScale.DeviceEnabled)
                        {
                            _OposScale.DataEventEnabled = true;
                        }
                    }
                }
                else
                {
                    _OposScale.Close();
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

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(_OposScale == null)
                {
                    return;
                }
                _OposScale.DataEventEnabled = false;
                _OposScale.StatusUpdateEvent -= StatusUpdateEvent;
                _OposScale.ReleaseDevice();
                _OposScale.Close();
                tbxConneted.Background = Brushes.DarkRed;
                tbxConneted.Text = "Disconnect";
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
                _OposScale = null;
            }
            catch (Exception ex) { 
                ex.ToString();
            }


        }

        private void btnOutputText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = Path.Combine(_settingMain._FolderPathScale, $"{DateTime.Now.ToString("yy-MM-dd-HH-mm-weight-data")}.csv");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"DATETIME,WEIGHT");
                    foreach (List<string> item in _Data)
                    {
                        writer.WriteLine($"{item[0]},{item[1]}");
                    }
                }
                UtilMethods.ShowMessageBox("Data saved to scanned_data.txt on your desktop.");
            }
            catch (Exception ex)
            {
                UtilMethods.ShowMessageBox($"Error: {ex.Message}");
            }
        }

        private void btnOuputPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _settingMain._FolderPathScanner = folderDialog.SelectedPath;
                tbxOuputPath.Text = _settingMain._FolderPathScanner;
            }
        }
    }

}
