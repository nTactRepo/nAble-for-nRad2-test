using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace nAble
{
    public static class ProcessHelpers
    {
        public static void OpenLKNavigator2()
        {
            OpenProcess("LK-Navigator2", @"C:\Program Files (x86)\KEYENCE\LK-Navigator 2\LK-Navigator2.exe");
        }

        public static void OpenProcess(string procName, string path)
        {
            if (!switchWindow(procName))
            {
                Process newProc = new Process();
                newProc.StartInfo.FileName = path;
                newProc.Start();
            }
        }

        static public void ShowThreaded(Form owner, Action action)
        {
            owner.Invoke(action, owner);
        }

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool turnon);

        //now we have switch window.
        public static bool switchWindow(string procWindow)
        {
            bool found = false;
            Process[] procs = Process.GetProcessesByName(procWindow);

            if (procs.Count() > 0)
            {
                found = true;

                foreach (Process proc in procs)
                {
                    //switch to process by name
                    SwitchToThisWindow(proc.MainWindowHandle, turnon: true);
                }
            }

            return found;
        }

    }
}
