using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using SlimDX;
using Newtonsoft.Json;

namespace Common.Motion
{
    [TypeConverter(typeof(ExpandableObjectConverter)), JsonObject, Serializable]
    public class Settings
    {

        public bool AllowMotionObjectGetCalls { get; set; }
        public float Gravity { get; set; }
        public float MaxDescendAngle { get; set; }
        public float MaxStepHeight { get; set; }
        public bool IsOnGroundDebug { get; set; }
        public float TimeStep { get; set; }
        public bool UseDummyMotion { get; set; }
        [Description("Needs program restart")]
        public bool UseMultiThreadedPhysics { get; set; }
        public bool UseSAT { get; set; }
        [Description("Needs program restart")]
        public bool UseSoftwareMeshes { get; set; }
        public bool VisualizeMotionBoundings { get; set; }
        public float SpeedMultiplier { get; set; }

        public Settings()
        {
            AllowMotionObjectGetCalls = true;
            Gravity = 9.82f;
            MaxDescendAngle = (float)System.Math.PI / 3f;
            MaxStepHeight = 0.3f;
            IsOnGroundDebug = false;
            TimeStep = 1 / 30f;
            UseDummyMotion = false;
            UseMultiThreadedPhysics = true;
            UseSAT = true;
            UseSoftwareMeshes = false;
            VisualizeMotionBoundings = false;
            SpeedMultiplier = 1.0f;
        }
    }
}
