using nAble;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace nAble_for_nRad2
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process instance = RunningInstance();

            if (instance == null)
            {
                try
                {
                    ApplicationConfiguration.Initialize();
                    Application.Run(new FormMain());
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"nAble Exiting -- error: {ex}");
                }
            }
            else
            {
                HandleRunningInstance(instance);
            }
        }

        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") ==
                    current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }

            return null;
        }

        public static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, WS_RESTORE);
            SetForegroundWindow(instance.MainWindowHandle);
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int WS_RESTORE = 9;
    }
}