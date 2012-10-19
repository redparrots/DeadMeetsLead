using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;

namespace Client.Game.Map.Effects
{

    public class CritEffect1 : GameEffect
    {
        public CritEffect1()
        {
            Duration = 0.5f;
            VisibilityLocalBounding = Vector3.Zero;
            MainGraphic = metaModel = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/CriticalStrike1.x"),
                Texture = new TextureFromFile("Models/Effects/CriticalStrike1.png"),
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                World = Matrix.Scaling(0.25f, 0.25f, 0.25f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(1.2f, 0f, 1f),
            };
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            float p = (acc - fadeDur) / Duration;
            ((MetaModel)MainGraphic).Opacity = 1 - Math.Max(0, p);
            if (acc > Duration)
                Remove();
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("CriticalStrike1", false));
        }
        float acc = 0;
        public float Duration { get; set; }
        float fadeDur = 0.2f;

        private MetaModel metaModel;
    }
    public class CritEffect2 : GameEffect
    {
        public CritEffect2()
        {
            Duration = 0.5f;
            VisibilityLocalBounding = Vector3.Zero;
            MainGraphic = metaModel = new MetaModel
            {
                SkinnedMesh = new SkinnedMeshFromFile("Models/Effects/CriticalStrikeWave1.x"),
                Texture = new TextureFromFile("Models/Effects/CriticalStrikeWave1.png"),
                HasAlpha = true,
                ReceivesAmbientLight = Priority.Never,
                ReceivesDiffuseLight = Priority.Never,
                World = Matrix.Scaling(0.2f, 0.2f, 0.2f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(0.8f, 0f, 1f),
            };
        }
        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            acc += e.Dtime;
            float p = (acc - fadeDur) / Duration;
            ((MetaModel)MainGraphic).Opacity = 1 - Math.Max(0, p);
            if (acc > Duration)
                Remove();
        }
        protected override void OnConstruct()
        {
            base.OnConstruct();
            var v = Scene.View.Content.Peek<global::Graphics.Renderer.Renderer.EntityAnimation>(MetaEntityAnimation);
            v.PlayAnimation(new AnimationPlayParameters("CriticalStrike1", false));
        }
        float acc = 0;
        public float Duration { get; set; }
        float fadeDur = 0.2f;

        private MetaModel metaModel;
    }

    public class CritEffect : EffectCollection
    {
        public CritEffect()
        {
            AddChild(new CritEffect1());
            AddChild(new CritEffect2());
        }
    }
}
