using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Graphics.GraphicsDevice
{
    public class GraphicsDevice10 : GraphicsDevice
    {

        public SlimDX.DXGI.SwapChain SwapChain;
        SlimDX.DXGI.Factory factory;
        SlimDX.Direct3D10.DepthStencilState depthStencilState;
        SlimDX.Direct3D10.Texture2D backBuffer, depthStencil;
        public SlimDX.Direct3D10.Device Device10;

        public override bool PrepareFrame()
        {
            return true;
        }
        
        public override void ToggleFullScreen(IntPtr handle)
        {
            //if (LostDevice != null)
            //    LostDevice();

            factory.SetWindowAssociation(handle, SlimDX.DXGI.WindowAssociationFlags.IgnoreAll);


            SwapChain.Dispose();
            backBuffer.Dispose();
            RenderView.Dispose();

            SwapChain = new SlimDX.DXGI.SwapChain(factory, Device10, new SlimDX.DXGI.SwapChainDescription
            {  
                BufferCount = 1,
                ModeDescription = new SlimDX.DXGI.ModeDescription(View.ClientSize.Width, View.ClientSize.Height, new Rational(60, 1), SlimDX.DXGI.Format.R8G8B8A8_UNorm),
                IsWindowed = false,
                OutputHandle = handle,
                SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0),
                SwapEffect = SlimDX.DXGI.SwapEffect.Discard,
                Usage = SlimDX.DXGI.Usage.RenderTargetOutput,
                Flags = SlimDX.DXGI.SwapChainFlags.AllowModeSwitch
            });
            backBuffer = SlimDX.Direct3D10.Texture2D.FromSwapChain<SlimDX.Direct3D10.Texture2D>(SwapChain, 0);
            RenderView = new SlimDX.Direct3D10.RenderTargetView(Device10, backBuffer);
            
            //Create(handle);
            
            //SwapChain.SetFullScreenState(true, null);
            
            //if (ResetDevice != null)
            //    ResetDevice();

        }

        public override void Destroy()
        {
            Device10.Dispose();
            SwapChain.Dispose();
            backBuffer.Dispose();
            RenderView.Dispose();
            DepthStencilView.Dispose();
            depthStencil.Dispose();
            depthStencilState.Dispose();
        }

        public override void Create(IntPtr handle)
        {
            var desc = new SlimDX.DXGI.SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new SlimDX.DXGI.ModeDescription(View.ClientSize.Width, 
                    View.ClientSize.Height, new Rational(60, 1), SlimDX.DXGI.Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = handle,
                SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0),
                SwapEffect = SlimDX.DXGI.SwapEffect.Discard,
                Usage = SlimDX.DXGI.Usage.RenderTargetOutput,
                Flags = SlimDX.DXGI.SwapChainFlags.AllowModeSwitch
            };
            
            SlimDX.Direct3D10.Device.CreateWithSwapChain(null, SlimDX.Direct3D10.DriverType.Hardware,
                SlimDX.Direct3D10.DeviceCreationFlags.Debug, desc, out Device10, out SwapChain);
            backBuffer = SlimDX.Direct3D10.Texture2D.FromSwapChain<SlimDX.Direct3D10.Texture2D>(SwapChain, 0);
            RenderView = new SlimDX.Direct3D10.RenderTargetView(Device10, backBuffer);

            SlimDX.Direct3D10.Texture2DDescription depthStencilDesc = new SlimDX.Direct3D10.Texture2DDescription
            {
                Width = View.Width,
                Height = View.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = SlimDX.DXGI.Format.D32_Float,
                SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0),
                Usage = SlimDX.Direct3D10.ResourceUsage.Default,
                BindFlags = SlimDX.Direct3D10.BindFlags.DepthStencil,
                CpuAccessFlags = SlimDX.Direct3D10.CpuAccessFlags.None,
                OptionFlags = SlimDX.Direct3D10.ResourceOptionFlags.None
            };
            depthStencil = new SlimDX.Direct3D10.Texture2D(Device10, depthStencilDesc);
            DepthStencilView = new SlimDX.Direct3D10.DepthStencilView(Device10, depthStencil,
                new SlimDX.Direct3D10.DepthStencilViewDescription
                {
                    Format = SlimDX.DXGI.Format.D32_Float,
                    FirstArraySlice = 0,
                    ArraySize = 1,
                    MipSlice = 0,
                    Dimension = SlimDX.Direct3D10.DepthStencilViewDimension.Texture2D
                });

            SlimDX.Direct3D10.DepthStencilStateDescription dssd = new SlimDX.Direct3D10.DepthStencilStateDescription
            {
                IsDepthEnabled = true,
                DepthWriteMask = SlimDX.Direct3D10.DepthWriteMask.All,
                DepthComparison = SlimDX.Direct3D10.Comparison.Less,
                IsStencilEnabled = true,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF,
                FrontFace = new SlimDX.Direct3D10.DepthStencilOperationDescription
                {
                    FailOperation = SlimDX.Direct3D10.StencilOperation.Keep,
                    DepthFailOperation = SlimDX.Direct3D10.StencilOperation.Increment,
                    PassOperation = SlimDX.Direct3D10.StencilOperation.Keep,
                    Comparison = SlimDX.Direct3D10.Comparison.Always
                },
                BackFace = new SlimDX.Direct3D10.DepthStencilOperationDescription
                {
                    FailOperation = SlimDX.Direct3D10.StencilOperation.Keep,
                    DepthFailOperation = SlimDX.Direct3D10.StencilOperation.Decrement,
                    PassOperation = SlimDX.Direct3D10.StencilOperation.Keep,
                    Comparison = SlimDX.Direct3D10.Comparison.Always
                }
            };

            depthStencilState = SlimDX.Direct3D10.DepthStencilState.FromDescription(Device10, dssd);
            Device10.OutputMerger.DepthStencilState = depthStencilState;
            Device10.OutputMerger.SetTargets(DepthStencilView, RenderView);
            Viewport10 = new SlimDX.Direct3D10.Viewport(0, 0, View.ClientSize.Width, View.ClientSize.Height, 0.0f, 1.0f);
            Device10.Rasterizer.SetViewports(Viewport10);

            //Stops Alt+enter from causing fullscreen skrewiness.
            factory = SwapChain.GetParent<SlimDX.DXGI.Factory>();
            factory.SetWindowAssociation(View.Handle, SlimDX.DXGI.WindowAssociationFlags.IgnoreAll);
        }

        public override void Present()
        {
            SwapChain.Present(0, SlimDX.DXGI.PresentFlags.None);
        }
    }
}
