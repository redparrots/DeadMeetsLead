using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Client.Game
{
    public enum ReplayState
    {
        None,
        Recording,
        Replaying
    }

    [Serializable]
    class ReplayFrame
    {
        public float Time;
        public Vector3 MainCharTranslation;
        public InputState InputState;
    }
    [Serializable]
    class Replay
    {
        protected Replay() { }

        public List<ReplayFrame> Frames = new List<ReplayFrame>();

        public void StartRecording(Game game)
        {
            state = ReplayState.Recording;
            Game.Random = new Random(0);
            game.SpawnRandomizer = new Random(0);
        }
        public void StopRecording()
        {
            state = ReplayState.None;
            SaveToFile(filename);
        }
        public void StartReplaying(Game game)
        {
            game.Input = staticInput = new StaticInput();
            state = ReplayState.Replaying;
            Game.Random = new Random(0);
            game.SpawnRandomizer = new Random(0);
            mainCharPositionReplay = new Common.Interpolator3();
            groundPosition = new Common.Interpolator3();
            planePosition = new Common.Interpolator3();
        }
        public void Update(float dtime)
        {
            time += dtime;
            if (state == ReplayState.Recording)
            {
                Frames.Add(new ReplayFrame
                {
                    Time = time,
                    MainCharTranslation = Game.Instance.Map.MainCharacter.Translation,
                    InputState = Game.Instance.Input.State
                });
            }
            else if (state == ReplayState.Replaying)
            {
                while (time > Frames[replayI].Time && replayI < Frames.Count - 1)
                {
                    replayI++;
                    staticInput.State = Frames[replayI].InputState;
                    float t = Frames[replayI].Time - Frames[replayI - 1].Time;
                    mainCharPositionReplay.ClearKeys();
                    mainCharPositionReplay.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = t,
                        Value = Frames[replayI].MainCharTranslation
                    });
                    groundPosition.ClearKeys();
                    groundPosition.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = t,
                        Value = Frames[replayI].InputState.MouseGroundPosition
                    });
                    planePosition.ClearKeys();
                    planePosition.AddKey(new Common.InterpolatorKey<Vector3>
                    {
                        TimeType = Common.InterpolatorKeyTimeType.Relative,
                        Time = t,
                        Value = Frames[replayI].InputState.MousePlanePosition
                    });
                }
                Game.Instance.Map.MainCharacter.Position =
                    (Game.Instance.Map.MainCharacter.Position + Frames[replayI].MainCharTranslation) / 2f;
                    //mainCharPositionReplay.Update(dtime);
                    //Frames[replayI].MainCharTranslation;
                var s = staticInput.State;
                s.MouseGroundPosition = groundPosition.Update(dtime);
                s.MousePlanePosition = planePosition.Update(dtime);
                staticInput.State = s;
            }
        }
        public void Restart()
        {
            Game.Random = new Random(0);
            Game.Instance.SpawnRandomizer = new Random(0);
            time = 0;
            if (state == ReplayState.Recording)
            {
                Frames.Clear();
            }
            else if (state == ReplayState.Replaying)
            {
                replayI = 0;
            }
        }
        public void Close()
        {
            if (state == ReplayState.Recording)
            {
                StopRecording();
            }
        }
        public void SaveToFile(String filename)
        {
            BinaryFormatter f = new BinaryFormatter();
            FileStream s = File.Open(filename, FileMode.Create);
            f.Serialize(s, this);
            s.Close();
        }
        public static Replay Load(String filename)
        {
            BinaryFormatter f = new BinaryFormatter();
            FileStream s = File.Open(filename, FileMode.Open);
            var r = (Replay)f.Deserialize(s);
            s.Close();
            return r;
        }
        public static Replay New(String filename)
        {
            return new Replay { filename = filename };
        }
        [NonSerialized]
        StaticInput staticInput;
        [NonSerialized]
        ReplayState state = ReplayState.None;
        [NonSerialized]
        String filename;
        [NonSerialized]
        float time = 0;
        [NonSerialized]
        int replayI = 0;
        [NonSerialized]
        Common.Interpolator3 mainCharPositionReplay, groundPosition, planePosition;
        [NonSerialized]
        Vector3 mainCharP;
    }
}
