using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.ComponentModel;

namespace Client.Game
{
    static partial class TestMaps
    {
        #region Base maps
        public static Map.Map Tiny(SlimDX.Direct3D9.Device device)
        {
            var m = Map.Map.New(new Client.Game.Map.MapSettings { 
                Size = new System.Drawing.SizeF(16, 16),
                Name = "Tiny"
            }, device);
            m.MainCharacter.Position = new SlimDX.Vector3(10, 5, 0);
            m.MainCharacter.PistolAmmo = 1000000;
            return m;
        }

        public static Map.Map Medium(SlimDX.Direct3D9.Device device)
        {
            var m = Map.Map.New(new Client.Game.Map.MapSettings
            {
                Size = new System.Drawing.SizeF(50, 50)
            }, device);
            m.MainCharacter.Position = new SlimDX.Vector3(45, 45, 0);
            m.MainCharacter.PistolAmmo = 1000000;
            return m;
        }
        public static Map.Map Large(SlimDX.Direct3D9.Device device)
        {
            var m = Map.Map.New(new Client.Game.Map.MapSettings
            {
                Size = new System.Drawing.SizeF(300, 300),
            }, device);
            m.MainCharacter.Position = new SlimDX.Vector3(200, 200, 0);
            m.MainCharacter.PistolAmmo = 1000000;
            return m;
        }

        public static Map.Map TinySparseRandom(SlimDX.Direct3D9.Device device)
        {
            return Random(device, new System.Drawing.SizeF(20, 20), 50, 50, 50);
        }
        public static Map.Map TinyNormalRandom(SlimDX.Direct3D9.Device device)
        {
            return Random(device, new System.Drawing.SizeF(20, 20), 50, 50, 400);
        }
        public static Map.Map TinyDenseRandom(SlimDX.Direct3D9.Device device)
        {
            return Random(device, new System.Drawing.SizeF(20, 20), 50, 50, 4000);
        }

        public static Map.Map MediumSparseRandom(SlimDX.Direct3D9.Device device)
        {
            return Random(device, new System.Drawing.SizeF(50, 50), 1000, 200, 100);
        }
        public static Map.Map NormalNormalRandom(SlimDX.Direct3D9.Device device)
        {
            return Random(device, new System.Drawing.SizeF(300, 300), 10000, 2000, 90000);
        }
        public static Map.Map NormalDenseRandom(SlimDX.Direct3D9.Device device)
        {
            return Random(device, new System.Drawing.SizeF(300, 300), 10000, 2000, 900000);
        }

        public static Map.Map LargeRandom(SlimDX.Direct3D9.Device device)
        {
            return Random(device, new System.Drawing.SizeF(1000, 1000), 10000, 2000, 10000);
        }

