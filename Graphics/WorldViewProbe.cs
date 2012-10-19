using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using Graphics.GraphicsDevice;

namespace Graphics
{
    public class WorldViewProbe
    {
        public WorldViewProbe()
        {
            WorldProbe = new Common.EmptyWorldProbe();
        }
        public WorldViewProbe(View view, Camera camera)
        {
            this.View = view;
            this.Camera = camera;
            WorldProbe = new Common.EmptyWorldProbe();
        }
        public View View { get; set; }
        public Camera Camera { get; set; }
        public Common.IWorldProbe WorldProbe { get; set; }

        public Ray ScreenRay() { return ScreenRay(View.LocalMousePosition); }
        public Ray ScreenRay(Point screenPosition) { return ScreenRay(Common.Math.ToVector2(screenPosition)); }
        public Ray ScreenRay(Vector2 screenPosition)
        {
            return ScreenRay(screenPosition, Camera.View * Camera.Projection, View.Viewport);
        }
        public Ray ScreenRay(Vector2 screenPosition, Matrix viewProjection, Viewport viewport)
        {
            Vector3 near3d = Vector3.Unproject(new Vector3(screenPosition, 0), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            Vector3 far3d = Vector3.Unproject(new Vector3(screenPosition, 1), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            Vector3 dir = far3d - near3d;
            dir.Normalize();
            return new Ray(near3d, dir);
        }

        public bool Intersect(out float distance) 
        {
            return Intersect(ScreenRay(), out distance); 
        }
        public bool Intersect(Point screenPosition, out float distance)
        {
            return Intersect(ScreenRay(screenPosition), out distance);
        }
        public bool Intersect(out Vector3 worldPosition)
        {
            return Intersect(ScreenRay(), out worldPosition);
        }
        public bool Intersect(object userdata, out Vector3 worldPosition)
        {
            return Intersect(ScreenRay(), userdata, out worldPosition);
        }
        public bool Intersect(Point screenPosition, out Vector3 worldPosition)
        {
            return Intersect(ScreenRay(screenPosition), out worldPosition);
        }
        public bool Intersect(Ray ray, out Vector3 worldPosition)
        {
            return WorldProbe.Intersect(ray, out worldPosition);
        }
        public bool Intersect(Ray ray, object userdata, out Vector3 worldPosition)
        {
            return WorldProbe.Intersect(ray, userdata, out worldPosition);
        }
        public bool Intersect(Ray ray, out float distance)
        {
            return WorldProbe.Intersect(ray, out distance);
        }
        public bool Intersect(Ray ray, object userdata, out float distance)
        {
            object ent;
            return WorldProbe.Intersect(ray, userdata, out distance, out ent);
        }
        public bool Intersect(Ray ray, out float distance, out object entity)
        {
            return WorldProbe.Intersect(ray, null, out distance, out entity);
        }
        public bool Intersect(Ray ray, object userdata, out float distance, out object entity)
        {
            return WorldProbe.Intersect(ray, null, out distance, out entity);
        }

        public object Pick()
        {
            object e;
            float d;
            Intersect(ScreenRay(), out d, out e);
            return e;
        }

        public Vector3 ToScreenSpace(Vector3 worldPosition)
        {
            return ToScreenSpace(worldPosition, Matrix.Identity);
        }
        public Vector3 ToScreenSpace(Vector3 worldPosition, Matrix world)
        {
            return ToScreenSpace(worldPosition, View.Viewport, world * Camera.View * Camera.Projection);
        }
        public Vector3 ToScreenSpace(Vector3 worldPosition, Viewport viewport, Matrix worldViewProjection)
        {
            return Vector3.Project(worldPosition, viewport.X, viewport.Y, viewport.Width, viewport.Height,
                viewport.MinZ, viewport.MaxZ, worldViewProjection);
        }
    }
}
