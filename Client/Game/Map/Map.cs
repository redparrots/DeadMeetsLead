using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;

namespace Client.Game.Map
{
    public class EditorDeployableAttribute : Attribute
    {
        public String Group { get; set; }
    }

    public enum MapType
    {
        Normal,
        Cinematic,
        Tutorial
    }

    [TypeConverter(typeof(ExpandableObjectConverter)), Serializable]
    public class MapSettings
    {
        public MapSettings()
        {
            Size = new System.Drawing.SizeF(10, 10);
            WaterHeight = +0.2f;
            Position = new System.Drawing.PointF(0, 0);
            FogColor = new Vector4(0.08f, 0.86f, 0.76f, 1.0f);
            DiffuseColor = new Vector3(1.34f, 1.26f, 0.8f);
            AmbientColor = new Vector3(0.4f, 0.76f, 0.78f);
            SpecularColor = new Vector3(1.35f, 1.15f, 0.9f);
            LightDirection = new Vector3(-0.6f, 0.7f, 0.7f);
            FogExponent = 10.5f;
            FogDistance = 35.1f;
            CameraZFar = 10f;
            Objectives = "???";
            MapType = MapType.Normal;
            Stages = 0;
            AmbienceTrack1 = Client.Sound.Stream.EmptyTrack;
            AmbienceTrack2 = Client.Sound.Stream.EmptyTrack;
            MusicTrack1 = Client.Sound.Stream.EmptyTrack;
            MusicTrack2 = Client.Sound.Stream.EmptyTrack;
        }

        public String Name { get; set; }
        public System.Drawing.PointF Position { get; set; }
        public System.Drawing.SizeF Size { get; set; }
        public float WaterHeight { get; set; }
        public MapType MapType { get; set; }
        private Vector3 ambientColor;
        public Vector3 AmbientColor { get { return ambientColor; } set { ambientColor = value; } }
        private Vector3 diffuseColor;
        public Vector3 DiffuseColor { get { return diffuseColor; } set { diffuseColor = value; } }
        private Vector3 specularColor;
        public Vector3 SpecularColor { get { return specularColor; } set { specularColor = value; } }
        private Vector4 fogColor;
        public Vector4 FogColor { get { return fogColor; } set { fogColor = value; } }
        private Vector3 lightDirection;
        public Vector3 LightDirection { get { return lightDirection; } set { lightDirection = value; } }
        public float FogExponent { get; set; }
        public float FogDistance { get; set; }
        public float CameraZFar { get; set; }
        public int MapVersion { get; set; }
        public Sound.Stream AmbienceTrack1 { get; set; }
        public Sound.Stream AmbienceTrack2 { get; set; }
        public Sound.Stream MusicTrack1 { get; set; }
        public Sound.Stream MusicTrack2 { get; set; }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Objectives { get; set; }
        [Obsolete]
        public string StaticDataSourceMap { get; set; }

        public int Stages { get; set; }
    }

    public class Map
    {
        public Map()
        {
            OnLostDevice = Release;
            OnResetDevice = () => { };
        }
        public string MapName { get; set; }

        public string DisplayName { get { return stringLocalizationStorage.GetString("$(MapName)"); } }
        public string MapObjectives { get { return stringLocalizationStorage.GetString("$(MapObjectives)"); } }

        public System.Action OnLostDevice;
        public System.Action OnResetDevice;

        public void Release()
        {
            Ground.SplatMap1.Resource9.Data.Dispose();
            Ground.SplatMap2.Resource9.Data.Dispose();
            //This solves the memory problem where map references the A8R8G8B8 items
            Ground.SplatMap1Values = null;
            Ground.SplatMap2Values = null;
        }

        /// <summary>
        /// Move static/dynamic objects in the wrong places to the right places
        /// </summary>
        public void NormalizeStaticDynamicObjects()
        {
            foreach (var v in new List<Graphics.Entity>(dynamicData.SceneRoot.Children))
                if (v is GameEntity && !((GameEntity)v).Dynamic)
                {
                    v.Remove();
                    staticData.SceneRoot.AddChild(v);
                }

            foreach (var v in new List<Graphics.Entity>(staticData.SceneRoot.Children))
                if (!(v is GameEntity) || ((GameEntity)v).Dynamic)
                {
                    v.Remove();
                    dynamicData.SceneRoot.AddChild(v);
                }
        }

        public void FindMainCharacter()
        {
            MainCharacter = (Units.MainCharacter)DynamicsRoot.GetByName("MainCharacter");
            if (MainCharacter == null)
                foreach (var v in DynamicsRoot.Offspring)
                    if (v is Units.MainCharacter)
                    {
                        MainCharacter = (Units.MainCharacter)v;
                        break;
                    }
        }

