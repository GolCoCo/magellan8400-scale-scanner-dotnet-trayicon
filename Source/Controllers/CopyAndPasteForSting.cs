using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Magellan8400ReaderTray.Controllers
{
    public class CopyAndPasteForString
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void PasteToFocusedApp(string text)
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    // Set the clipboard text
                    Clipboard.SetText(text);
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log them)
                    Console.WriteLine("Error setting clipboard text: " + ex.Message);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            // Optional: delay to ensure clipboard is set
            Thread.Sleep(100);

            IntPtr foregroundWindow = GetForegroundWindow();
            SetForegroundWindow(foregroundWindow);

            // Optional: delay to ensure the window is focused
            Thread.Sleep(100);

            // Simulate Ctrl+V to paste
            SendKeys.SendWait("^v");
        }
    }
}
