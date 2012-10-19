using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeedbackCommon
{
    [Serializable]
    public class Stage
    {
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public int Rage { get; set; }
        public float Time { get; set; }
        public int Ammo { get; set; }
        public int StageNumber { get; set; }
        public int MapStages { get; set; }
    }
}
