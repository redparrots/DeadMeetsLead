using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Graphics.Editors
{

    public partial class SceneEditor : InteractiveSceneManager
    {
        public SceneEditor()
        {
            GroundProbe = new WorldViewProbe();
        }

        public WorldViewProbe GroundProbe { get; set; }

        bool IntersectGround(out Vector3 world)
        {
            return GroundProbe.Intersect(out world);
        }

        protected override void OnAfterSceneChanged()
        {
            base.OnAfterSceneChanged(); 
            selected.Clear(); 
            ChangeState(new Default(this));
            GroundProbe.View = Scene.View;
            GroundProbe.Camera = Scene.Camera;
        }
        protected override bool IsClickable(Graphics.Entity e)
        {
            if (e.PickingLocalBounding == null || !e.IsVisible) return false;
            if (IsClickableCallback != null) return IsClickableCallback(e);
            else return true;
        }
        public Func<Entity, bool> IsClickableCallback;
        public Action<Entity, Vector3> SetTranslationCallback = (e, v) => e.Translation = v;
        public Func<Entity, Vector3> GetTranslationCallback = (e) => e.Translation;

        public event EventHandler SelectionChanged;

        public void MoveToCursor(params Entity[] entities)
        {
            Vector3 world;
            if (IntersectGround(out world))
            {
                Vector3 center = GetTranslationCallback(entities[0]);
                for (int i = 1; i < entities.Length; i++)
                    center += GetTranslationCallback(entities[i]);
                center /= entities.Length;

                for (int i = 0; i < entities.Length; i++)
                {
                    SetTranslationCallback(entities[i], GetTranslationCallback(entities[i]) + world - center);
                }
            }
        }
        public void StartMove(params Entity[] entities)
        {
            selected.Clear();
            selected.AddRange(entities);
            DoSelectionChanged();
            ChangeState(new MoveAlongGround(this));
        }

        public override void ProcessMessage(int m, EventArgs args)
        {
            base.ProcessMessage(m, args);
            state.ProcessMessage(m, args);
        }

        void ChangeState(IState newState)
        {
            if (state != null) state.OnExit();
            state = newState;
            if (state != null) state.OnEnter();
        }

        public void Select(params Entity[] entities)
        {
            InternalSelect(entities);
            DoSelectionChanged();
        }

        protected override void OnEntityRemoved(Entity e)
        {
            base.OnEntityRemoved(e);
            if (selected.Contains(e)) selected.Remove(e);
        }

        void InternalSelect(params Entity[] entities)
        {
            foreach (var e in entities)
            {
                if (e == null || !IsClickable(e)) return;
                if (!selected.Contains(e))
                    selected.Add(e);
            }
        }

        void DoSelectionChanged()
        {
            if (SelectionChanged != null) SelectionChanged(this, null);
        }

        Entity CurrentMouseOverEntity
        {
            get
            {
                return MouseOverEntity = (Entity)ClickablesProbe.Pick();
            }
        }
        
        public IEnumerable<Entity> Selected { get { return selected; } }
        List<Entity> selected = new List<Entity>();
        IState state;
    }
}
