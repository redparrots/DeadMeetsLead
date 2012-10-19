using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Sound;
using SlimDX;
using System.Windows.Forms;

namespace JMODTest
{
    internal partial class MainWindow
    {
        public abstract class TestState
        {
            public TestState()
            {
                soundManager = new SoundManager(new AudioDevice(), ManagerGlue.JMOD, 5, 200) { ContentPath = "Data/Sound/" };
                soundManager.LoadSounds(true);
            }

            public virtual void Init()
            { 
            }

            public virtual void Update(float dtime)
            {
            }

            public virtual void HandleKeyDown(KeyEventArgs e)
            {
            }

            protected Vector3 RandomClosePosition(float maxDistance)
            {
                double d = random.NextDouble();
                float length = (float)(1 - d * d) * maxDistance;
                Vector3 dir = Vector3.Normalize(new Vector3((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f));
                return length * dir;
            }

            protected Unit InsertUnit2(Vector3 position, IPlayable sound, bool is3DSound)
            {
                return new Unit { Position = position, Sound = sound, Is3DSound = is3DSound };
            }

            public Dictionary<ISoundChannel, Unit> Units { get { return units; } }
            public ISoundManager SoundManager { get { return soundManager; } }

            protected Random random = new Random();

            protected Dictionary<ISoundChannel, Unit> units = new Dictionary<ISoundChannel, Unit>();
            protected ISoundManager soundManager;
        }

        class SoundCrashTest : TestState
        {
            public override void Init()
            {
                availableSFXs = new Dictionary<SFX, bool>();
                availableSFXs[SFX.DogBite1] = true;
                availableSFXs[SFX.SilverShot1] = true;
                availableSFXs[SFX.Horn1] = true;
                availableSFXs[SFX.WolfBossWhine1] = true;
                availableSFXs[SFX.Jump1] = false;
                availableSFXs[SFX.HumanDeath1] = false;
                availableSFXs[SFX.TimeRunningOut1] = false;
                availableStreams = new Dictionary<Stream, bool>();
                availableStreams[Stream.BirdsAmbient1] = false;

                addStreamProb += addSfxProb;
                stopSoundProb += addStreamProb;
                pauseSoundProb += stopSoundProb;
            }

            public override void Update(float dtime)
            {
                timeUntilNextAction -= dtime;

                if (timeUntilNextAction <= 0)
                {
                    timeUntilNextAction = (float)random.NextDouble();

                    double r = random.NextDouble();

                    if (units.Count == 0)
                        r = 0;

                    if (r < addSfxProb)        // add sound
                    {
                        double d = random.NextDouble();
                        int nAdds = (int)(System.Math.Pow(d, 6) * maxSfxAdds) + 1;
                        Console.WriteLine("Adding {0} sound effects", nAdds);
                        while (nAdds-- > 0)
                        {
                            SFX sfx = availableSFXs.Keys.ToArray<SFX>()[random.Next(availableSFXs.Count)];
                            Vector3 v = RandomClosePosition(100);
                            Unit u = InsertUnit2(v, soundManager.GetSFX(sfx), availableSFXs[sfx]);
                            u.Play(false);
                            u.Channel.PlaybackStopped += (sender, ea) =>
                            {
                                lock (units)
                                {
                                    Console.WriteLine("END SFX " + (u.Virtual ? "(V) " : "") + sfx.ToString());
                                    units.Remove(u.Channel);
                                }
                            };
                            Console.WriteLine("ADD SFX {0} @ ({1}, {2}, {3})", sfx.ToString(), v.X, v.Y, v.Z);
                        }
                    }
                    else if (r < addStreamProb)
                    {
                        if (availableStreams.Count > 0)
                        {
                            Stream stream = availableStreams.Keys.ToArray<Stream>()[random.Next(availableStreams.Count)];
                            bool is3DSound = availableStreams[stream];
                            Vector3 v = RandomClosePosition(100);
                            Unit u = InsertUnit2(v, soundManager.GetStream(stream), is3DSound);
                            u.Play(false);
                            
                            u.Channel.PlaybackStopped += (sender, ea) =>
                            {
                                lock (units)
                                {
                                    Console.WriteLine("END Stream " + (u.Virtual ? "(V) " : "") + stream.ToString());
                                    units.Remove(u.Channel);
                                    availableStreams[stream] = is3DSound;
                                }
                            };
                            availableStreams.Remove(stream);
                            Console.WriteLine("ADD SFX {0} @ ({1}, {2}, {3})", stream.ToString(), v.X, v.Y, v.Z);
                        }
                    }
                    else if (r < stopSoundProb)
                    {
                        var chan = units.Keys.ToArray<ISoundChannel>()[random.Next(units.Count)];
                        chan.Stop();
                        Console.WriteLine("STOP SOUND " + units[chan].Sound.Name);
                        lock (units) { units.Remove(chan); }
                    }
                    else if (r < pauseSoundProb)
                    {
                        var chan = units.Keys.ToArray<ISoundChannel>()[random.Next(units.Count)];
                        chan.Paused = !chan.Paused;
                        Console.WriteLine("{0} SOUND {1}", chan.Paused ? "PAUSED" : "UNPAUSED", units[chan].Sound.Name);
                    }
                }
            }

