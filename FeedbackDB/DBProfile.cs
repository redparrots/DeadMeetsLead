using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using FeedbackCommon;

namespace FeedbackDB
{
    [Serializable]
    public class DBProfile : DBObject<Profile>
    {
        public DBProfile(Profile p) : base(p) { }

        public override void DBInsert(OdbcConnection connection)
        {
            DBInsert(connection, false);
        }

        public void DBInsert(OdbcConnection connection, bool ignoreIfExists)
        {
            string query = String.Format("INSERT INTO Profile (id, email, name, hwid, productKey, type, creationDate) VALUES(?, ?, ?, ?, ?, ?, ?){0};", ignoreIfExists ? " ON DUPLICATE KEY UPDATE id=id" : "");
            OdbcCommand cmd = new OdbcCommand(query, connection);
            cmd.Parameters.Add("id", OdbcType.Binary, 16).Value = Object.ID.ToByteArray();
            cmd.Parameters.Add("email", OdbcType.VarChar, 100).Value = Object.EMail ?? "Not specified";
            cmd.Parameters.Add("name", OdbcType.VarChar, 50).Value = Object.Name ?? "ERR: Not specified";
            cmd.Parameters.Add("hwid", OdbcType.VarChar, 20).Value = Object.HWID ?? "None";
            cmd.Parameters.Add("productKey", OdbcType.Char, 36).Value = "Not specified";
            cmd.Parameters.Add("type", OdbcType.Int).Value = Object.Type;
            cmd.Parameters.Add("creationDate", OdbcType.DateTime).Value = DateTime.Now;
            PrintCommandString(cmd, 70);
            DBExecute(cmd);
        }

        public static void DBCreateTable(OdbcConnection connection)
        {
            string tableCreationString = @"CREATE TABLE Profile (
	            id BINARY(16) NOT NULL,
                email VARCHAR(100),
	            name VARCHAR(50),
                hwid VARCHAR(20),
                productKey CHAR(36),
                type INT DEFAULT 0,
                creationDate DATETIME DEFAULT '2011-01-01 00:00:00',
	            PRIMARY KEY (id)
            );";
            DBExecute(new OdbcCommand(tableCreationString, connection), true);
        }

        public static void DBDropTable(OdbcConnection connection)
        {
            DBExecute(new OdbcCommand("DROP TABLE IF EXISTS Profile", connection), true);
        }
    }
}
