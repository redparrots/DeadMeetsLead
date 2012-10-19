using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;
using Graphics.Content;

namespace Graphics.Interface
{
    public class KeyCombination
    {
        public override string ToString()
        {
            if (Modifier != System.Windows.Forms.Keys.None)
                return Modifier + " + " + KeyToString();
            else
                return KeyToString();
        }
        String KeyToString()
        {
            switch (Key)
            {
                case System.Windows.Forms.Keys.D0: return "0";
                case System.Windows.Forms.Keys.D1: return "1";
                case System.Windows.Forms.Keys.D2: return "2";
                case System.Windows.Forms.Keys.D3: return "3";
                case System.Windows.Forms.Keys.D4: return "4";
                case System.Windows.Forms.Keys.D5: return "5";
                case System.Windows.Forms.Keys.D6: return "6";
                case System.Windows.Forms.Keys.D7: return "7";
                case System.Windows.Forms.Keys.D8: return "8";
                case System.Windows.Forms.Keys.D9: return "9";
                default: return Key.ToString();
            }
        }
        public System.Windows.Forms.Keys Key;
        public System.Windows.Forms.Keys Modifier;
        public class EqualityComparer : IEqualityComparer<KeyCombination>
        {
            public bool Equals(KeyCombination a, KeyCombination b)
            {
                if (a.Key != b.Key) return false;
                if (a.Modifier != b.Modifier) return false;
                return true;
            }

            public int GetHashCode(KeyCombination x)
            {
                return x.Key.GetHashCode() ^ x.Modifier.GetHashCode();
            }
        }
    }

    public class InterfaceManager : InteractiveSceneManager
    {
        public InterfaceScene InterfaceScene { get { return (InterfaceScene)Scene; } }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            //((Control)Scene.Root).Size = new Vector2(Scene.View.Size.Width, Scene.View.Size.Height);
            //Scene.Camera = new OrthoCamera
            //{
            //    Width = Scene.View.Size.Width,
            //    Height = Scene.View.Size.Height,
            //    Position = new Vector3(Scene.View.Size.Width / 2f, Scene.View.Size.Height / 2f, 0)
            //};
            Size s;
            if (Scene.View.WindowMode == WindowMode.Windowed)
            {
                s = Scene.View.Size;
            }
            else
            {
                s = new Size (Scene.View.GraphicsDevice.Settings.Resolution.Width, Scene.View.GraphicsDevice.Settings.Resolution.Height);
            }
            ((Control)Scene.Root).Size = new Vector2(s.Width, s.Height);
            Scene.Camera = new OrthoCamera
            {
                Width = s.Width,
                Height = s.Height,
                Position = new Vector3(s.Width / 2f, s.Height / 2f, 0)
            };
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (InterfaceScene.Focused != null)
                InterfaceScene.Focused.ProcessMessage(MessageType.KeyDown, e);
            if (!e.Handled && !e.SuppressKeyPress)
            {
                KeyCombination k = new KeyCombination { Key = e.KeyCode, Modifier = e.Modifiers };
                Control hot = InterfaceScene.GetByHotkey(k);
                if (hot != null)
                {
                    hot.ProcessMessage(MessageType.MouseDown, null);
                    hot.ProcessMessage(MessageType.Click, null);
                    hot.ProcessMessage(MessageType.MouseUp, null);
                    hot.ProcessMessage(MessageType.MouseLeave, null);
                    e.Handled = true;
                }
            }
        }
        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (InterfaceScene.Focused != null)
                InterfaceScene.Focused.ProcessMessage(MessageType.KeyUp, e);
            //if (!e.Handled && !e.SuppressKeyPress)
            //{
            //    KeyCombination k = new KeyCombination { Key = e.KeyCode, Modifier = e.Modifiers };
            //    Control hot = InterfaceScene.GetByHotkey(k);
            //    if (hot != null)
            //    {
            //        hot.ProcessMessage(MessageType.MouseUp, null);
            //        hot.ProcessMessage(MessageType.MouseLeave, null);
            //    }
            //}
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (InterfaceScene.Focused != null)
                InterfaceScene.Focused.ProcessMessage(MessageType.KeyPress, e);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            InterfaceScene.Focused = (MouseOverEntity as Control);
        }
    }
}
