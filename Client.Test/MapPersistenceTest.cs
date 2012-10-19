using System.IO;
using Client.Game.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Client.Test
{
    [TestClass]
    public class MapPersistenceTest
    {

        [TestInitialize]
        public void Initialize()
        {
            device = new Graphics.GraphicsDevice.GraphicsDevice9();
            device.Create();
            GameEntity.ContentPool = new Graphics.Content.ContentPool(device.Device9);
            Graphics.Boundings.Init(GameEntity.ContentPool);
            Graphics.Intersection.Init(GameEntity.ContentPool);
            Graphics.SpatialRelation.Init(GameEntity.ContentPool);
        }

        [TestCleanup]
        public void Cleanup()
        {
            device.Destroy();
        }

        Graphics.GraphicsDevice.GraphicsDevice9 device;

        [TestMethod]
        public void SaveLoadTest()
        {
            SaveLoadTest(MapPersistence.Instance);
            foreach (var v in MapPersistence.Instance.GetPersisters())
                SaveLoadTest(v);
        }

        void SaveLoadTest(IMapPersistence persister)
        {
            var tmpPath = Path.GetTempPath() + Guid.NewGuid();
            Directory.CreateDirectory(tmpPath);
            var tmpMap = tmpPath + "/temp.map";
            var map = Map.New(new MapSettings(), device.Device9);
            persister.Save(map, tmpMap);
            persister.Load(tmpMap, device.Device9);
            Directory.Delete(tmpPath, true);
        }

        [TestMethod]
        public void StaticMapSourceV1Test()
        {
            var persister = MapPersistence.Instance.GetPersisters()[0];

            var srcPath = Path.GetTempPath() + Guid.NewGuid();
            Directory.CreateDirectory(srcPath);
            var srcFilename = srcPath + "/src.map";
            var srcMap = Map.New(new MapSettings(), device.Device9);
            persister.Save(srcMap, srcFilename);

            var destPath = Path.GetTempPath() + Guid.NewGuid();
            Directory.CreateDirectory(destPath);
            var destFilename = destPath + "/dest.map";
#pragma warning disable 612,618
            var destMap = Map.New(new MapSettings { StaticDataSourceMap = srcFilename }, device.Device9);
#pragma warning restore 612,618
            persister.Save(destMap, destFilename);

            persister.Load(destFilename, device.Device9);
            persister.Load(srcFilename, device.Device9);

            Directory.Delete(srcPath, true);
            Directory.Delete(destPath, true);
        }

        [TestMethod]
        public void MultiSaveLoadTest()
        {
            var map = Map.New(new MapSettings(), device.Device9);
            for (int i = 0; i < 3; i++)
            {
                var tmpPath = Path.GetTempPath() + Guid.NewGuid();
                Directory.CreateDirectory(tmpPath);
                var tmpMap = tmpPath + "/temp.map";
                MapPersistence.Instance.Save(map, tmpMap);
                map = MapPersistence.Instance.Load(tmpMap, device.Device9);
                Directory.Delete(tmpPath, true);
            }
        }

        [TestMethod]
        public void MultiSaveLoadSameFileTest()
        {
            var tmpPath = Path.GetTempPath() + Guid.NewGuid();
            Directory.CreateDirectory(tmpPath);
            var tmpMap = tmpPath + "/temp.map";
            var map = Map.New(new MapSettings(), device.Device9);
            for (var i = 0; i < 3; i++)
            {
                MapPersistence.Instance.Save(map, tmpMap);
                map = MapPersistence.Instance.Load(tmpMap, device.Device9);
            }
            Directory.Delete(tmpPath, true);
        }

    }
}
