using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor
{
    public partial class ScriptsEditor : UserControl
    {
        public ScriptsEditor()
        {
            InitializeComponent();
        }

        public void LoadScripts(List<Client.Game.Map.Script> scripts)
        {
            this.scripts = scripts;
            scriptsListBox.Items.Clear();
            foreach (var v in scripts)
                scriptsListBox.Items.Add(v);
        }

        void UpdateScripts()
        {
            scripts.Clear();
            foreach (Client.Game.Map.Script v in scriptsListBox.Items)
                scripts.Add(v);
        }

        List<Client.Game.Map.Script> scripts;

        private void addButton_Click(object sender, EventArgs e)
        {
            Common.WindowsForms.SelectTypeDialog d = new Common.WindowsForms.SelectTypeDialog();

            d.LoadTypes(typeof(Client.Game.Map.GameEntity).Assembly, typeof(Client.Game.Map.MapScript));
            if (d.ShowDialog() == DialogResult.Cancel) return;

            var s = (Client.Game.Map.MapScript)Activator.CreateInstance((Type)d.SelectedObject);
            s.Name = ((Type)d.SelectedObject).Name + scripts.Count;
            scriptsListBox.Items.Add(s);
            scriptsListBox.SelectedItem = s;
            UpdateScripts();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            scriptsListBox.Items.Remove(scriptsListBox.SelectedItem);
            UpdateScripts();
        }

        private void scriptsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            scriptPropertyGrid.SelectedObject = scriptsListBox.SelectedItem;
            UpdateScripts();
        }

        //private void cloneButton_Click(object sender, EventArgs e)
        //{
        //    var olds = (Client.Game.Map.MapScript)scriptsListBox.SelectedItem;
        //    if (olds == null) return;
        //    var s = (Client.Game.Map.MapScript)olds.Clone();
        //    s.Name = olds.Name + scripts.Count;
        //    scriptsListBox.Items.Add(s);
        //    scriptsListBox.SelectedItem = s;
        //    UpdateScripts();
        //}

        private void upButton_Click(object sender, EventArgs e)
        {
            MoveSelected(-1);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            MoveSelected(1);
        }

        void MoveSelected(int d)
        {
            var s = scriptsListBox.SelectedItem;
            var i = scriptsListBox.Items.IndexOf(s);
            if (i + d < 0 || i + d >= scriptsListBox.Items.Count) return;
            scriptsListBox.Items.Remove(s);
            scriptsListBox.Items.Insert(i + d, s);
            scriptsListBox.SelectedItem = s;
            UpdateScripts();
        }

        private void scriptPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            List<object> items = new List<object>();
            foreach (var v in scriptsListBox.Items)
                items.Add(v);
            scriptsListBox.Items.Clear();
            foreach (var v in items)
                scriptsListBox.Items.Add(v);
        }

        private void cutButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Serializable, scriptsListBox.SelectedItem);
            removeButton_Click(null, null);
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            var olds = (Client.Game.Map.MapScript)scriptsListBox.SelectedItem;
            if (olds == null) return;
            var s = (Client.Game.Map.MapScript)olds.Clone();
            s.Name = olds.Name + scripts.Count;
            Clipboard.SetData(DataFormats.Serializable, s);
        }

        private void pasteButton_Click(object sender, EventArgs e)
        {
            var s = Clipboard.GetData(DataFormats.Serializable);
            scriptsListBox.Items.Add(s);
            scriptsListBox.SelectedItem = s;
            UpdateScripts();
        }
    }


    public class MapScriptListTypeEditor : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ScriptsEditor t = new ScriptsEditor();
            t.LoadScripts((List<Client.Game.Map.Script>)value);
            t.Dock = DockStyle.Fill;
            Form f = new Form { Size = new Size(800, 500) };
            f.Controls.Add(t);
            f.ShowDialog();
            return base.EditValue(context, provider, value);
        }
    }
}
