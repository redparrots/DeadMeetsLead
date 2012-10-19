using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using SlimDX;
using Graphics.Content;
using Graphics;

namespace Client
{
    static class Fonts
    {
        public static string DefaultFontFamily = "Georgia";
        public static float DefaultFontSize = 12;

        public static System.Drawing.Font SmallSystemFont =
            new System.Drawing.Font(DefaultFontFamily, 8, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel);

        public static System.Drawing.Font DefaultSystemFont = 
            new System.Drawing.Font(DefaultFontFamily, DefaultFontSize, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel);

        public static System.Drawing.Font DefaultUnderlinedSystemFont =
            new System.Drawing.Font(DefaultFontFamily, DefaultFontSize, System.Drawing.FontStyle.Underline,
                System.Drawing.GraphicsUnit.Pixel);

        public static System.Drawing.Font MediumSystemFont =
            new System.Drawing.Font(DefaultFontFamily, 15, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel);

        public static System.Drawing.Font LargeSystemFont =
            new System.Drawing.Font(DefaultFontFamily, 20, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel);


        public static System.Drawing.Font HugeSystemFont =
            new System.Drawing.Font(DefaultFontFamily, 35, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel);

        public static System.Drawing.Font BossSystemFont =
            new System.Drawing.Font(DefaultFontFamily, 80, System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Pixel);

        public static Font Default = new Font
        {
            Backdrop = System.Drawing.Color.Transparent,
            Color = System.Drawing.Color.White,
            SystemFont = DefaultSystemFont
        };
        public static Font DefaultUnderlined = new Font
        {
            Backdrop = System.Drawing.Color.Transparent,
            Color = System.Drawing.Color.White,
            SystemFont = DefaultUnderlinedSystemFont
        };
    }


    class ProgramInterface : Control
    {
        public ProgramInterface()
        {
            Dock = System.Windows.Forms.DockStyle.Fill;
            AddChild(Layer1);
            AddChild(Layer2);
            AddChild(Layer3);
        }
        public void ClearInterface()
        {
            ClearChildren();
            Layer1.ClearChildren();
            Layer2.ClearChildren();
            Layer3.ClearChildren();
            AddChild(Layer1);
            AddChild(Layer2);
            AddChild(Layer3);
        }
        public void AddFader()
        {
            var p = new Fader
            {
                AlphaMax = 0.6f,
                FadeLength = 0.5f,
                State = global::Graphics.Interface.FadeState.FadeingOut,
                Clickable = true,
                Size = new Vector2(Program.Settings.GraphicsDeviceSettings.Resolution.Width, Program.Settings.GraphicsDeviceSettings.Resolution.Height)
            };

            pauseFaders.Push(p);
            Program.Instance.Interface.AddChild(p);
        }

        public void RemoveFader()
        {
            var p = pauseFaders.Pop();
            p.FadeInAndRemove();
        }

