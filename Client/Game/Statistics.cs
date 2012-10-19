using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Game
{
    public class Statistics
    {
        public Statistics()
        {
            Actions = new Action();
            CharacterActions = new CharacterAction();
            Kills = new Kill();
            MapUnits = new MapUnit();
        }

        public Action Actions { get; private set; }
        public class Action
        {
            public int HitsTaken { get; set; }
            public int DamageDealt { get; set; }
            public int DamageTaken { get; set; }
            public int TimesNetted { get; set; }
        }

        public CharacterAction CharacterActions { get; private set; }
        public class CharacterAction
        {
            public int TotalShots { get; set; }
            public int ShotgunFired { get; set; }
            public int ShotgunSlugHits { get; set; }
            public int GhostRifleFired { get; set; }
            public int GhostRifleHits { get; set; }
            public int Slams { get; set; }
        }

        public Kill Kills { get; private set; }
        public class Kill
        {
            public int TotalKills { get; set; }      // shouldn't be needed after unit stats are added

            public int Brutes { get; set; }
            public int Bulls { get; set; }
            public int Clerics { get; set; }
            public int Commanders { get; set; }
            public int Ghouls { get; set; }
            public int Grunts { get; set; }
            public int Hounds { get; set; }
            public int Infected { get; set; }
            public int Mongrels { get; set; }
            public int Rotten { get; set; }
        }

        public MapUnit MapUnits { get; private set; }
        public class MapUnit
        {
            public int Brutes { get; set; }
            public int Bulls { get; set; }
            public int Clerics { get; set; }
            public int Commanders { get; set; }
            public int Ghouls { get; set; }
            public int Grunts { get; set; }
            public int Hounds { get; set; }
            public int Infected { get; set; }
            public int Mongrels { get; set; }
            public int Rotten { get; set; }
        }

        /* Not implemented */

        //public int Debug1 { get; set; }
        //public int Debug2 { get; set; }
        //public int Debug3 { get; set; }
        //public int Debug4 { get; set; }
        //public int Debug5 { get; set; }
        //public int Debug6 { get; set; }

        //public int Debug7 { get; set; }
        //public int Debug8 { get; set; }
        //public int Debug9 { get; set; }
    }
}
