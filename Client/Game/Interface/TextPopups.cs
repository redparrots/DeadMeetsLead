using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using SlimDX;
using Graphics.Content;
using Graphics;

namespace Client.Game.Interface
{
    interface ITextPopup
    {
        String Text { get; set; }
        String Title { get; set; }
        void Fadeout();
    }

    class BossIntroText : Label, ITextPopup
    {
        public BossIntroText()
        {
            Background = null;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Center;
            TextAnchor = global::Graphics.Orientation.Center;
            Position = new Vector2(0, 135);
            AutoSize = AutoSizeMode.Full;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.FromArgb(126, 0, 0, 0),
                Color = System.Drawing.Color.FromArgb(255, 249, 246, 154),
                SystemFont = Fonts.BossSystemFont
            };

            Updateable = true;
        }

        public void Fadeout() { Remove(); }
        public String Title { get; set; }


        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
        }
    }

    class BossIntroUnderText : Label, ITextPopup
    {
        public BossIntroUnderText()
        {
            Background = null;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Center;
            TextAnchor = global::Graphics.Orientation.Center;
            Position = new Vector2(0, 220);
            AutoSize = AutoSizeMode.Full;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.FromArgb(126, 0, 0, 0),
                Color = System.Drawing.Color.FromArgb(255, 249, 246, 154),
                SystemFont = Fonts.HugeSystemFont
            };

            Updateable = true;
        }

        public void Fadeout() { Remove(); }
        public String Title { get; set; }

        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
        }
    }

    class WarningPopupText : Label, ITextPopup
    {
        public WarningPopupText()
        {
            Background = null;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Top;
            TextAnchor = global::Graphics.Orientation.Center;
            AutoSize = AutoSizeMode.Full;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Black,
                Color = System.Drawing.Color.Red,
                SystemFont = Fonts.HugeSystemFont
            };
            yInterpolator.Value = 200;
            Common.InterpolatorKey<float> k;
            yInterpolator.AddKey(k = new Common.InterpolatorKey<float>
            {
                Time = 1,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 100
            });
            k.Passing += new EventHandler(k_Passing);
            Position = new Vector2(0, yInterpolator.Value);
            Updateable = true;
        }
        public void Fadeout() { Remove(); }
        public String Title { get; set; }
        void k_Passing(object sender, EventArgs e)
        {
            Remove();
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            var s = yInterpolator.Update(e.Dtime);
            Position = new Vector2(0, s);
        }
        [NonSerialized]
        Common.Interpolator yInterpolator = new Common.Interpolator();
    }


    class TimeLeftPopupText : Label, ITextPopup
    {
        public TimeLeftPopupText()
        {
            Background = null;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Center;
            TextAnchor = global::Graphics.Orientation.Center;
            AutoSize = AutoSizeMode.Full;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Transparent,
                Color = System.Drawing.Color.Yellow,
                SystemFont = Fonts.HugeSystemFont
            };
            DisplayTime = 1;
            Updateable = true;
        }
        public void Fadeout() { Remove(); }
        public String Title { get; set; }
        public float DisplayTime { get; set; }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            alphaInterpolator.Value = 1;
            Common.InterpolatorKey<float> k;
            alphaInterpolator.AddKey(k = new Common.InterpolatorKey<float>
            {
                Time = DisplayTime,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 0
            });
            k.Passing += new EventHandler(k_Passing);
        }
        void k_Passing(object sender, EventArgs e)
        {
            Remove();
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            textGraphic.Alpha = alphaInterpolator.Update(e.Dtime);
        }
        [NonSerialized]
        Common.Interpolator alphaInterpolator = new Common.Interpolator();
    }

    class RageLevelXPopupText : Label, ITextPopup
    {
        public RageLevelXPopupText()
        {
            Background = new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/IngameInterface/RageLevel1.png") { DontScale = true },
                Position = new Vector3(0, -57, 0)
            };
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Bottom;
            Position = new Vector2(0, 10);
            TextAnchor = global::Graphics.Orientation.Center;
            Size = new Vector2(646, 415);
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Transparent,
                Color = System.Drawing.Color.FromArgb(255, 0xff, 0x5d, 0x0e),
                SystemFont = Fonts.HugeSystemFont
            };
            DisplayTime = 3;
            Updateable = true;
        }
        public void Fadeout() { Remove(); }
        public String Title { get; set; }
        public float DisplayTime { get; set; }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            alphaInterpolator.Value = 1;
            Common.InterpolatorKey<float> k;
            alphaInterpolator.AddKey(k = new Common.InterpolatorKey<float>
            {
                Time = DisplayTime,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 0
            });
            k.Passing += new EventHandler(k_Passing);
        }
        void k_Passing(object sender, EventArgs e)
        {
            Remove();
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            float a = alphaInterpolator.Update(e.Dtime);
            foreach(Graphics.Content.Graphic g in AllGraphics)
                g.Alpha = a;
        }
        [NonSerialized]
        Common.Interpolator alphaInterpolator = new Common.Interpolator();
    }

    class FadingText : Label, ITextPopup
    {
        public FadingText()
        {
            FadeTime = 0.2f;
            TextGraphic.Alpha = 0;
            Updateable = true;
        }
        public virtual String Title { get; set; }
        public float FadeTime { get; set; }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            alpha.Value = 0;
            alpha.ClearKeys();
            alpha.AddKey(new Common.InterpolatorKey<float>
            {
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Time = FadeTime,
                Value = 1,
            });
        }
        public void Fadeout()
        {
            var k = new Common.InterpolatorKey<float>
            {
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Time = FadeTime,
                Value = 0
            };
            k.Passing += new EventHandler((o, e) => Remove());
            alpha.AddKey(k);
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (IsRemoved) return;
            alpha.Update(e.Dtime);
            Invalidate();
        }
        protected override void OnConstructed()
        {
            base.OnConstructed();
            foreach (var v in AllGraphics)
                if (v != null)
                    ((global::Graphics.Content.Graphic)v).Alpha = alpha.Value;
        }
        Common.Interpolator alpha = new Common.Interpolator { Value = 0 };
    }

    class SubtitleText : FadingText
    {
        public SubtitleText()
        {
            Background = null;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Bottom;
            TextAnchor = global::Graphics.Orientation.Center;
            AutoSize = AutoSizeMode.Full;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.FromArgb(100, 0, 0, 0),
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.HugeSystemFont
            };
            Position = new Vector2(0, 200);
        }
    }


    class TutorialText : Control, ITextPopup
    {
        public TutorialText()
        {
            Anchor = global::Graphics.Orientation.Bottom;
            Size = new Vector2(800, 300);
            Updateable = true;
            Position = new Vector2(0, 0);
            AddChild(title);
            AddChild(text);
        }

        public string Title { get { return title.Text; } set { title.Text = value; } }
        public string Text { get { return text.Text; } set { text.Text = value; } }

        public void Fadeout()
        {
            title.Fadeout();
            text.Fadeout();
            text.Removed += new EventHandler(text_Removed);
        }

        void text_Removed(object sender, EventArgs e)
        {
            Remove();
        }

        FadingText title = new FadingText
        {
            Background = null,
            Clickable = false,
            Overflow = global::Graphics.TextOverflow.Ignore,
            Anchor = global::Graphics.Orientation.TopLeft,
            TextAnchor = global::Graphics.Orientation.TopLeft,
            AutoSize = AutoSizeMode.Full,
            Font = new global::Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.FromArgb(100, 0, 0, 0),
                Color = System.Drawing.Color.Gold,
                SystemFont = Fonts.LargeSystemFont
            },
            FadeTime = 1
        };
        FadingText text = new FadingText
        {
            Background = null,
            Clickable = false,
            Anchor = global::Graphics.Orientation.TopLeft,
            Position = new Vector2(0, 40),
            Size = new Vector2(800, 300),
            TextAnchor = global::Graphics.Orientation.TopLeft,
            Font = new global::Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.FromArgb(100, 0, 0, 0),
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.MediumSystemFont
            },
            FadeTime = 1
        };
    }


    class CreditsText : Control, ITextPopup
    {
        public CreditsText()
        {
            Background = null;
            Clickable = false;
            Anchor = global::Graphics.Orientation.Center;
            Updateable = true;
            Position = new Vector2(0, 0);
            Size = new Vector2(800, 400);
            Duration = float.MaxValue;
            AddChild(title);
            AddChild(text);
        }
        public void Fadeout() { Remove(); }
        public float Duration { get; set; }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (IsRemoved) return;
            acc += e.Dtime;
            if (acc >= Duration)
                Remove();
        }
        float acc;
        public string Title { get { return title.Text; } set { title.Text = value; } }
        Label title = new Label
        {
            Background = null,
            Clickable = false,
            AutoSize = AutoSizeMode.Full,
            Overflow = global::Graphics.TextOverflow.Ignore,
            Anchor = global::Graphics.Orientation.Top,
            Dock = System.Windows.Forms.DockStyle.Fill,
            TextAnchor = global::Graphics.Orientation.Center,
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Black,
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.LargeSystemFont
            }
        };
        public string Text { get { return text.Text; } set { text.Text = value; } }
        Label text = new Label
        {
            Background = null,
            Clickable = false,
            AutoSize = AutoSizeMode.Full,
            Overflow = global::Graphics.TextOverflow.Ignore,
            Anchor = global::Graphics.Orientation.Top,
            Position = new Vector2(0, 40),
            Dock = System.Windows.Forms.DockStyle.Fill,
            TextAnchor = global::Graphics.Orientation.Center,
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Black,
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.HugeSystemFont
            }
        };
    }

    class ScrollingCreditsText : FlowLayout, ITextPopup
    {
        public ScrollingCreditsText()
        {
            Background = null;
            Clickable = false;
            Anchor = global::Graphics.Orientation.Bottom;
            Updateable = true;
            HorizontalFill = false;
            AutoSize = true;
            Newline = false;
            Position = new Vector2(0, -1000000000);
            //AutoSize = AutoSizeMode.Full;
            //TextAnchor = Orientation.Center;
            //Font = new Font
            //{
            //    SystemFont = Fonts.MediumSystemFont,
            //    Color = System.Drawing.Color.White
            //};
        }
        public void Fadeout() { }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime * 40f;
            Position = new Vector2(0, -Size.Y + acc);
        }
        float acc = 0;
        public string Title { get { return ""; } set { } }

        string text;
        public string Text { get { return text; } set { text = value; } }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            ClearChildren();
            var ss = text.Split('\n');
            foreach (var s in ss)
            {
                var str = s;
                Font font = new Font
                {
                    SystemFont = new System.Drawing.Font(Fonts.DefaultFontFamily, 20, 
                        System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel),
                    Color = System.Drawing.Color.White,
                    Backdrop = System.Drawing.Color.Black
                };
                int marginTop = 0, marginBottom = 0;

                if (str.StartsWith("[i]"))
                {
                    font.SystemFont = new System.Drawing.Font(Fonts.DefaultFontFamily, 30, 
                        System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
                    str = str.Substring(3);
                    marginTop = 60;
                    marginBottom = 10;
                }
                if (str.StartsWith("[e="))
                {
                    int end = str.IndexOf(']');
                    font.Encoding = Encoding.GetEncoding(str.Substring(3, end - 3));
                    str = str.Substring(end + 1);
                }
                if(str.StartsWith("[DML]"))
                {
                    Control c;
                    AddChild(c = new Control
                    {
                        Size = new Vector2(1280, 600)
                    });
                    c.AddChild(new Control
                    {
                        Anchor = Orientation.Center,
                        Background = new ImageGraphic
                        {
                            Texture = new TextureFromFile("Interface/Common/DeadMeetsLead1.png") { DontScale = true },
                        },
                        Size = new Vector2(1100, 502)
                    });
                }
                else if (str.StartsWith("[Keldyn]"))
                {
                    Control c;
                    AddChild(c = new Control
                    {
                        Size = new Vector2(1280, 600)
                    });
                    c.AddChild(new Control
                    {
                        Anchor = Orientation.Center,
                        Background = new ImageGraphic
                        {
                            Texture = new TextureFromFile("Interface/Common/KeldynLogo1.png") { DontScale = true },
                        },
                        Size = new Vector2(300, 433)
                    });
                }
                else
                {
                    AddChild(new Label
                    {
                        Background = null,
                        Clickable = false,
                        Size = new Vector2(1280, 30),
                        Overflow = global::Graphics.TextOverflow.Ignore,
                        TextAnchor = global::Graphics.Orientation.Center,
                        Font = font,
                        Margin = new System.Windows.Forms.Padding(0, marginTop, 0, marginBottom),
                        Text = str
                    });
                }
            }
            Position = new Vector2(0, -Size.Y + acc);
        }
    }

    class WarningFlashText : Label
    {
        public WarningFlashText()
        {
            Background = null;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Bottom;
            TextAnchor = global::Graphics.Orientation.Center;
            AutoSize = AutoSizeMode.Full;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Black,
                Color = System.Drawing.Color.Red,
                SystemFont = Fonts.HugeSystemFont
            };
            aInterpolator.Value = 1;
            float period = 0.6f;
            aInterpolator.AddKey(new Common.InterpolatorKey<float>
            {
                Time = period / 2f,
                Period = period,
                Repeat = true,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 0
            });
            aInterpolator.AddKey(new Common.InterpolatorKey<float>
            {
                Time = period,
                Period = period,
                Repeat = true,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 1
            });
            Position = new Vector2(0, 300);
            Updateable = true;
        }

        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            textGraphic.Alpha = aInterpolator.Update(e.Dtime);
        }
        [NonSerialized]
        Common.Interpolator aInterpolator = new Common.Interpolator();
    }

    class SpeachBubble : FadingText
    {
        public SpeachBubble()
        {
            Background = InterfaceScene.DefaultFormBorder;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Bottom;
            TextAnchor = global::Graphics.Orientation.TopLeft;
            Size = new Vector2(900, 140); //old value y = 115
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.FromArgb(100, 0, 0, 0),
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.LargeSystemFont
            };
            Position = new Vector2(0, 200);
        }
        string fullText;
        public override string Text
        {
            get
            {
                return fullText;
            }
            set
            {
                fullText = value;
            }
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            SetGraphic("SpeachBubble.Portrait", new ImageGraphic
            {
                Texture = new TextureFromFile("Interface/Common/Portrait1.png") { DontScale = true },
                SizeMode = SizeMode.AutoAdjust,
            });
            TextGraphic.Position = new Vector3(120, 20, 0);
            TextGraphic.Size = new Vector2(Size.X - 140, Size.Y - 40);
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            if (acc >= 0.03f && base.Text.Length != fullText.Length)
            {
                base.Text = fullText.Substring(0, t++);
                acc -= 0.03f;
            }
        }
        float acc = 0;
        int t;
    }


    class SilverYieldPopup : Label, ITextPopup
    {
        public SilverYieldPopup()
        {
            Background = null;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Top;
            Position = new Vector2(0, 300);
            TextAnchor = global::Graphics.Orientation.Center;
            AutoSize = AutoSizeMode.Full;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Transparent,
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.HugeSystemFont
            };
            DisplayTime = 1;
            Updateable = true;
        }
        public void Fadeout() { Remove(); }
        public String Title { get; set; }
        public float DisplayTime { get; set; }
        public int SilverSize { get; set; }
        public int SilverBeforeValue { get; set; }
        public int SilverAfterValue { get; set; }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            alphaInterpolator.Value = 1;
            Common.InterpolatorKey<float> k;
            alphaInterpolator.AddKey(k = new Common.InterpolatorKey<float>
            {
                Time = DisplayTime,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 0
            });
            k.Passing += new EventHandler(k_Passing);

            silverValueInterpolator.Value = SilverBeforeValue;
            silverValueInterpolator.AddKey(new Common.InterpolatorKey<float>
            {
                Time = DisplayTime,
                Value = SilverAfterValue
            });
        }

        void k_Passing(object sender, EventArgs e)
        {
            Remove();
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            textGraphic.Alpha = alphaInterpolator.Update(e.Dtime);
            Text = "" + (int)silverValueInterpolator.Update(e.Dtime);
        }
        [NonSerialized]
        Common.Interpolator alphaInterpolator = new Common.Interpolator();
        Common.Interpolator silverValueInterpolator = new Common.Interpolator();
    }

    class StageStartText : Label, ITextPopup
    {
        public StageStartText()
        {
            Background = null;
            Clickable = false;
            Overflow = global::Graphics.TextOverflow.Ignore;
            Anchor = global::Graphics.Orientation.Center;
            TextAnchor = global::Graphics.Orientation.Center;
            AutoSize = AutoSizeMode.Full;
            Font = new Graphics.Content.Font
            {
                Backdrop = System.Drawing.Color.Transparent,
                Color = System.Drawing.Color.White,
                SystemFont = Fonts.HugeSystemFont
            };
            DisplayTime = 1;
            Updateable = true;
        }
        public void Fadeout() { Remove(); }
        public String Title { get; set; }
        public float DisplayTime { get; set; }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            alphaInterpolator.Value = 1;
            Common.InterpolatorKey<float> k;
            alphaInterpolator.AddKey(k = new Common.InterpolatorKey<float>
            {
                Time = DisplayTime,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 0
            });
            k.Passing += new EventHandler(k_Passing);

            Program.Instance.SoundManager.GetSFX(Sound.SFX.Trombone1).Play(new Sound.PlayArgs { Looping = false });
        }
        void k_Passing(object sender, EventArgs e)
        {
            Remove();
        }
        protected override void OnUpdate(Graphics.UpdateEventArgs e)
        {
            base.OnUpdate(e);
            textGraphic.Alpha = alphaInterpolator.Update(e.Dtime);
        }
        [NonSerialized]
        Common.Interpolator alphaInterpolator = new Common.Interpolator();
    }
}