        public void NewSplatMap1(SlimDX.Direct3D9.Device device)
        {
            Ground.SplatMap1 = new Graphics.Content.UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>();
            Ground.SplatMap1.Resource9 = new SlimDX.Direct3D9.Texture(device,
                    (int)settings.Size.Width * 4, (int)settings.Size.Height * 4, 1,
                    SlimDX.Direct3D9.Usage.Dynamic, SlimDX.Direct3D9.Format.A8R8G8B8,
                    SlimDX.Direct3D9.Pool.Default);
        }

        public void NewSplatMap2(SlimDX.Direct3D9.Device device)
        {
            Ground.SplatMap2 = new Graphics.Content.UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>();
            Ground.SplatMap2.Resource9 = new SlimDX.Direct3D9.Texture(device,
                    (int)settings.Size.Width * 4, (int)settings.Size.Height * 4, 1,
                    SlimDX.Direct3D9.Usage.Dynamic, SlimDX.Direct3D9.Format.A8R8G8B8,
                    SlimDX.Direct3D9.Pool.Default);
        }

        public static Map New(MapSettings settings, SlimDX.Direct3D9.Device device)
        {
            Map m = new Map
            {
                Settings = settings
            };

            m.Ground = new Ground();
            m.InitGround();
            m.NewSplatMap1(device);
            m.NewSplatMap2(device);
            m.Ground.ConstructPieces(m);
            m.Ground.Init();
            m.MainCharacter = new Units.MainCharacter
            {
                Translation = new Vector3(settings.Size.Width / 2f, settings.Size.Height / 2f, 0),
                Name = "MainCharacter"
            };
            m.DynamicsRoot.AddChild(m.MainCharacter);

            return m;
        }

        public void SaveChart(String filename)
        {
            Bitmap b = new Bitmap((int)Settings.Size.Width, (int)Settings.Size.Height);
            var g = System.Drawing.Graphics.FromImage(b);
            g.Clear(Color.Black);
            foreach (var v in StaticsRoot.Offspring)
            {
                g.DrawRectangle(Pens.White, v.Translation.X, v.Translation.Y, 1, 1);
            }
            b.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
            g.Dispose();
            b.Dispose();
        }

        public void InitGround()
        {
            Ground.Height = 1;
            Ground.Heightmap = new Graphics.Software.Texel.R32F[
                    (int)(Settings.Size.Height / Constants.GroundPolySize),
                    (int)(Settings.Size.Width / Constants.GroundPolySize)];
            Ground.PieceSize = new System.Drawing.SizeF(
                Constants.GroundPieceSize,
                Constants.GroundPieceSize
                );
            Ground.Size = Settings.Size;
            Ground.Translation = Common.Math.ToVector3(Settings.Position);
        }

