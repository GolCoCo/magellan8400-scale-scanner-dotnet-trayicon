using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magellan8400ReaderTray.Models;
using OposScanner_CCO;
using System.Windows.Media;
using System.Windows.Documents;
using System.IO;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Magellan8400ReaderTray.Controllers
{
    public class ScannerController
    {
        private OPOSScannerClass _OposScanner = null;
        private SettingMain _settingMain;
        private List<List<string>> _Data;
        private string _DeviceName = "MagellanSC";
        private ListBox _listBox;
        public ScannerController(ListBox listBox)
        {
            this._Data = new List<List<string>>();
            _settingMain = SettingMain.Load<SettingMain>();
            if (_settingMain == null)
            {
                _settingMain = new SettingMain();
            }
            _listBox = listBox;
            _OposScanner = new OPOSScannerClass();
        }

        public bool Start()
        {
            try
            {
                _OposScanner.Open(_DeviceName);
                _OposScanner.ClaimDevice(1000);
                if (_OposScanner.Claimed)
                {
                    // Subscribe to the delegate.
                    _OposScanner.DataEvent += DataEvent;

                    // Enable the scanner and decoding events.
                    _OposScanner.DeviceEnabled = true;
                    _OposScanner.DataEventEnabled = true;
                    _OposScanner.DecodeData = true;

                    GoodBeep(ref _OposScanner, _DeviceName);
                    return true;

                }
                else
                {
                    _OposScanner.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                UtilMethods.ShowMessageBox($"{ex.Message}");
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (_OposScanner == null)
                {
                    return false;
                }
                _OposScanner.DataEvent -= DataEvent;

                // Disable, release and close the scanner.
                _OposScanner.DataEventEnabled = false;
                _OposScanner.ReleaseDevice();
                _OposScanner.Close();
                _OposScanner = null;
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        private void GoodBeep(ref OPOSScannerClass scanner, string profileName)
        {
            string command = string.Empty;

            command = "33 33 34";

            if (command != string.Empty)
            {
                int fodder = 0;
                scanner.DirectIO(-1, ref fodder, ref command);
            }
        }

        private void DataEvent(int Status)
        {
            string data = _OposScanner.ScanDataLabel;
            CopyAndPasteForString.PasteToFocusedApp(data);
            AppendLog(data);
            _OposScanner.DataEventEnabled = true;
            OutputText();
        }

        public void OutputText()
        {
            try
            {
                string filePath = Path.Combine(_settingMain._FolderPathScanner, $"{DateTime.Now.ToString("yy-MM-dd-HH-mm-scanner-data")}.csv");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"DATETIME,CODE");
                    foreach (List<string> item in _Data)
                    {
                        writer.WriteLine($"{item[0]},{item[1]}");
                    }
                }

            }
            catch (Exception ex)
            {
                UtilMethods.ShowMessageBox($"Error: {ex.Message}");
            }
        }

        private void AppendLog(string data)
        {
            //_listBox.Items.Add(data);
            string csdt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            _Data.Add(new List<string> { csdt, data });
        }
    }
}
0.44