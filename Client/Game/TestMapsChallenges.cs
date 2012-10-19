using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.ComponentModel;

namespace Client.Game
{
    partial class TestMaps
    {
        #region Easy
        [Description(@"
Rating: Easy
Description: Two zombies
Challenge: Kill them. Don't let the other zombie get behind your back
Misc: This map displays two things: It should be ""fun"" to just kill zombies and if they 
      manage to strike you you should really ""feel"" the strike
")]
        public static Map.Map ChallengeTwoZombies(SlimDX.Direct3D9.Device device)
        {
            var m = Tiny(device);
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Translation = new SlimDX.Vector3(4, 12, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Translation = new SlimDX.Vector3(12, 4, 0) });
            m.MainCharacter.Translation = new Vector3(8, 8, 0);
            m.MainCharacter.PistolAmmo = 0;
            return m;
        }
        #endregion



        #region Medium
        [Description(@"
Rating: Medium
Description: Zombies advancing towards you, but also a cleric to deal with
Challenge: Kill the cleric to stop him from spawning zombies
")]
        public static Map.Map ChallengeACleric(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            var center = new Vector3(m.Settings.Size.Width / 2f, m.Settings.Size.Height / 2f, 0);
            m.MainCharacter.Translation = center + new Vector3(20, 20, 0);
            m.MainCharacter.PistolAmmo = 20;

            var r = new Common.Bounding.Region();
            r.Nodes = new Common.Bounding.RegionNode[1];
            r.Nodes[0] = new Common.Bounding.RegionNode(new Vector3[]
            {
                center + new Vector3(-5, -5, 0),
                center + new Vector3(-5, +5, 0),
                center + new Vector3(5, 5, 0),
                center + new Vector3(5, -5, 0),
            });
            m.Regions.Add(new Map.Region
            {
                Name = "test",
                BoundingRegion = r
            });
            throw new NotImplementedException();
            /*m.DynamicsRoot.AddChild(new Map.SpawningPoint
            {
                SpawnCount = 1,
                Region = "test",
                SpawnType = "Cleric",
                Period = 0,
                Translation = center + new Vector3(-15, -15, 0),
                InitDelay = 5
            });
            m.DynamicsRoot.AddChild(new Map.SpawningPoint
            {
                SpawnCount = 20,
                Region = "test",
                SpawnType = "Grunt",
                Period = 3,
                Translation = center + new Vector3(10, -10, 0),
                InitDelay = 0
            });
            m.DynamicsRoot.AddChild(new Map.SpawningPoint
            {
                SpawnCount = 20,
                Region = "test",
                SpawnType = "Grunt",
                Period = 3,
                Translation = center + new Vector3(10, 10, 0),
                InitDelay = 0
            });
            m.DynamicsRoot.AddChild(new Map.SpawningPoint
            {
                SpawnCount = 20,
                Region = "test",
                SpawnType = "Grunt",
                Period = 3,
                Translation = center + new Vector3(-10, 10, 0),
                InitDelay = 0
            });*/

            for (int y = -3; y < 3; y++)
                for (int x = -3; x < 3; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt
                    {
                        Translation = center + new SlimDX.Vector3(x * 3, y * 3, 0),
                        State = Map.UnitState.RaisableCorpse
                    });

            return m;
        }

        [Description(@"
Rating: Medium
Description: A single bull
Challenge: Kill the bull without getting to close to him
")]
        public static Map.Map ChallengeSingleBull(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            m.MainCharacter.Translation = new Vector3(25, 25, 0);
            m.MainCharacter.PistolAmmo = 10;
            m.DynamicsRoot.AddChild(new Map.Units.Bull
            {
                Translation = new SlimDX.Vector3(20, 20, 0),
            });
            return m;
        }
        #endregion



        #region Hard
        [Description(@"
Rating: Hard
Description: First you fight some zombies advancing towards you, then mongrels spawn
Challenge: Mongrels are quick and cannot get too close to you or you will die
")]
        public static Map.Map ChallengeSuddenlyMongrels(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            m.MainCharacter.Translation = new Vector3(m.Settings.Size.Width/2f, m.Settings.Size.Height/2f, 0);
            m.MainCharacter.PistolAmmo = 20;

            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Translation = m.MainCharacter.Translation - new SlimDX.Vector3(5, 0, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Translation = m.MainCharacter.Translation + new SlimDX.Vector3(5, 0, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Translation = m.MainCharacter.Translation - new SlimDX.Vector3(0, 5, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Grunt { Translation = m.MainCharacter.Translation + new SlimDX.Vector3(0, 5, 0) });

            var r = new Common.Bounding.Region();
            r.Nodes = new Common.Bounding.RegionNode[1];
            r.Nodes[0] = new Common.Bounding.RegionNode(new Vector3[]
            {
                m.MainCharacter.Translation + new Vector3(-10, -10, 0), 
                m.MainCharacter.Translation + new Vector3(-10, +10, 0),
                m.MainCharacter.Translation + new Vector3(10, 10, 0),
                m.MainCharacter.Translation + new Vector3(10, -10, 0),
            });
            m.Regions.Add(new Map.Region
            {
                Name = "test",
                BoundingRegion = r
            });

            throw new NotImplementedException();
            /*
            m.DynamicsRoot.AddChild(new Map.SpawningPoint
            {
                SpawnCount = 2,
                Region = "test",
                SpawnType = "Mongrel",
                Period = 1,
                Translation = m.MainCharacter.Translation + new Vector3(15, -15, 0),
                InitDelay = 1
            });
            m.DynamicsRoot.AddChild(new Map.SpawningPoint
            {
                SpawnCount = 2,
                Region = "test",
                SpawnType = "Mongrel",
                Period = 1,
                Translation = m.MainCharacter.Translation + new Vector3(-15, -15, 0),
                InitDelay = 1
            });
            m.DynamicsRoot.AddChild(new Map.SpawningPoint
            {
                SpawnCount = 2,
                Region = "test",
                SpawnType = "Mongrel",
                Period = 1,
                Translation = m.MainCharacter.Translation + new Vector3(-15, 15, 0),
                InitDelay = 1
            });*/

            return m;
        }
        #endregion



        #region Unrated
        [Description(@"
Rating: UNRATED
Description: A large group of zombies.
Challenge: Don't let them get around you to your back. While they are in front of you you kill them faster than they attack you.
Misc: The sword primary attack is designed to kill a zombie a little quicker than he attacks, so that you can kill him before he strikes you.
")]
        public static Map.Map ChallengeZombies(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt { Translation = new SlimDX.Vector3(22 + x * 3, 22 + y * 3, 0) });
            m.MainCharacter.PistolAmmo = 0;
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: A large group of zombies with some commanders
Challenge: Same as zombies, but you also have to kill the commanders before they work up the frenzy of the zombies.
")]
        public static Map.Map ChallengeZombiesWithCommanders(SlimDX.Direct3D9.Device device)
        {
            var m = ChallengeZombies(device);
            m.DynamicsRoot.AddChild(new Map.Units.Commander { Translation = new SlimDX.Vector3(31, 31, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Commander { Translation = new SlimDX.Vector3(26, 31, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Commander { Translation = new SlimDX.Vector3(31, 26, 0) });
            m.MainCharacter.PistolAmmo = 0;
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: Zombies storm you from all directions.
Challenge: Same as zombies, but with added difficulty since they come from all directions.
")]
        public static Map.Map ChallengeZombieInvasion(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            m.MainCharacter.Position = new Vector3(m.Settings.Size.Width / 2f, m.Settings.Size.Width / 2f, 0);
            m.MainCharacter.PistolAmmo = 99999;
            List<Map.Point> spawningPoints = new List<Client.Game.Map.Point>();
            spawningPoints.Add(new Map.Point { Name = "Point1", Position = m.MainCharacter.Position + new Vector3(-10, 0, 0) });
            spawningPoints.Add(new Map.Point { Name = "Point2", Position = m.MainCharacter.Position + new Vector3(10, 0, 0) });
            spawningPoints.Add(new Map.Point { Name = "Point3", Position = m.MainCharacter.Position + new Vector3(0, -10, 0) });
            spawningPoints.Add(new Map.Point { Name = "Point4", Position = m.MainCharacter.Position + new Vector3(0, 10, 0) });
            foreach (var v in spawningPoints) m.DynamicsRoot.AddChild(v);
            
            m.Scripts.Add(new Client.Game.Map.SpawnScript
            {
                InitDelay = 0,
                TickPeriod = 4,
                SpawnCount = int.MaxValue,
                SpawnType = "Grunt",
                Point = "Point1"
            });
            m.Scripts.Add(new Client.Game.Map.SpawnScript
            {
                InitDelay = 1,
                TickPeriod = 4,
                SpawnCount = int.MaxValue,
                SpawnType = "Grunt",
                Point = "Point2"
            });
            m.Scripts.Add(new Client.Game.Map.SpawnScript
            {
                InitDelay = 2,
                TickPeriod = 4,
                SpawnCount = int.MaxValue,
                SpawnType = "Grunt",
                Point = "Point3"
            });
            m.Scripts.Add(new Client.Game.Map.SpawnScript
            {
                InitDelay = 3,
                TickPeriod = 4,
                SpawnCount = int.MaxValue,
                SpawnType = "Grunt",
                Point = "Point4"
            });
            
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: A cleric and piles of corpses which he raises to zombies
Challenge:
    * Avoid the scourged earth ability of the cleric
    * Avoid the fire breath ability of the cleric
    * Kill and avoid the zombies he raises
    * Kill the cleric
")]
        public static Map.Map ChallengeCleric(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt
                    {
                        Translation = new SlimDX.Vector3(22 + x * 3, 22 + y * 3, 0),
                        State = Map.UnitState.Dead
                    });
            m.DynamicsRoot.AddChild(new Map.Units.Cleric { Translation = new SlimDX.Vector3(31, 31, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Cleric { Translation = new SlimDX.Vector3(26, 31, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Cleric { Translation = new SlimDX.Vector3(31, 26, 0) });
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: A group of mongrels, kill them before they reach you.
Challenge: Use the shotgun to take out the mongrels before they get a chance to hit you
")]
        public static Map.Map ChallengeMongrels(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Mongrel
                    {
                        Translation = new SlimDX.Vector3(22 + x * 3, 22 + y * 3, 0)
                    });
            m.MainCharacter.PistolAmmo = 10;
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: A group of infected, kill them before they reach you.
Challenge: Use the shotgun to take out the infected before they get a chance to hit you. 
The Infected are slower but has got more hp than the mongrels, in addition their bite is infected and thus slowly kills you if you are bitten.
")]
        public static Map.Map ChallengeInfected(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Infected
                    {
                        Translation = new SlimDX.Vector3(22 + x * 3, 22 + y * 3, 0)
                    });
            m.MainCharacter.PistolAmmo = 30;
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: A group of zombies surround a bull.
Challenge:
    * Kill the zombies and the bull
    * Avoid being hit by the bull by staying behind him
    * Avoid being charged by the bull
")]
        public static Map.Map ChallengeBulls(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt
                    {
                        Translation = new SlimDX.Vector3(28 + x * 3, 28 + y * 3, 0)
                    });
            m.DynamicsRoot.AddChild(new Map.Units.Bull
            {
                Translation = new SlimDX.Vector3(35, 35, 0)
            });
            m.MainCharacter.PistolAmmo = 0;
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: A group of zombies form a wall which hinders you from reaching the clerics behind, who meanwhile summon more and more zombies.
Challenge:
    * Take out the clerics while fighting the zombies
")]
        public static Map.Map ChallengeGetToDaClerics(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);

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

            e.Draw(new Vector2(25, 40), new Graphics.Editors.GroundTexturePencil
            {
                Radius = 7,
                Color = new Vector4(50, 0, 0, 0),
                Type = Graphics.Editors.GroundTexturePencilType.Add
            });
            e.Draw(new Vector2(40, 25), new Graphics.Editors.GroundTexturePencil
            {
                Radius = 7,
                Color = new Vector4(50, 0, 0, 0),
                Type = Graphics.Editors.GroundTexturePencilType.Add
            });

            for (int y = 0; y < 25; y++)
                m.DynamicsRoot.AddChild(new Map.Units.Grunt
                {
                    Translation = new SlimDX.Vector3(28, 28, 0),
                    State = Map.UnitState.RaisableCorpse
                });

            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt
                    {
                        Translation = new SlimDX.Vector3(22 + x * 3, 22 + y * 3, 0),
                        State = Map.UnitState.Alive
                    });
            m.DynamicsRoot.AddChild(new Map.Units.Cleric { Translation = new SlimDX.Vector3(25, 25, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Cleric { Translation = new SlimDX.Vector3(25, 25, 0) });
            m.DynamicsRoot.AddChild(new Map.Units.Cleric { Translation = new SlimDX.Vector3(25, 25, 0) });
            foreach (var u in m.DynamicsRoot.Offspring)
                if (u is Map.Unit)
                    u.Translation = m.Ground.GetHeight(u.Translation) + Vector3.UnitZ * 0.1f;
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: A group of zombies and a cleric with a raisable bull behind. Fight the zombies and stop the 
cleric from raising the bull at the same time.
Challenge:
    * Stop the cleric from summoning the bull before the summoning is complete
")]
        public static Map.Map ChallengeClericAndBull(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);

            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt
                    {
                        Translation = new SlimDX.Vector3(22 + x * 3, 22 + y * 3, 0),
                        State = Map.UnitState.Alive
                    });
            m.DynamicsRoot.AddChild(new Map.Units.Cleric { Translation = new SlimDX.Vector3(25, 25, 0) });
            for (int i = 0; i < 10; i++)
                m.DynamicsRoot.AddChild(new Map.Units.Bull
                {
                    Translation = new SlimDX.Vector3(25, 25, 0),
                    State = Map.UnitState.RaisableCorpse
                });
            return m;
        }
        [Description(@"
Rating: UNRATED
Description: A group of zombies and a cleric on an inaccessible position.
Challenge:
    * Kill the cleric with a ranged weapon.
")]
        public static Map.Map ChallengeInaccessibleCleric(SlimDX.Direct3D9.Device device)
        {
            var m = Medium(device);

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

            e.Draw(new Vector2(22, 22), new Graphics.Editors.GroundTexturePencil
            {
                Radius = 7,
                Color = new Vector4(-1, 0, 0, 0),
                Type = Graphics.Editors.GroundTexturePencilType.Add
            });
            e.Draw(new Vector2(22, 22), new Graphics.Editors.GroundTexturePencil
            {
                Radius = 5,
                Color = new Vector4(1, 0, 0, 0),
                Type = Graphics.Editors.GroundTexturePencilType.Add
            });

            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 5; x++)
                    m.DynamicsRoot.AddChild(new Map.Units.Grunt
                    {
                        Translation = new SlimDX.Vector3(25 + x * 2, 25 + y * 2, 0),
                        State = Map.UnitState.Alive
                    });

            m.DynamicsRoot.AddChild(new Map.Units.Cleric { Translation = new SlimDX.Vector3(20, 20, 0) });
            foreach (var u in m.DynamicsRoot.Offspring)
                if (u is Map.Unit)
                    u.Translation = m.Ground.GetHeight(u.Translation) + Vector3.UnitZ * 0.1f;
            return m;
        }
        #endregion
    }
}
