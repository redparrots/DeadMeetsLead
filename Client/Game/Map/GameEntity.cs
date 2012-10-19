using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Windows.Forms;
using Graphics;
using Graphics.Content;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Client.Game.Map
{
    public enum EditorFollowGroupType
    {
        Heightmap,
        Water,
        HeightmapAndWater,
        ZeroPlane
    }

    [Serializable]
    public abstract class GameEntity : Entity, Effects.IGameEffect
    {
        public GameEntity()
        {
            EditorSelectable = true;
            EditorMinRandomScale = 1;
            EditorMaxRandomScale = 1;
            EditorFollowGroundType = EditorFollowGroupType.Heightmap;
            EditorPlacementLocalBounding = new Common.Bounding.Cylinder(Vector3.Zero, 3, 1f);
            Clickable = false;
            SeeThroughable = false;
            EditorLockTranslation = false;

            fader = new EntityFader(this);
        }
        public GameEntity(GameEntity copy)
            : base(copy)
        {
            //visibilityBoundingInited = copy.visibilityBoundingInited;
            editorSelectable = copy.editorSelectable;
            EditorMinRandomScale = copy.EditorMinRandomScale;
            EditorMaxRandomScale = copy.EditorMaxRandomScale;
            EditorFollowGroundType = copy.EditorFollowGroundType;
            EditorRandomRotation = copy.EditorRandomRotation;
        }

        public override object Clone()
        {
            cloneMemoryStream.Position = 0;
            cloneMemoryStream.SetLength(0);
            cloneFormatter.Serialize(cloneMemoryStream, this);
            cloneMemoryStream.Position = 0;
            //string s = System.Text.Encoding.ASCII.GetString(cloneMemoryStream.ToArray());
            return cloneFormatter.Deserialize(cloneMemoryStream);
        }
        static System.Runtime.Serialization.IFormatter cloneFormatter =
            new Common.XmlFormatter { Binder = ClientXmlFormatterBinder.Instance };
            //new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        static System.IO.MemoryStream cloneMemoryStream = new System.IO.MemoryStream();

        public override string ToString()
        {
            return GetType().Name;
        }

        #region Editor
        [NonSerialized]
        bool editorSelectable;
        public bool EditorSelectable { get { return editorSelectable; } set { editorSelectable = value; } }

        [Browsable(false)]
        public virtual float EditorMinRandomScale { get; set; }
        [Browsable(false)]
        public virtual float EditorMaxRandomScale { get; set; }
        [Browsable(false)]
        public virtual bool EditorRandomRotation { get; set; }
        public virtual EditorFollowGroupType EditorFollowGroundType { get; set; }
        public object EditorPlacementLocalBounding { get; set; }
        public bool EditorLockTranslation { get; set; }
        public float EditorHeight { get; set; }
        public object EditorPlacementWorldBounding
        {
            get
            {
                if (EditorPlacementLocalBounding == null) return null;
                return Common.Boundings.Transform(EditorPlacementLocalBounding, WorldMatrix);
            }
        }
        public void EditorInit()
        {
            if (EditorRandomRotation)
                Rotation = Quaternion.RotationAxis(Vector3.UnitZ, (float)(2 * Math.PI * Game.Random.NextDouble()));
            float scale = EditorMinRandomScale + (float)Game.Random.NextDouble() * (EditorMaxRandomScale - EditorMinRandomScale);
            Scale = new Vector3(scale, scale, scale);
        }
        public void EditorCreateNewName(Scene scene)
        {
            EditorCreateNewName(scene, GetType().Name);
        }
        public void EditorCreateNewName(Scene scene, String baseName)
        {
            int i = 1;
            foreach (var v in scene.Root.Offspring)
                if (v.GetType() == GetType() && v.Name != null && v.Name.Length > baseName.Length)
                {
                    int l;
                    if (int.TryParse(v.Name.Substring(baseName.Length), out l))
                        i = Math.Max(i, l) + 1;
                }
            Name = baseName + i;
        }
        public virtual void EditorHeightmapChanged(WorldViewProbe groundProbe)
        {
            var ray = new Ray(Translation + Vector3.UnitZ * 1000, -Vector3.UnitZ);
            float d;
            if (groundProbe.Intersect(ray, this, out d))
                Translation = ray.Position + ray.Direction * d + Vector3.UnitZ * EditorHeight;
        }
        #endregion

        //public float Distance(GameEntity obj)
        //{
        //    float rb = 0;
        //    if (obj.MotionObject != null)
        //        rb = Common.Boundings.Radius(obj.PhysicsLocalBounding);
        //    return Distance(obj.Position) - rb;
        //}
        //public float Distance(Vector3 position)
        //{
        //    float ra = 0;
        //    if (MotionObject != null && PhysicsLocalBounding != null)
        //        ra = Common.Boundings.Radius(PhysicsLocalBounding);
        //    var d = Common.Math.ToVector2(Translation - position).Length();
        //    return d - ra;
        //}
        public virtual float Orientation { get { return 0; } set { } }

        public virtual float Scaling { get { return Scale.X; } set { Scale = new Vector3(value, value, value); } }

        public virtual float LookatDir { get { return Orientation; } set { Orientation = value; } }

        public virtual void Stop()
        {
            fader.Fadeout();
        }

        public float FadeoutTime { get { return fader.FadeoutTime; } set { fader.FadeoutTime = value; } }
        public float FadeinTime { get { return fader.FadeinTime; } set { fader.FadeinTime = value; } }
        public float AutoFadeoutTime { get { return fader.AutoFadeoutTime; } set { fader.AutoFadeoutTime = value; } }

        [NonSerialized]
        EntityFader fader;

        public virtual Vector3 Position
        {
            get { return Translation; }
            set
            {
                Translation = value;
                if (MotionObject != null)
                    MotionObject.Position = value;
            }
        }

        [NonSerialized]
        bool seeThroughable;
        [Browsable(false)]
        public bool SeeThroughable { get { return seeThroughable; } set { seeThroughable = value; } }

        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            if (!IsInGame) return;

            //if (Program.Instance != null && physicsMesh != null)
            //    Program.Instance.Content.Release(physicsMesh);
            MotionObject = null;
            Game.Instance.Mechanics.Remove(this);
        }

        //public FMOD.Channel PlaySound(params string[] sounds)
        //{
        //    return null;
        //    //return Program.Instance.Sound.Play3DSound(sounds[r.Next(sounds.Length)], Position, Vector3.Zero);
        //}
        //static Random r = new Random();

        /// <summary>
        /// If this entity is used as a spawning point, this tells where the units will spawn.
        /// </summary>
        public virtual Vector3 SpawnAtPoint { get { return Translation + Vector3.UnitZ * 0.1f; } }

        [NonSerialized]
        Map map;
        public Map Map
        {
            get
            {
                if (IsInGame)
                    Map = Game.Instance.Map ?? map;
                return map;
            }
            set
            {
                map = value;
            }
        }
        public virtual void OnLostDevice()
        {
            if (PhysicsLocalBounding is Graphics.ContentPreloadable)
                ((ContentPreloadable)PhysicsLocalBounding).OnLostDevice(ContentPool);
        }
        public virtual void OnResetDevice()
        {
            ContentResetPhysicsBounding(PhysicsLocalBounding);
        }
        #region Physics

        [NonSerialized]
        Common.IMotion.IObject motionObject;
        public Common.IMotion.IObject MotionObject
        {
            get { return motionObject; }
            set
            {
                if (motionObject == value) return;
                if (motionObject != null) Game.Instance.Mechanics.MotionSimulation.Remove(motionObject);
                motionObject = value;
                if (motionObject != null) Game.Instance.Mechanics.MotionSimulation.Insert(motionObject);
            }
        }
        [NonSerialized]
        object physicsLocalBounding;
        public object PhysicsLocalBounding
        {
            get { return physicsLocalBounding; }
            set
            {
                CheckPhysicsLocalBounding(value);
                physicsLocalBounding = value;
                if (motionObject != null) motionObject.LocalBounding = value;
            }
        }
        [Browsable(false)]
        public object PhysicsWorldBounding
        {
            get { return Common.Boundings.Transform(PhysicsLocalBounding, WorldMatrix); }
        }
        void CheckPhysicsLocalBounding(object bounding)
        {
            if (bounding is MetaBoundingBox)
            {
                if (((MetaBoundingBox)bounding).Mesh is Graphics.Content.MetaResource<SkinnedMesh>)
                    throw new Exception("Physics boundings cannot be skinnedmesh!");
            }
            else if (bounding is Common.Bounding.Chain)
            {
                foreach (var v in ((Common.Bounding.Chain)bounding).Boundings)
                    CheckPhysicsLocalBounding(v);
            }
        }
        protected object CreateBoundingBoxFromModel(MetaModel model)
        {
            return new Graphics.MetaBoundingBox
            {
                Mesh = model.XMesh ?? model.SkinnedMesh,
                Transformation = model.World
            };
        }
        protected object CreateBoundingMeshFromModel(MetaModel model)
        {
            return CreateBoundingMeshFromModel(model, Matrix.Identity);
        }
        protected object CreateBoundingMeshFromModel(MetaModel model, Matrix transformation)
        {
            return new Common.Bounding.Chain
            {
                Boundings = new object[]
                {
                    new Graphics.MetaBoundingBox
                    {
                        Mesh = model.XMesh ?? model.SkinnedMesh,
                        Transformation = model.World
                    },
                    new Graphics.BoundingMetaMesh
                    {
                        Mesh = model.XMesh,
                        SkinnedMeshInstance = MetaEntityAnimation,
                        Transformation = model.World * transformation,
                    }
                },
                Shallow = true
            };
        }
        protected object CreatePhysicsMeshBounding(MetaModel model)
        {
            return CreatePhysicsMeshBounding(model, Matrix.Identity);
        }
        public static ContentPool ContentPool;
        protected object CreatePhysicsMeshBounding(MetaModel model, Matrix transformation)
        {
            BoundingMetaMesh boundingMetaMesh = new BoundingMetaMesh();
            BoundingBox boundingBox = new Graphics.MetaBoundingBox
            {
                Mesh = model.XMesh ?? model.SkinnedMesh,
                Transformation = model.World
            }.GetBoundingBox(ContentPool).Value;

            if (Program.Settings != null && Program.Settings.MotionSettings.UseSoftwareMeshes)
            {
                boundingMetaMesh.SoftwareMesh = Program.Instance.Content.Acquire<global::Graphics.Software.Mesh>(model.XMesh);
            }
            else
            {
                //if (model.XMesh is Graphics.Content.MeshFromFile)
                //    ((Graphics.Content.MeshFromFile)model.XMesh).Flags = SlimDX.Direct3D9.MeshFlags.Software;
                boundingMetaMesh.Mesh = model.XMesh ?? model.SkinnedMesh;
            }
            if(model.SkinnedMesh != null)
                boundingMetaMesh.SkinnedMeshInstance = MetaEntityAnimation;
            boundingMetaMesh.Transformation = model.World * transformation;

            boundingMetaMesh.Init(ContentPool);

            return new Common.Bounding.Chain
            {
                Boundings = new object[]
                {
                    boundingBox,
                    boundingMetaMesh
                },
                Shallow = true
            };
        }
        void ContentResetPhysicsBounding(object bounding)
        {
            if (bounding is Common.Bounding.Chain)
            {
                foreach (var v in ((Common.Bounding.Chain)bounding).Boundings)
                    ContentResetPhysicsBounding(v);
            }
            else if (bounding is Graphics.ContentPreloadable)
                ((ContentPreloadable)bounding).OnResetDevice(ContentPool);
        }

        #endregion

        public virtual void GameStart()
        {
        }
        public virtual void GameUpdate(float dtime)
        {
        }

        [NonSerialized]
        public bool Dynamic = true;

        [NonSerialized]
        public Dictionary<String, object> ScriptingUserdata = new Dictionary<string, object>();

        [Browsable(false)]
        public bool IsInGame { get { return Game.Instance != null; } }
    }
}
