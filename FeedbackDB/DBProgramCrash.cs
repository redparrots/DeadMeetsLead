using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using FeedbackCommon;

namespace FeedbackDB
{
    [Serializable]
    public class DBProgramCrash : DBObject<ProgramCrash>
    {
        public DBProgramCrash(ProgramCrash pc) : base(pc) { }

        public override void DBInsert(System.Data.Odbc.OdbcConnection connection)
        {
            var profile = Object.Profile;
            OdbcCommand cmd = new OdbcCommand(String.Format(
                "INSERT INTO ProgramCrash (fileID, {0}senderIP, dateSent, dateReceived, gameVersion, crashType) VALUES (?, {1}?, ?, ?, ?, ?);", 
                profile != null ? "profile, " : "",
                profile != null ? "?, " : ""),
                connection);
            cmd.Parameters.Add("fileID", OdbcType.Int).Value = Object.FileID;
            if (profile != null)
                cmd.Parameters.Add("profile", OdbcType.Binary, 16).Value = profile.ID.ToByteArray();
            cmd.Parameters.Add("senderIP", OdbcType.VarChar, 20).Value = Object.SenderIP ?? "Unknown";
            cmd.Parameters.Add("dateSent", OdbcType.DateTime).Value = Object.DateSent;
            cmd.Parameters.Add("dateReceived", OdbcType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("gameVersion", OdbcType.VarChar, 30).Value = Object.GameVersion ?? "Unknown";
            cmd.Parameters.Add("crashType", OdbcType.VarChar, 50).Value = Object.CrashType ?? "Unknown";
            PrintCommandString(cmd, 70);
            Object.ID = DBExecuteReturnAutoID(cmd);
        }

        public static void DBCreateTable(OdbcConnection connection)
        {
            string tableCreationString = @"CREATE TABLE ProgramCrash (
                id INT UNSIGNED AUTO_INCREMENT,
                fileID INT,
                profile BINARY(16),
                senderIP VARCHAR(20),
                dateSent DATETIME,
                dateReceived DATETIME,
                gameVersion VARCHAR(30),
                crashType VARCHAR(50),
                PRIMARY KEY (id),
                FOREIGN KEY (profile) REFERENCES Profile(id)
            )";
            DBExecute(new OdbcCommand(tableCreationString, connection), true);
        }

        public static void DBDropTable(OdbcConnection connection)
        {
            DBExecute(new OdbcCommand("DROP TABLE IF EXISTS ProgramCrash", connection), true);
        }
    }
}
