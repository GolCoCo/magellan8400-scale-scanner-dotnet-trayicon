using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magellan8400ReaderTray.SubViews;
using System.Windows;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;

namespace Magellan8400ReaderTray
{
    public static class LicenseKeyLocator
    {
        public static void FindandRegisterLicenseKey()
        {
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH" +
                "1fcHVXRWJYVkd1XkY=");
        }
    }

    public static class UtilMethods
    {
        public static void ShowMessageBox(string content)
        {
            WinMessageBox form = new WinMessageBox(content);
            SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
            form.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            form.ShowDialog();
        }
    }
}
