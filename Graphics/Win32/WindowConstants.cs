using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics.Win32
{
    //Ripped from SlimDX's framework
    public static class WindowConstants
    {
        public const int WM_SIZE = 0x5;
        public const int WM_SYSCOMMAND = 0x112;
        public const int WM_ACTIVATEAPP = 0x001C;
        public const int WM_POWERBROADCAST = 0x0218;

        public const int SC_SCREENSAVE = 0xF140;
        public const int SC_MONITORPOWER = 0xF170;

        public const int VK_LWIN = 0x5B;
        public const int VK_RWIN = 0x5C;

        public static readonly IntPtr SIZE_MINIMIZED = new IntPtr(1);
        public static readonly IntPtr SIZE_MAXIMIZED = new IntPtr(2);
        public static readonly IntPtr SIZE_RESTORED = new IntPtr(0);

        public static readonly IntPtr PBT_APMQUERYSUSPEND = new IntPtr(0x0000);
        public static readonly IntPtr PBT_APMRESUMESUSPEND = new IntPtr(0x0007);

        public const int WPF_RESTORETOMAXIMIZED = 2;

        public const int SW_RESTORE = 9;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const long WS_MAXIMIZE = 0x01000000;
        public const long WS_MINIMIZE = 0x20000000;
        public const long WS_POPUP = 0x80000000;
        public const long WS_SYSMENU = 0x00080000;

        public const long WS_EX_TOPMOST = 0x00000008;

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOREDRAW = 0x0008;

        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;

        public const int MONITOR_DEFAULTTOPRIMARY = 1;
    }
}
