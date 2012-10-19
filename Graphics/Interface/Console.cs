using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics.Interface
{
    public class Console : Label
    {
        public Console()
        {
            System.Console.SetOut(new System.IO.StreamWriter(text = new ConsoleStream { Console = this }) { AutoFlush = true });
        }

        ConsoleStream text;

        class ConsoleStream : System.IO.MemoryStream
        {
            public Console Console { get; set; }
            public override void Flush()
            {
                base.Flush();
                var s = System.Text.Encoding.ASCII.GetString(ToArray());
                var ss = s.Split('\n');
                int nlines = (int)(Console.Size.Y / 13f);
                s = "";
                for (int i = Math.Max(ss.Length - nlines, 0); i < ss.Length; i++)
                    s += ss[i] + "\n";
                Console.Text = s;
            }
        }
    }
}
