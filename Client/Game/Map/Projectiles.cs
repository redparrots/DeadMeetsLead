using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;
using Graphics.Content;

namespace Client.Game.Map
{

    [EditorDeployable, Serializable]
    public class Projectile : GameEntity
    {
        public Projectile()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshConcretize
                {
                    MeshDescription = new Graphics.Software.Meshes.IndexedPlane
                    {
                        Position = Vector3.Zero,
                        Size = new Vector2(1, 1),
                        UVMin = new Vector2(0, 0),
                        UVMax = new Vector2(1, 1),
                        Facings = global::Graphics.Facings.Frontside
                    },
                    Layout = global::Graphics.Software.Vertex.PositionNormalTexcoord.Instance,
                },
                World = Matrix.Translation(-0.5f, -1, 0) * Matrix.Scaling(0.08f, 1.0f, 1) 
                    //* Matrix.RotationZ((float)Math.PI/2f)
                    * Matrix.RotationX(-(float)Math.PI/2f)
                    ,
                Texture = new TextureFromFile("Models/Effects/Trajectory1.png"),
                HasAlpha = true,
                Opacity = 0.5f
            };
            VisibilityLocalBounding = new Common.Bounding.NonfittableBounding(Vector3.Zero, false, true);
            PickingLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            PhysicsLocalBounding = Vector3.Zero;

            TimeToLive = 0.4f;
            Updateable = true;
        }
        public Projectile(Projectile copy)
            : base(copy)
        {
            velocity = copy.velocity;
            timeToLive = TimeToLive = copy.TimeToLive;
            performer = copy.performer;
        }
        public override object Clone()
        {
            return new Projectile(this);
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if (Game.Instance == null) return;

            MotionObject = Game.Instance.Mechanics.MotionSimulation.CreateProjectile();
            MotionProjectile.LocalBounding = PhysicsLocalBounding;
            MotionProjectile.Velocity = Velocity;
            MotionProjectile.Acceleration = Acceleration;
            MotionProjectile.HitsObject += (sender, args) => { OnHitsObject(args.IObject, args.Intersection); };
            MotionProjectile.Position = Position;
            MotionProjectile.Rotation = Rotation;
            timeToLive = TimeToLive;
        }
        public override float Orientation
        {
            get
            {
                return (float)Common.Math.AngleFromVector3XY(Velocity);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Vector3 velocity;
        public Vector3 Velocity
        {
            get { return velocity; }
            set
            {
                velocity = value;
                if (MotionProjectile != null)
                    MotionProjectile.Velocity = value;
            }
        }

        Vector3 acceleration;
        public Vector3 Acceleration
        {
            get { return acceleration; }
            set
            {
                acceleration = value;
                if (MotionProjectile != null)
                    MotionProjectile.Acceleration = value;
            }
        }

        public float TimeToLive { get; set; }
        [NonSerialized]
        Unit performer;
        public Unit Performer { get { return performer; } set { performer = value; } }

        protected virtual void OnHitsObject(Common.IMotion.IObject obj, Vector3 intersection)
        {
            if (IsRemoved) return;

            Translation = intersection;
            if (obj.Tag is GroundPiece)
            {
                CreateGroundBulletHole(intersection);
            }
            else if (obj.Tag is Props.Prop)
            {
                var prop = obj.Tag as Props.Prop;
                if (prop.MainGraphic != null)
                {
                    var dir = Vector3.Normalize(velocity);
                    var dirLeft = Vector3.TransformNormal(dir, Matrix.RotationZ(0.1f));
                    var dirUp = dir;
                    dirUp.Z = 0.1f;
                    var mesh = new BoundingMetaMesh
                    {
                        Mesh = ((MetaModel)prop.MainGraphic).XMesh,
                        Transformation = ((MetaModel)prop.MainGraphic).World * prop.WorldMatrix
                    };
                    var p = intersection - dir;
                    var rayForward = new Ray(p, dir);
                    object distForward, distLeft, distUp;
                    var h1 = global::Graphics.Intersection.Intersect(rayForward, mesh, out distForward);
                    if (!h1) return;

                    var point = rayForward.Position + rayForward.Direction * ((Common.RayIntersection)distForward).Distance;
                    p = point - dir;
                    var rayLeft = new Ray(p, dirLeft);
                    var rayUp = new Ray(p, dirUp);

                    var h2 = global::Graphics.Intersection.Intersect(rayLeft, mesh, out distLeft);
                    var h3 = global::Graphics.Intersection.Intersect(rayUp, mesh, out distUp);
                    var pointLeft = rayLeft.Position + rayLeft.Direction * ((Common.RayIntersection)distLeft).Distance;
                    var pointUp = rayUp.Position + rayUp.Direction * ((Common.RayIntersection)distUp).Distance;

                    var vUp = Vector3.Normalize(pointUp - point);
                    var vLeft = Vector3.Normalize(pointLeft - point);

                    var forward = Vector3.Cross(vUp, vLeft);
                    var up = Vector3.Cross(vLeft, forward);
                    var left = vLeft;

                    Matrix rot = Common.Math.MatrixFromVectors(-left, up, forward, point);
                    CreateWallBulletHole(rot);
                }
            }
            /*Game.Instance.Scene.Add(new Props.Stone3
            {
                Translation = intersection,
                Scale = new Vector3(0.05f, 0.05f, 0.05f),
                PhysicsLocalBounding = null
            });*/
        }

        protected virtual void CreateGroundBulletHole(Vector3 position)
        {
            Scene.Add(new Props.BulletHoleDecal1 { Translation = position });
        }
        protected virtual void CreateWallBulletHole(Matrix world)
        {
            Scene.Add(new Props.BulletHolePlane1 { WorldMatrix = world });
        }

        public Common.IMotion.IProjectile MotionProjectile { get { return (Common.IMotion.IProjectile)MotionObject; } }

        protected virtual void OnTimeout()
        {
            Remove(); 
            /*Game.Instance.Scene.Add(new Props.Stone3
            {
                Translation = Translation,
                Scale = new Vector3(0.05f, 0.05f, 0.05f),
                PhysicsLocalBounding = null
            });*/
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            base.OnUpdate(e);
            if (DesignMode) return;
        
            Translation = MotionObject.InterpolatedPosition;
            Rotation = MotionObject.InterpolatedRotation;

            timeToLive -= e.Dtime;
            if (timeToLive <= 0)
                OnTimeout();
        }

        float timeToLive;
    }



}
