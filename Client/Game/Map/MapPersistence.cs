using System;
using System.Collections.Generic;
using System.IO;
using Common;
using Ionic.Zip;

namespace Client.Game.Map
{
    public interface IMapPersistence
    {
        Map Load(string mapFilename, SlimDX.Direct3D9.Device device);
        List<Common.Tuple<String, System.Action>> GetLoadSteps(string mapFilename, SlimDX.Direct3D9.Device device, out Map map);
        MapSettings LoadSettings(string mapFilename);
        StringLocalizationStorage LoadLanguageData(string mapFilename);
        void Reload(Map map);
        void Save(Map map, string filename);
        void RemoveAllFiles(Map map, String filename);
        int GetVersion(String filename);
    }

    public class MapPersistence : IMapPersistence
    {
        static readonly MapPersistence instance = new MapPersistence();
        public static MapPersistence Instance { get { return instance; } }
        public Map Load(string mapFilename, SlimDX.Direct3D9.Device device)
        {
            return persisters[GetVersion(mapFilename) - 1].Load(mapFilename, device);
        }

        public List<Common.Tuple<string, System.Action>> GetLoadSteps(string mapFilename, SlimDX.Direct3D9.Device device, out Map map)
        {
            return persisters[GetVersion(mapFilename) - 1].GetLoadSteps(mapFilename, device, out map);
        }

        public MapSettings LoadSettings(string mapFilename)
        {
            return persisters[GetVersion(mapFilename) - 1].LoadSettings(mapFilename);
        }

        public StringLocalizationStorage LoadLanguageData(string mapFilename)
        {
            return persisters[GetVersion(mapFilename) - 1].LoadLanguageData(mapFilename);
        }

        public void Reload(Map map)
        {
            persisters[GetVersion(map.MapFileName) - 1].Reload(map);
        }

        public void Save(Map map, string filename)
        {
            persisters[persisters.Length - 1].Save(map, filename);
        }

        public void RemoveAllFiles(Map map, string filename)
        {
            persisters[GetVersion(filename) - 1].RemoveAllFiles(map, filename);
        }

        public int GetVersion(string filename)
        {
            return persisters[persisters.Length - 1].GetVersion(filename);
        }

        public IMapPersistence[] GetPersisters()
        {
            return persisters;
        }

        readonly IMapPersistence[] persisters = new IMapPersistence[]
        {
            new MapPersistenceV1(),
        };
    }

    class MapPersistenceV1 : IMapPersistence
    {
        public Map Load(string filename, SlimDX.Direct3D9.Device device)
        {
            Map m;
            foreach (var v in GetLoadSteps(filename, device, out m))
                v.Second();
            return m;
        }
        public List<Common.Tuple<String, System.Action>> GetLoadSteps(string filename, SlimDX.Direct3D9.Device device, out Map outMap)
        {
            var baseFileName = Path.GetFileName(filename);
            var map = new Map { MapName = baseFileName, MapFileName = Path.GetFullPath(filename) };
            outMap = map;
            var mapFile = new ZipFile(filename);
            map.OnResetDevice = () => OnResetDevice(map);

            var a = new List<Common.Tuple<string, System.Action>>();

            a.Add(new Common.Tuple<string, System.Action>("Loading settings",
                () => map.Settings = LoadSettings(baseFileName, mapFile)));

            a.Add(new Common.Tuple<string, System.Action>("Loading static data", () => LoadStaticData(map, mapFile)));
            a.Add(new Common.Tuple<string, System.Action>("Loading dynamic data", () => LoadDynamicData(map, mapFile)));
            a.Add(new Common.Tuple<string, System.Action>("Loading language data",
                () => map.StringLocalizationStorage = LoadLanguageData(baseFileName, mapFile)));


            a.Add(new Common.Tuple<string, System.Action>("NormalizeStaticDynamicObjects",
                map.NormalizeStaticDynamicObjects));

            a.Add(new Common.Tuple<string, System.Action>("Init scene root", () =>
            {

                foreach (var v in map.DynamicsRoot.Offspring)
                    if (v is GameEntity)
                        ((GameEntity)v).Map = map;

                foreach (var v in map.StaticsRoot.Offspring)
                    if (v is GameEntity)
                        ((GameEntity)v).Map = map;

                map.FindMainCharacter();
            }));

            a.Add(new Common.Tuple<string, System.Action>("Creating ground", () =>
            {
                map.Ground = new Ground();
                map.InitGround();
            }));

            a.Add(new Common.Tuple<string, System.Action>("Loading SplatMap", () => LoadSplatMap(map, device, mapFile)));

            a.Add(new Common.Tuple<string, System.Action>("Loading SplatMap2", () => LoadSplatMap2(map, device, mapFile)));


            a.Add(new Common.Tuple<string, System.Action>("Loading Heightmap", () => LoadHeightmap(map, mapFile)));
            a.Add(new Common.Tuple<string, System.Action>("Constructing ground pieces", () => map.Ground.ConstructPieces(map)));

            a.Add(new Common.Tuple<string, System.Action>("Initializing ground", () =>
            {
                if (map.Ground.SplatMap1 != null && map.Ground.SplatMap2 != null)
                    map.Ground.Init();
            }));

            a.Add(new Common.Tuple<string, System.Action>("Done", () => 
            {
                mapFile.Dispose();
            }));

            return a;
        }
        public MapSettings LoadSettings(string mapFilename)
        {
            var mapFile = new ZipFile(mapFilename);
            var settings = LoadSettings(Path.GetFileName(mapFilename), mapFile);
            return settings;
        }
        MapSettings LoadSettings(string mapFilename, ZipFile mapFile)
        {
            MapSettings settings;
            using(var f = mapFile[GetSettingsFileName()].OpenReader())
                settings = (MapSettings)formatter.Deserialize(f);
            formatter.PersistenceData = null;
            return settings;
        }
        public StringLocalizationStorage LoadLanguageData(string mapFilename)
        {
            var mapFile = new ZipFile(mapFilename);
            var languageData = LoadLanguageData(Path.GetFileName(mapFilename), mapFile);
            return languageData;
        }
        StringLocalizationStorage LoadLanguageData(string mapFilename, ZipFile mapFile)
        {
            var sls = new StringLocalizationStorage();
            if (mapFile.ContainsEntry("strings.resx"))
                using (var s = mapFile["strings.resx"].OpenReader())
                    sls.AddLanguage("en", s);
            foreach(var v in mapFile.SelectEntries("strings.*.resx"))
            {
                var lang = v.FileName.Substring("strings.".Length);
                lang = lang.Substring(0, lang.Length - ".resx".Length);
                using (var s = v.OpenReader())
                    sls.AddLanguage(lang, s);
            }
            return sls;
        }

