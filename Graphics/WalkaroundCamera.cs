using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Graphics
{
    public class WalkaroundCameraInputHandler : FilteredInputHandler
    {
        LookatCartesianCamera camera;
        public LookatCartesianCamera Camera
        {
            get { return camera; }
            set
            {
                camera = value;
                UpdateRot();
            }
        }

        public WalkaroundCameraInputHandler()
        {
            Block(MessageType.KeyDown);
            Block(MessageType.KeyUp);
            Block(MessageType.MouseWheel);
            Block(MessageType.MouseUp);
            Block(MessageType.MouseDown);
            Block(MessageType.MouseMove);
            Block(MessageType.MouseClick);
            zfunc = (x, y) => 0;
        }


        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case System.Windows.Forms.Keys.W:
                    Drun.Y = -1;
                    break;
                case System.Windows.Forms.Keys.S:
                    Drun.Y = 1;
                    break;
                case System.Windows.Forms.Keys.A:
                    Drun.X = -1;
                    break;
                case System.Windows.Forms.Keys.D:
                    Drun.X = 1;
                    break;
                case System.Windows.Forms.Keys.OemBackslash:
                case System.Windows.Forms.Keys.Q:
                    Drun.Z = -1;
                    break;
                case System.Windows.Forms.Keys.Space:
                case System.Windows.Forms.Keys.E:
                    Drun.Z = 1;
                    break;
                default:
                    if(InputHandler != null)
                        InputHandler.ProcessMessage(MessageType.KeyDown, e);
                    break;
            }
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.W:
                    Drun.Y = 0;
                    break;
                case System.Windows.Forms.Keys.S:
                    Drun.Y = 0;
                    break;
                case System.Windows.Forms.Keys.A:
                    Drun.X = 0;
                    break;
                case System.Windows.Forms.Keys.D:
                    Drun.X = 0;
                    break;
                case System.Windows.Forms.Keys.OemBackslash:
                case System.Windows.Forms.Keys.Q:
                    Drun.Z = 0;
                    break;
                case System.Windows.Forms.Keys.Space:
                case System.Windows.Forms.Keys.E:
                    Drun.Z = 0;
                    break;
                default:
                    if (InputHandler != null)
                        InputHandler.ProcessMessage(MessageType.KeyUp, e); 
                    break;
            }
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                Zoom /= 1.1f;
            else
                Zoom *= 1.1f;
            UpdateRot();
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Right)
                rotating = false;
            else if (InputHandler != null)
                InputHandler.ProcessMessage(MessageType.MouseUp, e);
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                rotating = true;
                prevMouse = new Vector2(e.X, e.Y);
            }
            else if (InputHandler != null)
                InputHandler.ProcessMessage(MessageType.MouseDown, e);
        }
        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            if (!rotating && InputHandler != null)
                InputHandler.ProcessMessage(MessageType.MouseClick, e);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (rotating)
            {
                Vector2 mouse = new Vector2(e.X, e.Y);
                Vector2 dmouse = prevMouse - mouse;
                Zrot -= dmouse.X / 100;
                Xrot += dmouse.Y / 100;
                prevMouse = mouse;
                UpdateRot();
            }
            else if(InputHandler != null)
                InputHandler.ProcessMessage(MessageType.MouseMove, e);
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            Vector4 d = Vector2.Transform(Common.Math.ToVector2(Drun), Matrix.RotationZ(Zrot)) * e.Dtime * Speed;
            float pz = zfunc(Camera.Lookat.X, Camera.Lookat.Y);
            Camera.Lookat += new Vector3(d.X, d.Y, Drun.Z * e.Dtime * Speed);
            UpdateRot();
        }

        public void UpdateRot()
        {
            Vector4 p = Vector3.Transform(new Vector3(0, Zoom, 0), Matrix.RotationX(Xrot) * Matrix.RotationZ(Zrot));
            Camera.Position = Camera.Lookat + new Vector3(p.X, p.Y, p.Z);
        }

        /*public void Update(float dtime)
        {
            Vector4 d = Vector2.Transform(Common.Math.ToVector2(Drun), Matrix.RotationZ(Zrot)) * dtime * Speed;
            Camera.Lookat.X += d.X;
            Camera.Lookat.Y += d.Y;
            Camera.Lookat.Z = zfunc(Camera.Lookat.X, Camera.Lookat.Y) + 0;
            UpdateRot();
        }*/

        public delegate float ZFunc(float x, float y);
        public ZFunc zfunc;

        public float Speed = 7;
        public float Zrot = 0, Xrot = 1;
        public Vector3 Drun;
        public float Zoom = 30;
        Vector2 prevMouse;
        bool rotating = false;
    }
}
