using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;

namespace Graphics.Win32
{
    //Ripped from SlimDX's sample framework
    public static class NativeMethods
    {
        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(out NativeMessage message, IntPtr hwnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetClientRect(IntPtr hWnd, out NativeRectangle lpRect);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out NativeRectangle lpRect);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustWindowRect(ref NativeRectangle lpRect, uint dwStyle, [MarshalAs(UnmanagedType.Bool)]bool bMenu);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern uint SetThreadExecutionState(uint esFlags);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [SuppressUnmanagedCodeSecurityAttribute]
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        public static Rectangle GetClientRectangle(IntPtr handle)
        {
            NativeRectangle rect;
            if (!GetClientRect(handle, out rect))
                return Rectangle.Empty;

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        public static Rectangle GetWindowRectangle(IntPtr handle)
        {
            NativeRectangle rect;
            if (!GetWindowRect(handle, out rect))
                return Rectangle.Empty;

            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }
    }
}
