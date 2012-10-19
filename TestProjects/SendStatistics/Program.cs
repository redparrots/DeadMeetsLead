using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using FeedbackCommon;
using System.Runtime.Serialization.Formatters.Binary;

namespace SendStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = String.Format("http://localhost:{0}/ReceiveStatistics.aspx", port);

            gameInstanceTest.HttpPost(uri);
            Console.WriteLine("Sleeping...");
            System.Threading.Thread.Sleep(10000);
        }
        private static int port = 50807;

        public static string Parameterize(string key, string value)
        {
            return String.Format("{0}={1}", key, value);
        }

        private static Profile profileTest = new Profile { Name = "SendProfile" };

        private static GameInstance gameInstanceTest = new GameInstance
        {
            Profile = profileTest,
            MapName = "map0",
            MapVersion = 3,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now + new TimeSpan(1, 0, 0),
            TimePlayed = 1975f,
            Frames = 1337,
            Reason = "Finished map",
            EndPosX = 17.253512f,
            EndPosY = -125.512510f,
            EndPosZ = 5.5192f,
            Hitpoints = 74,
            Rage = 274,
            Ammunition = 13,
            MeleeWeapon = 1,
            RangedWeapon = 4
        };

        private static ProgramCrash programCrashTest = new ProgramCrash
        {
            //DateSent = DateTime.Now,
            
        };

        private static MapFeedback mapFeedback = new MapFeedback
        {
            GameInstance = gameInstanceTest,
            DifficultyRating = 3
        };
    }
}
