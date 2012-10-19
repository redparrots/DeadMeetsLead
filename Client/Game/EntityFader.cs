using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;

namespace Client.Game
{
    public class EntityFader
    {
        public EntityFader(Entity entity)
        {
            this.entity = entity;
            entity.AddedToScene += new EventHandler(entity_AddedToScene);
            entity.RemovedFromScene += new EventHandler(entity_RemovedFromScene);
        }

        void entity_RemovedFromScene(object sender, EventArgs e)
        {
            entity.Scene.View.Frame -= new View.FrameEventHandler(View_Frame);
        }

        void entity_AddedToScene(object sender, EventArgs e)
        {
            entity.Scene.View.Frame += new View.FrameEventHandler(View_Frame);
            if (entity.MainGraphic == null) return;

            hasAlphaDefault = ((MetaModel)entity.MainGraphic).HasAlpha;
            if (FadeinTime > 0)
            {
                fadeingIn = true;
                alpha.Value = 0;
                Common.InterpolatorKey<float> k;
                alpha.AddKey(k = new Common.InterpolatorKey<float>
                {
                    Time = FadeinTime,
                    TimeType = Common.InterpolatorKeyTimeType.Relative,
                    Value = 1
                });
                k.Passing += new EventHandler((o, e2) =>
                {
                    ((MetaModel)entity.MainGraphic).HasAlpha = hasAlphaDefault;
                    fadeingIn = false;
                    if (entity.MainGraphic != null)
                        ((MetaModel)entity.MainGraphic).Opacity = 1;
                });
                ((MetaModel)entity.MainGraphic).HasAlpha = true;
            }
            else
            {
                alpha.Value = 1;
                if (entity.MainGraphic != null)
                    ((MetaModel)entity.MainGraphic).Opacity = 1;
            }
        }

        void View_Frame(float dtime)
        {
            if (fadeingIn || fadeingOut)
            {
                if (entity.MainGraphic != null)
                    ((MetaModel)entity.MainGraphic).Opacity = alpha.Update(dtime);
            }
            acc += dtime;
            if (AutoFadeoutTime > 0 && acc > AutoFadeoutTime)
                Fadeout();
        }

        public void Fadeout()
        {
            if (fadeingOut || entity.MainGraphic == null) return;
            fadeingOut = true;
            ((MetaModel)entity.MainGraphic).HasAlpha = true;
            ((MetaModel)entity.MainGraphic).AlphaRef = 0;
            ((MetaModel)entity.MainGraphic).CastShadows = Priority.Never;
            var key = new Common.InterpolatorKey<float>
            {
                Time = FadeoutTime,
                TimeType = Common.InterpolatorKeyTimeType.Relative,
                Value = 0
            };
            key.Passing += new EventHandler(key_Passing);
            alpha.AddKey(key);
        }

        void key_Passing(object sender, EventArgs e)
        {
            if(!entity.IsRemoved)
                entity.Remove();
        }

        public float FadeoutTime = 1, FadeinTime = 0;
        
        float autoFadeoutTime = -1;
        public float AutoFadeoutTime { get { return autoFadeoutTime; } set { autoFadeoutTime = value; acc = 0; } }

        Common.Interpolator alpha = new Common.Interpolator { Value = 1 };

        Entity entity;

        bool hasAlphaDefault;
        float acc = 0;
        bool fadeingOut = false, fadeingIn = false;
    }
}
