using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Common;
using System.Runtime.Serialization.Formatters.Binary;

namespace Client.Game
{
    [Serializable]
    public struct InputState
    {
        public bool StrafeLeft { get; set; }
        public bool StrafeRight { get; set; }
        public bool WalkForward { get; set; }
        public bool WalkBackward { get; set; }
        public bool Jump { get; set; }
        public bool PrimaryFire { get; set; }
        public bool SecondaryFire { get; set; }
        public bool SwitchToMeleeWeapon { get; set; }
        public bool SwitchToRangedWeapon { get; set; }
        public Vector3 MouseGroundPosition { get; set; }
        public Vector3 MousePlanePosition { get; set; }
    }
    public interface IInput
    {
        InputState State { get; }
        void Update(float dtime);
    }

    class HardwareInput : IInput
    {
        public HardwareInput(Graphics.InteractiveSceneManager sceneManager)
        {
            sceneManager.MouseWheel += new MouseEventHandler(sceneManager_MouseWheel);
        }
        public void Update(float dtime)
        {
            prevMouseWheel = mouseWheel;
            mouseWheel = 0;

            var ray = Game.Instance.MainCharPlaneProbe.ScreenRay();
            if(Game.Instance != null && Game.Instance.Map != null && Game.Instance.Map.MainCharacter != null)
                ray.Position.Z -= Game.Instance.Map.MainCharacter.MainAttackToHeight;

            Vector3 ground, plane;
            Game.Instance.MainCharPlaneProbe.Intersect(ray, out plane);

            if (!Game.Instance.GroundProbe.Intersect(ray, out ground))
                ground = plane;

            ground.Z += Game.Instance.Map.MainCharacter.MainAttackToHeight;
            plane.Z += Game.Instance.Map.MainCharacter.MainAttackToHeight;

            State = new InputState
            {
                StrafeLeft = StrafeLeft,
                StrafeRight = StrafeRight,
                WalkForward = WalkForward,
                WalkBackward = WalkBackward,
                Jump = Jump,
                PrimaryFire = PrimaryFire,
                SecondaryFire = SecondaryFire,
                SwitchToMeleeWeapon = SwitchToMeleeWeapon,
                SwitchToRangedWeapon = SwitchToRangedWeapon,
                MouseGroundPosition = ground,
                MousePlanePosition = plane
            };
        }
        public InputState State { get; private set; }
        bool StrafeLeft { get { return (GetAsyncKeyState(Program.ControlsSettings.StrafeLeft) >> 8) != 0; } }
        bool StrafeRight { get { return (GetAsyncKeyState(Program.ControlsSettings.StrafeRight) >> 8) != 0; } }
        bool WalkForward { get { return (GetAsyncKeyState(Program.ControlsSettings.MoveForward) >> 8) != 0; } }
        bool WalkBackward { get { return (GetAsyncKeyState(Program.ControlsSettings.MoveBackward) >> 8) != 0; } }
        bool Jump { get { return (GetAsyncKeyState(Program.ControlsSettings.Jump) >> 8) != 0; } }
        bool PrimaryFire { get { return MouseInputActive && Control.MouseButtons == MouseButtons.Left; } }
        bool SecondaryFire { get { return MouseInputActive && Control.MouseButtons == MouseButtons.Right; } }
        bool SwitchToMeleeWeapon
        {
            get
            {
                return
                    (GetAsyncKeyState(Program.ControlsSettings.MeleeWeapon) >> 8) != 0 ||
                    prevMouseWheel > 0;
            }
        }
        bool SwitchToRangedWeapon
        {
            get
            {
                return (GetAsyncKeyState(Program.ControlsSettings.RangedWeapon) >> 8) != 0 ||
                    prevMouseWheel < 0;
            }
        }

        bool MouseInputActive
        {
            get
            {
                return
                    Program.Instance.Focused &&
                    Game.Instance.SceneControl.MouseState != Graphics.MouseState.Out;
            }
        }

        void sceneManager_MouseWheel(object sender, MouseEventArgs e)
        {
            mouseWheel = e.Delta;
        }
        int mouseWheel, prevMouseWheel;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys key);
    }

    class StaticInput : IInput
    {
        public InputState State { get; set; }
        public void Update(float dtime) { }
    }

}
