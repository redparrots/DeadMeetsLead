using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Client.Sound;
using SlimDX;
using Graphics;

namespace FMODTest
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            soundManager = new SoundManager(new AudioDevice(), ManagerGlue.FMOD, 1, 40) { ContentPath = "Data/Sound/" };
            soundManager.LoadSounds(true);

            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000 / 60;

            Vector3 middle = new Vector3(Size.Width / 2f, Size.Height / 2f, 0);
            position = middle;
            floatingPoint = middle;

            InsertUnit(middle, soundManager.GetSFX(SFX.DogBite1));

            //IPlayable ipSound = soundManager.GetSoundResourceGroup(soundManager.GetSFX(SFX.DogBite1), soundManager.GetSFX(SFX.HitBySword1));
            //intervalChannel = ipSound.PlayLoopedWithIntervals(0.1f, 5f, 2f, GetFloatingPoint, () => { return Vector3.Zero; });

            //movingUnitBasePosition = middle;
            //movingUnit = new Unit { Position = movingUnitBasePosition, Sound = soundManager.GetSFX(SFX.HumanDeath1) };
            ////var channel = movingUnit.Sound.Play(() => { return movingUnit.Position; }, () => { return Vector3.Zero; });
            //movingUnitChannel = movingUnit.Sound.Play();
            //movingUnitChannel.Looping = true;

            //var u1 = InsertUnit(middle + new Vector3(30, 0, 0), soundManager.GetSFX(SFX.DogBite1));
            //var u2 = InsertUnit(middle - new Vector3(30, 0, 0), soundManager.GetSFX(SFX.PiranhaAttack1));
            //panChannel1 = (SoundChannel)u1.Sound.Play(u1.Position, Vector3.Zero);
            //panChannel1.Looping = true;
            //panChannel2 = (SoundChannel)u2.Sound.Play(u2.Position, Vector3.Zero);
            //panChannel2.Looping = true;

            //InsertUnit(middle + new Vector3(30, 30, 0), soundManager.GetSFX(SFX.DogBite1));
            //InsertUnit(middle - new Vector3(30, -30, 0), soundManager.GetSFX(SFX.PiranhaAttack1));
            //InsertUnit(middle - new Vector3(30, 30, 0), soundManager.GetSFX(SFX.Charge1));

            //InsertUnit(middle + new Vector3(30, 30, 0), soundManager.GetSoundResourceGroup(soundManager.GetSFX(SFX.FireBreath1), soundManager.GetSFX(SFX.RottenTreeHitBySword1)));
            //InsertUnit(middle - new Vector3(30, 30, 0), soundManager.GetSFX(SFX.RifleFire1));
            //InsertUnit(middle - new Vector3(30, -30, 0), soundManager.GetSFX(SFX.HumanDeath1));
            
            //InsertUnit(middle + new Vector3(30, 30, 0), new SoundInstanceGroup(soundEffects[Sounds.SFX.HitsFlesh1], soundEffects[Sounds.SFX.SwordHitWood1]));
            //InsertUnit(middle - new Vector3(30, 30, 0), soundEffects[Sounds.SFX.SwordRing3]);
            //InsertUnit(middle - new Vector3(30, -30, 0), soundEffects[Sounds.SFX.MonsterSqueal1]);


            //volumeTestChannel = SoundManager.Instance.GetStream(Stream.InGameMusic4).Play();
            //volumeTestResource = volumeTestChannel.CurrentSoundResource;
            //volumeTestSoundGroup = SoundManager.Instance.GetSoundGroup(SoundGroups.Music);

            //volumeTestChannelVolume = volumeTestChannel.Volume;
            //volumeTestResourceVolume = volumeTestResource.Volume;
            //volumeTestSoundGroupVolume = volumeTestSoundGroup.Volume;

            //volumeTestChannel = SoundManager.Instance.GetSoundResourceGroup(SoundManager.Instance.GetStream(Graphics.Sound.Stream.InGameMusic1),
            //        SoundManager.Instance.GetStream(Graphics.Sound.Stream.MainMenuMusic1)).PlayLoopedWithIntervals(0.5f, 0.5f, 0.2f);
            //volumeTestChannel = SoundManager.Instance.GetSoundResourceGroup(SoundManager.Instance.GetStream(Graphics.Sound.Stream.InGameMusic2),
            //        SoundManager.Instance.GetStream(Graphics.Sound.Stream.InGameMusic3)).Play();
            //volumeTestChannel = SoundManager.Instance.GetStream(Stream.InGameMusic3).PlayLoopedWithIntervals(0.5f, 0.5f, 0.2f);
            //volumeTestResource = volumeTestChannel.CurrentSoundResource;

            //volumeTestChannelVolume = volumeTestChannel.Volume;

            //uint bufferLength = 0;
            //int numBuffers = 0;
            //((SoundManager)soundManager).FMODSystem.getDSPBufferSize(ref bufferLength, ref numBuffers);

            //var asdf = (SoundResource)soundManager.GetStream(Stream.TestSong);
            //float frequency = 0, dummy = 0;
            //int dummyI = 0;
            //asdf.Sound.getDefaults(ref frequency, ref dummy, ref dummy, ref dummyI);

            timer.Enabled = true;
        }
        SoundChannel panChannel1, panChannel2;
        ISoundChannel movingUnitChannel;
        float panLevel = 1f;

        ISoundChannel intervalChannel;
        Unit movingUnit;
        Vector3 movingUnitBasePosition;

        private Unit InsertUnit(Vector3 position, IPlayable sound)
        {
            var unit = new Unit { Position = position, Sound = sound, Cooldown = 2f };
            units.Add(unit);
            return unit;
        }

        public Vector3 GetFloatingPoint() { return floatingPoint; }
        private Vector3 floatingPoint;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            float speed = 2.5f;
            if (e.KeyCode == Keys.W)
                position.Y -= speed;
            else if (e.KeyCode == Keys.S)
                position.Y += speed;
            else if (e.KeyCode == Keys.A)
                position.X -= speed;
            else if (e.KeyCode == Keys.D)
                position.X += speed;
            else if (e.KeyCode == Keys.Up)
                floatingPoint.Y -= speed;
            else if (e.KeyCode == Keys.Down)
                floatingPoint.Y += speed;
            else if (e.KeyCode == Keys.Left)
                floatingPoint.X -= speed;
            else if (e.KeyCode == Keys.Right)
                floatingPoint.X += speed;
            else if (e.KeyCode == Keys.U)
            {
                if (loopingSoundChannel != null && loopingSoundChannel.Looping)
                    loopingSoundChannel.StopAfterCurrent();
                else
                {
                    loopingSoundChannel = soundManager.GetSFX(SFX.ShotgunFire1).Play(
                        new PlayArgs
                        {
                            Position =
                                new Vector3(Size.Width/2f, Size.Height/2f, 0)
                        });
                }
            }
            else if (e.KeyCode == Keys.I)
            {
                if (loopingSoundChannel != null)
                {
                    loopingSoundChannel.Stop();
                    loopingSoundChannel = null;
                }
            }
            else if (e.KeyCode == Keys.Q)
            {
                stopped = true;
                soundManager.StopAllChannels();
            }
            else if (e.KeyCode == Keys.R)
            {
                intervalChannel.StopAfterCurrent();
            }
            else if (e.KeyCode == Keys.Z)
            {
                intervalChannel.PlaybackSpeed -= 0.1f;
            }
            else if (e.KeyCode == Keys.X)
            {
                intervalChannel.PlaybackSpeed += 0.1f;
            }
            else if (e.KeyCode == Keys.C)
            {
                intervalChannel.Volume -= 0.1f;
            }
            else if (e.KeyCode == Keys.V)
            {
                intervalChannel.Volume += 0.1f;
            }
            else if (e.KeyCode == Keys.T)
            {
                soundManager.Volume -= 0.1f;
            }
            else if (e.KeyCode == Keys.Y)
            {
                soundManager.Volume += 0.1f;
            }
            /*
        else if (e.KeyCode == Keys.Oemplus)
        {
            //soundManager.SetMaxAudible("attacks", ++audible);
            //Text = audible + " channels";
        }
        else if (e.KeyCode == Keys.OemMinus)
        {
            //audible = audible > 0 ? audible - 1 : 0;
            //soundManager.SetMaxAudible("attacks", audible);
            //Text = audible + " audible attack sounds";
        }
        else if (e.KeyCode == Keys.R)
        {
            //List<Sound> list = new List<Sound>(sounds);
            //int index = 0;
            //Random r = new Random();
            //while (list.Count > 0)
            //{
            //    int i = r.Next(list.Count - 1);
            //    units[index++].Sound = list[i];
            //    list.RemoveAt(i);
            //}
        }
        else if (e.KeyCode == Keys.M)
        {
            //var rt = soundStreams[Sounds.Streams.RainThunder].Play();
            //thunderRain = rt.First;
            //thunderRainChannel = rt.Second;

            //Random r = new Random();
            //Unit u = new Unit 
            //{ 
            //    Position = new Vector3(Width / 2f, Height / 2f, 0) + new Vector3((float)(r.NextDouble() * 40 - 20), (float)(r.NextDouble() * 40 - 20), 0),
            //    Sound = soundStreams[Sounds.Streams.RainThunder],
            //    Cooldown = 0.1f
            //};
            //units.Add(u);
        }
            else if (e.KeyCode == Keys.Z)
            {
                rifleSpeed -= 0.1f;
                //soundManager.GetSFX(SFX.RifleFire1).
            }
            else if (e.KeyCode == Keys.X)
            {
                rifleSpeed += 0.1f;
            }
        else if (e.KeyCode == Keys.N)
        {
            //musicVolume = System.Math.Min(1f, musicVolume + 0.1f);
            //if (thunderRainChannel != null)
            //    soundManager.SetChannelVolume(thunderRainChannel, musicVolume);
        }
        else if (e.KeyCode == Keys.B)
        {
            //musicVolume = System.Math.Min(1f, musicVolume - 0.1f);
            //if (thunderRainChannel != null)
            //    soundManager.SetChannelVolume(thunderRainChannel, musicVolume);
        }*/
            else if (e.KeyCode == Keys.OemPipe)
            {
                if ((System.Windows.Forms.Control.ModifierKeys & Keys.Shift) != 0)
                    volumeTestResourceVolume = Clamp(volumeTestResourceVolume - 0.1f, 0, 1);
                else
                    volumeTestResourceVolume = Clamp(volumeTestResourceVolume + 0.1f, 0, 1);
                volumeTestResource.Volume = volumeTestResourceVolume;
            }
            else if (e.KeyCode == Keys.D1)
            {
                if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) != 0)
                    volumeTestChannelVolume = Clamp(volumeTestChannelVolume - 0.1f, 0, 1);
                else
                    volumeTestChannelVolume = Clamp(volumeTestChannelVolume + 0.1f, 0, 1);
                volumeTestChannel.Volume = volumeTestChannelVolume;
            }
            else if (e.KeyCode == Keys.D2)
            {
            }
            else if (e.KeyCode == Keys.D3)
            {
                fadeChannel = soundManager.GetSoundResourceGroup(soundManager.GetStream(Stream.InGameMusic1), 
                    soundManager.GetStream(Stream.ScoreScreenVictoryMusic1)).PlayLoopedWithIntervals(1, 3, 0.3f, new PlayArgs());
                //fadeChannel = soundManager.GetStream(Stream.InGameMusic1).PlayLoopedWithIntervals(0.5f, 0.5f, 2f);
                //fadeChannel = soundManager.GetStream(Stream.InGameMusic1).Play();
            }
            else if (e.KeyCode == Keys.D4)
            {
                if (fadeChannel != null)
                {
                    //fadeChannel.PlaybackStopped += ((sender, ee) => { soundManager.GetStream(Stream.InGameMusic1).Play(3f); });
                    fadeChannel.PlaybackStopped += ((sender, ee) => { fadeChannel = 
                        soundManager.GetStream(Stream.InGameMusic1).PlayLoopedWithIntervals(0.5f, 0.5f, 0, new PlayArgs()); });
                    fadeChannel.Stop(1f);
                }
            }
            else if (e.KeyCode == Keys.D5)
            {
                spread = System.Math.Min(360f, System.Math.Max(0f, spread - 15f));
                //SoundManager.Instance._3DSpread = spread;
            }
            else if (e.KeyCode == Keys.D6)
            {
                spread = System.Math.Min(360f, System.Math.Max(0f, spread + 15f));
                //SoundManager.Instance._3DSpread = spread;
            }
            else if (e.KeyCode == Keys.D8)
            {
                panLevel = System.Math.Min(1, System.Math.Min(1f, panLevel - 0.1f));
                SoundManager.Instance._3DPanLevel = panLevel;
                //panChannel1._3DPanLevel = panLevel;
                //panChannel2._3DPanLevel = panLevel;
            }
            else if (e.KeyCode == Keys.D9)
            {
                panLevel = System.Math.Min(1, System.Math.Min(1f, panLevel + 0.1f));
                SoundManager.Instance._3DPanLevel = panLevel;
                //panChannel1._3DPanLevel = panLevel;
                //panChannel2._3DPanLevel = panLevel;
            }
            else if (e.KeyCode == Keys.D0)
                System.Windows.Forms.MessageBox.Show("PanLevel: " + panLevel + Environment.NewLine + "Spread: " + spread);
            else if (e.KeyCode == Keys.Oemplus)
            {
            }
        }
        float volumeTestResourceVolume = 0.5f;
        float volumeTestChannelVolume = 0.5f;
        float volumeTestSoundGroupVolume = 0.5f;
        ISoundResource volumeTestResource;
        ISoundChannel volumeTestChannel;
        SoundGroup volumeTestSoundGroup;
        ISoundChannel fadeChannel;
        private float spread = 0f;

        private bool stopped = false;
        //private SoundInstance thunderRain;
        private FMOD.Channel thunderRainChannel;
        private float musicVolume = 0.5f;
        private int audible = 3;
        private ISoundChannel loopingSoundChannel;
        //private float rifleSpeed = 1.0f;

        private float Clamp(float value, float min, float max)
        {
            return System.Math.Min(max, System.Math.Max(min, value));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            float radius = 6f;

            foreach (var u in units)
            {
                e.Graphics.DrawEllipse(Pens.Red, u.Position.X - radius, u.Position.Y - radius, 2f * radius, 2f * radius);
                //e.Graphics.DrawString(u.Sound.ID.ToString(), circleFont, Brushes.Black, new PointF(u.Position.X - 5f, u.Position.Y - circleFont.Height/2f));
            }

            if (movingUnit != null)
                e.Graphics.DrawEllipse(Pens.Red, movingUnit.Position.X - radius, movingUnit.Position.Y - radius, 2f * radius, 2f * radius);

            e.Graphics.DrawEllipse(Pens.Black, position.X - radius, position.Y - radius, 2f * radius, 2f * radius);
        }
        Font circleFont = new Font(FontFamily.GenericMonospace, 8f);

        float totalTime = 0f;
        void timer_Tick(object sender, EventArgs e)
        {
            float dtime = 1 / 60f;
            totalTime += dtime;

            if (movingUnit != null)
                movingUnit.Position = movingUnitBasePosition + new Vector3((float)System.Math.Sin(totalTime) * 30f, 0, 0);

            if (!stopped)
                foreach (var u in units)
                {
                    u.Cooldown -= dtime;
                    if (u.Cooldown < 0)
                    {
                        var channel = u.Sound.Play(new PlayArgs{ Position = u.Position});
                        u.Cooldown += channel.CurrentSoundResource.Length;
                        //channel.PlaybackSpeed = rifleSpeed;
                        playingChannels.Add(channel);
                    }
                }

            List<ISoundChannel> toBeRemoved = new List<ISoundChannel>();
            foreach (var c in playingChannels)
            {
                var sc = (SoundChannel)c;
                    //bool isVirtual = false;
                    //sc.Channel.isVirtual(ref isVirtual);
                    //if (isVirtual)
                    //    System.Diagnostics.Debugger.Break();
            }
            foreach (var c in toBeRemoved)
                playingChannels.Remove(c);
            
            soundManager.Update(dtime, position, Vector3.Zero, -Vector3.UnitY, Vector3.UnitZ);
            //Text = "Channels currently playing: " + ((SoundManager)soundManager).DebugPlayingChannels()
            //    + "SRVol: " + volumeTestResourceVolume + ", ChanVol: " + volumeTestChannelVolume + ", SGVol: " + volumeTestSoundGroupVolume;

            Invalidate();
        }
        List<ISoundChannel> playingChannels = new List<ISoundChannel>();

        private class Unit
        {
            public Vector3 Position { get; set; }
            public IPlayable Sound { get; set; }

            public float Cooldown = float.PositiveInfinity;
        }

        Vector3 position;
        List<Unit> units = new List<Unit>();
        List<String> soundGroups = new List<string>();

        //Dictionary<Sounds.SFX, IPlayable> soundEffects;
        //Dictionary<Sounds.Streams, IPlayable> soundStreams;

        ISoundManager soundManager;

        Timer timer;
    }
}
