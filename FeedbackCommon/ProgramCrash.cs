using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace FeedbackCommon
{
    [Serializable]
    public class ProgramCrash : Sendable<ProgramCrash>
    {
        public int ID { get; set; }
        public int FileID { get; set; }
        public Profile Profile { get; set; }
        public String SenderIP { get; set; }
        public DateTime DateSent { get; set; }
        public string GameVersion { get; set; }
        public string CrashType { get; set; }

        public override string ParameterString
        {
            get { return "pc=" + ToHttpString(); }
        }
    }
}
