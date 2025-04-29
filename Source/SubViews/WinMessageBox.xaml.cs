using Syncfusion.Windows.Shared;
using System.Windows;

namespace Magellan8400ReaderTray.SubViews
{
    /// <summary>
    /// Interaction logic for WinMessageBox.xaml
    /// </summary>
    public partial class WinMessageBox : ChromelessWindow
    {
        private WinMessageBox _form;
        public WinMessageBox(string content)
        {
            InitializeComponent();
            _form = this;
            tbxContent.Text = content;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _form.Close();
        }
    }
}
