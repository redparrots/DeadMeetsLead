using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Graphics.GraphicsDevice
{
    public class Viewport
    {
        public Viewport() { }
        public Viewport(SlimDX.Direct3D9.Viewport viewport) 
        { 
            X = viewport.X;
            Y = viewport.Y;
            Width = viewport.Width;
            Height = viewport.Height;
            MinZ = viewport.MinZ;
            MaxZ = viewport.MaxZ;
        }
        public Viewport(SlimDX.Direct3D10.Viewport viewport)
        {
            X = viewport.X;
            Y = viewport.Y;
            Width = viewport.Width;
            Height = viewport.Height;
            MinZ = viewport.MinZ;
            MaxZ = viewport.MaxZ;
        }
        public Vector3 ToViewport(Vector3 clipSpacePosition)
        {
            Vector3 o;
            o.X = (clipSpacePosition.X + 1) * Width * 0.5f + X;
            o.Y = (1 - clipSpacePosition.Y) * Height * 0.5f + Y;
            o.Z = MinZ + clipSpacePosition.Z * (MaxZ - MinZ);
            return o;
        }
        public Vector3 FromViewport(Vector3 viewportPosition)
        {
            Vector3 o;
            o.X = (viewportPosition.X - X) / (Width * 0.5f) - 1;
            o.Y = 1 - (viewportPosition.Y - Y) / (Height * 0.5f);
            o.Z = (viewportPosition.Z - MinZ) / (MaxZ - MinZ);
            return o;
        }
        public float X, Y, Width, Height, MinZ, MaxZ;
    }

    public class GraphicsDevice
    {
        public GraphicsDevice() { }

        public Settings Settings = new Settings();

        public System.Windows.Forms.Control View;
        public System.Windows.Forms.Form FullscreenForm;
        public SlimDX.Direct3D10.RenderTargetView RenderView;
        public SlimDX.Direct3D10.DepthStencilView DepthStencilView;

        public SlimDX.Direct3D10.Viewport Viewport10;
        protected bool markedForReset = false;
        protected bool hasReportLost = true;

        public virtual event Action LostDevice;
        public virtual event Action ResetDevice;

        public virtual bool PrepareFrame()
        {
            return true;
        }

        public virtual void ToggleFullScreen(IntPtr handle) { }

        public virtual void ApplySettings() { }

        public void MarkForReset()
        {
            if (!markedForReset)
                hasReportLost = false;

            markedForReset = true;
        }

        public Direct3DVersion Direct3DVersion { get; set; }

        public virtual void Create() { }

        public virtual void Create(IntPtr handle) { }

        public virtual void Destroy() { }

        public virtual void Present() { }
    }
}