            // setting variables
            int maxSfxAdds = 8;
            float addSfxProb = 0.65f;
            float addStreamProb = 0.05f;
            float stopSoundProb = 0.15f;
            float pauseSoundProb = 0.15f;

            // temporary variables
            float timeUntilNextAction = 0;

            Dictionary<SFX, bool> availableSFXs;
            Dictionary<Stream, bool> availableStreams;
            Random random = new Random();
        }
        
        class Single2DStreamTest : TestState
        {
            public override void Init()
            {
                testUnit = InsertUnit2(new Vector3(-30, 0, 0), soundManager.GetStream(Stream.MainMenuMusic1), false);
                testUnit.Play(true);
            }

            public override void HandleKeyDown(KeyEventArgs e)
            {
                base.HandleKeyDown(e);
                switch (e.KeyCode)
                { 
                    case Keys.C:
                        testUnit.Channel.Paused = !testUnit.Channel.Paused;
                        break;
                    case Keys.V:
                        if ((e.Modifiers & Keys.Shift) != 0)
                            testUnit.Channel.StopAfterCurrent();
                        else
                        {
                            testUnit.Channel.Stop(1f);
                            lock (units) { units.Remove(testUnit.Channel); }
                        }
                        break;
                }
            }

            Unit testUnit;
        }
        
        class Single3DSFXTest : TestState
        {
            public override void Init()
            {
                InsertUnit2(new Vector3(-160, 0, 0), soundManager.GetSFX(SFX.DogBite1), true).Play(true);
                new System.Threading.Thread(() =>
                {
                    System.Threading.Thread.Sleep(200);
                    InsertUnit2(new Vector3(30, 0, 0), soundManager.GetSFX(SFX.Jump1), false).Play(false);
                }).Start();
            }

            public override void HandleKeyDown(KeyEventArgs e)
            {
                base.HandleKeyDown(e);
                switch (e.KeyCode)
                { 
                    case Keys.Z:
                        panValue = System.Math.Max(0, System.Math.Min(1, panValue - 0.1f));
                        SoundManager.Settings._3DPanMaxAdjustment = panValue;
                        break;
                    case Keys.X:
                        panValue = System.Math.Max(0, System.Math.Min(1, panValue + 0.1f));
                        SoundManager.Settings._3DPanMaxAdjustment = panValue;
                        break;
                }
            }
            float panValue = 0.5f;

            
            public override void Update(float dtime)
            {
                base.Update(dtime);
                nextTitleUpdate -= dtime;
                if (nextTitleUpdate < 0)
                {
                    Instance.Text = "Pan value: " + panValue + ", 3D-Panning: " + ((Client.Sound.SoundManager)SoundManager).DebugPanLevel;
                    nextTitleUpdate += 0.033f;
                }
            }
            float nextTitleUpdate = 0;
        }

        class LengthTest : TestState
        {
            public override void Init()
            {
                TestLength(soundManager.GetStream(Stream.MainMenuMusic1), false);
                TestLength(soundManager.GetSFX(SFX.Malaria1), true);
            }

            private void TestLength(IPlayable playable, bool is3DSound)
            {
                var unit = InsertUnit2(new Vector3(-30, 0, 0), playable, is3DSound);
                unit.Play(false);
                float length = unit.Channel.CurrentSoundResource.Length;
                
                Console.WriteLine("Length {0}: {1}", unit.Sound.Name, length);

                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    bool done = false;
                    unit.Channel.PlaybackStopped += (sender, args) => { done = true; };
                    while (!done)
                    {
                        Console.WriteLine("Elapsed {0}: {1}", unit.Sound.Name,((SoundChannel)unit.Channel).TimePlayed);
                        System.Threading.Thread.Sleep(500);
                    }
                    Console.WriteLine("Elapsed {0}: {1}", unit.Sound.Name, ((SoundChannel)unit.Channel).TimePlayed);
                });
            }
        }

