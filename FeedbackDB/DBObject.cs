using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace FeedbackDB
{
    public abstract class DBObject<T>
    {
        public DBObject(T objct)
        {
            Object = objct;
        }

        public abstract void DBInsert(OdbcConnection connection);

        protected static void DBExecute(OdbcCommand cmd)
        {
            cmd.ExecuteNonQuery();
        }

        protected static void DBExecute(OdbcCommand cmd, bool print)
        {
            if (print) PrintCommandString(cmd);
            DBExecute(cmd);
        }

        protected static R DBExecuteScalar<R>(OdbcCommand cmd)
        {
            PrintCommandString(cmd);
            object result = cmd.ExecuteScalar();
            if (result == null)
                throw new KeyNotFoundException("No such entry exists");
            return (R)cmd.ExecuteScalar();
        }

        protected static int DBExecuteReturnAutoID(OdbcCommand cmd)
        {
            DBExecute(cmd);
            return (int)new OdbcCommand("SELECT LAST_INSERT_ID();", cmd.Connection).ExecuteScalar();
        }

        protected static void PrintCommandString(OdbcCommand cmd)
        {
            PrintCommandString(cmd, 70);
        }
        protected static void PrintCommandString(OdbcCommand cmd, int charLimit)
        {
            string s = cmd.CommandText;
            foreach (OdbcParameter p in cmd.Parameters)
                s = ReplaceFirst(s, "?", p.Value != null ? p.Value.ToString() : "null");
            s = s.Replace(Environment.NewLine, " ").Replace("\t", " ");
            Console.WriteLine(String.Format("> {0}{1}", s.Substring(0, System.Math.Min(charLimit, s.Length)), s.Length > charLimit ? "..." : ""));
        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
                return text;
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public T Object { get; private set; }
    }
}
