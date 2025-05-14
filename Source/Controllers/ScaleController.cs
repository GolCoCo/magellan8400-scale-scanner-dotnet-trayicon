using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magellan8400ReaderTray.Models;
using OposScale_CCO;
using System.Windows.Media;
using OPOSCONSTANTSLib;
using OPOSConstants = OPOSCONSTANTSLib.OPOS_Constants;
using OPOSScaleConstants = OPOSCONSTANTSLib.OPOSScaleConstants;
using System.IO;
using System.Windows.Controls;

namespace Magellan8400ReaderTray.Controllers
{
    public class ScaleController
    {
        private OPOSScaleClass _OposScale = null;
        private SettingMain _settingMain;
        private List<List<string>> _Data;
        private string _DeviceName = "MagellanSC";
        private ListBox _listBox;

        public ScaleController(ListBox listBox)
        {
            this._Data = new List<List<string>>();
            _settingMain = SettingMain.Load<SettingMain>();
            if (_settingMain == null)
            {
                _settingMain = new SettingMain();
            }
            _listBox = listBox;
            _OposScale = new OPOSScaleClass();
        }

        public bool Start()
        {
            try
            {
                _OposScale.Open(_DeviceName);
                _OposScale.ClaimDevice(1000);
                if (_OposScale.Claimed)
                {
                    _OposScale.StatusNotify = (int)OPOSScaleConstants.SCAL_SN_ENABLED;
                    if (_OposScale.ResultCode == (int)OPOSConstants.OPOS_SUCCESS)
                    {
                        // Subscribe to the delegate.
                        _OposScale.StatusUpdateEvent += OnStatusUpdateEvent;

                        // Enable scale events.
                        _OposScale.DeviceEnabled = true;
                        if (_OposScale.DeviceEnabled)
                        {
                            _OposScale.DataEventEnabled = true;
                        }
                    }
                    return true;
                }
                else
                {
                    _OposScale.Close();
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
                if (_OposScale == null)
                {
                    return false;
                }
                _OposScale.DataEventEnabled = false;
                _OposScale.StatusUpdateEvent -= OnStatusUpdateEvent;
                _OposScale.ReleaseDevice();
                _OposScale.Close();
                _OposScale = null;
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        public string WeightFormat(int weight)
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
                weightStr = $"{dWeight}";
                string csdt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                //weightStr = $"{csdt} - Weight: {weightStr}";
                _Data.Add(new List<string> { csdt, weightStr });
            }

            return weightStr;
        }

        public string UnitAbbreviation(int units)
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

        public void OnStatusUpdateEvent(int value)
        {
            int status = (int)_OposScale.ResultCode;
            string data = "";
            if (value == (int)OPOSScaleConstants.SCAL_SUE_STABLE_WEIGHT)
            {
                data = WeightFormat(_OposScale.ScaleLiveWeight);
                CopyAndPasteForString.PasteToFocusedApp(data);
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNSTABLE)
            {
                data = "-> Scale weight unstable";
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_ZERO)
            {
                data = WeightFormat(_OposScale.ScaleLiveWeight);
                CopyAndPasteForString.PasteToFocusedApp(data);
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_OVERWEIGHT)
            {
                data = "-> Weight limit exceeded.";
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_NOT_READY)
            {
                data = "-> Scale not ready.";
            }
            else if (value == (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNDER_ZERO)
            {
                data = "-> Scale under zero weight.";
            }
            else
            {
                data = $"-> Unknown status {value}";
            }
            AppendLog(data);
            OutputText();
        }

        public void OutputText()
        {
            try
            {
                string filePath = Path.Combine(_settingMain._FolderPathScale, $"{DateTime.Now.ToString("yy-MM-dd-HH-mm-scanner-data")}.csv");
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine($"DATETIME,WEIGHT");
                    foreach (List<string> item in _Data)
                    {
                        writer.WriteLine($"{item[0]},{item[1]}");
                    }
                }
                //UtilMethods.ShowMessageBox($"Successful Data Saved. \n{filePath}");
            }
            catch (Exception ex)
            {
                UtilMethods.ShowMessageBox($"Error: {ex.Message}");
            }
        }

        private void AppendLog(string data)
        {
            //_listBox.Items.Add(data);
        }

    }
}
