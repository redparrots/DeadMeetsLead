using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics.GraphicsDevice;

namespace Graphics
{
    public abstract class Camera
    {
        public Camera() { }
        public Camera(Camera cpy)
        {
            this.ZFar = cpy.ZFar;
            this.ZNear = cpy.ZNear;
        }
        public abstract Camera Clone();
        public abstract Matrix View { get; }
        public abstract Matrix Projection { get; }
        public virtual Matrix ViewProjection { get { return View * Projection; } }
        public float ZNear = 1f;
        public float ZFar = 30f;
        public Vector3 Project(Vector3 worldPosition, Viewport viewport)
        {
            return Vector3.Project(worldPosition, viewport.X, viewport.Y, viewport.Width, viewport.Height,
                viewport.MinZ, viewport.MaxZ, ViewProjection);
        }
        public Ray Unproject(Vector2 screenPosition, Viewport viewport)
        {
            return Unproject(screenPosition, viewport, Matrix.Identity);
        }
        public Ray Unproject(Vector2 screenPosition, Viewport viewport, Matrix world)
        {
            return UnprojectWVP(screenPosition, viewport, world * View * Projection);
        }
        public static Ray UnprojectWVP(Vector2 screenPosition, Viewport viewport, Matrix worldViewProjection)
        {
            Vector3 near = new Vector3(screenPosition.X, screenPosition.Y, 0);
            Vector3 far = new Vector3(near.X, near.Y, 1);

            Vector3 near3d = Vector3.Unproject(near, viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, worldViewProjection);
            Vector3 far3d = Vector3.Unproject(far, viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, worldViewProjection);
            Vector3 dir = far3d - near3d;
            dir.Normalize();
            return new Ray(near3d, dir);
        }
        public Vector3[] GetCornersFromRectangle(Viewport viewport, System.Drawing.Point topLeft, System.Drawing.Point bottomRight)
        {
            return GetCornersFromRectangle(viewport, topLeft, bottomRight, ViewProjection);
        }
        public static Vector3[] GetCornersFromRectangle(Viewport viewport, System.Drawing.Point topLeft, 
            System.Drawing.Point bottomRight, Matrix viewProjection)
        {
            Vector3 ntl = Vector3.Unproject(new Vector3(topLeft.X, topLeft.Y, 0), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            Vector3 ntr = Vector3.Unproject(new Vector3(bottomRight.X, topLeft.Y, 0), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            Vector3 nbl = Vector3.Unproject(new Vector3(topLeft.X, bottomRight.Y, 0), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            Vector3 nbr = Vector3.Unproject(new Vector3(bottomRight.X, bottomRight.Y, 0), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);

            Vector3 ftl = Vector3.Unproject(new Vector3(topLeft.X, topLeft.Y, 1), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            Vector3 ftr = Vector3.Unproject(new Vector3(bottomRight.X, topLeft.Y, 1), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            Vector3 fbl = Vector3.Unproject(new Vector3(topLeft.X, bottomRight.Y, 1), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            Vector3 fbr = Vector3.Unproject(new Vector3(bottomRight.X, bottomRight.Y, 1), viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, viewProjection);
            return new Vector3[] { ntl, ntr, nbl, nbr, ftl, ftr, fbl, fbr };
        }
        public Vector3[] GetCornersFromRectangle()
        {
            return GetCornersFromRectangle(ViewProjection);
        }
        public static Vector3[] GetCornersFromRectangle(Matrix viewProjection)
        {
            return GetCornersFromRectangle(new Vector2(-1, 1), new Vector2(1, -1), viewProjection);
        }
        Vector3[] GetCornersFromRectangle(Vector2 topLeft, Vector2 bottomRight)
        {
            return GetCornersFromRectangle(topLeft, bottomRight, ViewProjection);
        }
        static Vector3[] GetCornersFromRectangle(Vector2 topLeft, Vector2 bottomRight, Matrix viewProjection)
        {
            Matrix invViewProjection = Matrix.Invert(viewProjection);
            Vector3 ntl = Vector3.TransformCoordinate(new Vector3(topLeft.X, topLeft.Y, 0), invViewProjection);
            Vector3 ntr = Vector3.TransformCoordinate(new Vector3(bottomRight.X, topLeft.Y, 0), invViewProjection);
            Vector3 nbl = Vector3.TransformCoordinate(new Vector3(topLeft.X, bottomRight.Y, 0), invViewProjection);
            Vector3 nbr = Vector3.TransformCoordinate(new Vector3(bottomRight.X, bottomRight.Y, 0), invViewProjection);

            Vector3 ftl = Vector3.TransformCoordinate(new Vector3(topLeft.X, topLeft.Y, 1), invViewProjection);
            Vector3 ftr = Vector3.TransformCoordinate(new Vector3(bottomRight.X, topLeft.Y, 1), invViewProjection);
            Vector3 fbl = Vector3.TransformCoordinate(new Vector3(topLeft.X, bottomRight.Y, 1), invViewProjection);
            Vector3 fbr = Vector3.TransformCoordinate(new Vector3(bottomRight.X, bottomRight.Y, 1), invViewProjection);
            return new Vector3[] { ntl, ntr, nbl, nbr, ftl, ftr, fbl, fbr };
        }
        public Common.Bounding.Frustum FrustumFromRectangle(Viewport viewport, System.Drawing.Point topLeft, System.Drawing.Point bottomRight)
        {
            Common.Bounding.Frustum f = new Common.Bounding.Frustum();
            f.planes = new Plane[6];
            Vector3[] corners = GetCornersFromRectangle(viewport, topLeft, bottomRight);
            Vector3 ntl = corners[0];
            Vector3 ntr = corners[1];
            Vector3 nbl = corners[2];
            Vector3 nbr = corners[3];

            Vector3 ftl = corners[4];
            Vector3 ftr = corners[5];
            Vector3 fbl = corners[6];
            Vector3 fbr = corners[7];

            f.planes[(int)Common.Bounding.FrustumPlanes.Left] = new Plane(ntl, nbl, ftl);
            f.planes[(int)Common.Bounding.FrustumPlanes.Top] = new Plane(ntl, ftl, ntr);
            f.planes[(int)Common.Bounding.FrustumPlanes.Right] = new Plane(ntr, ftr, nbr);
            f.planes[(int)Common.Bounding.FrustumPlanes.Bottom] = new Plane(nbl, nbr, fbr);
            f.planes[(int)Common.Bounding.FrustumPlanes.Near] = new Plane(ntl, ntr, nbl);
            f.planes[(int)Common.Bounding.FrustumPlanes.Far] = new Plane(ftr, ftl, fbl);
            return f;
        }
        public Common.Bounding.Frustum Frustum()
        {
            return Frustum(ViewProjection);
        }
        public static Common.Bounding.Frustum Frustum(Matrix viewProjection)
        {
            Common.Bounding.Frustum f = new Common.Bounding.Frustum();
            f.planes = new Plane[6];
            Vector3[] corners = GetCornersFromRectangle(viewProjection);
            Vector3 ntl = corners[0];
            Vector3 ntr = corners[1];
            Vector3 nbl = corners[2];
            Vector3 nbr = corners[3];

            Vector3 ftl = corners[4];
            Vector3 ftr = corners[5];
            Vector3 fbl = corners[6];
            Vector3 fbr = corners[7];

            f.planes[(int)Common.Bounding.FrustumPlanes.Left] = new Plane(ntl, nbl, ftl);
            f.planes[(int)Common.Bounding.FrustumPlanes.Top] = new Plane(ntl, ftl, ntr);
            f.planes[(int)Common.Bounding.FrustumPlanes.Right] = new Plane(ntr, ftr, nbr);
            f.planes[(int)Common.Bounding.FrustumPlanes.Bottom] = new Plane(nbl, nbr, fbr);
            f.planes[(int)Common.Bounding.FrustumPlanes.Near] = new Plane(ntl, ntr, nbl);
            f.planes[(int)Common.Bounding.FrustumPlanes.Far] = new Plane(ftr, ftl, fbl);
            return f;
        }
        public Vector3[] Frustum2DFromRectangle(Viewport viewport, System.Drawing.Point topLeft, System.Drawing.Point bottomRight)
        {
            Vector3[] corners = GetCornersFromRectangle(viewport, topLeft, bottomRight);
            return TriangleFromQuad(corners[2], corners[6], corners[3], corners[7]);
        }
        private Vector3[] TriangleFromQuad(Vector3 nl, Vector3 fl, Vector3 nr, Vector3 fr)
        {
            nl.Z = nr.Z = fl.Z = fr.Z = 0;
            Vector3 lDir = fl - nl;
            Vector3 rDir = fr - nr;
            Vector3 a;
            if (!Common.Math.LineLineIntersection(nl, -lDir, nr, -rDir, out a))
                System.Diagnostics.Debugger.Break();
            return new Vector3[]
            {
                a,
                a + (fl - nl),
                a + (fr - nr)
            };
        }
        public bool MouseXYPlaneIntersect(System.Drawing.Point mouse, Viewport viewport, out Vector3 position)
        {
            return MouseXYPlaneIntersect(mouse, viewport, 0, out position);
        }
        public bool MouseXYPlaneIntersect(System.Drawing.Point mouse, Viewport viewport, float planeZ, out Vector3 position)
        {
            Ray ray = Unproject(new Vector2(mouse.X, mouse.Y), viewport);
            float d;
            if (Ray.Intersects(ray, new Plane(new Vector4(0, 0, 1, planeZ)), out d))
            {
                position = ray.Position + ray.Direction * d;
                return true;
            }
            position = Vector3.Zero;
            return false;
        }

        public virtual Vector3 Position
        {
            get { throw new NotImplementedException(); }
            set { new NotImplementedException(); }
        }
    }
    public abstract class LookatCamera : Camera
    {
        public LookatCamera() 
        {
            Up = Vector3.UnitZ;
            AspectRatio = 1;
            FOV = (float)Math.PI / 3f;
        }
        public LookatCamera(LookatCamera cpy)
        {
            this.Lookat = cpy.Lookat;
            this.Up = cpy.Up;
            this.AspectRatio = cpy.AspectRatio;
            this.FOV = cpy.FOV;
        }
        public override Matrix View
        {
            get { return Matrix.LookAtLH(Position, Lookat, Up); }
        }
        public override Matrix Projection
        {
            get { return Matrix.PerspectiveFovLH(FOV, AspectRatio, ZNear, ZFar); }
        }
        public Vector3 Lookat { get; set; }
        public Vector3 Up { get; set; }
        public float AspectRatio { get; set; }
        public float FOV { get; set; }
    }
    public class LookatCartesianCamera : LookatCamera
    {
        public LookatCartesianCamera() { }
        public LookatCartesianCamera(LookatCartesianCamera cpy)
            : base(cpy)
        {
            this.Position = cpy.Position;
        }
        public LookatCartesianCamera(Vector3 position, Vector3 lookat, Vector3 up, float aspectRatio)
        {
            this.Position = position;
            this.Lookat = lookat;
            this.Up = up;
            this.AspectRatio = aspectRatio;
        }
        public override Camera Clone()
        {
            return new LookatCartesianCamera(this);
        }
        public Matrix Billboard(Vector3 position)
        {
            Vector3 camForward = Lookat - Position;
            Vector3 camUp = Vector3.Cross(camForward, Vector3.Cross(Up, camForward));
            return Matrix.Billboard(position, Position, camUp, camForward);
        }
        Vector3 position = Vector3.Zero;
        public override Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
    public class LookatSphericalCamera : LookatCamera
    {
        public LookatSphericalCamera()
        {
        }
        public LookatSphericalCamera(LookatSphericalCamera cpy)
            : base(cpy)
        {
            this.SphericalCoordinates = cpy.SphericalCoordinates;
        }
        public override Camera Clone()
        {
            return new LookatSphericalCamera(this);
        }

        Vector3 sphericalCoordinates;
        /// <summary>
        /// (Radius, Theta, Phi)
        /// </summary>
        public Vector3 SphericalCoordinates { get { return sphericalCoordinates; } set { sphericalCoordinates = value; } }

        public float Radius { get { return sphericalCoordinates.X; } set { sphericalCoordinates.X = value; } }
        /// <summary>
        /// Elevation
        /// </summary>
        public float Theta { get { return sphericalCoordinates.Y; } set { sphericalCoordinates.Y = value; } }
        /// <summary>
        /// Azimuth/Rotation around Z
        /// </summary>
        public float Phi { get { return sphericalCoordinates.Z; } set { sphericalCoordinates.Z = value; } }
        public override Vector3 Position
        {
            get
            {
                return Lookat + Common.Math.SphericalToCartesianCoordinates(SphericalCoordinates);
            }
            set     // should not be used normally. if you want to control the camera using position, use LookatCartesianCamera instead
            {
                SphericalCoordinates = Common.Math.CartesianToSphericalCoordinates(value - Lookat);
            }
        }
    }

    public class OrthoCamera : Camera
    {
        public OrthoCamera() { ZNear = 0; ZFar = 100; }
        public OrthoCamera(OrthoCamera cpy)
            : base(cpy)
        {
            this.Width = cpy.Width;
            this.Height = cpy.Height;
        }
        public override Camera Clone()
        {
            return new OrthoCamera(this);
        }
        public override Matrix View
        {
            get { return Matrix.Translation(-Position); }
        }
        public override Matrix Projection
        {
            get { return Matrix.OrthoLH(Width, -Height, ZNear, ZFar); }
        }
        public int Width, Height;
        Vector3 position = Vector3.Zero;
        public override Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
}
