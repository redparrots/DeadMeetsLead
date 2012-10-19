using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using SlimDX;
using System.Windows.Forms;
using Graphics.Content;

namespace Graphics.Interface
{
    // Label is to be moved out of this file, and only contain the subset necessary 
    // for it (text, autosize, document etc). The interface (at least readonly, selectable) is correct,
    // but functionallity is placed in the wrong place;
    // Label should have all the display functionallity
    // TextBox should have all interaction functionallity (text-input, selectable ect)
    public class TextBox : Label
    {
        public TextBox()
        {
            Background = (BorderGraphic)InterfaceScene.DefaultSlimBorder.Clone();
            Selectable = true;
            ReadOnly = false;
            Clickable = true;
        }

        protected override void OnTextChanged()
        {
            if (MaxLength > 0 && document.Text.Length > MaxLength)
                document.Text = document.Text.Substring(0, MaxLength);
            
            base.OnTextChanged();
        }

        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                if (readOnly == value) return;
                readOnly = value; caret.Visible = !readOnly;
            }
        }

        public int MaxLength
        {
            get { return maxLength; }
            set
            {
                maxLength = value;
                if (maxLength > 0 && Text.Length > maxLength)
                    Text = Text.Substring(0, maxLength);
            }
        }
        public bool Selectable { get { return selectable; } set { selectable = value; } }
        private int maxLength = 0;
    }
    public partial class Label : Control
    {
        public Label()
        {
            caret = new Caret(this);
            visualCaret = new Graphics.Interface.Caret();
            AddChild(visualCaret);
            Font = InterfaceScene.DefaultFont;
            Clickable = false;
            readOnly = true;
            selectable = false;
        }

        protected override void OnConstruct()
        {
            if (AutoSize != AutoSizeMode.None)
                AutoAdjustSize();
            base.OnConstruct();
            if (textGraphic != null && !String.IsNullOrEmpty(Text))
            {
                textGraphic.Size = InnerSize;
                textGraphic.Position = Common.Math.ToVector3(InnerOffset);
                SetGraphic("TextBox.TextGraphic", textGraphic);
            }
            else
                SetGraphic("TextBox.TextGraphic", null);
            if (selectable && selectionStart != null && selectionStart.Position != caret.Position)
            {
                var selectionGraphic = new GlyphsGraphic
                {
                    Overflow = TextOverflow.Hide,
                    Texture = new TextureConcretizer { TextureDescription = new Software.Textures.SingleColorTexture(System.Drawing.Color.FromArgb(100, System.Drawing.Color.Blue)) }
                };
                selectionGraphic.Glyphs.Glyphs = GetSelectionBoxes() ?? new List<Glyph>();
                selectionGraphic.Offset = textGraphic.Offset;
                selectionGraphic.Size = InnerSize;
                selectionGraphic.Position = Common.Math.ToVector3(InnerOffset);
                SetGraphic("TextBox.Selection", selectionGraphic);
            }
            else
                SetGraphic("TextBox.Selection", null);
        }


        AutoSizeMode autoSize = AutoSizeMode.None;
        public AutoSizeMode AutoSize
        {
            get { return autoSize; }
            set
            {
                if (autoSize == value) return;
                autoSize = value;
                Invalidate();
            }
        }

        void AutoAdjustSize()
        {
            if (IsRemoved) return;
            var ms = maxSize;
            if (Dock == DockStyle.Top || Dock == DockStyle.Bottom || Dock == DockStyle.Fill)
                ms.X = base.Size.X;
            if (Dock == DockStyle.Left || Dock == DockStyle.Right || Dock == DockStyle.Fill)
                ms.Y = base.Size.Y;
            Size = TextGraphic.AutoSize(autoSize, Scene.View.Content, base.Size, ms, Padding);
        }

        public override Vector2 Size
        {
            get
            {
                if (AutoSize != AutoSizeMode.None) AutoAdjustSize();
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        Vector2 maxSize = new Vector2(float.MaxValue, float.MaxValue);
        public Vector2 MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; if (AutoSize != AutoSizeMode.None) AutoAdjustSize(); }
        }

        protected bool selectable;

        public override void OnFocused()
        {
            OnCaretChanged();
        }

        public override void OnLostFocus()
        {
            HideCaret();
        }

        public void SelectAll()
        {
            selectionStart = new Caret(caret);
            selectionStart.Set(0);
            caret.Set(document.Lines.Last().Length, document.Lines.Count);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (selectable)
            {
                var screenPos = new Vector2(e.X, e.Y);
                selectionStart = null;
                Vector2 pos = screenPos - textGraphic.Offset - new Vector2(AbsoluteTranslation.X, AbsoluteTranslation.Y);
                caret.SetFromXY(pos);

                selectionStart = new Caret(caret);
            }

            Focus();
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if(!readOnly)
                Cursor.Current = Cursors.IBeam;
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            var screenPos = new Vector2(e.X, e.Y);
            base.OnMouseMove(e);

            if ((System.Windows.Forms.Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != 0)
            {
                caret.SetFromXY(screenPos - textGraphic.Offset - new Vector2(AbsoluteTranslation.X, AbsoluteTranslation.Y));
                OnCaretChanged();
            }

            if(!readOnly)
                Cursor.Current = Cursors.IBeam;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if(!readOnly)
                Cursor.Current = Cursors.Arrow;
        }
        
        protected virtual void OnTextChanged()
        {
            textGraphic.Text = document.Text;
            Invalidate();
            caret.Update();
            OnCaretChanged();
            if (autoSize != AutoSizeMode.None)
                AutoAdjustSize();
            if (TextChanged != null)
                TextChanged(this, null);
        }

        void OnCaretChanged()
        {
            if (Focused && !readOnly)
            {
                Vector3 caretLocal = caret.DocumentPosition + new Vector3(textGraphic.Offset.X, textGraphic.Offset.Y, 0);
                Vector2 oldOffset = textGraphic.Offset;
                Vector2 newOffset = oldOffset;
                if (caretLocal.X >= InnerSize.X) newOffset.X -= caretLocal.X - InnerSize.X + 1;
                else if (caretLocal.X < 0) newOffset.X -= caretLocal.X - 20;
                if (caretLocal.Y >= InnerSize.Y) newOffset.Y -= caretLocal.Y - InnerSize.Y + 1;
                else if (caretLocal.Y < 0) newOffset.Y -= caretLocal.Y - 20;
                newOffset.X = Math.Min(textGraphic.Offset.X, 0);
                newOffset.Y = Math.Min(textGraphic.Offset.Y, 0);
                textGraphic.Offset = newOffset;


                if (textGraphic.Offset != oldOffset)
                    OnTextChanged();
                else
                {
                    if (caret.Visible)
                        DisplayCaret(Common.Math.ToVector2(caretLocal +
                            Common.Math.ToVector3(InnerOffset) + new Vector3(-1, 0, 0)));
                    //if (selectionStart != null)
                    //    InterfaceManager.DisplaySelection(GetSelectionBoxes());
                }
            }
        }

        protected List<Glyph> GetSelectionBoxes()
        {
            if (selectionStart == null || selectionStart.Position == caret.Position)
                return null;


            var fi = Scene.View.Content.Acquire<FontImplementation>(Font);

            Caret start = selectionStart.Position < caret.Position ? selectionStart : caret;
            Caret end = selectionStart.Position < caret.Position ? caret : selectionStart;
            Vector3 startPos = start.DocumentPosition;
            Vector3 endPos = end.DocumentPosition;

            List<Glyph> selectionBoxes = new List<Glyph>();
            Vector2 sOffset = new Vector2(0, fi.CharacterHeight - 1);

            Caret it = new Caret(start) { Visible = false };
            for (int i = start.Row; i <= end.Row; i++)
            {
                Vector3 startp = it.DocumentPosition;
                it.MoveToEndOfLine();
                Vector3 endp = it.Position <= end.Position ? it.DocumentPosition : end.DocumentPosition;
                it.Position++;

                Glyph g = new Glyph();
                g.Position = startp;
                g.Size = new Vector2(endp.X - startp.X, endp.Y - startp.Y) + sOffset;
                g.UVMin = Vector2.Zero;
                g.UVMax = new Vector2(1, 1);
                selectionBoxes.Add(g);
            }

            Scene.View.Content.Release(fi);

            return selectionBoxes;
        }

        protected void UpdateSelection()
        {
            Invalidate();
        }

        protected string SelectedText()
        {
            if (selectionStart == null || selectionStart.Position == caret.Position)
                return "";

            return document.getStrRange(
                Math.Min(selectionStart.Position, caret.Position),
                Math.Max(selectionStart.Position, caret.Position));
        }

        public virtual String Text 
        { 
            get { return document.Text; }
            set
            {
                if (value == document.Text) return;
                selectionStart = null;
                UpdateSelection();
                document.Text = value ?? ""; 
                OnTextChanged();
            }
        }


        public Font Font
        {
            get { return textGraphic.Font; }
            set
            {
                textGraphic.Font = visualCaret.Font = (Font)value.Clone(); OnTextChanged();
            }
        }
        public int TextHeight { get { return textGraphic.TextHeight; } }

        public Graphics.Orientation TextAnchor
        {
            get { return textGraphic.Anchor; }
            set
            {
                if (textGraphic.Anchor == value) return;
                textGraphic.Anchor = value; OnTextChanged();
            }
        }
        public TextOverflow Overflow
        {
            get { return textGraphic.Overflow; }
            set
            {
                if (textGraphic.Overflow == value) return;
                textGraphic.Overflow = value; OnTextChanged();
            }
        }

        public event EventHandler TextChanged;

        protected Document document = new Document();        
        private Caret selectionStart = null;
        protected bool readOnly;

        protected TextGraphic textGraphic = new TextGraphic
        {
            Overflow = TextOverflow.Hide
        };
        public TextGraphic TextGraphic { get { return textGraphic; } set { textGraphic = value; Invalidate(); } }
        protected Caret caret;
    }
}
