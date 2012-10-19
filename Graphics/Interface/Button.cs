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
    public class ButtonBase : Control
    {
        public ButtonBase()
        {
            Clickable = true;
            DisplayHotkey = true;
        }

        protected override void OnClickableChanged()
        {
            base.OnClickableChanged();
            Invalidate();
        }

        private bool disabled = false;
        public bool Disabled { get { return disabled; } set { disabled = value; Invalidate(); } }

        protected override void OnConstruct()
        {
            if (AutoSize != AutoSizeMode.None)
                AutoAdjustSize();
            base.OnConstruct();

            if (Caption == null && Text != null)
                Caption = new TextGraphic { Anchor = textAnchor };

            if(Caption != null)
            {
                if (Caption is TextGraphic)
                {
                    if (Clickable && !Disabled)
                        ((TextGraphic)Caption).Font = font;
                    else
                    {
                        if (disabledFont != null)
                            ((TextGraphic)Caption).Font = disabledFont;
                        else
                            ((TextGraphic)Caption).Font = new Graphics.Content.Font(font.SystemFont, System.Drawing.Color.Gray, font.Backdrop);
                    }
                    ((TextGraphic)Caption).Text = text;
                }

                caption.Size = InnerSize;
                caption.Position = Common.Math.ToVector3(InnerOffset);

                SetGraphic("Button.Caption", Caption);

                /*caption.Model.World = Matrix.Translation(-Size.X / 2, -Size.Y / 2, 0);
                caption.Model.World *= Matrix.Scaling(captionScale, captionScale, 0);
                caption.Model.World *= Matrix.Translation(Size.X / 2, Size.Y / 2, 0);*/
            }

            if (HotkeyCaption != null)
            {
                SetGraphic("Button.HotkeyCaption", HotkeyCaption);
                //HotkeyCaption.Model.World = Matrix.Translation(2, 0, -1);
            }
        }

        Content.Font font = InterfaceScene.DefaultFont;
        public Content.Font Font
        {
            get { return font; }
            set { font = value; Invalidate(); }
        }

        Content.Font disabledFont = null;
        public Content.Font DisabledFont
        {
            get { return disabledFont; }
            set { disabledFont = value; Invalidate(); }
        }

        Graphic caption;
        public Graphic Caption
        {
            get { return caption; }
            set { caption = value; Invalidate(); }
        }

        private float captionScale = 1.0f;
        public float CaptionScale
        {
            get { return captionScale; }
            set { if (captionScale == value) return; captionScale = value; Invalidate(); }
        }

        String text;
        public String Text
        {
            get { return text; }
            set { if (text == value) return; text = value; Invalidate(); }
        }

        Orientation textAnchor = Orientation.Center;
        public Orientation TextAnchor
        {
            get { return textAnchor; }
            set { if (textAnchor == value) return; textAnchor = value; Invalidate(); }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Invalidate();   
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            Invalidate();
            base.OnMouseLeave(e);
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            Invalidate();
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (Hotkey != null)
                InterfaceManager.RegisterHotkey(Hotkey, this);
        }
        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            if (Hotkey != null)
                InterfaceManager.UnregisterHotkey(Hotkey, this);
        }

        KeyCombination hotkey;
        public KeyCombination Hotkey
        {
            get { return hotkey; }
            set
            {
                if (hotkey != null && !IsRemoved)
                    InterfaceManager.UnregisterHotkey(Hotkey, this);
                hotkey = value;
                if (!IsRemoved)
                {
                    InterfaceManager.RegisterHotkey(Hotkey, this);
                    DisplayHotkey = DisplayHotkey;
                }
            }
        }
        bool displayHotkey;
        public bool DisplayHotkey
        {
            get { return displayHotkey; }
            set {
                if (displayHotkey == value) return;
                displayHotkey = value;
                if (displayHotkey && Hotkey != null)
                {
                    HotkeyCaption = new TextGraphic
                    {
                        Text = Hotkey.ToString(),
                        Font = InterfaceScene.DefaultBackdropFont,
                        Size = Size
                    };
                }
            }
        }

        Graphic hotkeyCaption;
        Graphic HotkeyCaption
        {
            get { return hotkeyCaption; }
            set { hotkeyCaption = value; Invalidate(); }
        }


        #region AutoSize
        AutoSizeMode autoSize = AutoSizeMode.None;
        public AutoSizeMode AutoSize
        {
            get { return autoSize; }
            set
            {
                if (autoSize == value) return;
                autoSize = value;
                Invalidate();
            }
        }

        void AutoAdjustSize()
        {
            if (IsRemoved) return;
            if(Caption is TextGraphic)
                Size = ((TextGraphic)Caption).AutoSize(autoSize, Scene.View.Content, base.Size, maxSize, Padding);
        }

        public override Vector2 Size
        {
            get
            {
                if (AutoSize != AutoSizeMode.None) AutoAdjustSize();
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        Vector2 maxSize = new Vector2(float.MaxValue, float.MaxValue);
        public Vector2 MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; if (AutoSize != AutoSizeMode.None) AutoAdjustSize(); }
        }
        #endregion
    }

    public class Button : ButtonBase
    {
        public Button()
        {
            NormalTexture = new TextureFromFile("Graphics.Resources.ButtonBorder.png");
            HoverTexture = new TextureFromFile("Graphics.Resources.ButtonHoverBorder.png");
            ClickTexture = new TextureFromFile("Graphics.Resources.ButtonClickBorder.png");
            Background = new BorderGraphic
            {
                Layout = new Graphics.Content.BorderLayout(new Rectangle(0, 0, 4, 4), new Rectangle(3, 0, 1, 4), new Rectangle(0, 3, 4, 1), new Rectangle(3, 3, 1, 1))
                {
                    BackgroundStyle = BorderBackgroundStyle.Inner,
                    Border = new Vector2(4, 4)
                },
                Texture = new TextureFromFile("Graphics.Resources.ButtonBorder.png"),
                TextureSize = new Vector2(4, 4)
            };
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();

            if (HoverTexture != null && NormalTexture != null && ClickTexture != null)
            {
                if (Background is BorderGraphic)
                {
                    switch (MouseState)
                    {
                        case MouseState.Over:
                            if (!Disabled)
                                ((BorderGraphic)Background).Texture = HoverTexture;
                            break;
                        case MouseState.Out:
                            ((BorderGraphic)Background).Texture = NormalTexture;
                            break;
                        case MouseState.Down:
                            if (!Disabled)
                                ((BorderGraphic)Background).Texture = ClickTexture;
                            break;
                        case MouseState.Up:
                            if (!Disabled)
                                ((BorderGraphic)Background).Texture = HoverTexture;
                            break;
                    }
                }
                else if (Background is ImageGraphic)
                {
                    switch (MouseState)
                    {
                        case MouseState.Over:
                            if (!Disabled)
                                ((ImageGraphic)Background).Texture = HoverTexture;
                            break;
                        case MouseState.Out:
                            ((ImageGraphic)Background).Texture = NormalTexture;
                            break;
                        case MouseState.Down:
                            if (!Disabled)
                                ((ImageGraphic)Background).Texture = ClickTexture;
                            break;
                        case MouseState.Up:
                            if (!Disabled)
                                ((ImageGraphic)Background).Texture = HoverTexture;
                            break;
                    }
                }
                else if (Background is StretchingImageGraphic)
                {
                    switch (MouseState)
                    {
                        case MouseState.Over:
                            if (!Disabled)
                                ((StretchingImageGraphic)Background).Texture = HoverTexture;
                            break;
                        case MouseState.Out:
                            ((StretchingImageGraphic)Background).Texture = NormalTexture;
                            break;
                        case MouseState.Down:
                            if (!Disabled)
                                ((StretchingImageGraphic)Background).Texture = ClickTexture;
                            break;
                        case MouseState.Up:
                            if (!Disabled)
                                ((StretchingImageGraphic)Background).Texture = HoverTexture;
                            break;
                    }
                }
            }
        }

        public MetaResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D> NormalTexture, HoverTexture, ClickTexture;

    }
}
