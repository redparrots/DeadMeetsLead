using System;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DXGI;
using SlimDX.Windows;
using Graphics;
using Graphics.Content;
using System.Collections.Generic;
using Graphics.GraphicsDevice;

namespace GroundTextureEditor
{
    class Program : View
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            Application.QuickStartSimpleWindow(new Program());
        }

        public override void Init()
        {
            Graphics.Software.ITexture t = 
                new Graphics.Software.Texture<Graphics.Software.Texel.R8G8B8A8>(100, 100);
            for (int y = 0; y < 50; y++)
                for (int x = 0; x < 100; x++)
                    t[y, x] = new Graphics.Software.Texel.R8G8B8A8
                    {
                        R = 1,
                        G = 1,
                        B = 1,
                        A = 1,
                    };
            for (int y = 50; y < 100; y++)
                for (int x = 0; x < 100; x++)
                    t[y, x] = new Graphics.Software.Texel.R8G8B8A8
                    {
                        R = 0,
                        G = 1,
                        B = 1,
                        A = 1,
                    };
            var tex = t.ToTexture9(Device9);
            SlimDX.Direct3D9.Texture.ToFile(tex, "test.jpg", ImageFileFormat.Jpg);
            tex.Dispose();
        }
    }
}
