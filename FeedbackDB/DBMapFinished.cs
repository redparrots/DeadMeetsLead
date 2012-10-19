using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using FeedbackCommon;

namespace FeedbackDB
{
    [Serializable]
    public class DBMapFeedback : DBObject<MapFeedback>
    {
        public DBMapFeedback(MapFeedback mf) : base(mf) { }

        public override void DBInsert(System.Data.Odbc.OdbcConnection connection)
        {
            OdbcCommand cmd = new OdbcCommand("INSERT INTO MapFeedback (gi, difficultyRating, lengthRating, entertainmentRating, comments) VALUES (?, ?, ?, ?, ?);", connection);
            cmd.Parameters.Add("gi", OdbcType.Binary, 16).Value = Object.GameInstance.ID.ToByteArray();
            cmd.Parameters.Add("difficultyRating", OdbcType.Int).Value = Object.DifficultyRating;
            cmd.Parameters.Add("lengthRating", OdbcType.Int).Value = Object.LengthRating;
            cmd.Parameters.Add("entertainmentRating", OdbcType.Int).Value = Object.EntertainmentRating;
            cmd.Parameters.Add("comments", OdbcType.VarChar, 1000).Value = Object.Comments ?? "";
            PrintCommandString(cmd, 300);
            DBExecute(cmd);
        }

        public static void DBCreateTable(OdbcConnection connection)
        {
            string tableCreationString = @"CREATE TABLE MapFeedback (
                gi BINARY(16),
                difficultyRating INT,
                lengthRating INT,
                entertainmentRating INT,
                comments VARCHAR(1000),
                PRIMARY KEY (gi),
                FOREIGN KEY (gi) REFERENCES GameInstance(id)
            )";
            DBExecute(new OdbcCommand(tableCreationString, connection), true);
        }

        public static void DBDropTable(OdbcConnection connection)
        {
            DBExecute(new OdbcCommand("DROP TABLE IF EXISTS MapFeedback", connection), true);
        }
    }
}