        public void Reload(Map map)
        {
            using(var mapFile = new ZipFile(map.MapFileName))
                LoadDynamicData(map, mapFile);
            map.FindMainCharacter();
        }

        void OnResetDevice(Map map)
        {
            if (map.MapName == null) return;

            var mapFile = new ZipFile(map.MapFileName);

            using (var splat1Stream = mapFile[GetSplatTexture1FileName()].OpenReader())
                map.Ground.SplatMap1.Resource9 =
                    SlimDX.Direct3D9.Texture.FromStream(Program.Instance.Device9, splat1Stream);

            using (var splat2Stream = mapFile[GetSplatTexture2FileName()].OpenReader())
                map.Ground.SplatMap2.Resource9 =
                    SlimDX.Direct3D9.Texture.FromStream(Program.Instance.Device9,splat2Stream);
            
            map.Ground.InitSplatMapValues();
        }

        void LoadStaticData(Map map, ZipFile mapFile)
        {
#pragma warning disable 612,618
            if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                mapFile = new ZipFile(Path.Combine(Path.GetDirectoryName(map.MapFileName), map.Settings.StaticDataSourceMap));
#pragma warning restore 612,618

            using (var f = mapFile[GetStaticFileName()].OpenReader())
                map.StaticData = (MapStaticData)formatter.Deserialize(f);
            formatter.PersistenceData = null;
            if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                mapFile.Dispose();
        }

        void LoadDynamicData(Map map, ZipFile mapFile)
        {
            using (var f = mapFile[GetDynamicFileName()].OpenReader())
                map.DynamicData = (MapDynamicData)formatter.Deserialize(f);
            formatter.PersistenceData = null;
        }


        void LoadSplatMap(Map map, SlimDX.Direct3D9.Device device, ZipFile mapFile)
        {
#pragma warning disable 612,618
            if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                mapFile = new ZipFile(Path.Combine(Path.GetDirectoryName(map.MapFileName), map.Settings.StaticDataSourceMap));
#pragma warning restore 612,618

            if (!mapFile.ContainsEntry(GetSplatTexture1FileName()))
            {
                map.NewSplatMap1(device);
                if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                    mapFile.Dispose();
                return;
            }

            using(var splat1Stream = mapFile[GetSplatTexture1FileName()].OpenReader())
                map.Ground.SplatMap1 =
                    new Graphics.Content.UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>
                    {
                        Resource9 = SlimDX.Direct3D9.Texture.FromStream(device, splat1Stream)
                    };
            if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                mapFile.Dispose();
        }

        void LoadSplatMap2(Map map, SlimDX.Direct3D9.Device device, ZipFile mapFile)
        {
#pragma warning disable 612,618
            if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                mapFile = new ZipFile(Path.Combine(Path.GetDirectoryName(map.MapFileName), map.Settings.StaticDataSourceMap));
#pragma warning restore 612,618

            if (!mapFile.ContainsEntry(GetSplatTexture2FileName()))
            {
                map.NewSplatMap2(device);
                if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                    mapFile.Dispose();
                return;
            }

            using (var splat2Stream = mapFile[GetSplatTexture2FileName()].OpenReader())
                map.Ground.SplatMap2 =
                    new Graphics.Content.UnmanagedResource<SlimDX.Direct3D9.Texture, SlimDX.Direct3D10.Texture2D>
                    {
                        Resource9 = SlimDX.Direct3D9.Texture.FromStream(device, splat2Stream)
                    };
            if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                mapFile.Dispose();
        }

