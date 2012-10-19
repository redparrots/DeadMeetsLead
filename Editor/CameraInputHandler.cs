using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using SlimDX;
using System.Drawing;

namespace Editor
{
    class CameraInputHandler : FilteredInputHandler
    {
        public LookatSphericalCamera Camera { get; set; }
        public WorldView View { get; set; }

        public CameraInputHandler()
        {
            Block(MessageType.KeyDown);
            Block(MessageType.KeyUp);
            Block(MessageType.MouseWheel);
            Block(MessageType.MouseUp);
            Block(MessageType.MouseDown);
            Block(MessageType.MouseMove);
            Block(MessageType.MouseClick);
        }


        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Z)
            {
                cameraRotate = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.X)
            {
                cameraElevate = true;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.C)
            {
                cameraTilt = true;
            }
            else
                InputHandler.ProcessMessage(MessageType.KeyDown, e);
        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Z)
            {
                cameraRotate = false;
                if (cameraPan)
                {
                    panCamera = Camera.Clone();
                    UpdateMouseData();
                }
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.X)
            {
                cameraElevate = false;
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.C)
            {
                cameraTilt = false;
            }
            else
                InputHandler.ProcessMessage(MessageType.KeyUp, e);
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                Camera.Radius /= 1.1f;
            else
                Camera.Radius *= 1.1f;
            Vector3 world;
            Camera.MouseXYPlaneIntersect(new Point(0, 0), View.Viewport, out world);
            float dist = Math.Min((Camera.Position - world).Length(), 500);
            Camera.ZFar = dist;
            View.Renderer.Settings.FogDistance = (int)Camera.ZFar;
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (cameraRotate || cameraElevate) return;

            prevMouse = new Point(-1, -1);
            if (cameraPan)
            {
                cameraPan = false;
            }
            else
                InputHandler.ProcessMessage(MessageType.MouseUp, e);
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (cameraRotate || cameraElevate) return;

            prevMouse = e.Location;
            mousePosition = new Vector2(e.Location.X, e.Location.Y);
            if (Camera.MouseXYPlaneIntersect(e.Location, View.Viewport, out startMouseWorldPos))
                prevMouseWorldPos = startMouseWorldPos;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                cameraPan = true;
                panCamera = Camera.Clone();
                UpdateMouseData();
            }
            else
                InputHandler.ProcessMessage(MessageType.MouseDown, e);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            UpdateMouseData();

            if (cameraRotate)
            {
                Camera.Phi -= mouseDelta.X / 100;
                Camera.Theta += mouseDelta.Y / 100;
            }
            else if (cameraElevate)
            {
                float v = mouseDelta.Y / 100f;
                elevation += v;
                Camera.Lookat += Vector3.UnitZ * v;
            }
            else if (cameraTilt)
            {
                float v = mouseDelta.Y / 100f;
                Camera.Up = Vector3.TransformNormal(Camera.Up, Matrix.RotationAxis(Vector3.Normalize(Camera.Lookat - Camera.Position), v));
            }
            else if (cameraPan)
            {
                Camera.Lookat += mouseWorldDelta;
                Vector3 world;
                if (View.GroundProbe.WorldProbe.Intersect(new Ray(Camera.Lookat + Vector3.UnitZ * 1000, -Vector3.UnitZ), out world))
                    Camera.Lookat = world + Vector3.UnitZ * elevation;
            }
            else
                InputHandler.ProcessMessage(MessageType.MouseMove, e);
        }
        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            if (cameraRotate || cameraElevate) return;
            if (!cameraPan)
                InputHandler.ProcessMessage(MessageType.MouseClick, e);
        }

        void UpdateMouseData()
        {
            mouseDelta = new Vector3(View.LocalMousePosition.X - prevMouse.X, View.LocalMousePosition.Y - prevMouse.Y, 0);
            mousePosition = new Vector2(View.LocalMousePosition.X, View.LocalMousePosition.Y);
            prevMouse = View.LocalMousePosition;
            Camera camera = Camera;
            if (cameraPan) camera = panCamera;
            if (camera.MouseXYPlaneIntersect(View.LocalMousePosition, View.Viewport, out mouseWorldPos))
            {
                mouseWorldDelta = prevMouseWorldPos - mouseWorldPos;
                prevMouseWorldPos = mouseWorldPos;
            }
        }

        public bool CanRotate { get; set; }

        bool cameraPan = false, cameraRotate = false, cameraElevate = false, cameraTilt = false;
        float elevation = 0;
        Camera panCamera;
        Point prevMouse = new Point(-1, -1);
        Vector2 mousePosition;
        Vector3 prevMouseWorldPos, startMouseWorldPos;
        Vector3 mouseWorldDelta;
        Vector3 mouseWorldPos;
        Vector3 mouseDelta;
    }
}
