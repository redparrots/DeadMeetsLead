using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Graphics.Interface
{
    public partial class Label
    {
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (!readOnly)
            {
                e.Handled = true;
                switch (e.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Home:
                    case Keys.End:
                        e.SuppressKeyPress = true;
                        if (e.Shift && selectable)
                        {
                            if (selectionStart == null) selectionStart = new Caret(caret);
                        }
                        else if (selectionStart != null)
                        {
                            selectionStart = null;
                            UpdateSelection();
                        }
                        if (e.KeyCode == Keys.Left)
                        {
                            if (e.Control)
                                caret.MoveToBeginningOfWord();
                            else
                                caret--;
                        }
                        if (e.KeyCode == Keys.Right)
                        {
                            if (e.Control)
                                caret.MoveToEndOfWord();
                            else
                                caret++;
                        }
                        if (e.KeyCode == Keys.Up) caret.Up();
                        if (e.KeyCode == Keys.Down) caret.Down();
                        if (e.KeyCode == Keys.Home) caret.Set(0, caret.Row);
                        if (e.KeyCode == Keys.End) caret.Set(document.Lines[caret.Row].Length - 1, caret.Row);
                        OnCaretChanged();
                        break;
                    case Keys.Back:
                    case Keys.Delete:
                        if (selectionStart != null)
                        {
                            int selpos = selectionStart.Position;
                            DeleteSelection();
                            caret.Position = Math.Min(caret.Position, selpos);
                            OnTextChanged();
                        }
                        else {
                            if (caret > 0 && e.KeyCode == Keys.Back)
                            {
                                caret--;
                                document.Remove(caret, 1);
                                OnTextChanged();
                            }
                            else if (caret <= document.Length - 1 && e.KeyCode == Keys.Delete)
                            {
                                document.Remove(caret, 1);
                                OnTextChanged();
                            }
                        }
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.X:
                        if (e.Control)
                        {
                            if (selectionStart != null)
                            {
                                string s = SelectedText();
                                if (s.Length > 0)
                                    Clipboard.SetText(s);
                                DeleteSelection();
                                OnTextChanged();
                            }
                            e.SuppressKeyPress = true;
                        }
                        break;
                    case Keys.C:
                        if (e.Control)
                        {
                            if (selectionStart != null)
                            {
                                string s = SelectedText();
                                if (s.Length > 0)
                                    Clipboard.SetText(s);
                            }
                            e.SuppressKeyPress = true;
                        }
                        break;
                    case Keys.V:
                        if (e.Control)
                        {
                            if (selectionStart != null)
                                DeleteSelection();
                            string cb = Clipboard.GetText();
                            foreach (char c in cb)
                                InsertCharacter(c);
                            e.SuppressKeyPress = true;
                        }
                        break;
                    case Keys.A:
                        if (e.Control && selectable)
                        {
                            SelectAll();
                            e.SuppressKeyPress = true;
                        }
                        break;
                }
            }
            base.OnKeyDown(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!readOnly)
            {
                char keychar = (char)e.KeyChar;

                InsertCharacter(keychar);
            }
            base.OnKeyPress(e);
        }
        void InsertCharacter(char keychar)
        {
            if (IsValidKeyInput(keychar))
            {
                if (selectionStart != null)
                    DeleteSelection();

                document.Insert(caret, keychar);
                caret++;
                OnTextChanged();
            }
        }
        protected virtual bool IsValidKeyInput(char key)
        {
            return true;
        }

        protected void DeleteSelection()
        {
            if (selectionStart == null)
                return;

            Caret startCaret = selectionStart.Position < caret.Position ? selectionStart : caret;
            Caret endCaret = selectionStart.Position < caret.Position ? caret : selectionStart;

            document.Remove(startCaret, endCaret.Position - startCaret.Position);

            selectionStart = null;
            caret.Position = startCaret.Position;
        }

    }
}
