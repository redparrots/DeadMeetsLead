using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Editor
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            foreach (var v in worldView.GetStateNames())
                modeComboBox.Items.Add(v);
            modeComboBox.SelectedIndex = 0;

            worldView.StateChanged += new EventHandler(worldView_StateChanged);
            worldView.Frame += new Graphics.View.FrameEventHandler(worldView_Frame);

            worldView.InitEvent += new EventHandler(worldView_InitEvent);

            /*autoSaveTimer = new Timer();
            autoSaveTimer.Interval = 60 * 1000;
            autoSaveTimer.Tick += new EventHandler(autoSaveTimer_Tick);
            autoSaveTimer.Enabled = true;*/
        }

        string autoSaveDir = "EditorAutoSaves";
        void autoSaveTimer_Tick(object sender, EventArgs e)
        {
            if(CurrentMap != null)
            {
                if (!System.IO.Directory.Exists(autoSaveDir))
                    System.IO.Directory.CreateDirectory(autoSaveDir);

                var fn = fileName;
                if(String.IsNullOrEmpty(fn)) fn = "Unnamed";
                fn = System.IO.Path.GetFileName(fn);
                fn = autoSaveDir + "\\" + fn + "_AutoSave_" + DateTime.Now.ToString("yyMMdd_hhmm") + ".map";
                tipsLabel.Text = "Auto saving \"" + fn + "\".. ";
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        Client.Game.Map.MapPersistence.Instance.Save(CurrentMap, fn);
                    }
                    catch (Exception ex)
                    {
                        Invoke(new MethodInvoker(() => tipsLabel.Text = "ERROR AUTO SAVING: " + ex.Message.Substring(0, 20)));
                        System.Threading.Thread.Sleep(3000);
                    }
                    Invoke(new MethodInvoker(() => tipsLabel.Text = ""));
                });
            }
        }

        void worldView_Frame(float dtime)
        {
            fpsLabel.Text = worldView.FPS + " fps";
            SlimDX.Vector3 world;
            if (worldView.GroundProbe.Intersect(out world))
                mouseGroundPositionLabel.Text = 
                    world.X.ToString("#.00").PadLeft(6, ' ') + ";" +
                    world.Y.ToString("#.00").PadLeft(6, ' ') + ";" +
                    world.Z.ToString("#.00").PadLeft(6, ' ');
            else
                mouseGroundPositionLabel.Text = " - ";
        }

        void worldView_InitEvent(object sender, EventArgs e)
        {
            New(new Client.Game.Map.MapSettings());
            worldView.Focus();
        }

        void worldView_StateChanged(object sender, EventArgs e)
        {
            modeComboBox.SelectedItem = worldView.StateName;
            stateSettingsPropertyGrid.SelectedObject = worldView.StateSettings;
            splitContainer5.Panel1.Controls.Clear();
            if(worldView.StatePanel != null)
                splitContainer5.Panel1.Controls.Add(worldView.StatePanel);
        }


        public static MainWindow Instance;

        public Client.Game.Map.Map CurrentMap { get; set; }

        String fileName;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.AddExtension = true;
            d.DefaultExt = "map";
            d.Filter = "Map file (*.map)|*.map";
            if (d.ShowDialog() == DialogResult.Cancel) return;

            Open(d.FileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        void New(Client.Game.Map.MapSettings settings)
        {
            Text = "Leditor | Untitled";
            if (CurrentMap != null)
                CurrentMap.Ground.SplatMap1.Resource9.Data.Dispose();
            if (CurrentMap != null)
                CurrentMap.Ground.SplatMap2.Resource9.Data.Dispose();
            CurrentMap = Client.Game.Map.Map.New(settings, worldView.Device9);
            InitMap();
        }

        void ReOpen()
        {
            Open(fileName);
        }

        void Open(String filename)
        {
            if (CurrentMap != null)
                CurrentMap.Ground.SplatMap1.Resource9.Data.Dispose();
            if (CurrentMap != null)
                CurrentMap.Ground.SplatMap2.Resource9.Data.Dispose();
            fileName = filename;
            Text = "Leditor | " + System.IO.Path.GetFileNameWithoutExtension(filename);

            MapLoadingScreen mls = new MapLoadingScreen { Dock = DockStyle.Fill };
            Form f = new Form
            {
                Size = mls.Size,
                TopMost = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                ControlBox = false
            };
            f.Controls.Add(mls);
            f.Show();

            Client.Game.Map.Map map;
            var ls = Client.Game.Map.MapPersistence.Instance.GetLoadSteps(filename, Instance.worldView.Device9, out map);
            CurrentMap = map;
            foreach (var l in ls)
            {
                mls.AppendLoadingString(l.First);
                Application.DoEvents();
                l.Second();
            }
            
            mls.AppendLoadingString("Initializing map..");
            Application.DoEvents();
            InitMap();
            mls.AppendLoadingString("Done!");
            Application.DoEvents();
            f.Close();
            tipsLabel.Text = "Loading took " + ((int)mls.Time.TotalSeconds) + "s";
        }

        void InitMap()
        {
            worldView.InitMap(CurrentMap);
        }

        void Save()
        {
            if (fileName == null)
                SaveAs();
            else
                SaveAs(fileName);
        }

        void SaveAs()
        {
            SaveFileDialog d = new SaveFileDialog();
            d.AddExtension = true;
            d.DefaultExt = "map";
            d.Filter = "Map file (*.map)|*.map";
            if (d.ShowDialog() == DialogResult.Cancel) return;

            fileName = d.FileName;
            Text = "Leditor | " + System.IO.Path.GetFileNameWithoutExtension(fileName);

            SaveAs(fileName);
        }

        void SaveAs(String fileName)
        {
            worldView.Compile();
            if(CurrentMap.Settings.Name == null)
                CurrentMap.Settings.Name = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            String e = "";
            CurrentMap.CheckMap((s) => e += s + "\r\n");
            if (String.IsNullOrEmpty(e) || MessageBox.Show("Errors, save anyway?\r\n\r\n" + e, "Map errors", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Client.Game.Map.MapPersistence.Instance.Save(CurrentMap, fileName);
        }

        /*private void refreshSceneTreeButton_Click(object sender, EventArgs e)
        {
            sceneTreeView.Nodes.Clear();
            TreeNode root = new TreeNode();
            PopulateSceneTreeView(root, CurrentMap.SceneRoot);
            sceneTreeView.Nodes.Add(root);
            root.Expand();
        }*/

        /*void PopulateSceneTreeView(TreeNode n, Graphics.Entity e)
        {
            n.Text = e.GetType().Name + ": " + e.Name;
            n.Tag = e;
            foreach (var et in e.Children)
            {
                TreeNode n2 = new TreeNode();
                PopulateSceneTreeView(n2, et);
                n.Nodes.Add(n2);
            }
        }*/

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewMapDialog d = new NewMapDialog();
            if (d.ShowDialog() != DialogResult.OK) return;
            New(d.Settings);
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            String mode = ((string)modeComboBox.SelectedItem);
            worldView.StartMode(mode);
        }


        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs("Maps/TempMap");

            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo("../Client/bin/Debug/Client.exe", "-QuickStartMap=TempMap");
            psi.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            psi.UseShellExecute = false;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
            p.WaitForExit();
            System.IO.File.Delete("Maps/TempMap");
        }

        private void goToMainCharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            worldView.MoveCamera(CurrentMap.MainCharacter.Translation);
        }

        private void toogleEditorCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //worldView.EditorCamera = !worldView.EditorCamera;
            worldView.ResetCamera();
        }

        private void selectAllEntitiesOfTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.WindowsForms.SelectTypeDialog d = new Common.WindowsForms.SelectTypeDialog();
            d.LoadTypes(typeof(Client.Game.Map.GameEntity).Assembly, typeof(Client.Game.Map.GameEntity));
            if (d.ShowDialog() == DialogResult.Cancel) return;
            var type = (Type)d.SelectedObject;
            List<Graphics.Entity> toSelect = new List<Graphics.Entity>();
            foreach (var v in worldView.Scene.AllEntities)
                if (type.IsAssignableFrom(v.GetType()))
                    toSelect.Add(v);
            worldView.SelectEntities(toSelect.ToArray());
        }

        Timer autoSaveTimer;

        private void droppablesListView_MouseEnter(object sender, EventArgs e)
        {
            tipsLabel.Text = "You can select several entities";
        }

        private void droppablesListView_MouseLeave(object sender, EventArgs e)
        {
            tipsLabel.Text = "";
        }

        private void fogToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            worldView.FogEnabled = fogToolStripMenuItem.Checked;
        }

        private void dumpContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(System.IO.File.Exists("ContentDump.txt"))
                System.IO.File.Delete("ContentDump.txt");
            System.IO.File.AppendAllText("ContentDump.txt", worldView.Content.Dump());
        }

        private void scriptEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scriptEditorForm == null || scriptEditorForm.IsDisposed)
            {
                scriptEditorForm = new Form { Size = new Size(800, 500) };
                ScriptsEditor s;
                scriptEditorForm.Controls.Add(s = new ScriptsEditor { Dock = DockStyle.Fill });
                s.LoadScripts(CurrentMap.Scripts);
            }

            if (!scriptEditorForm.Visible)
                scriptEditorForm.Show();
            else
                scriptEditorForm.Hide();
        }
        System.Windows.Forms.Form scriptEditorForm;


        private void setMapSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PropertyGridDialog d = new PropertyGridDialog();
            d.SelectedObject = new RectangleF
            {
                X = CurrentMap.Ground.Translation.X,
                Y = CurrentMap.Ground.Translation.Y,
                Width = CurrentMap.Settings.Size.Width,
                Height = CurrentMap.Settings.Size.Height,
            };
            if (d.ShowDialog() == DialogResult.Cancel) return;
            CurrentMap.Resize(worldView.Device9, (RectangleF)d.SelectedObject);
            InitMap();
        }

        private void addOrientationToAllObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.WindowsForms.PromptDialog d = new Common.WindowsForms.PromptDialog();
            float val;
            if(d.ShowDialog() == DialogResult.Cancel || !float.TryParse(d.Value, out val)) return;
            foreach (var v in worldView.Scene.AllEntities)
                if (v is Client.Game.Map.GameEntity)
                    ((Client.Game.Map.GameEntity)v).Orientation += val;
        }

        private void reinitSelectedEntitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var v in worldView.SelectedEntities)
                if (v is Client.Game.Map.GameEntity)
                    ((Client.Game.Map.GameEntity)v).EditorInit();
        }

        private void mapSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PropertyGridDialog d = new PropertyGridDialog();
            d.SelectedObject = CurrentMap.Settings;
            if (d.ShowDialog() == DialogResult.Cancel) return;
        }

        private void resaveAllMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tipsLabel.Text = "Starting resave..";
            Application.DoEvents();
            foreach (var v in System.IO.Directory.GetFiles("Maps"))
            {
                if (v.EndsWith(".map"))
                {
                    try
                    {
                        var m = Client.Game.Map.MapPersistence.Instance.Load(v, worldView.Device9);
                        Client.Game.Map.MapPersistence.Instance.Save(m, v);
                        tipsLabel.Text = "Resaved " + v;
                    }
                    catch (Exception ex)
                    {
                        tipsLabel.Text = "Failed to resave " + v;
                    }
                    Application.DoEvents();
                }
            }
            tipsLabel.Text = "Resave done!";
        }

        private void setStaticDataSourceMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.AddExtension = true;
            d.DefaultExt = "map";
            d.Filter = "Map file (*.map)|*.map";
            if (d.ShowDialog() == DialogResult.Cancel) return;

            Client.Game.Map.MapPersistence.Instance.RemoveAllFiles(CurrentMap, fileName);
            CurrentMap.Settings.StaticDataSourceMap = System.IO.Path.GetFileName(d.FileName);
            InitMap();
        }

        private void createCameraPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var p = new Client.Game.Map.Point
            {
                Translation = worldView.Scene.Camera.Position
            };
            p.EditorCreateNewName(worldView.Scene, "CameraPosition");
            CurrentMap.StaticsRoot.AddChild(p);

            var l = new Client.Game.Map.Point
            {
                Translation = ((Graphics.LookatCamera)worldView.Scene.Camera).Lookat
            };
            l.EditorCreateNewName(worldView.Scene, "CameraLookat");
            CurrentMap.StaticsRoot.AddChild(l);
        }

        private void stringEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stringEditor != null)
            {
                stringEditor.Close();
            }
            else
            {
                stringEditor = new Form { Width = 800, Height = 500 };
                var c = new Common.WindowsForms.StringLocalizationStorageEditor
                {
                    StringLocalizationStorage = CurrentMap.StringLocalizationStorage,
                    Dock = DockStyle.Fill
                };
                stringEditor.Controls.Add(c);
                stringEditor.FormClosed += new FormClosedEventHandler((o, e2) => { c.SaveStorage(); stringEditor = null; });
                stringEditor.Show();
            }
        }
        Form stringEditor;


    }
}
