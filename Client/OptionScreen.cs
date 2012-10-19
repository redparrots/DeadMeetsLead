using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics.Interface;
using Graphics;
using Client.Sound;
using System.ComponentModel;
using System.Reflection;
using System.Resources;

namespace Client
{
    public class SettingsWindow : Window
    {
        private ButtonBase ok, cancel, apply;
        public SettingsWindow()
        {
            ControlBox = false;
            AddChild(ok = new StoneButton
            {
                Anchor = global::Graphics.Orientation.Bottom,
                Position = new SlimDX.Vector2(-130, 15),
                Text = Locale.Resource.GenOk
            });
            AddChild(cancel = new StoneButton
            {
                Anchor = global::Graphics.Orientation.Bottom,
                Position = new SlimDX.Vector2(0, 15),
                Text = Locale.Resource.GenCancel
            });
            AddChild(apply = new StoneButton
            {
                Anchor = global::Graphics.Orientation.Bottom,
                Position = new SlimDX.Vector2(130, 15),
                Text = Locale.Resource.GenApply
            });

            ok.Click += new EventHandler(ok_Click);
            cancel.Click += new EventHandler(cancel_Click);
            apply.Click += new EventHandler(apply_Click);
        }

        public SlimDX.Vector2 textBoxSize = new SlimDX.Vector2(200, 40);
        public float textBoxPositionStartY = 55;
        public float textBoxPositionOffset = 35;
        public Graphics.Orientation textBoxOrientation = global::Graphics.Orientation.TopLeft;
        public float textBoxPositionX = 5;

        public SlimDX.Vector2 dropBoxSize = new SlimDX.Vector2(100, 20);
        public float dropBoxPositionStartY = 50;
        public float dropBoxPositionOffset = 35;
        public Graphics.Orientation dropBoxOrientation = global::Graphics.Orientation.TopRight;
        public float dropBoxPositionX = 5;

        public void CreateTitle(string name)
        {
            AddChild(new Label
            {
                Anchor = global::Graphics.Orientation.Top,
                Background = null,
                Position = new SlimDX.Vector2(0, 20),
                AutoSize = AutoSizeMode.Full,
                Text = name,
                TextAnchor = global::Graphics.Orientation.Center,
                Font = new Graphics.Content.Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = System.Drawing.Color.White
                }
            });
        }

        void apply_Click(object sender, EventArgs e)
        {
            TriggerApply();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Closing();
            Close();
        }

        void ok_Click(object sender, EventArgs e)
        {
            TriggerApply();
            Closing();
            Close();
        }

