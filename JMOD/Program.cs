using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Runtime.InteropServices;

namespace JMOD
{
    class Program
    {
        //public Program()
        //{
        //    system = new SoundSystem(new Vector2(1, 30), 32);
        //}

        //public void Run()
        //{
        //    //ISound _3dsound;
        //    //system.CreateSound("MusicMono.wav", MODE._3D, out _3dsound);
        //    //var _3dchannel = _3dsound.Play();

        //    //ISound _2dsound;
        //    //system.CreateSound("MusicMono.wav", MODE._2D, out _2dsound);
        //    ////var _2dchannel = _2dsound.Play();

        //    //HorizontalBackAndForth(_3dchannel);
        //    ////CircleAroundListener(channel1);
        //    ////StaticFarawayPoint(channel1);

        //    Random random = new Random();

        //    ISound _3dsound;
        //    //system.CreateSound("MusicMono.wav", MODE._3D, out _3dsound);
        //    IChannel[] channels = new Channel[6];
        //    for (int i=0; i<channels.Length; i++)
        //    {
        //        system.CreateSound("MusicMono.wav", MODE._3D, out _3dsound);
        //        ((Sound)_3dsound).Priority = random.Next(256);
        //        channels[i] = _3dsound.Play();
        //        Console.WriteLine("Sound " + i + " started.");
        //        System.Threading.Thread.Sleep(1000);
        //    }
        //    System.Threading.Thread.Sleep(1000000);
        //}

        //public void StaticFarawayPoint(Channel channel)
        //{
        //    Vector3 sound1Position = new Vector3(200000, 200000, 0);
        //    if (channel.Sound.Is3DSound)
        //        channel.Set3DAttributes(sound1Position, Vector3.Zero);

        //    while (true)
        //    {
        //        system.Update(0.001f, Vector3.Zero, Vector3.Zero, Vector3.UnitY, Vector3.UnitZ);
        //        System.Threading.Thread.Sleep(10);
        //    }
        //}

        //public void HorizontalBackAndForth(Channel channel)
        //{
        //    float x = 0;
        //    float maxDistance = 20;
        //    channel.Paused = false;
        //    bool escWasPressed = false;
        //    while (true)
        //    {
        //        Vector3 listenerPosition = Vector3.Zero;
        //        if (channel.Sound.Is3DSound)
        //        {
        //            Vector3 sound1Position = listenerPosition + new Vector3(maxDistance * (float)System.Math.Cos(x), 0, 0);
        //            channel.Set3DAttributes(sound1Position, Vector3.Zero);
        //            Console.WriteLine("X: " + sound1Position.X);
        //        }
        //        if (GetAsyncKeyState(VK_ESCAPE) != 0)
        //        {
        //            if (!escWasPressed)
        //                channel.Paused = !channel.Paused;
        //            escWasPressed = true;
        //        }
        //        else
        //            escWasPressed = false;

        //        system.Update(0.001f, listenerPosition, Vector3.Zero, Vector3.UnitY, Vector3.UnitZ);
        //        System.Threading.Thread.Sleep(10);
        //        x += 0.01f;
        //    }
        //}

        //public void CircleAroundListener(Channel channel)
        //{
        //    float angle = 0;
        //    float distance = 10;
        //    while (true)
        //    {
        //        Vector3 listenerPosition = Vector3.Zero;
        //        if (channel.Sound.Is3DSound)
        //        {
        //            Vector3 sound1Position = listenerPosition + new Vector3(distance * (float)System.Math.Cos(angle), distance * (float)System.Math.Sin(angle), 0);
        //            channel.Set3DAttributes(sound1Position, Vector3.Zero);
        //            Console.WriteLine("Angle: " + angle);
        //        }

        //        system.Update(0.001f, listenerPosition, Vector3.Zero, Vector3.UnitY, Vector3.UnitZ);
        //        System.Threading.Thread.Sleep(10);
        //        angle += 0.02f;
        //    }
        //}

        //SoundSystem system;

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //static extern short GetAsyncKeyState(int key);
        //const int VK_ESCAPE = 0x1B;

        static void Main(string[] args)
        {
            Program p = new Program();
            //p.Run();
        }
    }
}
