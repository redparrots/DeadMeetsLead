using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Graphics.Renderer;

namespace Graphics.GraphicsDevice
{
    public class SettingConverters
    {
        public class AntiAliasingConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true; // display drop
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true; // drop-down vs combo
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                // note you can also look at context etc to build list
                return new StandardValuesCollection(MultiSampleTypes);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                    return ToAAFromAAString((string)value);
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return ToStringFromAA((SlimDX.Direct3D9.MultisampleType)value);
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public string ToStringFromAA(SlimDX.Direct3D9.MultisampleType m)
            {
                if (m == SlimDX.Direct3D9.MultisampleType.None)
                    return "None";
                if (m == SlimDX.Direct3D9.MultisampleType.TwoSamples)
                    return "2x";
                if (m == SlimDX.Direct3D9.MultisampleType.FourSamples)
                    return "4x";
                if (m == SlimDX.Direct3D9.MultisampleType.EightSamples)
                    return "8x";
                if (m == SlimDX.Direct3D9.MultisampleType.SixteenSamples)
                    return "16x";
                return "None";
            }

            public SlimDX.Direct3D9.MultisampleType ToAAFromAAString(string s)
            {
                if (s == "None")
                    return SlimDX.Direct3D9.MultisampleType.None;
                if (s == "2x")
                    return SlimDX.Direct3D9.MultisampleType.TwoSamples;
                if (s == "4x")
                    return SlimDX.Direct3D9.MultisampleType.FourSamples;
                if (s == "8x")
                    return SlimDX.Direct3D9.MultisampleType.EightSamples;
                if (s == "16x")
                    return SlimDX.Direct3D9.MultisampleType.SixteenSamples;

                return SlimDX.Direct3D9.MultisampleType.None;
            }

            public static List<SlimDX.Direct3D9.MultisampleType> MultiSampleTypes = new List<SlimDX.Direct3D9.MultisampleType>();
        }

        public class ResolutionListConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true; // display drop
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true; // drop-down vs combo
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                // note you can also look at context etc to build list
                return new StandardValuesCollection(Resolutions);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                    return ToResolutionFromResolutionString((string)value);
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return value.ToString();
                return base.ConvertTo(context, culture, value, destinationType);
            }

            public static Resolution ToResolutionFromResolutionString(string s)
            {
                Resolution size = new Resolution();
                string[] tmp = s.Split('x');
                size.Width = Int32.Parse(tmp[0].Substring(0, tmp[0].Length - 1));
                size.Height = Int32.Parse(tmp[1].Substring(1, tmp[1].Length - 1));
                return size;
            }

            public static List<Resolution> Resolutions = new List<Resolution>();
        }

        public class TextureFilteringConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true; // display drop
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true; // drop-down vs combo
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                // note you can also look at context etc to build list
                return new StandardValuesCollection(TextureFilteringTypesDict.Keys);
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is TextureFilterEnum)
                    return TextureFilteringTypesDict[(TextureFilterEnum)value];
                //if (value is string)
                //    return ToTextureFilterFromString((string)value);
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return value.ToString();
                return base.ConvertTo(context, culture, value, destinationType);
            }

            //public TextureFilters ToTextureFilterFromString(string s)
            //{
            //    return TextureFilteringTypesDict[s];
            //}

            public static Dictionary<Graphics.Renderer.TextureFilterEnum, Graphics.Renderer.TextureFilters> TextureFilteringTypesDict = new Dictionary<Graphics.Renderer.TextureFilterEnum, Graphics.Renderer.TextureFilters>();
        }

        public static SlimDX.Direct3D9.CreateFlags VertexProcessing;
        public static List<SlimDX.Direct3D9.Format> DepthBufferFormats;
        public static Version PixelShaderVersion;
    }
}
