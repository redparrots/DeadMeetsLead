using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using Graphics.Interface;

namespace Client.ProgramStates
{
    class IState
    {
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void OnLostDevice() { }
        public virtual void OnResetDevice() { }
        public virtual void Update(float dtime) { }
        public virtual void PreRender(float dtime) { }
        public virtual void Render(float dtime) { }
        public virtual void Pause()
        {
            Program.Instance.Interface.AddFader();
        }
        public virtual void Resume()
        {
            Program.Instance.Interface.RemoveFader();
        }
        public virtual void UpdateSound(float dtime)
        {
            Program.Instance.SoundManager.Update(dtime, Vector3.Zero, Vector3.Zero, Vector3.UnitX, Vector3.UnitZ);
        }
        public virtual void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        { 
        }
    }

    class LoadingStateState : IState
    {
        public override void Enter()
        {
            base.Enter();
            Program.Instance.Interface.AddChild(new Label
            {
                Text = Locale.Resource.GenLoadingDots,
                Font = new Graphics.Content.Font
                {
                    SystemFont = Fonts.LargeSystemFont,
                    Color = System.Drawing.Color.White
                },
                Background = null,
                AutoSize = Graphics.AutoSizeMode.Full,
                Anchor = Graphics.Orientation.BottomLeft,
                Position = new Vector2(10, 10)
            });
        }
        public override void Exit()
        {
            base.Exit();
            Program.Instance.Interface.ClearInterface();
        }
        public override void Update(float dtime)
        {
            base.Update(dtime);
            frame++;
            if (frame == 3)
                Program.Instance.ProgramState = NextState;
        }
        public override void Render(float dtime)
        {
            base.Render(dtime);

            Program.Instance.Device9.Clear(SlimDX.Direct3D9.ClearFlags.Target | 
                SlimDX.Direct3D9.ClearFlags.ZBuffer, 
                System.Drawing.Color.Black, 1.0f, 0);
        }
        int frame = 0;
        public IState NextState { get; set; }
    }
}
