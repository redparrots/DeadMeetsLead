using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace FeedbackCommon
{
    [Serializable]
    public class Profile : Sendable<Profile>
    {
        public Profile() { ID = System.Guid.NewGuid(); }
        public System.Guid ID { get; private set; }
        public string EMail { get; set; }
        public string Name { get; set; }
        public string HWID { get; set; }
        public ProfileType Type { get; set; }

        public override string ParameterString
        {
            get { return "p=" + ToHttpString(); }
        }
    }

    [Flags]
    public enum ProfileType
    {
        Normal = 0,
        Developer = 1,
        InHouseTester = 2,
        Trash = 3,

        PressPreview = 50,

        BetaTesterUnknown = 99,
        BetaTester00300 = 100,
        BetaTester00301 = 101,
        BetaTester00302 = 102,
        BetaTester00303 = 103,

        ChallengeAlpha = 200,
        ChallengeBeta = 210,
        Challenge = 220,

        FullGame = 300,
        FullGameTesting = 310,
    }
}
