//#define DEBUG_DEVICE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using SlimDX.Direct3D9;

namespace Graphics.GraphicsDevice
{
    public class GraphicsDevice9 : GraphicsDevice
    {
        public SlimDX.Direct3D9.Device Device9 { get; private set; }
        public SlimDX.Direct3D9.Direct3D D3D;

        public override event Action LostDevice;
        public override event Action ResetDevice;

        //public override void ApplySettings()
        //{
        //    if (Settings.RequiresDeviceReset)
        //    {
        //        MarkForReset();
        //        Settings.RequiresDeviceReset = false;
        //    }
        //}

        public override void Create()
        {
            D3D = new SlimDX.Direct3D9.Direct3D();
            InitParams();
#if DEBUG
            int adapter = 0;
            bool perfHud = false;
#endif

            Application.Log("Listing adapters..");

            for (int i = 0; i < D3D.AdapterCount; i++)
            {
                var res = D3D.GetAdapterIdentifier(i);
                Application.Log("[" + i + "] Adapter: " + res.Description);
#if DEBUG
                if (res.Description.Contains("PerfHUD"))
                {
                    adapter = i;
                    perfHud = true;
                    break;
                }
#endif
            }

            // Multithreaded flag:
            // "If you set that flag in D3D9 or D3D10, then the device will enter a critical section every time you call 
            // one of its methods (or a method on any objects created by the device, like a texture or vertex buffer). 
            // This makes it safe for you to do whatever you want from multiple threads, but it will end up serializing 
            // all of your API calls which will kill performance."
            // http://www.gamedev.net/community/forums/topic.asp?topic_id=533801&whichpage=1&#3450338
            //  * Needed since the physics thread performs mesh intersections which crashes the application if performed on an object being rendered
#if DEBUG
            if (perfHud)
            {
                Application.Log("Using NVidia PerfHUD adapter");
                Device9 = new Device(D3D, adapter, DeviceType.Reference, presentParams.DeviceWindowHandle,
                    CreateFlags.HardwareVertexProcessing | CreateFlags.FpuPreserve | CreateFlags.Multithreaded,   // see above
                    presentParams);
            }
            else
            {
#endif
                if (SettingConverters.VertexProcessing == CreateFlags.None)
                {
                    Device9 = new Device(D3D, 0, DeviceType.Hardware, presentParams.DeviceWindowHandle,
                        CreateFlags.HardwareVertexProcessing | CreateFlags.FpuPreserve | CreateFlags.Multithreaded,    // see above
                        presentParams);
                }
                else
                {
                    if (SettingConverters.VertexProcessing == CreateFlags.SoftwareVertexProcessing)
                    {
                        Device9 = new Device(D3D, 0, DeviceType.Hardware, presentParams.DeviceWindowHandle,
                                 SettingConverters.VertexProcessing | CreateFlags.FpuPreserve | CreateFlags.Multithreaded,    // see above
                                 presentParams);
                    }
                    else if (Settings.PureDevice)
                    {
                        Device9 = new Device(D3D, 0, DeviceType.Hardware, presentParams.DeviceWindowHandle,
                            SettingConverters.VertexProcessing | CreateFlags.FpuPreserve | CreateFlags.Multithreaded | CreateFlags.PureDevice,    // see above
                            presentParams);
                    }
                    else
                    {
                        Device9 = new Device(D3D, 0, DeviceType.Hardware, presentParams.DeviceWindowHandle,
                            SettingConverters.VertexProcessing | CreateFlags.FpuPreserve | CreateFlags.Multithreaded,    // see above
                            presentParams);
                    }
                }
#if DEBUG
            }
#endif

            markedForReset = false;
        }

        void Settings_RequestingRestart()
        {
            MarkForReset();
        }

        public override void Destroy()
        {
            Device9.Dispose();
            D3D.Dispose();
        }

        private void InitParams()
        {
            if (Settings.DeviceMode == DeviceMode.Windowed)
            {
                if (View != null)
                {
                    //39 height and 104 width is the minimum distances required for the device to operate correctly with the window (this needs further investigation)
                    if (View.ClientRectangle.Height < 39)
                        presentParams.BackBufferHeight = 39;
                    else
                        presentParams.BackBufferHeight = View.ClientRectangle.Height;

                    if (View.ClientRectangle.Width < 104)
                        presentParams.BackBufferWidth = 104;
                    else
                        presentParams.BackBufferWidth = View.ClientRectangle.Width;

                    presentParams.DeviceWindowHandle = View.Handle;
                }
                presentParams.Windowed = true;
            }
            else
            {
                presentParams.BackBufferWidth = Settings.Resolution.Width;
                presentParams.BackBufferHeight = Settings.Resolution.Height;

                presentParams.DeviceWindowHandle = FullscreenForm.Handle;
                presentParams.Windowed = false;
            }

            //Anti-aliasing
            presentParams.Multisample = Settings.AntiAliasing;

            //Vertical Syncronization
            if (Settings.VSync == VerticalSyncMode.On)
                presentParams.PresentationInterval = PresentInterval.One;
            else
                presentParams.PresentationInterval = PresentInterval.Immediate;

            //32 bits Z-Buffer with 24 bits depth & (??8 bits stencil??)
            presentParams.EnableAutoDepthStencil = true;
            
#if DEBUG_DEVICE
            throw new Exception("Needs research on what formats do use at different resolutions or monitors or graphics cards");
#endif

            if (Settings.Resolution.Width < 1900 && Settings.Resolution.Height < 1000)
                presentParams.AutoDepthStencilFormat = Format.D16;
            else
            {
                if (SettingConverters.DepthBufferFormats.Contains(SlimDX.Direct3D9.Format.D24X8))
                    presentParams.AutoDepthStencilFormat = Format.D24X8;
                else
                {
                    presentParams.AutoDepthStencilFormat = Format.D16;
                    Application.Log("Enforcing D16 for depth stencil");
                }
            }

            /*presentParams.BackBufferFormat = Format.X8R8G8B8;
            presentParams.BackBufferCount = 1;
            presentParams.SwapEffect = SwapEffect.Discard;*/
        }        

        public override void Present()
        {
            Result result = Device9.Present();
            if (result == ResultCode.DeviceLost)
                MarkForReset();
        }

        public override bool PrepareFrame()
        {
            if (markedForReset)
            {
                if (!hasReportLost)
                {
                    if (LostDevice != null) LostDevice();
                    hasReportLost = true;
                }
                if (Device9.TestCooperativeLevel() == ResultCode.DeviceLost)
                    return false;

                var ok = Reset();
                markedForReset = !ok;
                return ok;
            }
            return true;
        }

        public bool Reset()
        {
            InitParams();

            var r = Device9.Reset(presentParams);
            if (r.IsSuccess)
            {
                if (ResetDevice != null)
                    ResetDevice();
                return true;
            }
            else
                return false;
        }

        private SlimDX.Direct3D9.PresentParameters presentParams = new PresentParameters();
    }
}
