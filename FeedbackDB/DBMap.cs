using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeedbackCommon;
using System.Data.Odbc;

namespace FeedbackDB
{
    public class DBMap : DBObject<Map>
    {
        public DBMap(Map m) : base(m) { }

        public override void DBInsert(OdbcConnection connection)
        {
            OdbcCommand cmd = new OdbcCommand("INSERT INTO Map (name, stages, version) VALUES (?, ?, ?);", connection);
            cmd.Parameters.Add("name", OdbcType.VarChar, 50).Value = Object.Name ?? "ERR: Unknown";
            cmd.Parameters.Add("stages", OdbcType.Int).Value = Object.Stages;
            cmd.Parameters.Add("version", OdbcType.Int).Value = Object.Version;
            PrintCommandString(cmd, 70);
            Object.ID = DBExecuteReturnAutoID(cmd);
        }

        public static void DBCreateTable(OdbcConnection connection)
        {
            string tableCreationString = @"CREATE TABLE Map (
	            id INT UNSIGNED AUTO_INCREMENT,
	            name VARCHAR(50),
                stages INT DEFAULT -1,
                version INT,
	            PRIMARY KEY (id)
            );";
            DBExecute(new OdbcCommand(tableCreationString, connection), true);
        }

        public static void DBDropTable(OdbcConnection connection)
        {
            DBExecute(new OdbcCommand("DROP TABLE IF EXISTS Map", connection), true);
        }

        public static int DBLookupID(OdbcConnection connection, String name, int version, Stage stage)
        {
            string lookupIDString = String.Format("SELECT id FROM Map WHERE name='{0}' AND version={1}", name, version);

            int id = -1;
            try
            {
                id = (int)DBExecuteScalar<long>(new OdbcCommand(lookupIDString, connection));
            }
            catch (KeyNotFoundException ex)
            {
                int mapStages = -1;
                if (stage != null)
                    mapStages = stage.MapStages;

                DBMap map = new DBMap(new Map { Name = name, Version = version, Stages = mapStages });
                map.DBInsert(connection);
                id = map.Object.ID;
            }
            return id;
        }
    }
}
