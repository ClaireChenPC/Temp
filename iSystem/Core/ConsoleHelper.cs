// ReSharper disable UnusedMember.Global

using System.Diagnostics;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;
using static Vanara.PInvoke.Kernel32;
using static System.Console;
#if DEBUG
using System.Runtime.InteropServices;
#endif

namespace iSystem.Core
{
    public static class ConsoleHelper
    {
        public static void InitializeConsole(string title)
        {
            Trace.Listeners.AddRange(new TraceListener[] { new TextWriterTraceListener(Out) });

#if DEBUG
            // Get a pointer to the foreground window.  The idea here is that
            //IF the user is starting our application from an existing console
            //shell, that shell will be the uppermost window.  We'll get it
            //and attach to it
            //var handleWindow = GetForegroundWindow();
            //GetWindowThreadProcessId(handleWindow, out var u);
            //var process = Process.GetProcessById((int)u);

            //if (process.ProcessName == Process.GetCurrentProcess().ProcessName)
            //{
            //    AttachConsole((uint)process.Id);
            //}
            //else
            //{
            //    //no console AND we're in console mode ... create a new console.
            //    AllocConsole();

            //    var hRealOut = CreateFile(
            //        "CONOUT$",
            //        Kernel32.FileAccess.FILE_GENERIC_READ | Kernel32.FileAccess.FILE_GENERIC_WRITE,
            //        FileShare.Write,
            //        null,
            //        FileMode.OpenOrCreate,
            //        0);
            //    SetStdHandle(StdHandleType.STD_OUTPUT_HANDLE, (HFILE)hRealOut);
            //    SetOut(new StreamWriter(OpenStandardOutput(), OutputEncoding) { AutoFlush = true });
            //}

            // Get this console window's hWnd (window handle).
            var hWnd = GetConsoleWindow();
            if (HWND.NULL == hWnd) return;

            ForegroundColor = ConsoleColor.Green;
            Title = title;
            CursorVisible = false;

            // Get information about the monitor (display) that the window is (mostly) displayed on.
            // The .rcWork field contains the monitor's work area, i.e., the usable space excluding
            // the task-bar (and "application desktop tool-bars")
            var mi = new MONITORINFO();
            mi.cbSize = (uint)Marshal.SizeOf(mi);
            GetMonitorInfo(MonitorFromWindow(hWnd, MonitorFlags.MONITOR_DEFAULTTOPRIMARY), ref mi);

            // Get information about this window's current placement.
            var wp = new WINDOWPLACEMENT();
            wp.length = (uint)Marshal.SizeOf(wp);
            GetWindowPlacement(hWnd, ref wp);

            // Calculate the window's new position: lower left corner.
            // !! Inexplicably, on W10, work-area coordinates (0,0) appear to be (3,3) pixels
            // !! away from the true edge of the screen/task-bar.
            const int fudgeOffset = 3;
            wp.rcNormalPosition = new RECT
            {
                left = -fudgeOffset,
                top = mi.rcWork.bottom - (wp.rcNormalPosition.bottom - wp.rcNormalPosition.top) - mi.rcWork.top,
                right = wp.rcNormalPosition.right - wp.rcNormalPosition.left,
                bottom = fudgeOffset + mi.rcWork.bottom - mi.rcWork.top
            };

            // Place the window at the new position.
            SetWindowPlacement(hWnd, ref wp);

            try
            {
                ShowWindow(hWnd, ShowWindowCommand.SW_NORMAL);

                var closeMenu = GetSystemMenu(hWnd, false);
                RemoveMenu(closeMenu, (uint)SysCommand.SC_CLOSE, MenuFlags.MF_BYCOMMAND);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }

#else
            try
            {
                // Get this console window's hWnd (window handle).
                var hWnd = GetConsoleWindow();

                if (HWND.NULL == hWnd)
                    return;
                ShowWindow(hWnd, ShowWindowCommand.SW_HIDE);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
#endif
        }
    }
}