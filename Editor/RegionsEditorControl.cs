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
    public partial class RegionsEditorControl : UserControl
    {
        public RegionsEditorControl()
        {
            InitializeComponent();
            selectedRegionPropertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(selectedRegionPropertyGrid_PropertyValueChanged);
        }

        void selectedRegionPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            LoadRegions(MainWindow.Instance.CurrentMap);
        }

        public void LoadRegions(Client.Game.Map.Map map)
        {
            regionsListBox.Items.Clear();
            foreach (var v in map.Regions)
                regionsListBox.Items.Add(v);
        }

        public event EventHandler RegionSelected;
        public Client.Game.Map.Region SelectedRegion { get { return (Client.Game.Map.Region)regionsListBox.SelectedItem; } }

        private void regionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RegionSelected != null)
                RegionSelected(this, null);
            List<Client.Game.Map.Region> sel = new List<Client.Game.Map.Region>();
            foreach (Client.Game.Map.Region v in regionsListBox.SelectedItems)
                sel.Add(v);
            selectedRegionPropertyGrid.SelectedObjects = sel.ToArray();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            if (MainWindow.Instance.CurrentMap.GetRegion(newNameTextBox.Text) != null)
            {
                MessageBox.Show("A region with that name already exists");
                return;
            }
            Client.Game.Map.Region r = new Client.Game.Map.Region
            {
                Name = newNameTextBox.Text,
                BoundingRegion = new Common.Bounding.Region()
            };

            MainWindow.Instance.CurrentMap.Regions.Add(r);
            regionsListBox.Items.Add(r);
            regionsListBox.SelectedItem = r;
            newNameTextBox.Text = "";
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            var r = (Client.Game.Map.Region)regionsListBox.SelectedItem;
            regionsListBox.Items.Remove(r);
            MainWindow.Instance.CurrentMap.Regions.Remove(r);
        }
    }
}