        class VirtualChannelsTest : TestState
        {
            public override void Init()
            {
                SFX[] sounds = new SFX[] { SFX.Malaria1, SFX.PiranhaAttack1, SFX.RaiseDead1, SFX.FireBreath1 };
                playables = new IPlayable[sounds.Length];
                for (int i = 0; i < sounds.Length; i++)
                    playables[i] = SoundManager.GetSFX(sounds[i]);
            }

            public override void Update(float dtime)
            {
                timeUntilNextAction -= dtime;
                if (timeUntilNextAction <= 0)
                {
                    timeUntilNextAction = 1f + 3f * (float)random.NextDouble();
                    var sound = playables[random.Next(playables.Length)];
                    InsertUnit2(RandomClosePosition(70), sound, true).Play(false);
                }
            }

            private float timeUntilNextAction = 0;
            private IPlayable[] playables;
        }

        class LoopingInterruptedByVirtualityTest : TestState
        {
            public override void Init()
            {
                InsertUnit2(new Vector3(-10, -10, 0), soundManager.GetSFX(SFX.DemonGrowl1), false).Play(true);
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    System.Threading.Thread.Sleep(1500);
                    InsertUnit2(new Vector3(10, 10, 0), soundManager.GetSFX(SFX.DogBite1), true).Play(false);
                });
            }
        }

        class LoopedWithIntervalStreamTest : TestState
        {
            public override void Init()
            {
                var playable = soundManager.GetStream(Stream.BirdsAmbient1);
                testChannel = playable.PlayLoopedWithIntervals(1f, 2f, 0.5f, new PlayArgs());
            }

            public override void HandleKeyDown(KeyEventArgs e)
            {
                base.HandleKeyDown(e);
                switch (e.KeyCode)
                {
                    case Keys.P:
                        var sr = testChannel.CurrentSoundResource;
                        Console.WriteLine("Length of current sound resource: {0}", sr != null ? sr.Length : 0);
                        break;
                    case Keys.V:
                        testChannel.Stop();
                        break;
                    case Keys.T:
                        if ((e.Modifiers & Keys.Shift) != 0)
                            testChannel.Stop();
                        var playable = soundManager.GetStream(Stream.BirdsAmbient1);
                        testChannel = playable.PlayLoopedWithIntervals(1f, 2f, 0.5f, new PlayArgs());
                        break;
                }
            }

            ISoundChannel testChannel;
        }

        class StreamFadeInTest : TestState
        {
            public override void Init()
            {
                for (int i = 0; i < 5; i++)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                        {
                            while (true)
                            {
                                string s = "";
                                System.IO.StreamReader sr = System.IO.File.OpenText("RenderTreeLog.txt");
                                while (s != null)
                                    s = sr.ReadLine();
                                sr.Close();
                            }
                        });
                }

                var playable = soundManager.GetStream(Stream.InGameMusic1);
                var testChannel = playable.Play(new PlayArgs
                {
                    FadeInTime = 4f,
                });
            }
        }

        class ZombieHitTest : TestState
        {
            public override void Init()
            {
                //SFX[] effects = new SFX[] { SFX.BulletHitFlesh1, SFX.BulletHitFlesh2, SFX.SwordHitFlesh1, SFX.SwordHitFlesh2 };
                SFX[] effects = new SFX[] { SFX.BulletHitFlesh1 };
                IEnumerable<IPlayable> playables = effects.Select<SFX, IPlayable>(SoundManager.GetSFX);
                var srg = SoundManager.GetSoundResourceGroup(playables.ToArray<IPlayable>());

                Vector3 pos = new Vector3(0, 0, 0);

                testUnit = InsertUnit2(pos, srg, true);
                testUnit.Play(true);

                testUnit.Channel.PlaybackStopped += (o, e) => { System.Windows.Forms.MessageBox.Show("asdf"); };
            }

            float updateTitleAcc = 10;
            public override void Update(float dtime)
            {
                updateTitleAcc += dtime;
                if (updateTitleAcc > 0.2f)
                {
                    var sr = testUnit.Channel.CurrentSoundResource;
                    if (sr != null)
                        Instance.Text = sr.Name;
                    updateTitleAcc = 0f;
                }
            }

            public override void HandleKeyDown(KeyEventArgs e)
            {
                base.HandleKeyDown(e);

                switch (e.KeyCode)
                { 
                    case Keys.C:
                        testUnit.Channel.Paused = !testUnit.Channel.Paused;
                        break;
                    case Keys.V:
                        if ((e.Modifiers & Keys.Shift) != 0)
                            testUnit.Channel.StopAfterCurrent();
                        else
                            testUnit.Channel.Stop();
                        break;
                    case Keys.P:
                        testUnit.Play(true);
                        break;
                }
            }

            Unit testUnit;
        }

        class BusyStreamFadeInTest : TestState
        {
            public override void Init()
            {
                playable = SoundManager.GetStream(Stream.ScoreScreenVictoryMusic1);
                channel = playable.Play(new PlayArgs { Looping = true, FadeInTime = 1f });
            }

            public override void Update(float dtime)
            {
                cooldown -= dtime;
                if (cooldown < 0)
                {
                    cooldown = float.PositiveInfinity;
                    Console.WriteLine("Stopping...");
                    channel.Stop();
                    //playNextFrame = true;
                    Console.WriteLine("Playing stream again with fade-in");
                    playable.Play(new PlayArgs { Looping = true, FadeInTime = 1f });
                    playNextFrame = false;
                }
                if (playNextFrame)
                {
                    Console.WriteLine("Playing stream again with fade-in");
                    playable.Play(new PlayArgs { Looping = true, FadeInTime = 1f });
                    playNextFrame = false;
                }
            }

            bool playNextFrame = false;
            float cooldown = 15f;
            IPlayable playable;
            ISoundChannel channel;
        }

        class Reuse3DSourceVoiceTest : TestState
        {
            public override void Init()
            {
                playable = SoundManager.GetSoundResourceGroup(SoundManager.GetSFX(SFX.BulletHitFlesh1));
            }

            public override void Update(float dtime)
            {
                cooldown -= dtime;
                if (cooldown < 0)
                {
                    cooldown = float.PositiveInfinity;
                    InsertNewUnit();
                }
                secondCooldown -= dtime;
                if (secondCooldown < 0)
                {
                    secondCooldown = float.PositiveInfinity;
                    secondUnit = InsertUnit2(RandomClosePosition(30f), playable, true);
                    secondUnit.Play(false);
                    secondUnit.Channel.PlaybackStopped += new EventHandler(SecondUnit_Channel_PlaybackStopped);
                }
            }

            private void InsertNewUnit()
            {
                testUnit = InsertUnit2(Vector3.Zero, playable, true);
                testUnit.Play(false);
                testUnit.Channel.PlaybackStopped += new EventHandler(Channel_PlaybackStopped);
            }

            public override void HandleKeyDown(KeyEventArgs e)
            {
                base.HandleKeyDown(e);

                switch (e.KeyCode)
                {
                    //case Keys.P:
                    //    if (testUnit != null)
                    //    {
                    //        float[] outputMatrix = ((SoundChannel)testUnit.Channel).DebugGetOutputMatrix();
                    //        if (outputMatrix != null)
                    //            PrintArray(outputMatrix);
                    //    }
                    //    else
                    //        Console.WriteLine("Unit is null.");
                    //    break;
                    //case Keys.O:
                    //    if (secondUnit != null)
                    //    {
                    //        float[] outputMatrix = ((SoundChannel)secondUnit.Channel).DebugGetOutputMatrix();
                    //        storedSecondUnitOutputMatrix = outputMatrix;
                    //        if (outputMatrix != null)
                    //            PrintArray(outputMatrix);
                    //    }
                    //    else if (storedSecondUnitOutputMatrix != null)
                    //        PrintArray(storedSecondUnitOutputMatrix);
                    //    break;
                }
            }

            private static void PrintArray(float[] array)
            {
                Console.Write("[");
                for (int i = 0; i < array.Length; i++)
                    Console.Write(array[i] + (i == array.Length - 1 ? "" : ", "));
                Console.WriteLine("]");
            }

            void Channel_PlaybackStopped(object sender, EventArgs e)
            {
                cooldown = 1.0f;
                testUnit = null;
            }

            void SecondUnit_Channel_PlaybackStopped(object sender, EventArgs e)
            {
                secondUnit = null;
            }

            float[] storedSecondUnitOutputMatrix;
            float cooldown = 1f, secondCooldown = 4f;
            Unit testUnit, secondUnit;
            IPlayable playable;
        }
    }
}