        public Control Layer1 = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
        };
        public Control Layer2 = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
        };
        public Control Layer3 = new Control
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
        };

        protected Stack<Fader> pauseFaders = new Stack<Fader>();
    }

    public class Window : Form
    {
        public Window()
        {
            Size = new Vector2(0, 200);
            Background = new ImageGraphic
            {
                Texture = new TextureConcretizer { TextureDescription = 
                new global::Graphics.Software.Textures.SingleColorTexture(
                    System.Drawing.Color.FromArgb(178, 0, 0, 0))
                }
            };
            Padding = new System.Windows.Forms.Padding(10, 20, 10, 20);
        }
        protected override void OnConstruct()
        {
            if (!LargeWindow)
                Size = new Vector2(466, Size.Y);
            else
                Size = new Vector2(640, Size.Y);

            base.OnConstruct();
            if (!LargeWindow)
            {
                SetGraphic("TopFrame", new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/MenuTop1.png"),
                    Size = new Vector2(1024, 128),
                    Position = new Vector3(-62, -60, 0)
                });
                SetGraphic("ttBottomFrame", new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/MenuBottom1.png"),
                    Size = new Vector2(1024, 128),
                    Position = new Vector3(-62, Size.Y - 60, 0)
                });
            }
            else
            {
                SetGraphic("TopFrame", new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/MenuTop2.png"),
                    Size = new Vector2(1024, 128),
                    Position = new Vector3(-44, -27, 0)
                });
                SetGraphic("ttBottomFrame", new ImageGraphic
                {
                    Texture = new TextureFromFile("Interface/Menu/MenuBottom2.png"),
                    Size = new Vector2(1024, 64),
                    Position = new Vector3(-44, Size.Y - 32, 0)
                });
            }
        }
        bool largeWindow = false;
        public bool LargeWindow
        {
            get { return largeWindow; }
            set
            {
                largeWindow = value;
                if (!LargeWindow)
                    Size = new Vector2(466, Size.Y);
                else
                    Size = new Vector2(640, Size.Y);
                Invalidate();
            }
        }
    }

    public class SidewiseStretchingButton : ButtonBase
    {
        protected float backgroundLeftSize = 55, backgroundRightSize = 91, 
            backgroundHeight = 82, backgroundRightPadding = 10;
        protected Vector3 backgroundOffset = new Vector3(-10, -10, 0);
        protected String backgroundLeft = "Interface/Menu/ButtonLeft1.png";
        protected String backgroundMiddle = "Interface/Menu/ButtonMiddle1.png";
        protected String backgroundRight = "Interface/Menu/ButtonRight1.png";
        protected override void OnConstruct()
        {
            ImageGraphic left, middle, right;
            SetGraphic("LeftBackground", left = new ImageGraphic
            {
                Texture = new TextureFromFile(backgroundLeft) { DontScale = true },
                SizeMode = SizeMode.AutoAdjust,
                Position = backgroundOffset,
            });
            SetGraphic("MiddleBackground", middle = new ImageGraphic
            {
                Texture = new TextureFromFile(backgroundMiddle) { DontScale = true },
                Position = new Vector3(backgroundLeftSize, 0, 0) + backgroundOffset,
                TextureAdressMode = SlimDX.Direct3D9.TextureAddress.Wrap,
            });
            SetGraphic("RightBackground", right = new ImageGraphic
            {
                Texture = new TextureFromFile(backgroundRight) { DontScale = true },
                SizeMode = SizeMode.AutoAdjust
            });

            base.OnConstruct();
            if (MouseState == global::Graphics.MouseState.Down && !Disabled)
                Caption.Position = new Vector3(1, 1, 0) + Common.Math.ToVector3(InnerOffset);
            else
                Caption.Position = Common.Math.ToVector3(InnerOffset);
            middle.Size = new Vector2(Size.X - backgroundLeftSize - backgroundRightPadding - 2 * backgroundOffset.X, backgroundHeight);
            right.Position = new Vector3(Size.X - backgroundRightSize - 2 * backgroundOffset.X, 0, 0) + backgroundOffset;
        }
    }

    class LargeStoneButton : SidewiseStretchingButton
    {
        public LargeStoneButton()
        {
            Size = new Vector2(190, 62);
            Background = null;
            Font = new Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.White,
                Backdrop = System.Drawing.Color.Black
            };
            DisabledFont = new Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.Gray,
                Backdrop = System.Drawing.Color.Black
            };
        }

        protected override void OnConstruct()
        {
            backgroundLeft = "Interface/Menu/ButtonLeft1.png";
            backgroundMiddle = "Interface/Menu/ButtonMiddle1.png";
            backgroundRight = "Interface/Menu/ButtonRight1.png";
            if (!Disabled)
            {
                if (MouseState == MouseState.Down)
                {
                    backgroundLeft = "Interface/Menu/ButtonLeftDown1.png";
                    backgroundMiddle = "Interface/Menu/ButtonMiddleDown1.png";
                    backgroundRight = "Interface/Menu/ButtonRightDown1.png";
                }
                else if (MouseState == MouseState.Over)
                {
                    backgroundLeft = "Interface/Menu/ButtonLeftMouseOver1.png";
                    backgroundMiddle = "Interface/Menu/ButtonMiddleMouseOver1.png";
                    backgroundRight = "Interface/Menu/ButtonRightMouseOver1.png";
                }
            }
            base.OnConstruct();
        }

        protected override void OnClick(EventArgs e)
        {
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.ButtonClickLarge1).Play(new Sound.PlayArgs());
            base.OnClick(e);
        }
    }

    public class StoneButton : SidewiseStretchingButton
    {
        public StoneButton()
        {
            Size = new Vector2(117, 38);
            Background = null;
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White,
                Backdrop = System.Drawing.Color.Black
            };
            DisabledFont = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.Gray,
                Backdrop = System.Drawing.Color.Black
            };
            Margin = new System.Windows.Forms.Padding(4);
            backgroundLeftSize = 52;
            backgroundRightSize = 30;
            backgroundHeight = 49;
            backgroundRightPadding = 5;
            backgroundOffset = new Vector3(-7, -7, 0);
        }

        protected override void OnConstruct()
        {
            backgroundLeft = "Interface/Menu/ButtonLeft2.png";
            backgroundMiddle = "Interface/Menu/ButtonMiddle2.png";
            backgroundRight = "Interface/Menu/ButtonRight2.png";
            if (!Disabled)
            {
                if (MouseState == MouseState.Down)
                {
                    backgroundLeft = "Interface/Menu/ButtonLeftDown2.png";
                    backgroundMiddle = "Interface/Menu/ButtonMiddleDown2.png";
                    backgroundRight = "Interface/Menu/ButtonRightDown2.png";
                }
                else if (MouseState == MouseState.Over)
                {
                    backgroundLeft = "Interface/Menu/ButtonLeftMouseOver2.png";
                    backgroundMiddle = "Interface/Menu/ButtonMiddleMouseOver2.png";
                    backgroundRight = "Interface/Menu/ButtonRightMouseOver2.png";
                }
            }
            base.OnConstruct();
        }

        protected override void OnClick(EventArgs e)
        {
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.ButtonClickLarge1).Play(new Sound.PlayArgs());
            base.OnClick(e);
        }
    }

    public class StoneCheckbox : CheckboxBase
    {
        public StoneCheckbox()
        {
            Size = new Vector2(117, 38);
            Background = null;
            CheckedGraphic = null;
            Margin = new System.Windows.Forms.Padding(4);
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White,
                Backdrop = System.Drawing.Color.Black
            };
        }


        protected float backgroundLeftSize = 52, backgroundRightSize = 30, backgroundHeight = 49;
        protected Vector3 backgroundOffset = new Vector3(-7, -7, 0);
        protected String backgroundLeft = "Interface/Menu/ButtonLeft2.png";
        protected String backgroundMiddle = "Interface/Menu/ButtonMiddle2.png";
        protected String backgroundRight = "Interface/Menu/ButtonRight2.png";
        protected override void OnConstruct()
        {
            backgroundLeft = "Interface/Menu/ButtonLeft2.png";
            backgroundMiddle = "Interface/Menu/ButtonMiddle2.png";
            backgroundRight = "Interface/Menu/ButtonRight2.png";
            if (!Disabled)
            {
                if (MouseState == MouseState.Over || Checked)
                {
                    backgroundLeft = "Interface/Menu/ButtonLeftMouseOver2.png";
                    backgroundMiddle = "Interface/Menu/ButtonMiddleMouseOver2.png";
                    backgroundRight = "Interface/Menu/ButtonRightMouseOver2.png";
                }
                else if (MouseState == MouseState.Down)
                {
                    backgroundLeft = "Interface/Menu/ButtonLeftDown2.png";
                    backgroundMiddle = "Interface/Menu/ButtonMiddleDown2.png";
                    backgroundRight = "Interface/Menu/ButtonRightDown2.png";
                }
            }

            ImageGraphic left, middle, right;
            SetGraphic("LeftBackground", left = new ImageGraphic
            {
                Texture = new TextureFromFile(backgroundLeft) { DontScale = true },
                SizeMode = SizeMode.AutoAdjust,
                Position = backgroundOffset,
            });
            SetGraphic("MiddleBackground", middle = new ImageGraphic
            {
                Texture = new TextureFromFile(backgroundMiddle) { DontScale = true },
                Position = new Vector3(backgroundLeftSize, 0, 0) + backgroundOffset,
                TextureAdressMode = SlimDX.Direct3D9.TextureAddress.Wrap,
            });
            SetGraphic("RightBackground", right = new ImageGraphic
            {
                Texture = new TextureFromFile(backgroundRight) { DontScale = true },
                SizeMode = SizeMode.AutoAdjust
            });

            base.OnConstruct();
            if (MouseState == global::Graphics.MouseState.Down && !Disabled)
                Caption.Position = new Vector3(1, 1, 0) + Common.Math.ToVector3(InnerOffset);
            else
                Caption.Position = Common.Math.ToVector3(InnerOffset);
            middle.Size = new Vector2(Size.X - backgroundLeftSize - 10 - 2 * backgroundOffset.X, backgroundHeight);
            right.Position = new Vector3(Size.X - backgroundRightSize - 2 * backgroundOffset.X, 0, 0) + backgroundOffset;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.ButtonClickSmall1).Play(new Sound.PlayArgs());
        }
    }

    class Dialog : Window
    {
        public Dialog()
        {
            Size = new Vector2(600, 200);
            Anchor = global::Graphics.Orientation.Center;
            AddChild(title);
            AddChild(text);
            AddChild(buttonsFlow);
            okButton.Click += new EventHandler(okButton_Click);
            yesButton.Click += new EventHandler(yesButton_Click);
            noButton.Click += new EventHandler(noButton_Click);
            cancelButton.Click += new EventHandler(cancelButton_Click);
            ControlBox = false;
            LargeWindow = true;
            DialogResult = System.Windows.Forms.DialogResult.None;
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        void noButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.No;
            Close();
        }

        void yesButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Yes;
            Close();
        }

        void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            buttonsFlow.ClearChildren();

            if (MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.OKCancel ||
                MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.YesNoCancel ||
                MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.RetryCancel)
            buttonsFlow.AddChild(cancelButton);

            if (MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.YesNo ||
                MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.YesNoCancel)
                buttonsFlow.AddChild(noButton);

            if (MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.YesNo ||
                MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.YesNoCancel)
                buttonsFlow.AddChild(yesButton);

            if (MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.OK
                || MessageBoxButtons == System.Windows.Forms.MessageBoxButtons.OKCancel)
                buttonsFlow.AddChild(okButton);
        }


        public string Text { get { return text.Text; } set { text.Text = value; } }
        public string Title { get { return title.Text; } set { title.Text = value; } }
        Label title = new Label
        {
            Dock = System.Windows.Forms.DockStyle.Top,
            AutoSize = AutoSizeMode.Vertical,
            Font = new Font
            {
                SystemFont = Fonts.LargeSystemFont,
                Color = System.Drawing.Color.Gold
            },
            Background = null,
            Clickable = false,
        };
        Label text = new Label
        {
            Dock = System.Windows.Forms.DockStyle.Top,
            AutoSize = AutoSizeMode.Vertical,
            Background = null,
            Clickable = false,
            Anchor = global::Graphics.Orientation.TopRight
        };
        protected FlowLayout buttonsFlow = new FlowLayout
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            Origin = FlowOrigin.BottomRight
        };
        protected ButtonBase okButton = new StoneButton
        {
            Text = Locale.Resource.GenOk,
            Margin = new System.Windows.Forms.Padding(10, 10, 10, 10),
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White
            },
            Hotkey = new KeyCombination { Key = System.Windows.Forms.Keys.Enter },
        };
        ButtonBase yesButton = new StoneButton
        {
            Text = Locale.Resource.GenYes,
            Margin = new System.Windows.Forms.Padding(10, 10, 10, 10),
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White
            },
            Hotkey = new KeyCombination { Key = System.Windows.Forms.Keys.Enter },
        };
        ButtonBase noButton = new StoneButton
        {
            Text = Locale.Resource.GenNo,
            Margin = new System.Windows.Forms.Padding(10, 10, 10, 10),
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White
            },
            Hotkey = new KeyCombination { Key = System.Windows.Forms.Keys.Escape },
        };
        protected ButtonBase cancelButton = new StoneButton
        {
            Text = Locale.Resource.GenCancel,
            Margin = new System.Windows.Forms.Padding(10, 10, 10, 10),
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White
            },
            Hotkey = new KeyCombination { Key = System.Windows.Forms.Keys.Escape },
        };
        Control image;
        public Control Image
        {
            get { return image; }
            set
            {
                image = value; if (image != null)
                {
                    text.Size = new Vector2(500, 250);
                    AddChild(image);
                }
            }
        }
        public System.Windows.Forms.DialogResult DialogResult { get; private set; }
        System.Windows.Forms.MessageBoxButtons messageBoxButtons = System.Windows.Forms.MessageBoxButtons.OK;
        public System.Windows.Forms.MessageBoxButtons MessageBoxButtons
        {
            get { return messageBoxButtons; }
            set { messageBoxButtons = value; Invalidate(); }
        }

        public static void Show(Form d)
        {
            Show(d, () => { });
        }
        public static void Show(Form d, Action result)
        {
            d.Closed += new EventHandler((o2, e2) =>
            {
                Program.Instance.Interface.RemoveFader();
                result();
            });
            Program.Instance.Interface.AddFader();
            Program.Instance.Interface.AddChild(d);
        }
        public static void Show(String title, String text)
        {
            Show(title, text, System.Windows.Forms.MessageBoxButtons.OK, (r) => { });
        }
        public static void Show(String title, String text, System.Windows.Forms.MessageBoxButtons buttons, Action<System.Windows.Forms.DialogResult> result)
        {
            var d = new Dialog
            {
                LargeWindow = false,
            };

            d.Title = title;
            d.Text = text;
            d.MessageBoxButtons = buttons;
            Show(d, () => result(d.DialogResult));
        }
    }

    class StoneDropDownBar : DropDownBar
    {
        public StoneDropDownBar()
        {
            Size = new Vector2(243, 28);
            Padding = new System.Windows.Forms.Padding(1, 1, 25, 1);
            Background = new ImageGraphic
            {
                SizeMode = SizeMode.AutoAdjust
            };

            if (Disabled)
                NormalTexture = new TextureFromFile("Interface/Common/DropdownInactive1.png") { DontScale = true };
            else
                NormalTexture = new TextureFromFile("Interface/Common/Dropdown1.png") { DontScale = true };

            HoverTexture = new TextureFromFile("Interface/Common/DropdownMouseOver1.png") { DontScale = true };
            ClickTexture = new TextureFromFile("Interface/Common/DropdownMouseOver1.png") { DontScale = true };
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White
            };
            DropDownBackground = //InterfaceScene.DefaultSlimBorder;
                new StretchingImageGraphic
                {
                    Texture = new TextureConcretizer
                    {
                        TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                            System.Drawing.Color.FromArgb(220, 0, 0, 0))
                    }
                };
        }
        protected override Control CreateDropDownItem(object item)
        {
            return new Button
            {
                Text = item.ToString(),
                Background = new StretchingImageGraphic(),
                NormalTexture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.Transparent)
                },
                HoverTexture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(
                        System.Drawing.Color.FromArgb(100, 255, 255, 255))
                },
                ClickTexture = new TextureConcretizer
                {
                    TextureDescription = new global::Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Gold)
                },
                Font = Font
            };
        }
    }

    enum TextBoxValidInput
    {
        All,
        ProfileName,
        Email
    }
    class StoneTextBox : TextBox
    {
        public StoneTextBox()
        {
            Background = null;
            Size = new Vector2(350, 28);
            Padding = new System.Windows.Forms.Padding(3);
            Font = new Font
            {
                SystemFont = Fonts.MediumSystemFont,
                Color = System.Drawing.Color.White,
            };
            ValidInput = TextBoxValidInput.All;
        }
        protected override void OnConstruct()
        {
            SetGraphic("LeftBackground", new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Common/TextBoxLeft1.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust
            });
            SetGraphic("MiddleBackground", new StretchingImageGraphic
            {
                Texture = new TextureFromFile("Interface/Common/TextBoxMiddle1.png") { DontScale = true },
                Position = new Vector3(3, 0, 0),
                Size = new Vector2(Size.X - 6, 32),
            });
            SetGraphic("RightBackground", new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Common/TextBoxRight1.png") { DontScale = true },
                Position = new Vector3(Size.X - 3, 0, 0),
                SizeMode = SizeMode.AutoAdjust
            });
            base.OnConstruct();
        }
        public TextBoxValidInput ValidInput { get; set; }

        protected override bool IsValidKeyInput(char key)
        {
            if (ValidInput == TextBoxValidInput.All) return true;
            else if (ValidInput == TextBoxValidInput.ProfileName)
            {
                foreach (char c in invalidPathChars) if (c == key) return false;
                return char.IsLetterOrDigit(key) || key == ' ';
            }
            else if (ValidInput == TextBoxValidInput.Email)
            {
                return char.IsLetterOrDigit(key) ||
                    key == '@' || key == '_' || char.IsPunctuation(key);
            }
            else
                throw new ArgumentException();
        }
        static char[] invalidPathChars = System.IO.Path.GetInvalidFileNameChars();
    }
    static class Extensions
    {
        public static bool IsHex(this char c)
        {
            return (c >= '0' && c <= '9') ||
                 (c >= 'a' && c <= 'f') ||
                 (c >= 'A' && c <= 'F');
        }
    }

    class ClickableTextButton : Label
    {
        public ClickableTextButton()
        {
            Font = Fonts.Default;
            Clickable = true;
            Background = InterfaceScene.DefaultSlimBorder;
            Padding = new System.Windows.Forms.Padding(2);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            if (MouseState == global::Graphics.MouseState.Over || MouseState == global::Graphics.MouseState.Down)
                Font = new Font
                {
                    SystemFont = Fonts.DefaultSystemFont,
                    Color = System.Drawing.Color.White,
                };
            else
                Font = new Font
                {
                    SystemFont = Fonts.DefaultSystemFont,
                    Color = System.Drawing.Color.FromArgb(255, 200, 200, 200),
                };
        }

        protected override void OnClick(EventArgs e)
        {
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.ButtonClickSmall1).Play(new Sound.PlayArgs());
            base.OnClick(e);
        }
    }

    class PagedFlowLayout : Control
    {
        public PagedFlowLayout()
        {
}
        public void AddControl(Control c)
        {
            controls.Add(c);
            Invalidate();
        }
        public void RemoveControl(Control c)
        {
            controls.Remove(c);
            Invalidate();
        }
        public void ClearControls()
        {
            controls.Clear();
            Invalidate();
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            ClearChildren();
            flow.Size = new Vector2(Size.X, Size.Y - 40);
            AddChild(flow);
            int nPerPage;
            if (controls.Count == 0)
                nPerPage = 1000;
            else
                nPerPage = ((int)(flow.Size.X / (controls[0].Size.X + controls[0].Margin.Horizontal))) *
                    ((int)(flow.Size.Y / (controls[0].Size.Y + controls[0].Margin.Vertical)));
            if (nPerPage == 0) nPerPage = 1;
            flow.ClearChildren();
            int i = 0;
            foreach(var v in controls)
            {
                if (i / nPerPage == page)
                {
                    flow.AddChild(v);
                }
                i++;
            }

            if ((page + 1) * nPerPage < i)
            {
                var nextPage = new StoneButton
                {
                    Text = Locale.Resource.GenNext,
                    Anchor = global::Graphics.Orientation.BottomRight
                };
                nextPage.Click += new EventHandler((o, e) => Page++);
                AddChild(nextPage);
            }

            if (page > 0)
            {
                var prevPage = new StoneButton
                {
                    Text = Locale.Resource.GenPrev,
                    Anchor = global::Graphics.Orientation.BottomLeft
                };
                prevPage.Click += new EventHandler((o, e) => Page--);
                AddChild(prevPage);
            }
            int nPages = (int)Math.Ceiling(controls.Count / (float)nPerPage);
            if (nPages > 1)
            {
                AddChild(new Label
                {
                    Anchor = global::Graphics.Orientation.Bottom,
                    AutoSize = AutoSizeMode.Full,
                    Position = new Vector2(0, 10),
                    Text = Locale.Resource.GenPage + " " + (page + 1) + "/" + nPages,
                    Background = null,
                });
            }
        }
        int page = 0;
        public int Page { get { return page; } set { page = value; Invalidate(); } }
        List<Control> controls = new List<Control>();
        FlowLayout flow = new FlowLayout { HorizontalFill = false };
    }

    class MultiColorTextBox : FlowLayout
    {
        public MultiColorTextBox()
        {
            Background = InterfaceScene.DefaultSlimBorder;
            HorizontalFill = false;
            Newline = false;
        }

        string text = "";
        public string Text { get { return text; } set { text = value; Invalidate(); } }

        Font font = InterfaceScene.DefaultFont;
        public Font Font { get { return font; } set { font = value; Invalidate(); } }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            ClearChildren();
            var parts = text.Split(new string[] { "#" }, StringSplitOptions.None);
            Control row = new FlowLayout
            {
                Newline = false,
                AutoSize = true,
            };
            foreach (var s in parts)
            {
                string str = s;
                System.Drawing.Color c = font.Color;
                string sc;
                if (s.Length >= 6)
                {
                    sc = s.Substring(0, 6);
                    int h;
                    if (int.TryParse(sc, System.Globalization.NumberStyles.HexNumber, null, out h))
                    {
                        c = System.Drawing.Color.FromArgb(h);
                        c = System.Drawing.Color.FromArgb(255, c);
                        str = s.Substring(6);
                    }
                }

                var lines = str.Split('\n');
                for(int i=0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    row.AddChild(new Label
                    {
                        Text = line,
                        Font = new Font
                        {
                            SystemFont = font.SystemFont,
                            Color = c,
                            Backdrop = font.Backdrop
                        },
                        AutoSize = AutoSizeMode.Full,
                        Background = null,
                        Clickable = false
                    });
                    if (lines.Length > 1 && i < lines.Length - 1)
                    {
                        AddChild(row);
                        row = new FlowLayout
                        {
                            Newline = false,
                            AutoSize = true
                        };
                    }
                }
            }
            AddChild(row);
        }
    }
}
