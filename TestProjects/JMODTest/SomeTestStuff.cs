using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Client.Sound;

namespace JMODTest
{
    internal class SomeTestStuff
    {
        private void Run()
        {
            soundManager = new SoundManager(new AudioDevice(), ManagerGlue.JMOD, 1, 30);
            soundManager.ContentPath = "Data/Sound/";
            soundManager.LoadSounds(true);

            playable = soundManager.GetSFX(SFX.SwordSwish1);
            //playable = soundManager.GetStream(Stream.MainMenuMusic1);
            channel = playable.Play(new Client.Sound.PlayArgs { Looping = true });
            //channel.Looping = true;
            //channel.PlaybackStopped += new EventHandler(channel_PlaybackStopped);
            //channel.PlaybackStopped += (sender, ea) => { Console.WriteLine("Playback ended. Restarting..."); channel = p.Play(); };

            float dtime = 1 / 60f;
            while (true)
            {
                soundManager.Update(dtime, Vector3.Zero, Vector3.Zero, Vector3.UnitX, Vector3.UnitZ);
                System.Threading.Thread.Sleep((int)(dtime * 1000));
                //VolumeTest(dtime / 4f);
                //PauseTest(dtime / 4f);
                //GlobalMuteTest(dtime / 4f);
            }
        }


        //void channel_PlaybackStopped(object sender, EventArgs e)
        //{
        //    // hack:
        //    Console.WriteLine("Playback ended. Restarting...");
        //    channel = playable.Play();
        //    channel.PlaybackStopped += new EventHandler(channel_PlaybackStopped);
        //}

        private float volumeAcc = 0;
        private void VolumeTest(float inc)
        {
            volumeAcc += inc;
            float volume = 0f;
            if (volumeAcc < 1)
                volume = volumeAcc;
            else if (volumeAcc < 2)
                volume = 2 - volumeAcc;
            else
                volumeAcc = 0;

            channel.Volume = volume;
            Console.WriteLine(volume);
        }

        private float pauseAcc = 0;
        private void PauseTest(float inc)
        {
            pauseAcc += inc;
            if (pauseAcc > 1)
            {
                if (channel.Paused)
                    Console.WriteLine("Resuming...");
                else
                    Console.WriteLine("Pausing...");
                channel.Paused = !channel.Paused;
                pauseAcc = 0;
            }
        }

        private float globalMuteAcc = 0;
        private void GlobalMuteTest(float inc)
        {
            globalMuteAcc += inc;
            if (globalMuteAcc > 1)
            {
                soundManager.Muted = !soundManager.Muted;
                if (soundManager.Muted)
                    Console.WriteLine("Muted");
                else
                    Console.WriteLine("Audible");
                globalMuteAcc = 0;
            }
        }

        private IPlayable playable;
        private ISoundChannel channel;
        private SoundManager soundManager;
    }
}
