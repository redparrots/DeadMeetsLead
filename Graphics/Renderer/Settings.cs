using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.ComponentModel;
using Graphics.Content;
using Newtonsoft.Json;

namespace Graphics.Renderer
{
    public enum TextureFilterEnum
    {
        [Common.ResourceStringAttribute("VideoFilteringBilinear")]
        Bilinear,
        [Common.ResourceStringAttribute("VideoFilteringTrilinear")]
        Trilinear,
        [Common.ResourceStringAttribute("VideoFilteringAnisotropic2x")]
        Anisotropic2x,
        [Common.ResourceStringAttribute("VideoFilteringAnisotropic4x")]
        Anisotropic4x,
        [Common.ResourceStringAttribute("VideoFilteringAnisotropic8x")]
        Anisotropic8x,
        [Common.ResourceStringAttribute("VideoFilteringAnisotropic16x")]
        Anisotropic16x,
    }

    [Serializable]
    public struct TextureFilters
    {
        public TextureFilterEnum TextureFilter;
        public SlimDX.Direct3D9.TextureFilter TextureFilterMin;
        public SlimDX.Direct3D9.TextureFilter TextureFilterMag;
        public SlimDX.Direct3D9.TextureFilter TextureFilterMip;
        public int AnisotropicLevel;

        public override bool Equals(object obj)
        {
            return (((TextureFilters)obj).AnisotropicLevel == AnisotropicLevel && ((TextureFilters)obj).TextureFilterMag == TextureFilterMag &&
                ((TextureFilters)obj).TextureFilterMin == TextureFilterMin && ((TextureFilters)obj).TextureFilterMip == TextureFilterMip);
        }

        public override int GetHashCode()
        {
            return TextureFilterMip.GetHashCode() + TextureFilterMin.GetHashCode() + TextureFilterMag.GetHashCode() + AnisotropicLevel.GetHashCode();
        }

        //public override string ToString()
        //{
        //    if (TextureFilterMin == SlimDX.Direct3D9.TextureFilter.Anisotropic || TextureFilterMag == SlimDX.Direct3D9.TextureFilter.Anisotropic)
        //        return String.Format(Locale.Resource.VideoFilteringAnisotropic, AnisotropicLevel);
        //    else if (TextureFilterMin == SlimDX.Direct3D9.TextureFilter.Linear && TextureFilterMag == SlimDX.Direct3D9.TextureFilter.Linear)
        //    {
        //        if (TextureFilterMip == SlimDX.Direct3D9.TextureFilter.Point)
        //            return Locale.Resource.VideoFilteringBilinear;
        //        else
        //            return Locale.Resource.VideoFilteringTrilinear;
        //    }
        //    else
        //        return Locale.Resource.GenNone;
        //}
    }

    [TypeConverter(typeof(ExpandableObjectConverter)), JsonObject, Serializable]
    public class Settings
    {
        public enum Setting
        {
            TextureFiltering,
            ShadowsEnable,
            ShadowQuality,
            ViewDistance,
            FogColor,
            LightDirection,
            DiffuseFactor,
            AmbientFactor,
            AlphaEnable,
            ShadowBias,
            RenderWithPostEffect
        }
        
        public enum AnimationQualities
        {
            [Common.ResourceStringAttribute("VideoEnumLow")]
            Low,
            [Common.ResourceStringAttribute("VideoEnumMedium")]
            Medium,
            [Common.ResourceStringAttribute("VideoEnumHigh")]
            High
        }

        public enum LightingQualities
        {
            [Common.ResourceStringAttribute("VideoEnumLow")]
            Low,
            [Common.ResourceStringAttribute("VideoEnumMedium")]
            Medium,
            [Common.ResourceStringAttribute("VideoEnumHigh")]
            High
        }

        public enum ShadowQualities
        {
            [Common.ResourceStringAttribute("VideoEnumNoShadows")]
            NoShadows,
            [Common.ResourceStringAttribute("VideoEnumLowest")]
            Lowest,
            [Common.ResourceStringAttribute("VideoEnumLow")]
            Low,
            [Common.ResourceStringAttribute("VideoEnumMedium")]
            Medium,
            [Common.ResourceStringAttribute("VideoEnumHigh")]
            High,
            [Common.ResourceStringAttribute("VideoEnumHighest")]
            Highest
        }

        public enum TerrainQualities
        {
            [Common.ResourceStringAttribute("VideoEnumLow")]
            Low,
            [Common.ResourceStringAttribute("VideoEnumMedium")]
            Medium,
            [Common.ResourceStringAttribute("VideoEnumHigh")]
            High
        }