        static Map.Map Random(SlimDX.Direct3D9.Device device, System.Drawing.SizeF size, int nHeightIncs, int nSplatDraws, int nProps)
        {
            var m = Map.Map.New(new Client.Game.Map.MapSettings
            {
                Size = size
            }, device);
            m.MainCharacter.Position = new SlimDX.Vector3(m.Settings.Size.Width/2f, m.Settings.Size.Height/2f, 0);

            Random r = new Random();

            // Heightmap
            Graphics.Editors.GroundTextureEditor e = new Graphics.Editors.GroundTextureEditor
            {
                Size = m.Settings.Size,
                SoftwareTexture = new Graphics.Software.Texture<Graphics.Software.Texel.R32F>[] { new Graphics.Software.Texture<Graphics.Software.Texel.R32F>(m.Ground.Heightmap) },
                Pencil = new Graphics.Editors.GroundTexturePencil
                {
                    Color = new SlimDX.Vector4(1, 0, 0, 0),
                    Radius = 5,
                    Type = Graphics.Editors.GroundTexturePencilType.Add
                }
            };
            e.TextureValuesChanged += new Graphics.Editors.TextureValuesChangedEventHandler(
                (o, args) => m.Ground.UpdatePieceMeshes(args.ChangedRegion));
            for (int i = 0; i < nHeightIncs; i++)
            {
                var p = new Graphics.Editors.GroundTexturePencil
                {
                    Radius = 1 + (float)(r.NextDouble()*10),
                    Color = new SlimDX.Vector4((float)(r.NextDouble() * 2 - 1), 0, 0, 0),
                    Type = Graphics.Editors.GroundTexturePencilType.Add
                };
                e.Draw(
                    new SlimDX.Vector2((float)(r.NextDouble() * m.Settings.Size.Width),
                        (float)(r.NextDouble() * m.Settings.Size.Height)), 
                        p);
            }

            // Splatmap
            e = new Graphics.Editors.GroundTextureEditor
            {
                Size = m.Settings.Size,
                SoftwareTexture = new Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8>[] {
                    new Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8>(
                        Graphics.TextureUtil.ReadTexture<Graphics.Software.Texel.A8R8G8B8>(m.Ground.SplatMap1.Resource9, 0))
                },
                Texture9 = new SlimDX.Direct3D9.Texture[] { m.Ground.SplatMap1.Resource9 },
                Pencil = new Graphics.Editors.GroundTexturePencil
                {
                    Color = new SlimDX.Vector4(1, 0, 0, 0),
                    Radius = 5,
                    Type = Graphics.Editors.GroundTexturePencilType.Add
                }
            };
            for (int i = 0; i < nSplatDraws; i++)
            {
                SlimDX.Vector4 v = Vector4.Zero;
                float d = (float)r.NextDouble();
                switch (r.Next(5))
                {
                    case 0:
                        v = new Vector4(d, -d, -d, -d);
                        break;
                    case 1:
                        v = new Vector4(-d, d, -d, -d);
                        break;
                    case 2:
                        v = new Vector4(-d, -d, d, -d);
                        break;
                    case 3:
                        v = new Vector4(-d, -d, -d, d);
                        break;
                    case 4:
                        v = new Vector4(-d, -d, -d, -d);
                        break;
                }
                var p = new Graphics.Editors.GroundTexturePencil
                {
                    Radius = 1 + (float)(r.NextDouble() * 10),
                    Color = v, 
                    Type = Graphics.Editors.GroundTexturePencilType.AddSaturate
                };
                e.Draw(
                    new SlimDX.Vector2((float)(r.NextDouble() * m.Settings.Size.Width),
                        (float)(r.NextDouble() * m.Settings.Size.Height)), 
                        p);
            }

            // Props
            List<Type> propTypes = new List<Type>();
            foreach (var v in typeof(Map.Props.Prop).Assembly.GetTypes())
                if (typeof(Map.Props.Prop).IsAssignableFrom(v))
                    propTypes.Add(v);
            propTypes.Remove(typeof(Map.Props.Prop));
            for (int i = 0; i < nProps; i++)
            {
                var p = (Map.Props.Prop)Activator.CreateInstance(propTypes[r.Next(propTypes.Count)]);
                p.Position = new SlimDX.Vector3(
                    (float)(r.NextDouble() * m.Settings.Size.Width), 
                    (float)(r.NextDouble() * m.Settings.Size.Height), 0);

                float rotation = (float)(2 * Math.PI * r.NextDouble());
                p.Rotation = Quaternion.RotationAxis(Vector3.UnitZ, rotation);

                p.Position = m.Ground.GetHeight(p.Position);
                m.DynamicsRoot.AddChild(p);
            }

            m.MainCharacter.Position = m.Ground.GetHeight(m.MainCharacter.Position);

            return m;
        }
        public static Map.Map MediumSparseRandomWithUnits(SlimDX.Direct3D9.Device device)
        {
            var m = MediumSparseRandom(device);
            Random r = new Random();

            int nUnits = 100;

            // Units
            List<Type> propTypes = new List<Type>();
            foreach (var v in typeof(Map.Unit).Assembly.GetTypes())
                if (typeof(Map.Unit).IsAssignableFrom(v))
                    propTypes.Add(v);
            propTypes.Remove(typeof(Map.Unit));
            propTypes.Remove(typeof(Map.Units.MainCharacter));
            for (int i = 0; i < nUnits; i++)
            {
                var p = (Map.Unit)Activator.CreateInstance(propTypes[r.Next(propTypes.Count)]);
                p.Position = new SlimDX.Vector3(
                    (float)(r.NextDouble() * m.Settings.Size.Width),
                    (float)(r.NextDouble() * m.Settings.Size.Height), 0);
                p.Position = m.Ground.GetHeight(p.Position);
                m.DynamicsRoot.AddChild(p);
            }
            return m;
        }

