using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Content;

namespace Graphics.Interface
{
    public enum FadeState 
    { 
        /// <summary>
        /// The screen is fading out to black
        /// </summary>
        FadeingOut, 
        /// <summary>
        /// The screen is black
        /// </summary>
        FadedOut, 
        /// <summary>
        /// The screen is fading in from black
        /// </summary>
        FadeingIn, 
        /// <summary>
        /// The screen is visible
        /// </summary>
        FadedIn 
    }

    public class Fader : Control
    {
        public Fader()
        {
            Background = new StretchingImageGraphic
            {
                Texture = new TextureConcretizer { TextureDescription = 
                    new Graphics.Software.Textures.SingleColorTexture(System.Drawing.Color.Black) }
            };
            FadeLength = 1 / 2f;
            AlphaMin = 0;
            AlphaMax = 1;
            Updateable = true;
            Dock = System.Windows.Forms.DockStyle.Fill;
        }

        protected virtual void OnStateChanged()
        {
            if (removeAfterStateChange)
            {
                Remove();
                removeAfterStateChange = false;
            }
            Invalidate();
        }

        FadeState state = FadeState.FadedIn;
        public FadeState State
        {
            get { return state; }
            set
            {
                if (state == value) return;
                state = value;
                Common.InterpolatorKey<float> key;
                switch (state)
                {
                    case FadeState.FadedIn:
                        alpha.ClearKeys();
                        alpha.Value = AlphaMin;
                        break;
                    case FadeState.FadeingOut:
                        alpha.Value = AlphaMin;
                        alpha.AddKey(key = new Common.InterpolatorKey<float>
                        {
                            Time = FadeLength,
                            TimeType = Common.InterpolatorKeyTimeType.Relative,
                            Value = AlphaMax
                        });
                        key.Passing += new EventHandler((o, e) => { state = FadeState.FadedOut; OnStateChanged(); });
                        break;
                    case FadeState.FadedOut:
                        alpha.ClearKeys();
                        alpha.Value = AlphaMax;
                        break;
                    case FadeState.FadeingIn:
                        alpha.Value = AlphaMax;
                        alpha.AddKey(key = new Common.InterpolatorKey<float>
                        {
                            Time = FadeLength,
                            TimeType = Common.InterpolatorKeyTimeType.Relative,
                            Value = AlphaMin
                        });
                        key.Passing += new EventHandler((o, e) => { state = FadeState.FadedIn; OnStateChanged(); });
                        break;
                }
                switch (state)
                {
                    case FadeState.FadedOut:
                    case FadeState.FadeingOut:
                        Clickable = true;
                        break;
                    case FadeState.FadedIn:
                    case FadeState.FadeingIn:
                        Clickable = false;
                        break;
                }
                OnStateChanged();
            }
        }

        public void FadeInAndRemove()
        {
            State = FadeState.FadeingIn;
            removeAfterStateChange = true;
        }
        bool removeAfterStateChange = false;

        /// <summary>
        /// The time in seconds the fade takes
        /// </summary>
        public float FadeLength { get; set; }

        public float AlphaMax { get; set; }
        public float AlphaMin { get; set; }

        protected override void OnConstruct()
        {
            base.OnConstruct();
            Background.Alpha = alpha.Value;
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            float oldVal = alpha.Value;
            alpha.Update(e.Dtime);
            if (oldVal != alpha.Value)
            {
                Visible = alpha.Value > AlphaMin;
                Invalidate();
            }
        }

        Common.Interpolator alpha = new Common.Interpolator();
    }

    //public class FaderOld : Control
    //{
    //    public FaderOld()
    //    {
    //        Background = new StretchingImageGraphic
    //        {
    //        };
    //        FadeLength = 1 / 2f;
    //        AlphaMin = 0;
    //        AlphaMax = 1;
    //    }

    //    FadeState state = FadeState.FadedIn;
    //    public FadeState State { get { return state; }
    //        set
    //        {
    //            if (state == value) return;
    //            state = value;
    //            switch (state)
    //            {
    //                case FadeState.FadedIn:
    //                case FadeState.FadeingOut:
    //                    alpha = AlphaMin;
    //                    break;
    //                case FadeState.FadedOut:
    //                case FadeState.FadeingIn:
    //                    alpha = AlphaMax;
    //                    break;
    //            }
    //            Invalidate();
    //        }
    //    }

    //    /// <summary>
    //    /// The time in seconds the fade takes
    //    /// </summary>
    //    public float FadeLength { get; set; }

    //    public float AlphaMax { get; set; }
    //    public float AlphaMin { get; set; }

    //    protected override void Construct()
    //    {
    //        Size = new SlimDX.Vector2(Scene.View.Width, Scene.View.Height);
    //        ((StretchingImageGraphic)Background).Texture =
    //            new TextureConcretizer { Texture = Software.ITexture.SingleColorTexture(System.Drawing.Color.FromArgb((int)(alpha * 255), 0, 0, 0)) };
    //        base.Construct();
    //    }

    //    protected override void OnUpdate(UpdateEventArgs e)
    //    {
    //        base.OnUpdate(e);
    //        if (State == FadeState.FadeingOut)
    //        {
    //            alpha += (AlphaMax - AlphaMin) * e.Dtime / FadeLength;
    //            Invalidate();

    //            if (alpha >= AlphaMax)
    //            {
    //                alpha = AlphaMax;
    //                State = FadeState.FadedOut;
    //            }
    //        }
    //        else if (State == FadeState.FadeingIn)
    //        {
    //            alpha -= (AlphaMax - AlphaMin) * e.Dtime / FadeLength;
    //            Invalidate();

    //            if (alpha <= AlphaMin)
    //            {
    //                alpha = AlphaMin;
    //                State = FadeState.FadedIn;
    //            }
    //        }
    //    }
    //    public override bool Visible
    //    {
    //        get
    //        {
    //            return base.Visible && alpha > AlphaMin;
    //        }
    //        set
    //        {
    //            base.Visible = value;
    //        }
    //    }
    //    float alpha = 1;
    //}
}
