using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Graphics
{
    public sealed class RawInput
    {
        #region Win32

        private const int RIDEV_INPUTSINK = 0x00000100;
        private const int RID_INPUT = 0x10000003;

        private const int FAPPCOMMAND_MASK = 0xF000;
        private const int FAPPCOMMAND_MOUSE = 0x8000;
        private const int FAPPCOMMAND_OEM = 0x1000;

        private const int RIM_TYPEMOUSE = 0;
        private const int RIM_TYPEKEYBOARD = 1;
        private const int RIM_TYPEHID = 2;

        private const int RIDI_DEVICENAME = 0x20000007;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_INPUT = 0x00FF;
        private const int VK_OEM_CLEAR = 0xFE;
        private const int VK_LAST_KEY = VK_OEM_CLEAR; // this is a made up value used as a sentinel

        private const ushort HID_USAGE_PAGE_GENERIC = 0x01;
        private const ushort HID_USAGE_GENERIC_MOUSE = 0x02;

        private const int RIDEV_NOLEGACY = 0x00000030;

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;
            [FieldOffset(16)]
            public RAWMOUSE mouse;
            [FieldOffset(16)]
            public RAWKEYBOARD keyboard;
            [FieldOffset(16)]
            public RAWHID hid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSize;
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWHID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizHid;
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BUTTONSSTR
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonFlags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonData;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RAWMOUSE
        {
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(0)]
            public ushort usFlags;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            public uint ulButtons;
            [FieldOffset(4)]
            public BUTTONSSTR buttonsStr;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            public uint ulRawButtons;
            [FieldOffset(12)]
            public int lLastX;
            [FieldOffset(16)]
            public int lLastY;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(20)]
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWKEYBOARD
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort MakeCode;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Flags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Reserved;
            [MarshalAs(UnmanagedType.U2)]
            public ushort VKey;
            [MarshalAs(UnmanagedType.U4)]
            public uint Message;
            [MarshalAs(UnmanagedType.U4)]
            public uint ExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsage;
            [MarshalAs(UnmanagedType.U4)]
            public int dwFlags;
            public IntPtr hwndTarget;
        }
        
        [DllImport("User32.dll")]
        extern static uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

        [DllImport("User32.dll")]
        extern static uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

        [DllImport("User32.dll")]
        extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

        [DllImport("User32.dll")]
        extern static uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        #endregion

        public event KeyPressEventHandler KeyPressed;
        public event KeyEventHandler KeyDown, KeyUp;
        public bool[] PressedKeys = new bool[256];

        public event MouseEventHandler MouseMove;
        public System.Drawing.Point MousePosition { get; set; }

        public void RegisterKeyboard(IntPtr hwnd, bool exclusive)
        {
            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];

            rid[0].usUsagePage = HID_USAGE_PAGE_GENERIC;
            rid[0].usUsage = 0x06;
            rid[0].dwFlags = RIDEV_INPUTSINK;
            if (exclusive) rid[0].dwFlags |= RIDEV_NOLEGACY;
            rid[0].hwndTarget = hwnd;

            if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
            {
                throw new ApplicationException("Failed to register raw input device(s).");
            }
        }

        public void RegisterMouse(IntPtr hwnd)
        {
            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];

            rid[0].usUsagePage = HID_USAGE_PAGE_GENERIC;
            rid[0].usUsage = HID_USAGE_GENERIC_MOUSE;
            rid[0].dwFlags = RIDEV_INPUTSINK;
            rid[0].hwndTarget = hwnd;

            if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
            {
                throw new ApplicationException("Failed to register raw input device(s).");
            }
        }

        public void ProcessMessage(Message message)
        {
            if(message.Msg != WM_INPUT) return;

            uint dwSize = 0;

            GetRawInputData(message.LParam,
                             RID_INPUT, IntPtr.Zero,
                             ref dwSize,
                             (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));

            IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
            try
            {
                if (buffer != IntPtr.Zero &&
                    GetRawInputData(message.LParam,
                                     RID_INPUT,
                                     buffer,
                                     ref dwSize,
                                     (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
                {
                    RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));
                    
                    if (raw.header.dwType == RIM_TYPEKEYBOARD)
                    {
                        ushort key = raw.keyboard.VKey;
                        Keys keyData;
                        keyData = (Keys)Enum.Parse(typeof(Keys), Enum.GetName(typeof(Keys), key));

                        // On most keyboards, "extended" keys such as the arrow or 
                        // page keys return two codes - the key's own code, and an
                        // "extended key" flag, which translates to 255. This flag
                        // isn't useful to us, so it can be disregarded.
                        if (key > VK_LAST_KEY)
                        {
                            return;
                        }

                        if (raw.keyboard.Flags == 0)
                        {
                            if (KeyPressed != null)
                                KeyPressed(this, new KeyPressEventArgs((char)raw.keyboard.MakeCode));

                            if (!PressedKeys[(int)keyData])
                            {
                                PressedKeys[(int)keyData] = true;
                                if (KeyDown != null) KeyDown(this, new KeyEventArgs(keyData));
                            }
                        }
                        else
                        {
                            PressedKeys[(int)keyData] = false;
                            if (KeyUp != null) KeyUp(this, new KeyEventArgs(keyData));
                        }
                    }
                    else if (raw.header.dwType == RIM_TYPEMOUSE)
                    {
                        MousePosition = new System.Drawing.Point(
                            MousePosition.X + raw.mouse.lLastX,
                            MousePosition.Y + raw.mouse.lLastY);
                        if(MouseMove != null)
                            MouseMove(this, new MouseEventArgs(MouseButtons.None, 0,
                                MousePosition.X, MousePosition.Y, 0));
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

    }
}

