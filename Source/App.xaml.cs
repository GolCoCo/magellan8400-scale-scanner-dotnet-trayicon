using Syncfusion.SfSkinManager;
using System.Windows;
using Magellan8400ReaderTray.Views;
using Magellan8400ReaderTray.Controllers;

namespace Magellan8400ReaderTray
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            LicenseKeyLocator.FindandRegisterLicenseKey();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            WinSWMain form = new WinSWMain();
            SfSkinManager.SetTheme(form, new Theme() { ThemeName = "Office2019Black" });
            form.ShowDialog();
            base.OnStartup(e);
        }
    }
}
