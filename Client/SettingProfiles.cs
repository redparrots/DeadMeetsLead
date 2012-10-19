using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class SettingProfiles
    {

        public static Graphics.Renderer.Settings.AnimationQualities GetAnimationQuality(VideoQualities q)
        {
            if (q == VideoQualities.Low)
                return Graphics.Renderer.Settings.AnimationQualities.Low;
            else if (q == VideoQualities.Medium)
                return Graphics.Renderer.Settings.AnimationQualities.Medium;
            else if (q == VideoQualities.High)
                return Graphics.Renderer.Settings.AnimationQualities.High;
            else
                return Graphics.Renderer.Settings.AnimationQualities.High;
        }

        public static Graphics.Renderer.Settings.LightingQualities GetLightingQuality(VideoQualities q)
        {
            if (q == VideoQualities.Low)
                return Graphics.Renderer.Settings.LightingQualities.Low;
            else if (q == VideoQualities.Medium)
                return Graphics.Renderer.Settings.LightingQualities.Medium;
            else if (q == VideoQualities.High)
                return Graphics.Renderer.Settings.LightingQualities.High;
            else
                return Graphics.Renderer.Settings.LightingQualities.High;
        }

        public static Graphics.Renderer.Settings.ShadowQualities GetShadowQuality(VideoQualities q)
        {
            if (q == VideoQualities.Low)
                return Graphics.Renderer.Settings.ShadowQualities.NoShadows;
            else if (q == VideoQualities.Medium)
                return Graphics.Renderer.Settings.ShadowQualities.Low;
            else if (q == VideoQualities.High)
                return Graphics.Renderer.Settings.ShadowQualities.Medium;
            else
                return Graphics.Renderer.Settings.ShadowQualities.High;
        }

        public static Graphics.Renderer.Settings.TerrainQualities GetTerrainQuality(VideoQualities q)
        {
            if (q == VideoQualities.Low)
                return Graphics.Renderer.Settings.TerrainQualities.Low;
            else if (q == VideoQualities.Medium)
                return Graphics.Renderer.Settings.TerrainQualities.Medium;
            else if (q == VideoQualities.High)
                return Graphics.Renderer.Settings.TerrainQualities.High;
            else
                return Graphics.Renderer.Settings.TerrainQualities.High;
        }
    }
}