        #endregion

        #region Zombies

        public static Map.Map SingleGrunt(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Grunt
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            return m;
        }
        public static Map.Map SingleInactiveGrunt(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            Map.Units.Grunt g;
            m.DynamicsRoot.AddChild(g = new Map.Units.Grunt
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            g.ClearAbilities();
            return m;
        }
        public static Map.Map SingleRaisableGrunt(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            Map.Units.Grunt g;
            m.DynamicsRoot.AddChild(g = new Map.Units.Grunt
            {
                Position = new SlimDX.Vector3(2, 8, 0),
                State = Map.UnitState.RaisableCorpse
            });
            return m;
        }
        public static Map.Map SingleDeadGrunt(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            Map.Units.Grunt g;
            m.DynamicsRoot.AddChild(g = new Map.Units.Grunt
            {
                Position = new SlimDX.Vector3(2, 8, 0),
                State = Map.UnitState.Dead
            });
            return m;
        }
        public static Map.Map SingleBrute(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Brute
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            return m;
        }
        public static Map.Map SingleCommander(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Commander
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            return m;
        }
        public static Map.Map SingleDemonLord(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            m.DynamicsRoot.AddChild(new Map.Units.DemonLord
            {
                Position = new SlimDX.Vector3(32, 32, 0),
            });
            return m;
        }
        public static Map.Map SingleWolfBoss(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            var w = new Map.Units.WolfBoss
            {
                Position = new SlimDX.Vector3(32, 32, 0),
            };
            w.EditorInit();
            m.DynamicsRoot.AddChild(w);
            return m;
        }

        public static Map.Map GruntAndCommander(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Grunt
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            m.DynamicsRoot.AddChild(new Map.Units.Commander
            {
                Position = new SlimDX.Vector3(2, 2, 0),
            });
            return m;
        }

        #endregion

        #region Voodoo priests
        public static Map.Map SingleCleric(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Cleric
            {
                Position = new SlimDX.Vector3(5, 10, 0),
            });
            return m;
        }
        public static Map.Map HarmlessCleric(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            Map.Units.Cleric c = new Map.Units.Cleric
            {
                Position = new SlimDX.Vector3(5, 10, 0),
            };
            c.ClearAbilities();
            c.AddAbility(new Map.Units.FireBreath
            {
                Damage = 0
            });
            m.DynamicsRoot.AddChild(c);
            return m;
        }
        public static Map.Map ClericRaise(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Cleric
            {
                Position = new SlimDX.Vector3(5, 10, 0),
            });
            m.DynamicsRoot.AddChild(new Map.Units.Grunt
            {
                Position = new SlimDX.Vector3(4, 9, 0),
                State = Map.UnitState.Dead
            });
            return m;
        }
        public static Map.Map ClericSlowRaise(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Cleric
            {
                Position = new SlimDX.Vector3(5, 10, 0),
            });
            m.DynamicsRoot.AddChild(new Map.Units.Grunt
            {
                Position = new SlimDX.Vector3(4, 9, 0),
                State = Map.UnitState.RaisableCorpse,
                RaiseFromCorpseTime = 100000
            });
            return m;
        }
        #endregion

        #region Ghouls
        public static Map.Map SingleMongrel(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Mongrel
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            return m;
        }
        public static Map.Map SingleInfected(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Infected
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            return m;
        }
        public static Map.Map SingleHound(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Hound
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            return m;
        }
        #endregion

        #region Bulls
        public static Map.Map SingleBull(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Bull
            {
                Position = new SlimDX.Vector3(2, 8, 0),
            });
            return m;
        }
        #endregion

        #region Tests
        public static Map.Map ChainPull(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Position = new SlimDX.Vector3(36, 36, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Position = new SlimDX.Vector3(31, 31, 0) });
            return m;
        }
        public static Map.Map ChainPullRaisable(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Position = new SlimDX.Vector3(36, 36, 0), State = Client.Game.Map.UnitState.RaisableCorpse });
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Position = new SlimDX.Vector3(31, 31, 0), State = Client.Game.Map.UnitState.RaisableCorpse });
            return m;
        }
        public static Map.Map PackOfImmobileZombies(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                {
                    Map.Units.Grunt g;
                    m.DynamicsRoot.AddChild(g = new Map.Units.Grunt { Position = new SlimDX.Vector3(37 + x * 1, 37 + y * 1, 0) });
                    g.ClearAbilities();
                }
            m.MainCharacter.PistolAmmo = 100;
            return m;
        }
        public static Map.Map PackOfStonesZombies(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                {
                    Map.Units.Grunt g;
                    m.DynamicsRoot.AddChild(g = new Map.Units.Grunt
                    {
                        Position = new SlimDX.Vector3(37 + x * 1, 37 + y * 1, 0),
                        HitPoints = int.MaxValue,
                        MaxHitPoints = int.MaxValue
                    });
                    g.ClearAbilities();
                }
            m.MainCharacter.PistolAmmo = 100;
            return m;
        }
        public static Map.Map PackOfEndlessZombies(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            Action<int,int> insertGrunt = null;
            insertGrunt = (int x, int y) =>
            {
                Map.Units.Grunt g;
                m.DynamicsRoot.AddChild(g = new Map.Units.Grunt
                {
                    Position = new SlimDX.Vector3(37 + x * 1, 37 + y * 1, 0),
                });
                g.ClearAbilities();
                g.Killed += new Client.Game.Map.Destructible.KilledEventHandler((o, e, s) =>
                {
                    insertGrunt(x, y);
                    if (!g.IsRemoved)
                        g.Remove();
                });
            };
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                {
                    insertGrunt(x, y);
                }
            m.MainCharacter.PistolAmmo = 100;
            return m;
        }
        static Map.Map SpreadOutZombies(SlimDX.Direct3D9.Device device, int count)
        {
            var m = Large(device);
            Random r = new Random();
            for (int y = 0; y < count; y++)
                m.DynamicsRoot.AddChild(new Map.Units.Grunt
                {
                    Position =
                        new SlimDX.Vector3(
                        (float)(r.NextDouble() * m.Settings.Size.Width),
                        (float)(r.NextDouble() * m.Settings.Size.Height),
                        0)
                });
            m.MainCharacter.PistolAmmo = 100;
            return m;
        }
        public static Map.Map TenSpreadOutZombies(SlimDX.Direct3D9.Device device)
        {
            return SpreadOutZombies(device, 10);
        }
        public static Map.Map OneHundredSpreadOutZombies(SlimDX.Direct3D9.Device device)
        {
            return SpreadOutZombies(device, 100);
        }
        public static Map.Map OneThousandSpreadOutZombies(SlimDX.Direct3D9.Device device)
        {
            return SpreadOutZombies(device, 1000);
        }
        public static Map.Map OneHundredZombiesInGroup(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt { Position = new SlimDX.Vector3(15 + x, 15 + y, 0) });
            return m;
        }
        public static Map.Map OneHundredBoxZombiesInGroup(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.BoxGrunt { Position = new SlimDX.Vector3(15 + x, 15 + y, 0) });
            return m;
        }
        public static Map.Map OneThousandZombiesInGroup(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 30; y++)
                for (int x = 0; x < 30; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt { Position = new SlimDX.Vector3(15 + x, 15 + y, 0) });
            return m;
        }
        public static Map.Map RegionSpawnPointTest(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            var r = new Common.Bounding.Region();
            r.Nodes = new Common.Bounding.RegionNode[1];
            r.Nodes[0] = new Common.Bounding.RegionNode(new Vector3[]
            {
                new Vector3(30, 30, 0), new Vector3(30, 40, 0),
                new Vector3(40, 40, 0), new Vector3(40, 30, 0),
            });
            m.Regions.Add(new Map.Region { Name = "test", BoundingRegion = r });
            throw new NotImplementedException();
            /*m.DynamicsRoot.AddChild(new Map.SpawningPoint
            {
                SpawnCount = 10,
                Region = "test",
                SpawnType = "Grunt",
                Period = 1,
                Position = new Vector3(35, 35, 0)
            });*/
            return m;
        }
        public static Map.Map DestructibleTest(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Props.Rottentree1
            {
                Position = new SlimDX.Vector3(7, 7, 0),
            });
            return m;
        }
        public static Map.Map AmmoBoxesTest(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.MainCharacter.PistolAmmo = 0;
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.AmmoBox
                    {
                        Position = new SlimDX.Vector3(3 + x, 3 + y, 0)
                    });
            return m;
        }
        public static Map.Map TalismansTest(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Talisman
                    {
                        Position = new SlimDX.Vector3(3 + x, 3 + y, 0)
                    });
            return m;
        }
        public static Map.Map InformationPopupTest(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.Scripts.Add(new Client.Game.Map.InformationPopupScript { Text = "Information!", InitDelay = 2 });
            return m;
        }
        public static Map.Map SomeBrutes(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Brute
                    {
                        Position = new SlimDX.Vector3(30 + x, 30 + y, 0),
                    });
            return m;
        }
        public static Map.Map SomeBrutesAndCommanders(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 2; y++)
                for (int x = 0; x < 2; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Commander
                    {
                        Position = new SlimDX.Vector3(30 + x, 30 + y, 0),
                    });
            for (int y = 3; y < 5; y++)
                for (int x = 3; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Brute
                    {
                        Position = new SlimDX.Vector3(30 + x, 30 + y, 0),
                    });
            return m;
        }

        public static Map.Map ManyDestructiblesTest(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            for (int y = 0; y < 18; y++)
                for (int x = 0; x < 18; x++)
                    m.DynamicsRoot.AddChild(new Map.Props.Stone3
                    {
                        Position = new SlimDX.Vector3(3 + x*0.2f, 3 + y*0.2f, 0),
                        IsDestructible = true,
                        Scale = new Vector3(0.1f, 0.1f, 0.1f),
                        PhysicsLocalBounding = null,
                        DisplayHPBar = false,
                        HitPoints = 1,
                        InRangeRadius = 0
                    });
            return m;
        }
        #endregion

        #region Path test

        //public static Map.Map BasicPathTest(SlimDX.Direct3D9.Device device)
        //{
        //    var m = Map.Map.Load("Maps/pathtest.map", device);
        //    m.MainCharacter.Position = new SlimDX.Vector3(m.Settings.Size.Width/2f, m.Settings.Size.Height/2f, 0);
        //    m.DynamicsRoot.AddChild(new Map.Units.Grunt { Position = new Vector3(38, 50, 0) });
        //    throw new NotImplementedException();
        //    /*m.Script = (_, manager) =>
        //        {
        //            foreach (var o in Game.Instance.Mechanics.MotionSimulation.All)
        //            {
        //                if (o is Common.Motion.NPC)
        //                {
        //                    ((Common.Motion.NPC)o).NavMesh = m.NavMesh;
        //                    ((Common.Motion.NPC)o).Pursue(m.MainCharacter.MotionUnit, 1f);
        //                }
        //            }
        //        };*/
        //    return m;
        //}

        #endregion

    }
}