        void LoadHeightmap(Map map, ZipFile mapFile)
        {
#pragma warning disable 612,618
            if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                mapFile = new ZipFile(Path.Combine(Path.GetDirectoryName(map.MapFileName), map.Settings.StaticDataSourceMap));
#pragma warning restore 612,618

            if (!mapFile.ContainsEntry(GetHeightmapFileName()))
            {
                if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                    mapFile.Dispose();
                return;
            }

            using (var f2 = mapFile[GetHeightmapFileName()].OpenReader())
                DeserializeHieghtmap(map, f2);

            if (!string.IsNullOrEmpty(map.Settings.StaticDataSourceMap))
                mapFile.Dispose();
        }
        protected virtual void DeserializeHieghtmap(Map map, Stream stream)
        {
            // Heightmap serialization method 3
            var d = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(stream);
            d.GetNextEntry();
            var br = new BinaryReader(d);
            var height = br.ReadInt32();
            var width = br.ReadInt32();
            var hm = new Graphics.Software.Texel.R32F[height, width];
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    hm[y, x] = new Graphics.Software.Texel.R32F(br.ReadSingle());

            map.Ground.Heightmap = hm;
            d.Close();
        }

        static readonly XmlFormatter formatter = new XmlFormatter
        {
            Binder = ClientXmlFormatterBinder.Instance
        };

        public void Save(Map map, string filename)
        {
            map.Settings.MapVersion++;
            map.MapName = Path.GetFileName(filename);

            var mapFile = new ZipFile();

            WriteToFile(map, filename, mapFile);

            mapFile.Save(filename);
        }


        protected virtual void WriteToFile(Map map, string filename, ZipFile mapFile)
        {
            var mapBaseFileName = map.MapName;

            mapFile.AddEntry(GetStaticFileName(), (name, f) =>
                formatter.Serialize(f, map.StaticData));

            mapFile.AddEntry(GetDynamicFileName(), (name, f) =>
                formatter.Serialize(f, map.DynamicData));

            foreach(var language in map.StringLocalizationStorage.GetLanguages())
            {
                string fn = null;
                if(language == "en") fn = "strings.resx";
                else fn = "strings." + language + ".resx";
                
                mapFile.AddEntry(fn, (name, stream) => 
                    map.StringLocalizationStorage.WriteLanguageToStream(language, stream));
            }

            mapFile.AddEntry(GetSettingsFileName(), (name, f) =>
                formatter.Serialize(f, map.Settings));

            if (map.Ground.SplatMap1 != null)
                mapFile.AddEntry(GetSplatTexture1FileName(), (name, stream) =>
                    SlimDX.Direct3D9.BaseTexture.ToStream(map.Ground.SplatMap1.Resource9,
                        SlimDX.Direct3D9.ImageFileFormat.Png).CopyTo(stream));

            if (map.Ground.SplatMap2 != null)
                mapFile.AddEntry(GetSplatTexture2FileName(), (name, stream) =>
                    SlimDX.Direct3D9.BaseTexture.ToStream(map.Ground.SplatMap2.Resource9,
                        SlimDX.Direct3D9.ImageFileFormat.Png).CopyTo(stream));

            
            mapFile.AddEntry(GetHeightmapFileName(), (name, stream) =>
                SerializeHeightmap(map, stream));
        }
        protected virtual void SerializeHeightmap(Map map, Stream stream)
        {
            // Heightmap serialization method 3
            var i = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(stream);
            var entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry("Heightmap");
            i.PutNextEntry(entry);
            var bw = new BinaryWriter(i);
            bw.Write(map.Ground.Heightmap.GetLength(0));
            bw.Write(map.Ground.Heightmap.GetLength(1));
            for (var y = 0; y < map.Ground.Heightmap.GetLength(0); y++)
                for (var x = 0; x < map.Ground.Heightmap.GetLength(1); x++)
                    bw.Write(map.Ground.Heightmap[y, x].R);
            i.Close();
        }

        public virtual void RemoveAllFiles(Map map, String filename)
        {
            if (FileSystem.Instance.FileExists(filename))
                FileSystem.Instance.DeleteFile(filename);
        }

        public virtual int GetVersion(string filename)
        {
            return 1;
        }

        protected virtual string GetStaticFileName()
        {
            return "static.xml";
        }
        protected virtual string GetDynamicFileName()
        {
            return "dynamic.xml";
        }
        protected virtual string GetSettingsFileName()
        {
            return "settings.xml";
        }
        protected virtual string GetSplatTexture1FileName()
        {
            return "splat1.png";
        }
        protected virtual string GetSplatTexture2FileName()
        {
            return "splat2.png";
        }
        protected virtual string GetHeightmapFileName()
        {
            return "heightmap";
        }
    }


}
