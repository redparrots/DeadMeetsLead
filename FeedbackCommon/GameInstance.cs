using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.Odbc;

namespace FeedbackCommon
{
    [Serializable]
    public class GameInstance : Sendable<GameInstance>
    {
        public GameInstance()
        {
            ID = System.Guid.NewGuid();
        }

        public System.Guid ID { get; private set; }
        public Profile Profile { get; set; }
        public string MapName { get; set; }
        public int MapVersion { get; set; }
        public String SenderIP { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public float TimePlayed { get; set; }
        public int Frames { get; set; }
        public string Reason { get; set; }
        public float EndPosX { get; set; }
        public float EndPosY { get; set; }
        public float EndPosZ { get; set; }
        public int Hitpoints { get; set; }
        public int Rage { get; set; }
        public int Ammunition { get; set; }
        public int MeleeWeapon { get; set; }
        public int RangedWeapon { get; set; }
        public string Difficulty { get; set; }
        public string GameVersion { get; set; }
        public Stage HighestStage { get; set; }
        public int HitsTaken { get; set; }
        public int DamageDealt { get; set; }
        public int DamageTaken { get; set; }
        public int TimesNetted { get; set; }
        public int NSlams { get; set; }
        public int GBShots { get; set; }
        public int TotalShots { get; set; }
        public int KilledUnits { get; set; }

        public override string ParameterString
        {
            get { return "gi=" + ToHttpString(); }
        }
    }
}
