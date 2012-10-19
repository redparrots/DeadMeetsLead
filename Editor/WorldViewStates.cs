using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using SlimDX;
using System.Drawing;

namespace Editor
{
    public partial class WorldView
    {
        class IState : FilteredInputHandler
        {
            public IState(WorldView view) { this.view = view; }
            public virtual void OnEnter() { }
            public virtual void OnExit() { }
            public virtual void OnRender() { }
            protected WorldView view;
            public virtual object Settings { get { return null; } }
            public virtual System.Windows.Forms.Control StatePanel { get { return null; } }
        }

        public List<string> GetStateNames()
        { 
            List<string> s = new List<string>();
            foreach (var v in GetType().Assembly.GetTypes())
                if (typeof(IState).IsAssignableFrom(v) && v != typeof(IState) && !v.IsAbstract)
                    s.Add(v.Name);
            return s;
        }

        abstract class ScaleRotateElevateState : IState
        {
            public ScaleRotateElevateState(WorldView view) : base(view) { }

            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (e.KeyCode == System.Windows.Forms.Keys.R || 
                    e.KeyCode == System.Windows.Forms.Keys.S ||
                    e.KeyCode == System.Windows.Forms.Keys.E)
                {
                    InputHandler = null;
                    mouseYStart = view.LocalMousePosition.Y;
                    if (e.KeyCode == System.Windows.Forms.Keys.R)
                        rotating = true;
                    else if (e.KeyCode == System.Windows.Forms.Keys.S)
                        scaling = true;
                    else if (e.KeyCode == System.Windows.Forms.Keys.E)
                        elevating = true;
                }
            }
            protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
            {
                base.OnKeyUp(e);
                if (e.KeyCode == System.Windows.Forms.Keys.R ||
                    e.KeyCode == System.Windows.Forms.Keys.S ||
                    e.KeyCode == System.Windows.Forms.Keys.E)
                {
                    InputHandler = GetDefaultInputHandler();
                    if (e.KeyCode == System.Windows.Forms.Keys.R)
                        rotating = false;
                    else if (e.KeyCode == System.Windows.Forms.Keys.S)
                        scaling = false;
                    else if (e.KeyCode == System.Windows.Forms.Keys.E)
                        elevating = false;
                }
            }
            protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (rotating)
                {
                    float d = (e.Y - mouseYStart) / 100f;
                    mouseYStart = e.Y;
                    foreach (var v in GetAdjustables())
                        if (v is Client.Game.Map.GameEntity)
                            ((Client.Game.Map.GameEntity)v).Orientation += d;
                }
                if (scaling)
                {
                    float d = (e.Y - mouseYStart) / 100f;
                    mouseYStart = e.Y;
                    foreach (var v in GetAdjustables())
                        if (v is Client.Game.Map.GameEntity)
                            ((Client.Game.Map.GameEntity)v).Scaling += d;
                }
                if (elevating)
                {
                    float d = (e.Y - mouseYStart) / 100f;
                    mouseYStart = e.Y;
                    foreach (var v in GetAdjustables())
                        if (v is Client.Game.Map.GameEntity)
                        {
                            ((Client.Game.Map.GameEntity)v).EditorHeight += d;
                            ((Client.Game.Map.GameEntity)v).Position += Vector3.UnitZ * d;
                        }
                }
            }
            protected abstract IEnumerable<Graphics.Entity> GetAdjustables();
            protected bool IsScalingRotatingElevating { get { return rotating || scaling || elevating; } }
            protected abstract InputHandler GetDefaultInputHandler();
            bool rotating = false, scaling = false, elevating = false;
            int mouseYStart;
        }

        class DefaultState : ScaleRotateElevateState
        {
            public DefaultState(WorldView view) : base(view) { }
            public override void OnEnter()
            {
                Editor = new Graphics.Editors.SceneEditor 
                { 
                    Scene = view.Scene,
                    GroundProbe = view.GroundProbe
                };
                Editor.IsClickableCallback = (e) =>
                {
                    var ge = e as Client.Game.Map.GameEntity;
                    if (ge != null && ge.EditorSelectable)
                    {
                        if (GameEntitiesList.SelectedItems.Count == 0) return true;
                        foreach (System.Windows.Forms.ListViewItem v in GameEntitiesList.SelectedItems)
                            if (((Type)v.Tag) == ge.GetType()) return true;
                    }
                    return false;
                };
                Editor.GetTranslationCallback = (e) => ((Client.Game.Map.GameEntity)e).Position;
                Editor.SetTranslationCallback = (e, v) => ((Client.Game.Map.GameEntity)e).Position = v;
                editorRenderer = new Graphics.Editors.SceneEditor.Renderer9(Editor)
                {
                    BoundingVolumesRenderer = view.bvRenderer
                };
                Editor.SelectionChanged += new EventHandler(Editor_SelectionChanged);
                InputHandler = Editor;
                GameEntitiesList.SelectedItems.Clear();
                defaultStatePanel.GameEntityList = GameEntitiesList;
            }
            protected override IEnumerable<Entity> GetAdjustables()
            {
                return Editor.Selected;
            }
            protected override InputHandler GetDefaultInputHandler()
            {
                return Editor;
            }
            void Editor_SelectionChanged(object sender, EventArgs e)
            {
                defaultStatePanel.SetSelection(Editor.Selected);
            }
            public override void OnExit()
            {
                Editor.SelectionChanged -= new EventHandler(Editor_SelectionChanged);
            }
            public override void OnRender()
            {
                editorRenderer.Render();
            }
            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (e.KeyCode == System.Windows.Forms.Keys.Delete)
                {
                    foreach (Entity et in new List<Entity>(Editor.Selected))
                        et.Remove();
                }
                else if (e.KeyCode == System.Windows.Forms.Keys.C && e.Control)
                {
                    view.clipboard.Clear();
                    view.clipboard.AddRange(Editor.Selected);
                }
                else if (e.KeyCode == System.Windows.Forms.Keys.V && e.Control)
                {
                    if (view.clipboard.Count > 0)
                        view.ChangeState(new DropState(view, () => { return (Client.Game.Map.GameEntity)((Client.Game.Map.GameEntity)view.clipboard.First()).Clone(); }));
                }
            }
            
            public Graphics.Editors.SceneEditor Editor;
            Graphics.Editors.SceneEditor.Renderer9 editorRenderer;


            public override System.Windows.Forms.Control StatePanel
            {
                get
                {
                    return defaultStatePanel;
                }
            }

            public static GameEntitiesList GameEntitiesList = new GameEntitiesList { Dock = System.Windows.Forms.DockStyle.Fill };
            static DefaultStatePanel defaultStatePanel = new DefaultStatePanel { Dock = System.Windows.Forms.DockStyle.Fill };
        }


        public delegate Graphics.Entity NewDropDelegate();
        class DropState : ScaleRotateElevateState
        {
            public DropState(WorldView view)
                : base(view)
            {
                this.newDropCallback = () => { return null; };
                CreateNewDrop();
            }
            public DropState(WorldView view, NewDropDelegate newDrop)
                : base(view)
            {
                this.newDropCallback = newDrop;
                CreateNewDrop();
            }
            public override void OnEnter()
            {
                base.OnEnter();
                DefaultState.GameEntitiesList.SelectedIndexChanged += new EventHandler(gel_SelectedIndexChanged);
                gel_SelectedIndexChanged(null, null);
            }
            void gel_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (DefaultState.GameEntitiesList.SelectedItems.Count > 0)
                {
                    Random random = new Random();
                    newDropCallback = () =>
                        {
                            var t = (Type)DefaultState.GameEntitiesList.SelectedItems[
                                random.Next(DefaultState.GameEntitiesList.SelectedItems.Count)].Tag;
                            return (Graphics.Entity)Activator.CreateInstance(t);
                        };
                    //Eewh.. damn Select method
                    System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                    {
                        System.Threading.Thread.Sleep(300);
                        MainWindow.Instance.Invoke(new System.Windows.Forms.MethodInvoker(() => view.Select()));
                    });
                    if (dropping != null)
                        dropping.Remove();
                    CreateNewDrop();
                }
            }
            public override void OnExit()
            {
                if (dropping != null)
                    dropping.Remove();
                DefaultState.GameEntitiesList.SelectedIndexChanged -= new EventHandler(gel_SelectedIndexChanged);
            }
            protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
            {
                base.OnMouseDown(e);
                isDropping = true;
                dropAcc = dropSettings.DropPeriod + 1;
                dropInitAcc = 0;
                TryDoDrop();
            }
            protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
            {
                base.OnMouseUp(e);
                isDropping = false;
            }
            protected override void OnUpdate(UpdateEventArgs e)
            {
                base.OnUpdate(e);
                if (isDropping)
                {
                    dropInitAcc += e.Dtime;
                    if (dropInitAcc >= 0.3f)
                    {
                        dropAcc += e.Dtime;
                        if (dropAcc > dropSettings.DropPeriod)
                        {
                            dropAcc = 0;

                            TryDoDrop();
                        }
                    }
                }
            }
            void TryDoDrop()
            {
                if (!dropSettings.IgnorePlacementBoundings && dropping != null)
                {
                    object b = ((Client.Game.Map.GameEntity)dropping).EditorPlacementWorldBounding;
                    if (b != null && view.PlacementBoundings.Cull(b).Count > 1)
                        return;
                }

                CreateNewDrop();
            }
            protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (dropping == null) return;
                if(!IsScalingRotatingElevating)
                    UpdateDroppingPosition();
            }
            protected override IEnumerable<Entity> GetAdjustables()
            {
                yield return dropping;
            }
            protected override InputHandler GetDefaultInputHandler()
            {
                return null;
            }
            void UpdateDroppingPosition()
            {
                Vector3 world;
                if (view.GroundProbe.Intersect(dropping, out world))
                    dropping.Position = world;
            }
            void CreateNewDrop()
            {
                Graphics.Entity e = newDropCallback();
                if (e == null) return;
                if (e is Client.Game.Map.GameEntity)
                {
                    var e_ = (Client.Game.Map.GameEntity)e;
                    e_.Map = MainWindow.Instance.CurrentMap;
                    e_.EditorInit();
                    e_.EditorCreateNewName(MainWindow.Instance.worldView.Scene);
                }
                dropping = e as Client.Game.Map.GameEntity;
                if(e is Client.Game.Map.GameEntity && !((Client.Game.Map.GameEntity)e).Dynamic)
                    MainWindow.Instance.CurrentMap.StaticsRoot.AddChild(e);
                else
                    MainWindow.Instance.CurrentMap.DynamicsRoot.AddChild(e);
                UpdateDroppingPosition();
            }
            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (e.KeyCode == System.Windows.Forms.Keys.N)
                {
                    dropping.Remove();
                    CreateNewDrop();
                }
                /*if(e.KeyCode == System.Windows.Forms.Keys.R)
                    if (dropping is Map.Prop)
                    {
                        ((Map.Prop)dropping).Orientation+=2;
                        if (((int)((Map.Prop)dropping).Orientation) >= Enum.GetValues(typeof(Orientation)).Length)
                            ((Map.Prop)dropping).Orientation = (Orientation)1;
                    }*/
            }
            
            public override System.Windows.Forms.Control StatePanel
            {
                get
                {
                    return DefaultState.GameEntitiesList;
                }
            }
            public override object Settings
            {
                get
                {
                    return dropSettings;
                }
            }
            class DropSettings
            {
                public float DropPeriod { get; set; }
                public bool IgnorePlacementBoundings { get; set; }
                public DropSettings()
                {
                    DropPeriod = 0.02f;
                    IgnorePlacementBoundings = false;
                }
            }
            static DropSettings dropSettings = new DropSettings();
            Random r = new Random();
            Client.Game.Map.GameEntity dropping;
            NewDropDelegate newDropCallback;
            float dropAcc = 0, dropInitAcc = 0;
            bool isDropping = false;
        }

        class PathingState : IState
        {
            public PathingState(WorldView view) : base(view) { }
            public override void OnEnter()
            {
                oldInputHandler = view.StateInputHandler;
                editor = new Graphics.Editors.BoundingRegionEditor(view, view.GroundProbe)
                {
                    NodeScale = 0.1f
                };
                editor.Region = MainWindow.Instance.CurrentMap.NavMesh.BoundingRegion;
                editorRenderer = new Graphics.Editors.BoundingRegionEditor.Renderer9(editor)
                {
                    Camera = view.Scene.Camera,
                    StateManager = view.StateManager
                };
                view.StateInputHandler = editor;
            }
            public override void OnExit()
            {
                editor.Compile();
                view.StateInputHandler = oldInputHandler;
            }
            public override void OnRender()
            {
                editorRenderer.Render(view);
            }
            InputHandler oldInputHandler;
            Graphics.Editors.BoundingRegionEditor editor;
            Graphics.Editors.BoundingRegionEditor.Renderer9 editorRenderer;
        }


        class RegionsState : IState
        {
            public RegionsState(WorldView view) : base(view) { }
            public override void OnEnter()
            {
                oldInputHandler = view.StateInputHandler;

                if (regionsControl == null)
                {
                    regionsControl = new RegionsEditorControl { Dock = System.Windows.Forms.DockStyle.Fill };
                    regionsControl.LoadRegions(MainWindow.Instance.CurrentMap);
                }

                regionsControl.RegionSelected += new EventHandler(regionsControl_RegionSelected);


                editor = new Graphics.Editors.BoundingRegionEditor(view, view.GroundProbe)
                {
                    NodeScale = 0.1f
                };
                editorRenderer = new Graphics.Editors.BoundingRegionEditor.Renderer9(editor)
                {
                    Camera = view.Scene.Camera,
                    StateManager = view.StateManager
                };
                view.StateInputHandler = editor;
            }

            void regionsControl_RegionSelected(object sender, EventArgs e)
            {
                if (regionsControl.SelectedRegion != null)
                    editor.Region = regionsControl.SelectedRegion.BoundingRegion;
                else
                    editor.Region = null;
            }
            public override void OnExit()
            {
                editor.Compile();
                view.StateInputHandler = oldInputHandler;
                regionsControl.RegionSelected -= new EventHandler(regionsControl_RegionSelected);
            }
            public override void OnRender()
            {
                editorRenderer.Render(view);
            }
            public override System.Windows.Forms.Control StatePanel
            {
                get
                {
                    return regionsControl;
                }
            }
            InputHandler oldInputHandler;
            Graphics.Editors.BoundingRegionEditor editor;
            Graphics.Editors.BoundingRegionEditor.Renderer9 editorRenderer;
            static RegionsEditorControl regionsControl;
        }

        class SplattingState : IState
        {
            public SplattingState(WorldView view)
                : base(view)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();

                oldInputHandler = view.StateInputHandler;
                editor1 = new Graphics.Editors.GroundTextureEditor
                {
                    Camera = view.Scene.Camera,
                    GroundIntersect = view.GroundProbe,
                    SoftwareTexture = new Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8>[]
                    {
                        new Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8>(
                            TextureUtil.ReadTexture<Graphics.Software.Texel.A8R8G8B8>(MainWindow.Instance.CurrentMap.Ground.SplatMap1.Resource9, 0)),
                        new Graphics.Software.Texture<Graphics.Software.Texel.A8R8G8B8>(
                            TextureUtil.ReadTexture<Graphics.Software.Texel.A8R8G8B8>(MainWindow.Instance.CurrentMap.Ground.SplatMap2.Resource9, 0)),
                    },
                    Texture9 = new SlimDX.Direct3D9.Texture[]
                    {
                        MainWindow.Instance.CurrentMap.Ground.SplatMap1.Resource9,
                        MainWindow.Instance.CurrentMap.Ground.SplatMap2.Resource9
                    },
                    Position = MainWindow.Instance.CurrentMap.Ground.Translation,
                    Size = MainWindow.Instance.CurrentMap.Ground.Size,
                    Viewport = view.Viewport
                };
                editor1.TextureValuesChanged += new Graphics.Editors.TextureValuesChangedEventHandler(editor_TextureValuesChanged);
                editorRenderer = new Graphics.Editors.GroundTextureEditorRenderer(editor1);
                editor1.Pencil = settings.Pencil1;

                view.StateInputHandler = editor1;
            }
            public override void OnRender()
            {
                base.OnRender();
                editorRenderer.Render(view, view.Scene.Camera);
            }

            void editor_TextureValuesChanged(object sender, Graphics.Editors.TextureValuesChangedEvent e)
            {
                //MainWindow.Instance.CurrentMap.Ground.UpdatePieceTextures(e.ChangedRegion);
            }

            public override void OnExit()
            {
                base.OnExit();
                view.StateInputHandler = oldInputHandler;
            }

            enum Textures
            {
                Mud,
                Rock,
                Pebbles,
                Sand,
                Grass,
                Moss,
                Mayatile,
                Grass2,
                Test4
            }

            class SplattingSettings
            {
                public SplattingSettings()
                {
                    opacity = 100;
                }

                public Graphics.Editors.GroundTexturePencil Pencil1 = new Graphics.Editors.GroundTexturePencil
                {
                    Radius = 1,
                    Type = Graphics.Editors.GroundTexturePencilType.AddReplace,
                    Color = new Vector4(-1, -1, -1, -1),
                    Colors = new float[8]
                };

                public Graphics.Editors.GroundTexturePencil Pencil2 = new Graphics.Editors.GroundTexturePencil
                {
                    Radius = 1,
                    Type = Graphics.Editors.GroundTexturePencilType.AddSaturate,
                    Color = new Vector4(-1, -1, -1, -1)
                };

                private int opacity;
                public int Opacity
                {
                    get { return opacity; }
                    set
                    {
                        opacity = value;
                        Texture = texture;
                    }
                }

                public Graphics.Editors.GroundTexturePencilType PencilType
                {
                    get
                    {
                        return Pencil1.Type;
                    }
                    set
                    {
                        Pencil1.Type = value;
                    }
                }

                public float Radius { get { return Pencil1.Radius; } set { Pencil1.Radius = value; Pencil2.Radius = value; } }

                public float Hardness { get { return Pencil1.Hardness; } set { Pencil1.Hardness = value; Pencil2.Hardness = value; } }

                public float MousePaintPeriod { get { return Pencil1.MousePaintPeriod; } set { Pencil1.MousePaintPeriod = value; } }

                Textures texture;
                public Textures Texture
                {
                    get
                    {
                        return texture;
                    }
                    set
                    {
                        texture = value;
                        if (value == Textures.Rock)
                        {
                            Pencil1.Colors[0] = 1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = -1 * ((float)opacity / 100.0f);
                        }
                        else if (value == Textures.Pebbles)
                        {
                            Pencil1.Colors[0] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = 1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = -1 * ((float)opacity / 100.0f);
                        }
                        else if (value == Textures.Sand)
                        {
                            Pencil1.Colors[0] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = 1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = -1 * ((float)opacity / 100.0f);
                        }
                        else if (value == Textures.Grass)
                        {
                            Pencil1.Colors[0] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = 1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = -1 * ((float)opacity / 100.0f);
                        }
                        else if (value == Textures.Moss)
                        {
                            Pencil1.Colors[0] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = 1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = -1 * ((float)opacity / 100.0f);
                        }
                        else if (value == Textures.Mayatile)
                        {
                            Pencil1.Colors[0] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = 1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = -1 * ((float)opacity / 100.0f);
                        }
                        else if (value == Textures.Grass2)
                        {
                            Pencil1.Colors[0] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = 1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = -1 * ((float)opacity / 100.0f);
                        }
                        else if (value == Textures.Test4)
                        {
                            Pencil1.Colors[0] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = 1 * ((float)opacity / 100.0f);
                        }
                        else if (value == Textures.Mud)
                        {
                            Pencil1.Colors[0] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[1] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[2] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[3] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[4] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[5] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[6] = -1 * ((float)opacity / 100.0f);
                            Pencil1.Colors[7] = -1 * ((float)opacity / 100.0f);
                        }
                    }
                }
            }
            static SplattingSettings settings = new SplattingSettings();
            public override object Settings
            {
                get
                {
                    return settings;
                }
            }
            InputHandler oldInputHandler;
            Graphics.Editors.GroundTextureEditor editor1;
            Graphics.Editors.GroundTextureEditorRenderer editorRenderer;
        }

        class HeightmapState : IState
        {
            public HeightmapState(WorldView view)
                : base(view)
            {
            }
            public override void OnEnter()
            {
                base.OnEnter();
                oldInputHandler = view.StateInputHandler;
                editor = new Graphics.Editors.GroundTextureEditor
                {
                    Camera = view.Scene.Camera,
                    GroundIntersect = view.GroundProbe,
                    SoftwareTexture = new Graphics.Software.Texture<Graphics.Software.Texel.R32F>[]
                    {
                        new Graphics.Software.Texture<Graphics.Software.Texel.R32F>(
                            MainWindow.Instance.CurrentMap.Ground.Heightmap)
                    },
                    Position = MainWindow.Instance.CurrentMap.Ground.Translation,
                    Size = MainWindow.Instance.CurrentMap.Ground.Size,
                    Viewport = view.Viewport
                };
                editor.TextureValuesChanged += new Graphics.Editors.TextureValuesChangedEventHandler(editor_TextureValuesChanged);
                editorRenderer = new Graphics.Editors.GroundTextureEditorRenderer(editor);
                InputHandler = editor;
                view.StateInputHandler = this;
                editor.Pencil = pencil;
            }
            public override void OnRender()
            {
                base.OnRender();
                editorRenderer.Render(view, view.Scene.Camera);

                Vector3 worldPos;
                if (!editor.GroundIntersect.Intersect(out worldPos)) return;
                view.DrawCircle(editor.Camera, Matrix.Identity, worldPos + Vector3.UnitZ * pencil.Color.X, 
                    editor.Pencil.Radius, 12, Color.LightYellow);
            }
            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                base.OnKeyDown(e);
                if (e.KeyCode == System.Windows.Forms.Keys.R)
                    pencil.Color = new Vector4(pencil.Color.X + 0.03f, 0, 0, 0);
                else if (e.KeyCode == System.Windows.Forms.Keys.E)
                    pencil.Color = new Vector4(pencil.Color.X - 0.03f, 0, 0, 0);
                else if (e.KeyCode == System.Windows.Forms.Keys.A)
                    pencil.Type = Graphics.Editors.GroundTexturePencilType.Add;
                else if (e.KeyCode == System.Windows.Forms.Keys.S)
                    pencil.Type = Graphics.Editors.GroundTexturePencilType.Flatten;
                else if (e.KeyCode == System.Windows.Forms.Keys.D)
                    pencil.Type = Graphics.Editors.GroundTexturePencilType.Set;
                else if (e.KeyCode == System.Windows.Forms.Keys.F)
                    pencil.Type = Graphics.Editors.GroundTexturePencilType.Smooth;
            }

            void editor_TextureValuesChanged(object sender, Graphics.Editors.TextureValuesChangedEvent e)
            {
                MainWindow.Instance.CurrentMap.Ground.UpdatePieceMeshes(e.ChangedRegion);

                var bb = new BoundingBox();
                bb.Minimum = Common.Math.ToVector3(MainWindow.Instance.CurrentMap.Settings.Position) +
                    new Vector3(e.ChangedRegion.X * MainWindow.Instance.CurrentMap.Settings.Size.Width,
                        e.ChangedRegion.Y * MainWindow.Instance.CurrentMap.Settings.Size.Height, float.MinValue);
                bb.Maximum = Common.Math.ToVector3(MainWindow.Instance.CurrentMap.Settings.Position) +
                    new Vector3(e.ChangedRegion.Right * MainWindow.Instance.CurrentMap.Settings.Size.Width,
                        e.ChangedRegion.Bottom * MainWindow.Instance.CurrentMap.Settings.Size.Height, float.MaxValue);

                foreach(var v in view.sceneQuadtree.Cull(bb))
                    if(v is Client.Game.Map.GameEntity && ((Client.Game.Map.GameEntity)v).EditorSelectable &&
                        !((Client.Game.Map.GameEntity)v).EditorLockTranslation)
                    {
                        ((Client.Game.Map.GameEntity)v).EditorHeightmapChanged(view.GroundProbe);
                    }
            }
            public override void OnExit()
            {
                base.OnExit();
                view.StateInputHandler = oldInputHandler;
            }
            InputHandler oldInputHandler;
            Graphics.Editors.GroundTextureEditor editor;
            Graphics.Editors.GroundTextureEditorRenderer editorRenderer;
            Graphics.Editors.GroundTexturePencil pencil = new Graphics.Editors.GroundTexturePencil
            {
                Color = new Vector4(0.1f, 0, 0, 0),
                Radius = 5,
            };
            public override object Settings
            {
                get
                {
                    return pencil;
                }
            }

        }



        class CamerasState : IState
        {
            public CamerasState(WorldView view) : base(view) { }
            public override void OnEnter()
            {
                cameraAnglesEditor = new CameraAnglesEditorControl { Dock = System.Windows.Forms.DockStyle.Fill };
                cameraAnglesEditor.CameraAngles = MainWindow.Instance.CurrentMap.CameraAngles;
            }


            public override System.Windows.Forms.Control StatePanel
            {
                get
                {
                    return cameraAnglesEditor;
                }
            }
            CameraAnglesEditorControl cameraAnglesEditor;
        }
    }


}
