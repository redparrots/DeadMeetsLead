using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Text.RegularExpressions;
using Graphics.Content;

namespace Graphics.Interface
{
    public partial class Label
    {
        void DisplayCaret(Vector2 position)
        {
            visualCaret.Active = true;
            visualCaret.Position = position;
        }
        void HideCaret()
        {
            visualCaret.Active = false;
        }


        Interface.Caret visualCaret;

        public class Caret
        {
            public event EventHandler Change = null;

            public Caret(Label textBox) 
            { 
                this.textBox = textBox;
                this.Visible = true;
            }
            public Caret(Caret cpy)
            {
                this.textBox = cpy.textBox;
                this.position = cpy.position;
                this.IsSet = cpy.IsSet;
                this.column = cpy.column;
                this.row = cpy.row;
                this.realcolumn = cpy.realcolumn;
            }

            public void SetFromXY(Vector2 documentPos)
            {
                IsSet = true;
                var fi = textBox.Scene.View.Content.Acquire<FontImplementation>(textBox.Font);
                int lineNr = (int)(documentPos.Y / fi.CharacterHeight);
                if (lineNr >= textBox.document.Lines.Count) lineNr = textBox.document.Lines.Count - 1;
                if (lineNr < 0) lineNr = 0;
                Document.Line line = textBox.document.Lines[lineNr];
                float prevx = 0;
                //find the char in the line
                for (int i = 0; i <= line.Value.Length; i++)
                {
                    String w = line.Value.Substring(0, i);
                    float x = fi.TextWidth(w);
                    if (x > documentPos.X)
                    {
                        if (x - documentPos.X < documentPos.X - prevx) i++;
                        this.Position = Math.Max(0, line.startIndex + i - 1);
                        return;
                    }
                    prevx = x;
                }

                this.Position = line.startIndex + line.Value.Length;
            }
            public void Set(int position)
            {

                this.Position = position;
            }
            public void Set(Caret p)
            {
                position = p.position;
                IsSet = true;
                column = p.column;
                realcolumn = p.realcolumn;
                row = p.row;
                if (Visible)
                    textBox.UpdateSelection();
            }
            public void Set(int col, int row)
            {
                if (row < 0) row = 0;
                if (row >= textBox.document.Lines.Count) row = textBox.document.Lines.Count - 1;
                Document.Line l = textBox.document.Lines[row];
                if (col < 0) col = 0;
                if (col >= l.Length) col = Math.Max(l.Length - 1, 0);
                this.row = row;
                position = l.startIndex + col;
                realcolumn = col;
                if (Change != null) Change(this, null);
                if (Visible)
                    textBox.UpdateSelection();
            }
            public void Update()
            {
                Set(Column, Row);
            }

            public void Up()
            {
                if (row > 0)
                    Set(column, --row);
            }
            public void Down()
            {
                if (row < textBox.document.Lines.Count - 1)
                    Set(column, ++row);
            }
            public void Forward(int n)
            {
                Position += n;
            }
            public static implicit operator int(Caret a)
            {
                return a.position;
            }
            public static implicit operator bool(Caret a)
            {
                return a.IsSet;
            }
            public static Caret operator --(Caret a)
            {
                a.Position--;
                return a;
            }
            public static Caret operator ++(Caret a)
            {
                a.Position++;
                return a;
            }
            public static Caret operator +(Caret a, int b)
            {
                a.Forward(b);
                return a;
            }
            public void MoveToEndOfWord()
            {
                string s = textBox.document.Lines[row].Value.Substring(realcolumn);
                Match m = Regex.Match(s, "(\\W|$)");
                if (m.Success)
                {
                    int offset = m.Index;
                    m = Regex.Match(s.Substring(offset), "(\\w|$)");
                    if (m.Success)
                        offset += m.Index;
                    Position += offset;
            }
            }
            public void MoveToBeginningOfWord()
            {
                Match m = Regex.Match(textBox.document.Lines[row].Value.Substring(0, realcolumn).Trim(), "(\\W|^)", RegexOptions.RightToLeft);
                if (m.Success)
                    Position -= realcolumn - m.Index - m.Length;
            }
            public void MoveToEndOfLine()
            {
                Document.Line line = textBox.document.Lines[row];
                int delta = line.Length - (Position - line.startIndex) - 1;
                Position += delta;
            }

            public int Position
            {
                get { return position; }
                set
                {
                    if (value < 0) return;
                    if (value >= textBox.document.Length) return;
                    IsSet = true;
                    position = value;
                    Document.Line l = textBox.document.GetLine(position);
                    if (l != null)
                    {
                        realcolumn = column = position - l.startIndex;
                        row = l.Index;
                    }
                    else
                    {
                        realcolumn = column = row = 0;
                    }
                    if (Change != null)
                        Change(this, null);
                    if (Visible)
                        textBox.UpdateSelection();
                }
            }

            public bool Visible { get; set; }

            /// <summary>
            /// The geometrical position of the caret relative to the document.
            /// </summary>
            public Vector3 DocumentPosition
            {
                get
                {
                    Document.Line line = textBox.document.Lines[row];
                    var fi = textBox.Scene.View.Content.Acquire<FontImplementation>(textBox.Font);
                    float w = fi.TextWidth(line.Value.Substring(0, realcolumn));
                    textBox.Scene.View.Content.Release(fi);
                    return new Vector3(w, row * fi.CharacterHeight, 0);
                }
            }

            Label textBox;
            int position = 0;
            public bool IsSet = false;

            int column = 0, row = 0;
            int realcolumn = 0;

            public int Row { get { return row; } set { Set(0, value); } }
            public int Column { get { return realcolumn; } }
        }
    }
}
