using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Game
{
    class RendererSettingsController
    {
        public RendererSettingsController()
        {
            PercentageLightIncrease = new Common.Interpolator
            {
                Value = Program.DefaultSettings.RendererSettings.PercentageLightIncrease,
                //ChangeSpeed = 0.05f,
            };
            ColorChannelPercentageIncrease = new Common.Interpolator3
            {
                Value = Program.DefaultSettings.RendererSettings.ColorChannelPercentageIncrease,
                //ChangeSpeed = 0.2f,
            };
        }
        public void Update(float dtime)
        {
            Game.Instance.Renderer.Settings.PercentageLightIncrease = PercentageLightIncrease.Update(dtime);            
            Game.Instance.Renderer.Settings.ColorChannelPercentageIncrease = ColorChannelPercentageIncrease.Update(dtime);
        }

        public Common.Interpolator PercentageLightIncrease { get; private set; }
        public Common.Interpolator3 ColorChannelPercentageIncrease { get; private set; }
    }
}
