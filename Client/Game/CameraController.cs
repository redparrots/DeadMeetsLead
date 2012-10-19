using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics;

namespace Client.Game
{
    class CameraController
    {
        public CameraController()
        {
            ControllingCamera = true;
        }

        public LookatSphericalCamera Camera { get; set; }
        public void Init()
        {
            Camera.Lookat = Game.Instance.Map.MainCharacter.Position;
            oldOptimalLookat = Camera.Lookat;
            Update(1);
            Update(1);
            Update(1);
        }

        public void Update(float dtime)
        {
            if (!ControllingCamera) return;
            // 1. Calculate the optimal position of the camera
            Vector3 optimalLookat = Camera.Lookat;

            var mainCharVP = Vector3.TransformCoordinate(Game.Instance.Map.MainCharacter.Position,
                Camera.ViewProjection);

            Vector3 mouseVP = Game.Instance.Input.State.MousePlanePosition;

            mouseVP = Vector3.TransformCoordinate(mouseVP, Camera.ViewProjection);
            var d = mouseVP - mainCharVP;
            if (d.Length() > 0.5) d = Vector3.Normalize(d) * 0.5f;

            mouseVP = mainCharVP + d;

            optimalLookat = Vector3.TransformCoordinate((mainCharVP + mouseVP) / 2,
                Matrix.Invert(Camera.ViewProjection));

            // 2. Interpolate between that value and the old position
            //cameraLookat.ClearKeys();
            //if ((cameraLookat.Value - optimalLookat).Length() > 20)
            //    cameraLookat.Value = optimalLookat;
            //cameraLookat.AddKey(new Common.InterpolatorKey<Vector3>
            //{
            //    TimeType = Common.InterpolatorKeyTimeType.Relative,
            //    Time = Common.Math.Clamp(dtime * 100, 0.1f, 0.8f),
            //    Value = optimalLookat
            //});
            //optimalLookat = cameraLookat.Update(dtime);
            float p = 3.125f * dtime; // 0.05f = x * 0.016
            optimalLookat =
                oldOptimalLookat * Math.Max(0, 1 - p) +
                optimalLookat * Math.Min(1, p);
            oldOptimalLookat = optimalLookat;

            // 3. Add camera shake if there is any
            if (shakeTimeout > 0)
            {
                shakeTimeout -= dtime;

                float sp = 1 - (shakeTimeout / shakeDuration);
                var forward = Vector3.Normalize(Camera.Position - Camera.Lookat);
                var right = Vector3.Cross(forward, Camera.Up);
                float v = (float)(Math.Sin(sp * Math.PI) * Math.Sin(sp * Math.PI * shakePeriod) * shakeAmplitude);
                shake = right * v;
            }

            Camera.Lookat = Program.Settings.CameraLookatOffset + optimalLookat + shake;
#if DEBUG
            Camera.ZFar = Program.Settings.OffsetCameraZFar + Game.Instance.Map.Settings.CameraZFar + Program.Settings.CameraSphericalCoordinates.X;
#else
            Camera.ZFar = Game.Instance.Map.Settings.CameraZFar + Program.Settings.CameraSphericalCoordinates.X;
#endif
            Camera.ZNear = Program.Settings.CameraZNear;
            Camera.SphericalCoordinates = Program.Settings.CameraSphericalCoordinates;
        }

        public void LightShake()
        {
            shakeDuration = 0.3f;
            shakeTimeout = shakeDuration;
            shakeAmplitude = 0.1f;
            shakePeriod = 5;
            shake = Vector3.Zero;
        }
        public void MediumShake()
        {
            shakeDuration = 0.7f;
            shakeTimeout = shakeDuration;
            shakeAmplitude = 0.3f;
            shakePeriod = 10;
            shake = Vector3.Zero;
        }
        public void LargeShake()
        {
            shakeDuration = 1.5f;
            shakeTimeout = shakeDuration;
            shakeAmplitude = 0.4f;
            shakePeriod = 20;
            shake = Vector3.Zero;
        }

        public void LongSmoothShake()
        {
            shakeDuration = 2.5f;
            shakeTimeout = shakeDuration;
            shakeAmplitude = 0.2f;
            shakePeriod = 40;
            shake = Vector3.Zero;
        }

        /*public void Shake(float time, float amplitude, float fallof)
        {
            shakeTimeout = time;
            shakeAmplitude = amplitude;
            shakeFallof = fallof;
            shake = Vector3.Zero;
        }*/

        public bool ControllingCamera { get; set; }

        Vector3 oldOptimalLookat;

        float shakeTimeout = -1;
        float shakeDuration = -1;
        float shakeAmplitude, shakePeriod;
        Vector3 shake = Vector3.Zero;

        Common.Interpolator3 cameraLookat = new Common.Interpolator3();
    }
}
