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
    public partial class GameEntitiesList : UserControl
    {
        public GameEntitiesList()
        {
            InitializeComponent();


            foreach (Type t in typeof(Client.Game.Map.Map).Assembly.GetTypes())
            {
                string group = "";
                bool deployable = false;
                foreach (var v in Attribute.GetCustomAttributes(t, true))
                    if (v is Client.Game.Map.EditorDeployableAttribute)
                    {
                        deployable = true;
                        group = ((Client.Game.Map.EditorDeployableAttribute)v).Group;
                        break;
                    }
                if (group == null) group = "";
                ListViewGroup g;
                if(!groups.TryGetValue(group, out g))
                {
                    entitiesListView.Groups.Add(g = new ListViewGroup
                    {
                        Header = group
                    });
                    groups.Add(group, g);
                }

                if (deployable)
                    entitiesListView.Items.Add(new ListViewItem { Text = t.Name, Tag = t, Group = g });
            }
            entitiesListView.SelectedIndexChanged += new EventHandler(entitiesListView_SelectedIndexChanged);
        }

        void entitiesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null) SelectedIndexChanged(this, e);
        }
        Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();


        public event EventHandler SelectedIndexChanged;

        public ListView.SelectedListViewItemCollection SelectedItems { get { return entitiesListView.SelectedItems; } }
        //public List<Type> SelectedTypes { get { return entitiesTreeView.selected } }

    }
}