        public virtual void Closing()
        {
            ok.Click -= new EventHandler(ok_Click);
            cancel.Click -= new EventHandler(cancel_Click);
            apply.Click -= new EventHandler(apply_Click);
        }
        public virtual void TriggerApply() { }
    }

    public class VolumeControllerAttribute : Attribute { }
    public class ControlsAttribute : Attribute { }
    public class VideoSettingsAttribute : Attribute { }

    public class AudioOptionsWindow : SettingsWindow
    {
        private List<string> percentages;

        public static event Action ApplySettings;

        public AudioOptionsWindow(bool inGame)
        {
            Size = new SlimDX.Vector2(500, 400);
            audioSettings = new PublicAudioSettings(inGame);
            CreateTitle(Locale.Resource.SoundSettingsTitle);

            openedInGame = inGame;

            percentages = new List<string>();
            percentages.Add("0%");
            percentages.Add("10%");
            percentages.Add("20%");
            percentages.Add("30%");
            percentages.Add("40%");
            percentages.Add("50%");
            percentages.Add("60%");
            percentages.Add("70%");
            percentages.Add("80%");
            percentages.Add("90%");
            percentages.Add("100%");

            int i = 0;
            foreach (var v in typeof(PublicAudioSettings).GetProperties())
            {
                if (v.IsDefined(typeof(VolumeControllerAttribute), false))
                {
                    Label t;
                    AddChild(t = new Label
                    {
                        Anchor = textBoxOrientation,
                        Background = null,
                        Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * i),
                        Size = textBoxSize,
                    });

                    StoneDropDownBar s;

                    AddChild(s = new StoneDropDownBar
                    {
                        Anchor = dropBoxOrientation,
                        Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * i)
                    });

                    switch (v.Name)
                    {
                        case "MasterVolume":
                            t.Text = Locale.Resource.SoundMasterVolume;
                            Program.Instance.Tooltip.SetToolTip(s, Locale.Resource.AudioMasterToolTip);
                            break;
                        case "AmbientVolume":
                            t.Text = Locale.Resource.SoundAmbientVolume;
                            Program.Instance.Tooltip.SetToolTip(s, Locale.Resource.AudioAmbientToolTip);
                            break;
                        case "MusicVolume":
                            t.Text = Locale.Resource.SoundMusicVolume;
                            Program.Instance.Tooltip.SetToolTip(s, Locale.Resource.AudioMusicToolTip);
                            break;
                        case "SoundVolume":
                            t.Text = Locale.Resource.SoundEffectsVolume;
                            Program.Instance.Tooltip.SetToolTip(s, Locale.Resource.AudioSoundEffectsToolTip);
                            break;
                        default:
                            break;
                    }

                    dropDownBars.Add(s);
                    for (int j = 0; j < percentages.Count; j++)
                        s.AddItem(percentages[j]);
                    i++;

                    if (audioSettings.AudioDevice.IsValid)
                        s.SelectedItem = "" + Math.Round((float)v.GetValue(audioSettings, null) * 100f) + "%";
                }
            }
            if(!inGame)
                if (Program.Instance != null && Program.Instance.SoundManager != null)
                {
                    var sm = Program.Instance.SoundManager;
                    IEnumerable<AudioDevice> audioDevices = sm.AudioDeviceList;

                    AddChild(new Label
                    {
                        Anchor = textBoxOrientation,
                        Background = null,
                        Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * i),
                        Size = textBoxSize,
                        Text = Locale.Resource.SoundAudioDriver
                    });

                    StoneDropDownBar t;
                    AddChild(t = new StoneDropDownBar
                    {
                        Anchor = dropBoxOrientation,
                        Font = new Graphics.Content.Font
                        {
                            Color = System.Drawing.Color.White,
                            SystemFont = Fonts.SmallSystemFont
                        },
                        Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * i)
                    });

                    Program.Instance.Tooltip.SetToolTip(t, Locale.Resource.AudioDriverToolTip);

                    dropDownBars.Add(t);
                    foreach (var device in audioDevices)
                        t.AddItem(device);
                    i++;

                    if (audioSettings.AudioDevice.IsValid)
                        t.SelectedItem = audioSettings.AudioDevice;
                }
        }

        public override void TriggerApply()
        {
            base.TriggerApply();

            int i = 0;
            foreach (var v in typeof(PublicAudioSettings).GetProperties())
            {
                if (v.IsDefined(typeof(VolumeControllerAttribute), false))
                {
                    if (!string.IsNullOrEmpty(dropDownBars[i].Text))
                        v.SetValue(audioSettings, float.Parse(dropDownBars[i].Text.Substring(0, dropDownBars[i].Text.Length - 1)) * 0.01f, null);
                    i++;
                }
            }
            if (!openedInGame)
            {
                AudioDevice audioDevice = dropDownBars[i].SelectedItem is AudioDevice ? (AudioDevice)dropDownBars[i].SelectedItem : new AudioDevice();
                if (audioDevice.IsValid)
                    audioSettings.AudioDevice = audioDevice;
            }
            i++;

            if (ApplySettings != null)
                ApplySettings();
        }

        private bool openedInGame = false;
        private List<StoneDropDownBar> dropDownBars = new List<StoneDropDownBar>();
        private PublicAudioSettings audioSettings;
    }

    public class ControlsOptionsWindow : SettingsWindow
    {
        public ControlsOptionsWindow()
        {
            Size = new SlimDX.Vector2(500, 520);
            CreateTitle(Locale.Resource.Controls);
            keys.Clear();
            int i = 0;
            foreach (var v in typeof(PublicControlsSettings).GetProperties())
            {
                var a = Attribute.GetCustomAttributes(v, false);
                foreach(ControlsAttribute type in a)
                {
                    Label t;
                    AddChild(t = new Label
                    {
                        Anchor = textBoxOrientation,
                        Background = null,
                        Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY * 1.25f + textBoxPositionOffset * i * 1.5f),
                        Size = textBoxSize
                    });

                    switch (v.Name)
                    {
                        case "MeleeWeapon":
                            t.Text = Locale.Resource.ControlMelee;
                            break;
                        case "RangedWeapon":
                            t.Text = Locale.Resource.ControlRanged;
                            break;
                        case "MoveForward":
                            t.Text = Locale.Resource.ControlMoveForward;
                            break;
                        case "MoveBackward":
                            t.Text = Locale.Resource.ControlMoveBackward;
                            break;
                        case "StrafeLeft":
                            t.Text = Locale.Resource.ControlStrafeLeft;
                            break;
                        case "StrafeRight":
                            t.Text = Locale.Resource.ControlStrafeRight;
                            break;
                        case "Jump":
                            t.Text = Locale.Resource.ControlJump;
                            break;
                        default:
                            t.Text = v.Name;
                            break;
                    }

                    ControlBox c;
                    AddChild(c = new ControlBox(Util.KeyToString((System.Windows.Forms.Keys)v.GetValue(Program.ControlsSettings, null)))
                    {
                        Anchor = dropBoxOrientation,
                        Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + 7.5f + dropBoxPositionOffset * i * 1.5f),
                        TextAnchor = global::Graphics.Orientation.Center
                    });
                    c.OnChanging += new EventHandler(t_OnChanging);
                    c.SwitchedToSameKey += new EventHandler(t_SwitchedToSameKey);
                    textBoxes.Add(c);
                    keys.Add(c.Key);
                    i++;
                }
            }
        }

        void t_SwitchedToSameKey(object sender, EventArgs e)
        {
            foreach(ControlBox cb in textBoxes)
            {
                if (cb.Key.Equals(((ControlBox)sender).Key) && !cb.Equals(sender))
                {
                    cb.Key = System.Windows.Forms.Keys.None;
                    cb.Text = "";
                }
            }
            
        }

        void t_OnChanging(object sender, EventArgs e)
        {
            foreach (ControlBox c in textBoxes)
            {
                if (!c.Equals(sender))
                {
                    c.Reset();
                }
            }
        }

        public override void TriggerApply()
        {
            System.Windows.Forms.KeysConverter a = new System.Windows.Forms.KeysConverter();
            int i = 0;
            foreach (var v in typeof(PublicControlsSettings).GetProperties())
            {
                var a1 = Attribute.GetCustomAttributes(v, false);
                foreach (ControlsAttribute c in a1)
                {
                    string text = textBoxes[i++].Text;
                    if(text == "")
                        v.SetValue(Program.ControlsSettings, System.Windows.Forms.Keys.None, null); // textBoxes[i].Key, null);
                    else
                        v.SetValue(Program.ControlsSettings, (System.Windows.Forms.Keys)a.ConvertFromString(text), null);
                }
            }
        }

        public class ControlBox : StoneCheckbox
        {
            public ControlBox(String s)
            {
                TextAnchor = global::Graphics.Orientation.Center;
                keyConverter = new System.Windows.Forms.KeysConverter();
                if (s == "")
                    Key = System.Windows.Forms.Keys.None;
                else
                    Key = (System.Windows.Forms.Keys)keyConverter.ConvertFromString(s);
                Text = s;
                AutoCheck = false;
            }

            public void Reset()
            {
                if (Key == System.Windows.Forms.Keys.None)
                    Text = "";
                else
                    Text = Util.KeyToString(Key);

                changing = false;
                Checked = false;
            }

            protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
            {
                Text = "";
                changing = true;
                Checked = true;
                // This is a fix so that the user can click on another key while another one is still in binding-mode.
                if (OnChanging != null)
                    OnChanging(this, null);
            }

            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                if (changing)
                {
                    if (e.KeyCode == System.Windows.Forms.Keys.Escape)
                    {
                        changing = false;
                        Checked = false;
                        Reset();
                    }
                    else
                    {
                        keys.Remove(Key);
                        Checked = false;
                        changing = false;
                        Text = Util.KeyToString(e.KeyCode);
                        Key = (System.Windows.Forms.Keys)keyConverter.ConvertFromString(Text);
                        keys.Add(Key);

                        if (keys.Contains(e.KeyCode))
                            if (SwitchedToSameKey != null)
                                SwitchedToSameKey(this, null);
                    }
                }
            }

            System.Windows.Forms.KeysConverter keyConverter;
            public System.Windows.Forms.Keys Key;
            public event EventHandler OnChanging;
            public event EventHandler SwitchedToSameKey;
            bool changing = false;
        }

        private static List<System.Windows.Forms.Keys> keys = new List<System.Windows.Forms.Keys>();
        private List<ControlBox> textBoxes = new List<ControlBox>();
    }

    public class VideoOptionsWindow : SettingsWindow
    {
        public event Action ApplySettings;

        public VideoOptionsWindow()
        {
            //Layout
            Anchor = global::Graphics.Orientation.Center;
            Size = new SlimDX.Vector2(500, 510);
            
            CreateTitle(Locale.Resource.VideoSettingsTitle);

            AddChild(new Label()
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 0),
                Size = textBoxSize,
                Text = Locale.Resource.VideoOverallVideo
            });

            AddChild(overall = new StoneDropDownBar()
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 0)
            });
            Program.Instance.Tooltip.SetToolTip(overall, Locale.Resource.VideoOverallToolTip);

            //overall.AddItem(new DropDownItemVideoQuality(VideoQualities.Custom));
            //overall.AddItem(new DropDownItemVideoQuality(VideoQualities.Low));
            //if (Graphics.GraphicsDevice.SettingConverters.PixelShaderVersion.Major >= 3)
            //{
            //    overall.AddItem(new DropDownItemVideoQuality(VideoQualities.Medium));
            //    overall.AddItem(new DropDownItemVideoQuality(VideoQualities.High));
            //    overall.AddItem(new DropDownItemVideoQuality(VideoQualities.Ultra));
            //}

            //overall.SelectedItem = new DropDownItemVideoQuality(videoSettings.VideoQuality);
            overall.Updateable = true;
            overall.Update += new Graphics.UpdateEventHandler(overall_Update);

            #region VideoSettings

            #region AnimationQuality

            AddChild(new Label()
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 1),
                Size = textBoxSize,
                Text = Locale.Resource.VideoAnimation
            });

            AddChild(animation = new StoneDropDownBar()
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 1)
            });

            Program.Instance.Tooltip.SetToolTip(animation, Locale.Resource.VideoAnimationToolTip);

            //animation.AddItem(new DropDownItemAnimationQuality(Graphics.Renderer.Settings.AnimationQualities.Low));
            //if (Graphics.GraphicsDevice.SettingConverters.PixelShaderVersion.Major >= 3)
            //{
            //    animation.AddItem(new DropDownItemAnimationQuality(Graphics.Renderer.Settings.AnimationQualities.Medium));
            //    animation.AddItem(new DropDownItemAnimationQuality(Graphics.Renderer.Settings.AnimationQualities.High));
            //}
            //animation.SelectedItem = new DropDownItemAnimationQuality(videoSettings.Animations);

            #endregion

            #region AntiAliasing

            AddChild(new Label()
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 2),
                Size = textBoxSize,
                Text = Locale.Resource.VideoAntiAliasing
            });

            AddChild(antiAliasing = new StoneDropDownBar()
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 2),
            });

            Program.Instance.Tooltip.SetToolTip(antiAliasing, Locale.Resource.VideoAntiAliasingToolTip);

            //foreach (var a in global::Graphics.GraphicsDevice.SettingConverters.AntiAliasingConverter.MultiSampleTypes)
            //    antiAliasing.AddItem(new DropDownItem<SlimDX.Direct3D9.MultisampleType>(a));

            //antiAliasing.SelectedItem = new DropDownItemAntiAliasing(videoSettings.AntiAliasing);

            #endregion

            #region LightingQuality

            AddChild(new Label()
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 3),
                Size = textBoxSize,
                Text = Locale.Resource.VideoLightingQuality
            });

            AddChild(lightingQuality = new StoneDropDownBar()
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 3),
            });

            Program.Instance.Tooltip.SetToolTip(lightingQuality, Locale.Resource.VideoLightingToolTip);

            //lightingQuality.AddItem(new DropDownItemLightingQuality(Graphics.Renderer.Settings.LightingQualities.Low));
            //if (Graphics.GraphicsDevice.SettingConverters.PixelShaderVersion.Major >= 3)
            //{
            //    lightingQuality.AddItem(new DropDownItemLightingQuality(Graphics.Renderer.Settings.LightingQualities.Medium));
            //    lightingQuality.AddItem(new DropDownItemLightingQuality(Graphics.Renderer.Settings.LightingQualities.High));
            //}
            //lightingQuality.SelectedItem = new DropDownItemLightingQuality(videoSettings.Lighting);

            #endregion

            #region Resolution

            AddChild(new Label()
            {
                Anchor = textBoxOrientation,
                Background = null,
                Clickable = false,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 4),
                Size = textBoxSize,
                Text = Locale.Resource.VideoResolution
            });

            AddChild(resolution = new StoneDropDownBar()
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 4),
            });

            Program.Instance.Tooltip.SetToolTip(resolution, Locale.Resource.VideoResolutionToolTip);

            //if (videoSettings.WindowMode == global::Graphics.WindowMode.Fullscreen)
            //{
            //    foreach (var a in global::Graphics.GraphicsDevice.SettingConverters.ResolutionListConverter.Resolutions)
            //        resolution.AddItem(a.ToString());

            //    resolution.SelectedItem = videoSettings.Resolution.ToString();
            //}

            #endregion

            #region ShadowQuality

            AddChild(new Label()
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 5),
                Size = textBoxSize,
                Text = Locale.Resource.VideoShadowQuality
            });

            AddChild(shadowQuality = new StoneDropDownBar()
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 5),
            });

            Program.Instance.Tooltip.SetToolTip(shadowQuality, Locale.Resource.VideoShadowsToolTip);

            //shadowQuality.AddItem(new DropDownItemShadowQuality(Graphics.Renderer.Settings.ShadowQualities.NoShadows));
            //if (Graphics.GraphicsDevice.SettingConverters.PixelShaderVersion.Major >= 3)
            //{
            //    shadowQuality.AddItem(new DropDownItemShadowQuality(Graphics.Renderer.Settings.ShadowQualities.Lowest));
            //    shadowQuality.AddItem(new DropDownItemShadowQuality(Graphics.Renderer.Settings.ShadowQualities.Low));
            //    shadowQuality.AddItem(new DropDownItemShadowQuality(Graphics.Renderer.Settings.ShadowQualities.Medium));
            //    shadowQuality.AddItem(new DropDownItemShadowQuality(Graphics.Renderer.Settings.ShadowQualities.High));
            //    shadowQuality.AddItem(new DropDownItemShadowQuality(Graphics.Renderer.Settings.ShadowQualities.Highest));
            //}
            //if (videoSettings.Shadows == Graphics.Renderer.Settings.ShadowQualities.NoShadows)
            //    shadowQuality.SelectedItem = new DropDownItemShadowQuality(Graphics.Renderer.Settings.ShadowQualities.NoShadows);
            //else
            //    shadowQuality.SelectedItem = new DropDownItemShadowQuality(videoSettings.Shadows);

            #endregion

            #region TerrainQuality

            AddChild(new Label()
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 6),
                Size = textBoxSize,
                Text = Locale.Resource.VideoTerrainQuality
            });

            AddChild(terrainQuality = new StoneDropDownBar()
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 6),
            });

            Program.Instance.Tooltip.SetToolTip(terrainQuality, Locale.Resource.VideoTerrainToolTip);

            //terrainQuality.AddItem(new DropDownItemTerrainQuality(Graphics.Renderer.Settings.TerrainQualities.Low));
            //if (Graphics.GraphicsDevice.SettingConverters.PixelShaderVersion.Major >= 3)
            //{
            //    terrainQuality.AddItem(new DropDownItemTerrainQuality(Graphics.Renderer.Settings.TerrainQualities.Medium));
            //    terrainQuality.AddItem(new DropDownItemTerrainQuality(Graphics.Renderer.Settings.TerrainQualities.High));
            //}
            //terrainQuality.SelectedItem = new DropDownItemTerrainQuality(videoSettings.Terrain);

            #endregion

            #region TextureFiltering

            AddChild(new Label()
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 7),
                Size = textBoxSize,
                Text = Locale.Resource.VideoTextureFiltering
            });

            AddChild(textureFiltering = new StoneDropDownBar()
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 7),
            });

            Program.Instance.Tooltip.SetToolTip(textureFiltering, Locale.Resource.VideoTextureFilterToolTip);

            //foreach (var tf in global::Graphics.GraphicsDevice.SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict)
            //    textureFiltering.AddItem(new DropDownItemTextureFiltering(tf.Key));

            //textureFiltering.SelectedItem = new DropDownItemTextureFiltering(videoSettings.TextureFiltering.TextureFilter);

            #endregion

            #region VSync

            AddChild(new Label
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 8),
                Size = textBoxSize,
                Text = Locale.Resource.VideoVerticalSync
            });

            AddChild(vSync = new StoneDropDownBar
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 8),
            });

            Program.Instance.Tooltip.SetToolTip(vSync, Locale.Resource.VideoVerticalSyncToolTip);

            //vSync.AddItem(new DropDownItemVerticalSync(Graphics.GraphicsDevice.VerticalSyncMode.Off));
            //vSync.AddItem(new DropDownItemVerticalSync(Graphics.GraphicsDevice.VerticalSyncMode.On));

            //vSync.SelectedItem = new DropDownItemVerticalSync(videoSettings.VerticalSync);

            #endregion

            #region WindowMode

            AddChild(new Label
            {
                Anchor = textBoxOrientation,
                Background = null,
                Position = new SlimDX.Vector2(textBoxPositionX, textBoxPositionStartY + textBoxPositionOffset * 9),
                Size = textBoxSize,
                Text = Locale.Resource.VideoWindowMode
            });

            AddChild(windowMode = new StoneDropDownBar
            {
                Anchor = dropBoxOrientation,
                Position = new SlimDX.Vector2(dropBoxPositionX, dropBoxPositionStartY + dropBoxPositionOffset * 9),
            });

            Program.Instance.Tooltip.SetToolTip(windowMode, Locale.Resource.VideoDisplayModeToolTip);

            //windowMode.AddItem(new DropDownItemWindowMode(WindowMode.Fullscreen));
            //windowMode.AddItem(new DropDownItemWindowMode(WindowMode.FullscreenWindowed));
            //windowMode.AddItem(new DropDownItemWindowMode(WindowMode.Windowed));

            //windowMode.SelectedItem = new DropDownItemWindowMode(videoSettings.WindowMode);

            //if(videoSettings.WindowMode == WindowMode.Fullscreen)
            //    resolution.Disabled = false;
            //else
            //    resolution.Disabled = true;

            #endregion

            #endregion
        }

        float tic = 1;
        float accTime = 0;

        void UpdateOverallSettings()
        {
            var vq = ((DropDownItem<VideoQualities>)overall.SelectedItem).Value;
            if (vq != VideoQualities.Custom)
            {
                animation.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.AnimationQualities>(SettingProfiles.GetAnimationQuality(vq));
                lightingQuality.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.LightingQualities>(SettingProfiles.GetLightingQuality(vq));
                shadowQuality.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.ShadowQualities>(SettingProfiles.GetShadowQuality(vq));
                terrainQuality.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.TerrainQualities>(SettingProfiles.GetTerrainQuality(vq));
            }
        }

        void overall_Update(object sender, Graphics.UpdateEventArgs e)
        {
            if(((DropDownItem<VideoQualities>)overall.SelectedItem) == null)
                return;
            var videoQuality = ((DropDownItem<VideoQualities>)overall.SelectedItem).Value;
            if (PreviousOverallVideoQuality != videoQuality)
            {
                UpdateOverallSettings();
                PreviousOverallVideoQuality = videoQuality;
            }

            if (videoQuality != VideoQualities.Custom)
            {
                var aq = animation.SelectedItem as DropDownItem<Graphics.Renderer.Settings.AnimationQualities>;
                var lq = lightingQuality.SelectedItem as DropDownItem<Graphics.Renderer.Settings.LightingQualities>;
                var sq = shadowQuality.SelectedItem as DropDownItem<Graphics.Renderer.Settings.ShadowQualities>;
                var tq = terrainQuality.SelectedItem as DropDownItem<Graphics.Renderer.Settings.TerrainQualities>;

                if (aq != null && aq.Value != SettingProfiles.GetAnimationQuality(videoQuality))
                    overall.SelectedItem = new DropDownItem<VideoQualities>(VideoQualities.Custom);
                else if (lq != null && lq.Value != SettingProfiles.GetLightingQuality(videoQuality))
                    overall.SelectedItem = new DropDownItem<VideoQualities>(VideoQualities.Custom);
                else if (sq != null && sq.Value != SettingProfiles.GetShadowQuality(videoQuality))
                    overall.SelectedItem = new DropDownItem<VideoQualities>(VideoQualities.Custom);
                else if (tq != null && tq.Value != SettingProfiles.GetTerrainQuality(videoQuality))
                    overall.SelectedItem = new DropDownItem<VideoQualities>(VideoQualities.Custom);
            }

            accTime += e.Dtime;
            if (tic < accTime)
            {
                UpdateOverallSettings();
                if (((DropDownItem<VideoQualities>)overall.SelectedItem).Value == VideoQualities.Low)
                    shadowQuality.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.ShadowQualities>(Graphics.Renderer.Settings.ShadowQualities.NoShadows);
                accTime = 0;
            }
        }

        public override void Closing()
        {
            base.Closing();
            //videoSettings.Close();
        }

        public override void TriggerApply()
        {
            if (ApplySettings != null)
                ApplySettings();
        }

        class DropDownItemAntiAliasing : DropDownItem<SlimDX.Direct3D9.MultisampleType>
        {
            public DropDownItemAntiAliasing(SlimDX.Direct3D9.MultisampleType antiAliasing) : base(antiAliasing) { }
            public override string Text
            {
                get
                {
                    switch (Value)
                    {
                        case SlimDX.Direct3D9.MultisampleType.None:
                            return Locale.Resource.GenNone;
                        case SlimDX.Direct3D9.MultisampleType.TwoSamples:
                            return String.Format(Locale.Resource.VideoEnumAntiAliasing, 2);
                        case SlimDX.Direct3D9.MultisampleType.FourSamples:
                            return String.Format(Locale.Resource.VideoEnumAntiAliasing, 4);
                        case SlimDX.Direct3D9.MultisampleType.EightSamples:
                            return String.Format(Locale.Resource.VideoEnumAntiAliasing, 8);
                        case SlimDX.Direct3D9.MultisampleType.SixteenSamples:
                            return String.Format(Locale.Resource.VideoEnumAntiAliasing, 16);
                        default:
                            return Locale.Resource.GenNone;
                    }
                }
            }
        }

        class DropDownItem<T>
        {
            public DropDownItem(T value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
            public override bool Equals(object obj)
            {
                if (!(obj is T)) return false;
                return Value.Equals((T)obj);
            }
            public T Value;
            public virtual string Text { get { Enum e = Value as Enum; return Util.GetLocaleResourceString(e); } }
        }

        public VideoQualities PreviousOverallVideoQuality;

        private DropDownBar animation, antiAliasing, lightingQuality, resolution, shadowQuality, terrainQuality, textureFiltering, vSync, windowMode, overall;

        public VideoQualities OverallVideoQuality
        {
            get { return ((DropDownItem<VideoQualities>)overall.SelectedItem).Value; }
            set { overall.SelectedItem = new DropDownItem<VideoQualities>(value); }
        }

        public Graphics.Renderer.Settings.AnimationQualities AnimationQuality
        {
            get { return ((DropDownItem<Graphics.Renderer.Settings.AnimationQualities>)animation.SelectedItem).Value; }
            set { animation.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.AnimationQualities>(value); }
        }

        public Graphics.Renderer.Settings.LightingQualities LightingQuality
        {
            get { return ((DropDownItem<Graphics.Renderer.Settings.LightingQualities>)lightingQuality.SelectedItem).Value; }
            set { lightingQuality.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.LightingQualities>(value); }
        }

        public Graphics.Renderer.Settings.ShadowQualities ShadowQuality
        {
            get { return ((DropDownItem<Graphics.Renderer.Settings.ShadowQualities>)shadowQuality.SelectedItem).Value; }
            set { shadowQuality.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.ShadowQualities>(value); }
        }

        public Graphics.Renderer.Settings.TerrainQualities TerrainQuality
        {
            get { return ((DropDownItem<Graphics.Renderer.Settings.TerrainQualities>)terrainQuality.SelectedItem).Value; }
            set { terrainQuality.SelectedItem = new DropDownItem<Graphics.Renderer.Settings.TerrainQualities>(value); }
        }

        public Graphics.Renderer.TextureFilterEnum TextureFilter
        {
            get { return ((DropDownItem<Graphics.Renderer.TextureFilterEnum>)textureFiltering.SelectedItem).Value; }
            set { textureFiltering.SelectedItem = new DropDownItem<Graphics.Renderer.TextureFilterEnum>(value); }
        }

        public SlimDX.Direct3D9.MultisampleType AntiAliasing
        {
            get { return ((DropDownItem<SlimDX.Direct3D9.MultisampleType>)antiAliasing.SelectedItem).Value; }
            set { antiAliasing.SelectedItem = new DropDownItemAntiAliasing(value); }
        }

        public Graphics.GraphicsDevice.Resolution Resolution
        {
            get { return Graphics.GraphicsDevice.SettingConverters.ResolutionListConverter.ToResolutionFromResolutionString((string)resolution.SelectedItem); }
            set { resolution.SelectedItem = value.ToString(); }
        }

        public Graphics.GraphicsDevice.VerticalSyncMode VSync
        {
            get { return ((DropDownItem<Graphics.GraphicsDevice.VerticalSyncMode>)vSync.SelectedItem).Value; }
            set { vSync.SelectedItem = new DropDownItem<Graphics.GraphicsDevice.VerticalSyncMode>(value); }
        }

        public WindowMode WindowMode
        {
            get { return ((DropDownItem<WindowMode>)windowMode.SelectedItem).Value; }
            set { windowMode.SelectedItem = new DropDownItem<WindowMode>(value); }
        }

        public VideoQualities[] AvailableVideoQualities
        {
            set
            {
                foreach (VideoQualities vq in value)
                    overall.AddItem(new DropDownItem<VideoQualities>(vq));
            }
        }

        public Graphics.Renderer.Settings.AnimationQualities[] AvailableAnimationQualities
        {
            set
            {
                foreach (Graphics.Renderer.Settings.AnimationQualities aq in value)
                    animation.AddItem(new DropDownItem<Graphics.Renderer.Settings.AnimationQualities>(aq));
            }
        }

        public Graphics.Renderer.Settings.LightingQualities[] AvailableLightingQualities {
            set
            {
                foreach (Graphics.Renderer.Settings.LightingQualities lq in value)
                    lightingQuality.AddItem(new DropDownItem<Graphics.Renderer.Settings.LightingQualities>(lq));
            }
        }

        public Graphics.Renderer.Settings.ShadowQualities[] AvailableShadowQualities
        {
            set
            {
                foreach (Graphics.Renderer.Settings.ShadowQualities sq in value)
                    shadowQuality.AddItem(new DropDownItem<Graphics.Renderer.Settings.ShadowQualities>(sq));
            }
        }

        public Graphics.Renderer.Settings.TerrainQualities[] AvailableTerrainQualities
        {
            set
            {
                foreach (Graphics.Renderer.Settings.TerrainQualities tq in value)
                    terrainQuality.AddItem(new DropDownItem<Graphics.Renderer.Settings.TerrainQualities>(tq));
            }
        }

        public Dictionary<Graphics.Renderer.TextureFilterEnum, Graphics.Renderer.TextureFilters> AvailableTextureFilters
        {
            set
            {
                foreach (var tf in value)
                    textureFiltering.AddItem(new DropDownItem<Graphics.Renderer.TextureFilterEnum>(tf.Key));
            }
        }

        public SlimDX.Direct3D9.MultisampleType[] AvailableMultiSampleTypes
        {
            set
            {
                foreach (SlimDX.Direct3D9.MultisampleType mt in value)
                    antiAliasing.AddItem(new DropDownItemAntiAliasing(mt));
            }
        }

        public Graphics.GraphicsDevice.Resolution[] AvailableResolutions
        {
            //get
            //{
            //    return ((IEnumerable<Graphics.GraphicsDevice.Resolution>)resolution.Items).ToArray();
            //}
            set
            {
                foreach(Graphics.GraphicsDevice.Resolution r in value)
                    resolution.AddItem(r.ToString());
            }
        }

        public Graphics.GraphicsDevice.VerticalSyncMode[] AvailableVSyncs
        {
            set
            {
                foreach (Graphics.GraphicsDevice.VerticalSyncMode s in value)
                    vSync.AddItem(new DropDownItem<Graphics.GraphicsDevice.VerticalSyncMode>(s));
            }
        }

        public Graphics.WindowMode[] AvailableWindowModes
        {
            set
            {
                foreach(Graphics.WindowMode wm in value)
                    windowMode.AddItem(new DropDownItem<Graphics.WindowMode>(wm));
            }
        }
    }

    public class OptionScreen : Window
    {
        private bool inGame;
        public bool InGame
        {
            get
            {
                return inGame;
            }
            set
            {
                inGame = value;
                Video.Visible = !inGame;
            }
        }

        public SlimDX.Vector2 textBoxSize = new SlimDX.Vector2(120, 20);
        public float textBoxPositionStartY = 45;
        public float textBoxPositionOffset = 25;
        public Graphics.Orientation textBoxOrientation = global::Graphics.Orientation.TopLeft;
        public float textBoxPositionX = 10;

        public SlimDX.Vector2 dropBoxSize = new SlimDX.Vector2(100, 20);
        public float dropBoxPositionStartY = 45;
        public float dropBoxPositionOffset = 25;
        public Graphics.Orientation dropBoxOrientation = global::Graphics.Orientation.TopRight;
        public float dropBoxPositionX = 70;
        
        public float additionalYOffset = 25;

        private ButtonBase Audio = new StoneButton()
        {
            Position = new SlimDX.Vector2(0, 70),
            Anchor = global::Graphics.Orientation.Top,
            Text = Locale.Resource.Audio,
        };
        private ButtonBase Controls = new StoneButton()
        {
            Position = new SlimDX.Vector2(0, 115),
            Anchor = global::Graphics.Orientation.Top,
            Text = Locale.Resource.Controls,
        };
        private ButtonBase Video = new StoneButton()
        {
            Position = new SlimDX.Vector2(0, 160),
            Anchor = global::Graphics.Orientation.Top,
            Text = Locale.Resource.Video,
        };
        private ButtonBase BackToMainMenu = new StoneButton()
        {
            Position = new SlimDX.Vector2(0, 240),
            Anchor = global::Graphics.Orientation.Top,
            Text = Locale.Resource.GenClose,
        };

        public OptionScreen()
        {
            Anchor = global::Graphics.Orientation.Center;
            AddChild(new Label
            {
                Text = Locale.Resource.Options,
                Font = new Graphics.Content.Font
                {
                    SystemFont = Fonts.HugeSystemFont,
                    Color = System.Drawing.Color.White
                },
                AutoSize = AutoSizeMode.Full,
                Anchor = global::Graphics.Orientation.Top,
                Background = null,
                Position = new SlimDX.Vector2(0, 10),
            });
            AddChild(Audio);
            AddChild(Controls);
            AddChild(Video);

            AddChild(BackToMainMenu);

            Audio.Click += new EventHandler(Audio_Click);
            Controls.Click += new EventHandler(Controls_Click);
            Video.Click += new EventHandler(Video_Click);

            BackToMainMenu.Click += new EventHandler(BackToMainMenu_Click);
            Size = new SlimDX.Vector2(100, 320);
            ControlBox = false;
        }

        void BackToMainMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        void Video_Click(object sender, EventArgs e)
        {
            var video = Program.CreateVideoOptionsWindow();
            Program.Instance.Interface.AddChild(video);
            video.Closed += new EventHandler(video_Closed);
            Visible = false;
        }

        void video_ApplySettings()
        {

        }

        void video_Closed(object sender, EventArgs e)
        {
            Visible = true;
        }

        void Controls_Click(object sender, EventArgs e)
        {
            var controls = new ControlsOptionsWindow
            {
                Anchor = global::Graphics.Orientation.Center,
            };
            Program.Instance.Interface.AddChild(controls);
            controls.Closed += new EventHandler(controls_Closed);
            Visible = false;
        }

        void controls_Closed(object sender, EventArgs e)
        {
            Visible = true;
        }

        void Audio_Click(object sender, EventArgs e)
        {
            var audio = new AudioOptionsWindow(InGame)
            {
                Anchor = global::Graphics.Orientation.Center,
            };
            Program.Instance.Interface.AddChild(audio);
            audio.Closed += new EventHandler(audio_Closed);
            Visible = false;
        }

        void audio_Closed(object sender, EventArgs e)
        {
            Visible = true;
        }
    }
}