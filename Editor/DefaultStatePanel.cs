using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Graphics;

namespace Editor
{
    public partial class DefaultStatePanel : UserControl
    {
        public DefaultStatePanel()
        {
            InitializeComponent();
        }

        public GameEntitiesList GameEntityList
        {
            set { splitContainer1.Panel1.Controls.Clear(); splitContainer1.Panel1.Controls.Add(value); }
        }

        public void SetSelection(IEnumerable<Entity> entities)
        {
            var ses = entities.ToArray();
            selectedEntityPropertyGrid.SelectedObjects = ses;
            if (ses.Length > 1)
                selectedEntityLabel.Text = ses.Length + " objects";
            else if (ses.Length == 1)
                selectedEntityLabel.Text = ses[0].GetType().Name + ":" + ses[0].Name;
            else
                selectedEntityLabel.Text = "";
        }
    }
}
