using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace FeedbackCommon
{
    [Serializable]
    public class MapFeedback : Sendable<MapFeedback>
    {
        public GameInstance GameInstance { get; set; }

        public int DifficultyRating { get; set; }
        public int LengthRating { get; set; }
        public int EntertainmentRating { get; set; }
        public String Comments { get; set; }

        public override string ParameterString
        {
            get { return "mf=" + ToHttpString(); }
        }
    }
}
