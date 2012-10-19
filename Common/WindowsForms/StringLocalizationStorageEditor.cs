using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Common.WindowsForms
{
    public partial class StringLocalizationStorageEditor : UserControl
    {
        public StringLocalizationStorageEditor()
        {
            InitializeComponent();
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                cultureComboBox.Items.Add(ci);
                if (ci.Name == "en")
                    cultureComboBox.SelectedItem = ci;
            }
        }

        StringLocalizationStorage stringLocalizationStorage;
        public StringLocalizationStorage StringLocalizationStorage
        {
            get { return stringLocalizationStorage; }
            set { SaveStorage(); stringLocalizationStorage = value; LoadStorage(); }
        }

        CultureInfo currentCulture = new CultureInfo("en");
        public CultureInfo CurrentCulture { get { return currentCulture; } set { SaveStorage(); currentCulture = value; LoadStorage(); } }

        bool storageLoaded = false;

        public void SaveStorage()
        {
            if (!storageLoaded) return;

            stringLocalizationStorage.StringStorage[currentCulture.Name].Clear();
            foreach (var v in dataGridView.Rows)
                if((string)((DataGridViewRow)v).Cells[0].Value != null)
                    stringLocalizationStorage.StringStorage[currentCulture.Name]
                        [(string)((DataGridViewRow)v).Cells[0].Value] = 
                            (string)((DataGridViewRow)v).Cells[1].Value;
        }

        void LoadStorage()
        {
            if (stringLocalizationStorage == null) return;

            storageLoaded = true;
            dataGridView.Rows.Clear();
            Dictionary<String, String> s;
            if (!stringLocalizationStorage.StringStorage.TryGetValue(currentCulture.Name, out s))
                stringLocalizationStorage.StringStorage[currentCulture.Name] = s = 
                    new Dictionary<string, string>();

            foreach (var v in s)
                dataGridView.Rows.Add(v.Key, v.Value);
        }

        private void cultureComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentCulture = (CultureInfo)cultureComboBox.SelectedItem;
        }

        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.CellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ((TextBox)e.Control).Multiline = true;
        }
    }
}
