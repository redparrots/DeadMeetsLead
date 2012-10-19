using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Graphics
{
    public abstract class ILogger
    {
        public virtual void Close() { }
        public abstract void Write(params String[] msg);
        public virtual void WriteRaw(String msg) { Write(msg); }
        protected String Time()
        {
            var d = DateTime.Now - Application.ProgramStartTime;
            return d.Hours.ToString().PadLeft(2, '0') + ":" +
                    d.Minutes.ToString().PadLeft(2, '0') + ":" +
                    d.Seconds.ToString().PadLeft(2, '0') + "." +
                    d.Milliseconds.ToString().PadLeft(3, '0');
        }
    }

    public class MultiLogger : ILogger
    {
        public MultiLogger(params ILogger[] loggers)
        {
            Loggers = new List<ILogger>(loggers);
        }
        public override void Write(params string[] msg)
        {
            foreach (var v in Loggers)
                v.Write(msg);
        }
        public List<ILogger> Loggers { get; private set; }
    }

    public class ConsoleLogger : ILogger
    {
        public override void Write(params String[] msg)
        {
            StringBuilder b = new StringBuilder();
            b.Append(Time()).Append("| ").Append(msg[0]).Append("\r\n");
            for (int i = 1; i < msg.Length; i++)
                b.Append("            | ").Append(msg[i]).Append("\r\n");
            Console.WriteLine(b.ToString());
        }
    }

    public abstract class FileLogger : ILogger
    {
        public FileLogger(String filename)
        {
            lock (locker)
            {
                file = new StreamWriter(filename);
                file.AutoFlush = true;
            }
        }
        public override void Close()
        {
            file.Close();
        }

        public override void WriteRaw(String msg)
        {
            lock (locker)
            {
                file.Write(msg);
            }
        }


        protected object locker = new object();
        StreamWriter file;
    }

    public class TextLogger : FileLogger
    {
        public TextLogger(String log) : base(log + ".txt")
        {
        }

        public override void Write(params String[] msg)
        {
            lock (locker)
            {
                StringBuilder b = new StringBuilder();
                b.Append(Time()).Append("| ").Append(msg[0]).Append("\r\n");
                for (int i = 1; i < msg.Length; i++)
                    b.Append("            | ").Append(msg[i]).Append("\r\n");
                WriteRaw(b.ToString());
            }
        }
    }

    public class CSVLogger : FileLogger
    {
        public CSVLogger(String log, params String[] cols) : base(log + ".csv")
        {
            lock (locker)
            {
                StringBuilder b = new StringBuilder();
                b.Append("Time");
                foreach (String s in cols)
                    b.Append("; ").Append(s);
                b.Append("\r\n");
                WriteRaw(b.ToString());
            }
        }

        public override void Write(params String[] msg)
        {
            lock (locker)
            {
                StringBuilder b = new StringBuilder();
                b.Append(Time());
                foreach (String s in msg)
                    b.Append("; ").Append(s);
                b.Append("\r\n");
                WriteRaw(b.ToString());
            }
        }
    }
}