        [NonSerialized, JsonIgnore]
        public Dictionary<AnimationQualities, int> AnimationQualityPriorityRelation = new Dictionary<AnimationQualities, int>();
        [NonSerialized, JsonIgnore]
        public Dictionary<LightingQualities, int> LightingQualityPriorityRelation = new Dictionary<LightingQualities, int>();
        [NonSerialized, JsonIgnore]
        public Dictionary<Priority, int> PriorityRelation = new Dictionary<Priority, int>();
        [NonSerialized, JsonIgnore]
        public Dictionary<ShadowQualities, int> ShadowQualityRelation = new Dictionary<ShadowQualities, int>();
        [NonSerialized, JsonIgnore]
        public Dictionary<ShadowQualities, int> ShadowQualityPriorityRelation = new Dictionary<ShadowQualities, int>();
        [NonSerialized, JsonIgnore]
        public Dictionary<TerrainQualities, int> TerrainQualityPriorityRelation = new Dictionary<TerrainQualities, int>();

        private Vector3 lightDirection;
        [CategoryAttribute("Render Effects"), DefaultValueAttribute(true)]
        public Vector3 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; }
        }

        private Vector3 colorChannelPercentageIncrease;
        [CategoryAttribute("Post Effects"), DefaultValueAttribute(true)]
        public Vector3 ColorChannelPercentageIncrease
        {
            get { return colorChannelPercentageIncrease; }
            set { colorChannelPercentageIncrease = value; }
        }

        private Vector3 additiveLightColor;
        [CategoryAttribute("Post Effects"), DefaultValueAttribute(true)]
        public Vector3 AdditiveLightColor
        {
            get { return additiveLightColor; }
            set { additiveLightColor = value; }
        }

        private float percentageLightIncrease;
        [CategoryAttribute("Post Effects"), DefaultValueAttribute(true)]
        public float PercentageLightIncrease
        {
            get { return percentageLightIncrease; }
            set { percentageLightIncrease = value; }
        }

        private Vector4 fogColor;
        [CategoryAttribute("Render Effects"), DefaultValueAttribute(true)]
        public Vector4 FogColor
        {
            get { return fogColor; }
            set { fogColor = value; }
        }

        private Vector3 diffuseColor;
        [CategoryAttribute("Render Effects"), DefaultValueAttribute(true)]
        public Vector3 DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        private Vector3 ambientColor;
        [CategoryAttribute("Render Effects"), DefaultValueAttribute(true)]
        public Vector3 AmbientColor
        {
            get { return ambientColor; }
            set { ambientColor = value; }
        }

        private Vector3 specularColor;
        [CategoryAttribute("Render Effects"), DefaultValueAttribute(true)]
        public Vector3 SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        private float textureSize;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public float TextureSize
        {
            get { return textureSize; }
            set { textureSize = value; }
        }

        private LightingQualities lightingQuality;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public LightingQualities LightingQuality
        {
            get { return lightingQuality; }
            set { lightingQuality = value; }
        }

        private ShadowQualities shadowQuality;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public ShadowQualities ShadowQuality
        {
            get { return shadowQuality; }
            set { shadowQuality = value; }
        }

        private AnimationQualities animationQuality;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public AnimationQualities AnimationQuality
        {
            get { return animationQuality; }
            set { animationQuality = value; }
        }

        private TerrainQualities terrainQuality;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public TerrainQualities TerrainQuality
        {
            get { return terrainQuality; }
            set { terrainQuality = value; }
        }

        private float fogDistance;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public float FogDistance
        {
            get { return fogDistance; }
            set { fogDistance = value; }
        }

        //private bool renderWithPostEffect;
        //[CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        //public bool RenderWithPostEffect
        //{
        //    get { return renderWithPostEffect; }
        //    set { renderWithPostEffect = value; }
        //}

        private bool shadowsEnable;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public bool ShadowsEnable
        {
            get
            {
                if (ShadowQuality != ShadowQualities.NoShadows)
                    shadowsEnable = true;
                else
                    shadowsEnable = false;

                return shadowsEnable;
            }
            private set {}
        }

        private bool waterEnable;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public bool WaterEnable
        {
            get { return waterEnable; }
            set { waterEnable = value; }
        }

        private float shadowBias;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public float ShadowBias
        {
            get { return shadowBias; }
            set { shadowBias = value; }
        }

        private SlimDX.Direct3D9.FillMode fillMode;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public SlimDX.Direct3D9.FillMode FillMode
        {
            get { return fillMode; }
            set { fillMode = value; }
        }

        private SlimDX.Direct3D9.Cull cullMode;
        [CategoryAttribute("Renderer Setrtings"), DefaultValueAttribute(true)]
        public SlimDX.Direct3D9.Cull CullMode
        {
            get { return cullMode; }
            set { cullMode = value; }
        }

        private float waterLevel;
        [CategoryAttribute("Render Effects"), DefaultValueAttribute(true)]
        public float WaterLevel
        {
            get { return waterLevel; }
            set { waterLevel = value; }
        }

        private float fogExponent;
        [CategoryAttribute("Render Effects"), DefaultValueAttribute(true)]
        public float FogExponent
        {
            get { return fogExponent; }
            set { fogExponent = value; }
        }

        private bool renderAlphaObjects;
        [CategoryAttribute("Renderer Settings"), DefaultValueAttribute(true)]
        public bool RenderAlphaObjects
        {
            get { return renderAlphaObjects; }
            set { renderAlphaObjects = value; }
        }

        private bool renderSplatObjects;
        [CategoryAttribute("Render Settings"), DefaultValueAttribute(true)]
        public bool RenderSplatObjects
        {
            get { return renderSplatObjects; }
            set { renderSplatObjects = value; }
        }

        private bool renderSkinnedMeshes;
        [CategoryAttribute("Render Settings"), DefaultValueAttribute(true)]
        public bool RenderSkinnedMeshes
        {
            get { return renderSkinnedMeshes; }
            set { renderSkinnedMeshes = value; }
        }

        private bool renderXMeshes;
        [CategoryAttribute("Render Settings"), DefaultValueAttribute(true)]
        public bool RenderXMeshes
        {
            get { return renderXMeshes; }
            set { renderXMeshes = value; }
        }

        [NonSerialized]
        float cullSceneInterval;
        public float CullSceneInterval { get { return cullSceneInterval; } set { cullSceneInterval = value; } }

        [System.ComponentModel.TypeConverter(typeof(Graphics.GraphicsDevice.SettingConverters.TextureFilteringConverter))]
        public TextureFilters TextureFilter { get; set; }

        public Settings()
        {
            ShadowQualityRelation.Add(ShadowQualities.NoShadows, 0);
            ShadowQualityRelation.Add(ShadowQualities.Lowest, 1500);
            ShadowQualityRelation.Add(ShadowQualities.Low, 2500);
            ShadowQualityRelation.Add(ShadowQualities.Medium, 3500);
            ShadowQualityRelation.Add(ShadowQualities.High, 4500);
            ShadowQualityRelation.Add(ShadowQualities.Highest, 5500);

            RenderAlphaObjects = true;
            RenderSplatObjects = true;
            RenderXMeshes = true;
            RenderSkinnedMeshes = true;

            PriorityRelation.Add(Priority.Never, 0);
            PriorityRelation.Add(Priority.Low, 1);
            PriorityRelation.Add(Priority.Medium, 2);
            PriorityRelation.Add(Priority.High, 3);

            ShadowQualityPriorityRelation.Add(ShadowQualities.NoShadows, 0);
            ShadowQualityPriorityRelation.Add(ShadowQualities.Lowest, 1);
            ShadowQualityPriorityRelation.Add(ShadowQualities.Low, 1);
            ShadowQualityPriorityRelation.Add(ShadowQualities.Medium, 2);
            ShadowQualityPriorityRelation.Add(ShadowQualities.High, 3);
            ShadowQualityPriorityRelation.Add(ShadowQualities.Highest, 3);

            AnimationQualityPriorityRelation.Add(AnimationQualities.Low, 0);
            AnimationQualityPriorityRelation.Add(AnimationQualities.Medium, 1);
            AnimationQualityPriorityRelation.Add(AnimationQualities.High, 2);

            TerrainQualityPriorityRelation.Add(TerrainQualities.Low, 0);
            TerrainQualityPriorityRelation.Add(TerrainQualities.Medium, 1);
            TerrainQualityPriorityRelation.Add(TerrainQualities.High, 2);

            LightingQualityPriorityRelation.Add(LightingQualities.Low, 0);
            LightingQualityPriorityRelation.Add(LightingQualities.Medium, 1);
            LightingQualityPriorityRelation.Add(LightingQualities.High, 2);

            if (GraphicsDevice.SettingsUtilities.VideoMemory < 270)
                ShadowQuality = ShadowQualities.NoShadows;


            FillMode = SlimDX.Direct3D9.FillMode.Solid;

            FogDistance = 35.1f;
            FogExponent = 10.5f;
            FogColor = new Vector4(0.08f, 0.86f, 0.76f, 1.0f);
            DiffuseColor = new Vector3(1.34f, 1.26f, 0.8f);
            AmbientColor = new Vector3(0.4f, 0.76f, 0.78f);
            SpecularColor = new Vector3(1.35f, 1.15f, 0.9f);

            LightDirection = new Vector3(-0.6f, 0.7f, 0.7f);
            RenderAlphaObjects = true;
            ShadowBias = 1.0f / 500.0f;
            WaterLevel = 0.2f;
            WaterEnable = true;
            //RenderWithPostEffect = false;
            AdditiveLightColor = Vector3.Zero;
            TextureSize = 3f;
            ColorChannelPercentageIncrease = new Vector3(0, 0.26f, 0.18f);
            CullSceneInterval = 0.1f;

            TextureFilter = new TextureFilters
            {
                AnisotropicLevel = 1,
                TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Linear,
                TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Linear,
                TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Point
            };
        }
    }
}
