using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace Graphics.Editors
{
    public partial class SceneEditor
    {
        class IState : InputHandler
        {
            public IState(SceneEditor editor) { this.editor = editor; }
            protected SceneEditor editor;
            public virtual void OnEnter() { }
            public virtual void OnExit() { }
            public virtual void Render(Renderer9 renderer) { }
        }

        class Default : IState
        {
            public Default(SceneEditor editor) : base(editor) { }
            protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
            {
                if (editor.selected.Contains(editor.CurrentMouseOverEntity))
                {
                    editor.ChangeState(new MoveAlongGround(editor));
                    return;
                }
                else
                {
                    editor.ChangeState(new Selecting(editor));
                    return;
                }
            }
            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (e.KeyCode == System.Windows.Forms.Keys.Space && editor.selected.Count > 0)
                {
                    editor.ChangeState(new MoveAlongGround(editor));
                    return;
                }
            }
        }

        class MoveAlongGround : IState
        {
            public MoveAlongGround(SceneEditor editor) : base(editor) { }
            public override void OnEnter()
            {
                Vector3 world;
                //if (editor.IntersectGround(out world))
                if (editor.GroundProbe.Intersect(editor.selected.First(), out world))
                {
                    foreach (Entity e in editor.selected)
                    {
                        diffs[e] = editor.GetTranslationCallback(e) - world;
                    }
                }
                else
                {
                    Vector3 center = Vector3.Zero;
                    foreach (Entity e in editor.selected)
                        center += editor.GetTranslationCallback(e);
                    center /= (float)editor.selected.Count;
                    foreach (Entity e in editor.selected)
                        diffs[e] = editor.GetTranslationCallback(e) - center;
                }

            }
            protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
            {
                editor.ChangeState(new Default(editor));
            }
            protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
            {
                editor.GroundProbe.Intersect(editor.selected.First(), out world);
                UpdateTranslations();
            }
            protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
            {
                base.OnKeyPress(e);
                if (e.KeyChar == 'r' || e.KeyChar == 'R')
                    height += 0.04f;
                else if (e.KeyChar == 'f' || e.KeyChar == 'F')
                    height -= 0.04f;
                UpdateTranslations();
            }
            protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
            {
                base.OnKeyUp(e);
                if (e.KeyCode == System.Windows.Forms.Keys.Space)
                {
                    editor.ChangeState(new Default(editor));
                    return;
                }
            }
            void UpdateTranslations()
            {
                foreach (Entity et in editor.selected)
                {
                    editor.SetTranslationCallback(et, diffs[et] + world + new Vector3(0, 0, height));
                    var ray = new Ray(editor.GetTranslationCallback(et) + Vector3.UnitZ*100, -Vector3.UnitZ);
                    float d;
                    if(editor.GroundProbe.Intersect(ray, et, out d))
                        editor.SetTranslationCallback(et, ray.Position + ray.Direction * d);
                        
                    et.Invalidate();
                }
            }
            Dictionary<Entity, Vector3> diffs = new Dictionary<Entity, Vector3>();
            float height = 0;
            Vector3 world;
        }

        class Selecting : IState
        {
            public Selecting(SceneEditor editor) : base(editor) { }
            public override void OnEnter()
            {
                startMousePosition = editor.Scene.View.LocalMousePosition;
                startTime = DateTime.Now;
                editor.selected.Clear();
            }
            public override void OnExit()
            {
                base.OnExit();
            }
            protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
            {
                var view = editor.Scene.View;
                if (view.LocalMousePosition == startMousePosition)
                {
                    if ((DateTime.Now - startTime).TotalSeconds < 1)
                        editor.InternalSelect(editor.CurrentMouseOverEntity);
                    editor.DoSelectionChanged();
                    editor.ChangeState(new Default(editor));
                    return;
                }
                else
                {
                    Point topLeft = startMousePosition;
                    Point bottomRight = view.LocalMousePosition;
                    if (topLeft.X > bottomRight.X)
                    {
                        topLeft.X = bottomRight.X;
                        bottomRight.X = startMousePosition.X;
                    }
                    if (topLeft.Y > bottomRight.Y)
                    {
                        topLeft.Y = bottomRight.Y;
                        bottomRight.Y = startMousePosition.Y;
                    }

                    if (topLeft.X == bottomRight.X || topLeft.Y == bottomRight.Y)
                    {
                        editor.ChangeState(new Default(editor));
                        return;
                    }

                    var bounding = editor.Scene.Camera.FrustumFromRectangle(view.Viewport, topLeft, bottomRight);
                    foreach (Entity et in editor.Clickables.Cull(bounding))
                        if(et.AllGraphics.Count() > 0)
                            editor.InternalSelect(et);
                    editor.DoSelectionChanged();
                }
                editor.ChangeState(new Default(editor));
            }
            public override void Render(Renderer9 renderer)
            {
                var l = editor.Scene.View.LocalMousePosition;
                editor.Scene.View.Draw2DLines(new Vector2[] { 
                    new Vector2(startMousePosition.X, startMousePosition.Y), 
                    new Vector2(startMousePosition.X, l.Y),
                    new Vector2(l.X, l.Y),
                    new Vector2(l.X, startMousePosition.Y), 
                    new Vector2(startMousePosition.X, startMousePosition.Y)
                }, Color.FromArgb(0, 255, 0));
            }
            Point startMousePosition;
            DateTime startTime;
        }
    }
}
