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
    public class InterfaceScene : Graphics.Scene
    {
        public InterfaceScene()
        {
            Init();
        }

        public InterfaceScene(View view)
        {
            Init();
            View = view;
            OnViewChanged();
        }

        void Init()
        {
            Root = new Control
            {
                Scene = this,
                ZIndexRange = 99,
                LocalZIndex = 99
            };
        }

        protected override void OnViewChanged()
        {
            base.OnViewChanged();
            //((Control)Root).Size = new Vector2(View.Size.Width, View.Size.Height);
            //Camera = new OrthoCamera
            //{
            //    Width = View.Size.Width,
            //    Height = View.Size.Height,
            //    Position = new Vector3(View.Size.Width / 2f, View.Size.Height / 2f, 0)
            //};
            ((Control)Root).Size = new Vector2(View.GraphicsDevice.Settings.Resolution.Width, View.GraphicsDevice.Settings.Resolution.Height);
            Camera = new OrthoCamera
            {
                Width = View.GraphicsDevice.Settings.Resolution.Width,
                Height = View.GraphicsDevice.Settings.Resolution.Height,
                Position = new Vector3(View.GraphicsDevice.Settings.Resolution.Width / 2f, View.GraphicsDevice.Settings.Resolution.Height / 2f, 0),
                ZNear = 0,
                ZFar = 100
            };
            Application.Log("Screen Resolution: " + View.GraphicsDevice.Settings.Resolution);
        }

        public Control Focused
        {
            get { return focused; }
            set
            {
                if (focused != null) focused.OnLostFocus();
                focused = value;
                if (focused != null) focused.OnFocused();
            }
        }

        public void MessageBox(String message)
        {
            Form f = new Form
            {
                Anchor = Orientation.Center,
                Size = new Vector2(200, 150),
                Clickable = true
            };
            Label t = new Label
            {
                Text = message,
                TextAnchor = Orientation.Center,
                Anchor = Orientation.Top,
                Size = new Vector2(200, 100),
                Background = null,
                Position = new Vector2(0, 10),
            };
            f.AddChild(t);
            Button b = new Button
            {
                Text = "Ok",
                Anchor = Orientation.Bottom,
                Position = new Vector2(0, 10),
                Size = new Vector2(100, 20)
            };
            b.MouseClick += new System.Windows.Forms.MouseEventHandler((e, o) => f.Remove());
            f.AddChild(b);
            Root.AddChild(f);
        }

        public void RegisterHotkey(KeyCombination key, Control control)
        {
            List<Control> b = new List<Control>();
            if (!hotkeys.TryGetValue(key, out b))
                hotkeys[key] = b = new List<Control>();
            b.Add(control);
        }
        public void UnregisterHotkey(KeyCombination key, Control control)
        {
            hotkeys[key].Remove(control);
        }
        public Control GetByHotkey(KeyCombination key)
        {
            List<Control> b = new List<Control>();
            if (hotkeys.TryGetValue(key, out b) && b.Count != 0)
            {
                foreach (var v in b)
                    if (v.IsVisible)
                        return v;
            }
            return null;
        }

        Control focused;
        Dictionary<KeyCombination, List<Control>> hotkeys = new Dictionary<KeyCombination, List<Control>>(new KeyCombination.EqualityComparer());

        static Content.Font defaultFont = new Content.Font(
            new System.Drawing.Font("Arial", 12, GraphicsUnit.Pixel),
            Color.White,
            Color.Transparent
        );
        public static Content.Font DefaultFont { get { return (Content.Font)defaultFont.Clone(); } set { defaultFont = value; } }

        static Content.Font defaultBackdropFont = new Content.Font(
            new System.Drawing.Font("Arial", 12, GraphicsUnit.Pixel),
            Color.White,
            Color.FromArgb(200, Color.Black)
        );
        public static Content.Font DefaultBackdropFont { get { return (Content.Font)defaultBackdropFont.Clone(); } set { defaultBackdropFont = value; } }

        static BorderGraphic defaultSlimBorder = new Graphics.Content.BorderGraphic
        {
            Layout = new BorderLayout(new Rectangle(0, 0, 2, 2), new Rectangle(1, 1, 1, 1), new Rectangle(1, 1, 1, 1), new Rectangle(1, 1, 1, 1))
            {
                BackgroundStyle = BorderBackgroundStyle.Inner,
                Border = new Vector2(2, 2)
            },
            Texture = new TextureFromFile("Graphics.Resources.SlimBorder.png"),
            TextureSize = new Vector2(2, 2)
        };
        public static BorderGraphic DefaultSlimBorder { get { return (BorderGraphic)defaultSlimBorder.Clone(); } set { defaultSlimBorder = value; } }

        static BorderGraphic defaultFormBorder = new BorderGraphic
        {
            Layout = new BorderLayout(new Rectangle(0, 0, 16, 16), new Rectangle(15, 15, 1, 1), new Rectangle(15, 15, 1, 1), new Rectangle(15, 15, 1, 1))
            {
                BackgroundStyle = BorderBackgroundStyle.Inner,
                Border = new Vector2(16, 16)
            },
            Texture = new TextureFromFile("Graphics.Resources.FormBorder.png"),
            TextureSize = new Vector2(16, 16)
        };
        public static BorderGraphic DefaultFormBorder { get { return (BorderGraphic)defaultFormBorder.Clone(); } set { defaultFormBorder = value; } }


        static BorderGraphic defaultFormBorderOutlined = new BorderGraphic
        {
            Layout = new BorderLayout(new Rectangle(0, 0, 16, 16), new Rectangle(15, 0, 1, 16), new Rectangle(0, 15, 16, 1), new Rectangle(15, 15, 1, 1))
            {
                BackgroundStyle = BorderBackgroundStyle.Inner,
                Border = new Vector2(16, 16)
            },
            Texture = new TextureFromFile("Graphics.Resources.FormBorderOutlined.png"),
            TextureSize = new Vector2(16, 16)
        };
        public static BorderGraphic DefaultFormBorderOutlined { get { return (BorderGraphic)defaultFormBorderOutlined.Clone(); } set { defaultFormBorderOutlined = value; } }

        static BorderGraphic defaultFormBorderFilled = new BorderGraphic
        {
            Layout = new BorderLayout(new Rectangle(0, 0, 16, 16), new Rectangle(15, 15, 1, 1), new Rectangle(15, 15, 1, 1), new Rectangle(15, 15, 1, 1))
            {
                BackgroundStyle = BorderBackgroundStyle.Inner,
                Border = new Vector2(16, 16)
            },
            Texture = new TextureFromFile("Graphics.Resources.FormBorderFilled.png"),
            TextureSize = new Vector2(16, 16)
        };
        public static BorderGraphic DefaultFormBorderFilled { get { return (BorderGraphic)defaultFormBorderFilled.Clone(); } set { defaultFormBorderFilled = value; } }

    }
}
