using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MathCalc.Gui
{
    internal static class Win32
    {
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageW([InAttribute] System.IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        public const int WM_GETTEXT = 0x000D; // https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-gettext
        public const int WM_COPY = 0x0301; // https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-messages
        public const int WM_PASTE = 0x0302;
        public const int WM_GETTEXTLENGTH = 0x000E;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(int handle, out int processId);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
        [DllImport("kernel32.dll")]
        internal static extern int GetCurrentThreadId();


        internal static string GetSelectedTextFromActiveWindow()
        {
            int activeWinPtr = GetForegroundWindow().ToInt32();
            int activeThreadId = 0, processId;
            activeThreadId = GetWindowThreadProcessId(activeWinPtr, out processId);
            int currentThreadId = GetCurrentThreadId();
            if (activeThreadId != currentThreadId)
                AttachThreadInput(activeThreadId, currentThreadId, true);
            IntPtr activeCtrlId = GetFocus();

            int maxLength = 1 + SendMessageW(activeCtrlId, WM_GETTEXTLENGTH, 0, 0);
            IntPtr buffer = Marshal.AllocHGlobal(maxLength);
            //SendMessageW(activeCtrlId, WM_GETTEXTLENGTH, maxLength, buffer);
            SendMessageW(activeCtrlId, WM_GETTEXT, maxLength, buffer);
            string w = Marshal.PtrToStringUni(buffer);
            //string w = Marshal.Ptr(buffer);
            Marshal.FreeHGlobal(buffer);
            return w ?? string.Empty;
        }


        internal static void CopyTextFromActiveWindow()
        {
            //int activeWinPtr = GetForegroundWindow().ToInt32();
            //int activeThreadId = 0, processId;
            //activeThreadId = GetWindowThreadProcessId(activeWinPtr, out processId);
            //int currentThreadId = GetCurrentThreadId();
            //if (activeThreadId != currentThreadId)
            //    AttachThreadInput(activeThreadId, currentThreadId, true);
            IntPtr activeCtrlId = GetFocus();
            SendMessageW(activeCtrlId, WM_COPY, 0, 0);
        }
    }
}
