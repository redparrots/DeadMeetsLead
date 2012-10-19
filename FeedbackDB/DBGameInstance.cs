using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeedbackCommon;
using System.Data.Odbc;

namespace FeedbackDB
{
    public class DBGameInstance : DBObject<GameInstance>
    {
        public DBGameInstance(GameInstance gi) : base(gi) { }

        public override void DBInsert(OdbcConnection connection)
        {
            new DBProfile(Object.Profile).DBInsert(connection, true);

            OdbcCommand cmd = new OdbcCommand(
                @"INSERT INTO GameInstance (id, profile, map, senderIP, startTime, endTime, dateReceived, timePlayed, nFrames, 
                    reason, endPosX, endPosY, endPosZ, hp, rage, ammunition, meleeWeapon, rangedWeapon, difficulty, gameVersion, 
                    stageNumber, stageHP, stageMaxHP, stageRage, stageTime, stageAmmo,
                    hitsTaken, damageDealt, damageTaken, timesNetted, nSlams, gbShots, totalShots, killedUnits) 
                  Values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);",
                connection);
            cmd.Parameters.Add("id", OdbcType.Binary, 16).Value = Object.ID.ToByteArray();
            cmd.Parameters.Add("profile", OdbcType.Binary, 16).Value = Object.Profile.ID.ToByteArray();
            cmd.Parameters.Add("map", OdbcType.Int).Value = DBMap.DBLookupID(connection, Object.MapName, Object.MapVersion, Object.HighestStage);
            cmd.Parameters.Add("senderIP", OdbcType.VarChar, 20).Value = Object.SenderIP ?? "";
            cmd.Parameters.Add("startTime", OdbcType.DateTime).Value = Object.StartTime;
            cmd.Parameters.Add("endTime", OdbcType.DateTime).Value = Object.EndTime;
            cmd.Parameters.Add("dateReceived", OdbcType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("timePlayed", OdbcType.Decimal).Value = Object.TimePlayed;
            cmd.Parameters.Add("nFrames", OdbcType.Int).Value = Object.Frames;
            cmd.Parameters.Add("reason", OdbcType.VarChar, 100).Value = Object.Reason ?? "";
            cmd.Parameters.Add("endPosX", OdbcType.Decimal).Value = Object.EndPosX;
            cmd.Parameters.Add("endPosY", OdbcType.Decimal).Value = Object.EndPosY;
            cmd.Parameters.Add("endPosZ", OdbcType.Decimal).Value = Object.EndPosZ;
            cmd.Parameters.Add("hp", OdbcType.Int).Value = Object.Hitpoints;
            cmd.Parameters.Add("rage", OdbcType.Int).Value = Object.Rage;
            cmd.Parameters.Add("ammunition", OdbcType.Int).Value = Object.Ammunition;
            cmd.Parameters.Add("meleeWeapon", OdbcType.Int).Value = Object.MeleeWeapon;
            cmd.Parameters.Add("rangedWeapon", OdbcType.Int).Value = Object.RangedWeapon;
            cmd.Parameters.Add("difficulty", OdbcType.VarChar, 15).Value = Object.Difficulty ?? "";
            cmd.Parameters.Add("gameVersion", OdbcType.VarChar, 30).Value = Object.GameVersion ?? "";

            var stage = Object.HighestStage;
            cmd.Parameters.Add("stageNumber", OdbcType.Int).Value = stage != null ? stage.StageNumber : -1;
            cmd.Parameters.Add("stageHP", OdbcType.Int).Value = stage != null ? stage.HitPoints : -1;
            cmd.Parameters.Add("stageMaxHP", OdbcType.Int).Value = stage != null ? stage.MaxHitPoints : -1;
            cmd.Parameters.Add("stageRage", OdbcType.Int).Value = stage != null ? stage.Rage : -1;
            cmd.Parameters.Add("stageTime", OdbcType.Decimal).Value = stage != null ? stage.Time : -1;
            cmd.Parameters.Add("stageAmmo", OdbcType.Int).Value = stage != null ? stage.Ammo : -1;

            cmd.Parameters.Add("hitsTaken", OdbcType.Int).Value = Object.HitsTaken;
            cmd.Parameters.Add("damageDealt", OdbcType.Int).Value = Object.DamageDealt;
            cmd.Parameters.Add("damageTaken", OdbcType.Int).Value = Object.DamageTaken;
            cmd.Parameters.Add("timesNetted", OdbcType.Int).Value = Object.TimesNetted;
            cmd.Parameters.Add("nSlams", OdbcType.Int).Value = Object.NSlams;
            cmd.Parameters.Add("gbShots", OdbcType.Int).Value = Object.GBShots;
            cmd.Parameters.Add("totalShots", OdbcType.Int).Value = Object.TotalShots;
            cmd.Parameters.Add("killedUnits", OdbcType.Int).Value = Object.KilledUnits;

            PrintCommandString(cmd, 300);
            DBExecute(cmd);
        }

        public static void DBCreateTable(OdbcConnection connection)
        {
            string tableCreationString =
                @"CREATE TABLE GameInstance (
                id BINARY(16),
                profile BINARY(16),
        	    map INT UNSIGNED,
	            senderIP VARCHAR(20),
	            startTime DATETIME,
	            endTime DATETIME,
                dateReceived DATETIME,
                timePlayed DECIMAL(18,3),
                nFrames INT,
                reason VARCHAR(100),
                endPosX DECIMAL(18,3),
                endPosY DECIMAL(18,3),
                endPosZ DECIMAL(18,3),
                hp INT,
                rage INT,
                ammunition INT,
                meleeWeapon INT,
                rangedWeapon INT,
                difficulty VARCHAR(15),
                gameVersion VARCHAR(30),
                stageNumber INT,
                stageHP INT,
                stageMaxHP INT,
                stageRage INT,
                stageTime DECIMAL(18,3),
                stageAmmo INT,
                hitsTaken INT,
                damageDealt INT,
                damageTaken INT,
                timesNetted INT,
                nSlams INT,
                gbShots INT,
                totalShots INT,
                killedUnits INT,
                PRIMARY KEY (id),
                FOREIGN KEY (profile) REFERENCES Profile(id),
                FOREIGN KEY (map) REFERENCES Map(id)
                );";
            DBExecute(new OdbcCommand(tableCreationString, connection), true);
        }

        public static void DBDropTable(OdbcConnection connection)
        {
            DBExecute(new OdbcCommand("DROP TABLE IF EXISTS GameInstance", connection), true);
        }
    }
}