        [Common.CodeState(State = Common.CodeState.Incomplete, Details = "Doesn't work yet")]
        public void Resize(SlimDX.Direct3D9.Device device, System.Drawing.RectangleF rectangle)
        {
            var hm = Ground.Heightmap;
            //var oldSm1 = Ground.SplatMap1;
            //var oldSm2 = Ground.SplatMap2;
            Settings.Size = new System.Drawing.SizeF(rectangle.Width, rectangle.Height);
            InitGround();
            int yOldWorldStart = (int)(Ground.Translation.Y / Constants.GroundPolySize);
            int xOldWorldStart = (int)(Ground.Translation.X / Constants.GroundPolySize);
            int yNewWorldStart = (int)(rectangle.Y / Constants.GroundPolySize);
            int xNewWorldStart = (int)(rectangle.X / Constants.GroundPolySize);
            int xNewOffs = xNewWorldStart - xOldWorldStart;
            int yNewOffs = yNewWorldStart - yOldWorldStart;

            int yReadOffset = yNewOffs;
            int xReadOffset = xNewOffs;

            for (int y = 0; y < Ground.Heightmap.GetLength(0); y++)
                for (int x = 0; x < Ground.Heightmap.GetLength(1); x++)
                {
                    if (y + yReadOffset >= 0 && x + xReadOffset >= 0 &&
                        y + yReadOffset < hm.GetLength(0) && x + xReadOffset < hm.GetLength(1))
                        Ground.Heightmap[y, x] = hm[y + yReadOffset, x + xReadOffset];
                }
            //var oldSmR1 = Graphics.TextureUtil.ReadTexture<Graphics.Software.Texel.A8R8G8B8>(oldSm1, 0);
            //var oldSmR2 = Graphics.TextureUtil.ReadTexture<Graphics.Software.Texel.A8R8G8B8>(oldSm2, 0);

            Ground.SplatMap1.Resource9 = new SlimDX.Direct3D9.Texture(device,
                    (int)Settings.Size.Width * 4, (int)Settings.Size.Height * 4, 1,
                    SlimDX.Direct3D9.Usage.Dynamic, SlimDX.Direct3D9.Format.A8R8G8B8,
                    SlimDX.Direct3D9.Pool.Default);
            Ground.SplatMap2.Resource9 = new SlimDX.Direct3D9.Texture(device,
                    (int)Settings.Size.Width * 4, (int)Settings.Size.Height * 4, 1,
                    SlimDX.Direct3D9.Usage.Dynamic, SlimDX.Direct3D9.Format.A8R8G8B8,
                    SlimDX.Direct3D9.Pool.Default);
            /*var ld = Ground.SplatMap.GetLevelDescription(0);

            int ySMOldWorldStart = (int)((Ground.Translation.Y / Constants.GroundPolySize) * 4f);
            int xSMOldWorldStart = (int)((Ground.Translation.X / Constants.GroundPolySize) * 4f);
            int ySMNewWorldStart = (int)((rectangle.Y / Constants.GroundPolySize) * 4f);
            int xSMNewWorldStart = (int)((rectangle.X / Constants.GroundPolySize) * 4f);
            int xSMNewOffs = xSMNewWorldStart - xSMOldWorldStart;
            int ySMNewOffs = ySMNewWorldStart - ySMOldWorldStart;

            Graphics.Software.Texel.A8R8G8B8[,] p = new Graphics.Software.Texel.A8R8G8B8[
                (int)Settings.Size.Height * 4, (int)Settings.Size.Width * 4];
            
            for (int y = 0; y < p.GetLength(0); y++)
                for (int x = 0; x < p.GetLength(1); x++)
                {
                    if (y + ySMNewOffs >= 0 && x + xSMNewOffs >= 0 &&
                        y + ySMNewOffs < oldSmR.GetLength(0) && x + xSMNewOffs < oldSmR.GetLength(1))
                        p[y, x] = oldSmR[y + ySMNewOffs, x + xSMNewOffs];
                }

            Graphics.TextureUtil.WriteTexture(Ground.SplatMap, p, p[0, 0].Size);
            */
            Settings.Position = new System.Drawing.PointF(rectangle.X, rectangle.Y);
            Ground.Translation = new Vector3(rectangle.X, rectangle.Y, 0);
            Ground.ConstructPieces(this);
            Ground.Init();
        }

        public void CheckMap(Action<String> errors)
        {
            foreach (var v in Scripts)
                v.CheckParameters(this, errors);
        }

        public Region GetRegion(String name)
        {
            foreach (var v in Regions)
                if (v.Name == name) return v;
            return null;
        }

        public CameraAngle GetCameraAngle(String name)
        {
            foreach (var v in CameraAngles)
                if (v.Name == name) return v;
            return null;
        }

        public Graphics.Entity DynamicsRoot { get { return dynamicData.SceneRoot; } }
        public Graphics.Entity StaticsRoot { get { return staticData.SceneRoot; } }
        public Common.Pathing.NavMesh NavMesh { get { return staticData.NavMesh; } }
        public List<Script> Scripts { get { return dynamicData.Scripts; } }
        public List<Region> Regions { get { return staticData.Regions; } }
        public List<CameraAngle> CameraAngles { get { return staticData.CameraAngles; } }
        
        MapDynamicData dynamicData = new MapDynamicData();
        public MapDynamicData DynamicData { get { return dynamicData; } set { dynamicData = value; } }
        MapStaticData staticData = new MapStaticData();
        public MapStaticData StaticData { get { return staticData; } set { staticData = value; } }
        Common.StringLocalizationStorage stringLocalizationStorage = new Common.StringLocalizationStorage();
        public Common.StringLocalizationStorage StringLocalizationStorage { get { return stringLocalizationStorage; } set { stringLocalizationStorage = value; } }

        public Units.MainCharacter MainCharacter;
        MapSettings settings = new MapSettings();
        public MapSettings Settings { get { return settings; } set { settings = value; } }
        Ground ground;
        public Ground Ground { get { return ground; } set { ground = value; } }

        public string MapFileName { get; set; }
    }

    [Serializable]
    public class MapDynamicData
    {
        public List<Script> Scripts = new List<Script>();
        public Graphics.Entity SceneRoot = new Graphics.Entity();
    }
    [Serializable]
    public class MapStaticData
    {
        public List<Region> Regions = new List<Region>();
        public Common.Pathing.NavMesh NavMesh = new Common.Pathing.NavMesh();
        public Graphics.Entity SceneRoot = new Graphics.Entity();
        public List<CameraAngle> CameraAngles = new List<CameraAngle>();
    }

    [Serializable]
    public class CameraAngle
    {
        public CameraAngle()
        {
            Up = Vector3.UnitZ;
        }
        public String Name { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Lookat { get; set; }
        public Vector3 Up { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    static public class Constants
    {
        public static float GroundPieceSize = 5;
        public static float GroundPolySize = 0.4f;
    }
}
