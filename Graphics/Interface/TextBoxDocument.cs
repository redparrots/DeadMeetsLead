using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics.Interface
{
    public partial class Label
    {
        public class Document
        {
            public List<Line> Lines = new List<Line>();
            public int Length { get { return Lines.Last().endIndex; } }

            public Document()
            {
                Lines.Add(new Line("", 0, 0));
            }

            public Document(String document)
            {
                Text = document;
            }

            public Document(Document cpy)
            {
                foreach (Line l in cpy.Lines)
                    Lines.Add(l.Clone());
            }

            public String Text
            {
                get
                {
                    String d = "";
                    foreach (Line l in Lines)
                        d += l.Value + "\n";
                    return d.Substring(0, Math.Max(0, d.Length - 1));
                }
                set
                {
                    Lines.Clear();
                    int c = 0;
                    foreach (String s in SplitBy(value, '\n'))
                    {
                        Line l = new Line(s, c, Lines.Count);
                        Lines.Add(l);
                        c += s.Length + 1;
                    }
                    if (Lines.Count == 0) Lines.Add(new Line("", 0, 0));
                }
            }

            public char GetCharAt(int pos)
            {
                foreach (Line l in Lines)
                    if (l.endIndex > pos) return l.Value[pos - l.startIndex];
                return (char)0;
            }

            public string getStrRange(int startpos, int endpos)
            {
                string res = "";
                bool inrange = false;
                foreach (Line l in Lines)
                {
                    if (!inrange && l.endIndex > startpos)
                    {
                        if (l.endIndex > endpos)
                            return l.Value.Substring(startpos-l.startIndex, endpos-startpos);

                        res = l.Value.Substring(startpos-l.startIndex, l.endIndex-startpos-1) + "\n";
                        inrange = true;
                    }
                    else if (inrange)
                    {
                        if (l.endIndex > endpos)
                            return res + l.Value.Substring(0, endpos-l.startIndex);
                        else
                            res += l.Value + "\n";
                    }
                }
                return "";      // range doesn't fit into text
            }

            public class Line
            {
                public Line(String value, int startIndex, int index)
                {
                    this.Value = value;
                    this.startIndex = startIndex;
                    this.Index = index;
                }
                public Line Clone()
                {
                    Line l = new Line(Value, startIndex, Index);
                    return l;
                }

                public String Value;
                public int Index;
                public int startIndex;
                public int Length { get { return Value.Length + 1; } } //includeds the newline char
                public int endIndex { get { return startIndex + Length; } }
            }

            
            public Line GetLine(int pos)
            {
                foreach (Line line in Lines)
                    if (pos >= line.startIndex && pos < line.endIndex)
                        return line;
                return null;
            }
            public void CalcLineIndices()
            {
                int c = 0;
                foreach (Line l in Lines)
                {
                    l.startIndex = c;
                    c += l.Length;
                }
            }

            List<String> SplitBy(String s, char split)
            {
                List<String> l = new List<String>();
                int previ = 0, i = 0;
                while ((i = s.IndexOf(split, i)) >= 0)
                {
                    l.Add(s.Substring(previ, i - previ));
                    i++;
                    previ = i;
                }
                i = s.Length;
                if (i - previ > 0)
                    l.Add(s.Substring(previ, i - previ));
                return l;
            }

            public void Insert(Caret start, char value)
            {
                Insert(start, "" + value);
            }
            public void Insert(Caret start, String value)
            {
                if (value.Contains('\r') || value.Contains('\n'))
                {
                    Text = Text.Insert(start.Position, value.Replace("\r\n", "\n").Replace('\r', '\n'));
                }
                else
                {
                    Document.Line line = Lines[start.Row];
                    line.Value = line.Value.Insert(start.Column, value);
                    CalcLineIndices();
                }
            }

            public void Remove(Caret start, int count)
            {
                String t = Text;
                if(t.Length >= start.Position + count)
                    Text = t.Remove(start.Position, count);
            }
        }
    }
}
