#define LOG_DEVICE_SETTINGS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Management;
using Newtonsoft.Json;
using Graphics.Renderer;

namespace Graphics.GraphicsDevice
{
    [Serializable]
    public struct Resolution
    {
        public int Width, Height;

        public override string ToString()
        {
            return Width + " x " + Height;
        }
    }

    public enum DeviceMode
    {
        Windowed,
        Fullscreen
    }

    public enum VerticalSyncMode
    {
        [Common.ResourceStringAttribute("VideoVSyncOff")]
        Off,
        [Common.ResourceStringAttribute("VideoVSyncOn")]
        On
    }

    public static class SettingsUtilities
    {
        public static Graphics.Renderer.Results Initialize(DeviceMode deviceMode)
        {
            if (!Directory.Exists(Graphics.Application.ApplicationDataFolder + "Logs"))
                Directory.CreateDirectory(Graphics.Application.ApplicationDataFolder + "Logs");
            SlimDX.Direct3D9.Direct3D d3d = new SlimDX.Direct3D9.Direct3D();
#if LOG_DEVICE_SETTINGS
            System.IO.StreamWriter deviceSettingsLogFile = new System.IO.StreamWriter(Graphics.Application.ApplicationDataFolder + "Logs/DeviceSettingsLog.txt");
            deviceSettingsLogFile.WriteLine("======== Graphics Device Capabilities ========");
#endif
            int maxWidth = 700;
            int maxHeight = 500;
            foreach (SlimDX.Direct3D9.DisplayMode dm in d3d.Adapters[0].GetDisplayModes(SlimDX.Direct3D9.Format.X8R8G8B8))
            {
                if(!SettingConverters.ResolutionListConverter.Resolutions.Contains(new Resolution() { Width = dm.Width, Height = dm.Height }))
                {
                    if (dm.Width > maxWidth || dm.Height > maxHeight)
                    {
                        SettingConverters.ResolutionListConverter.Resolutions.Add(new Resolution() { Width = dm.Width, Height = dm.Height });

                        if(dm.Width > maxWidth)
                            maxWidth = dm.Width;

                        if (dm.Height > maxHeight)
                            maxHeight = dm.Height;
                    }
                }
            }

            if (d3d.CheckDeviceMultisampleType(0, SlimDX.Direct3D9.DeviceType.Hardware, SlimDX.Direct3D9.Format.X8R8G8B8,
                deviceMode == DeviceMode.Fullscreen ? false : true, SlimDX.Direct3D9.MultisampleType.None))
            {
                SettingConverters.AntiAliasingConverter.MultiSampleTypes.Add(SlimDX.Direct3D9.MultisampleType.None);
            }

            if (d3d.CheckDeviceMultisampleType(0, SlimDX.Direct3D9.DeviceType.Hardware, SlimDX.Direct3D9.Format.X8R8G8B8,
                deviceMode == DeviceMode.Fullscreen ? false : true, SlimDX.Direct3D9.MultisampleType.TwoSamples))
            {
                SettingConverters.AntiAliasingConverter.MultiSampleTypes.Add(SlimDX.Direct3D9.MultisampleType.TwoSamples);
            }

            if (d3d.CheckDeviceMultisampleType(0, SlimDX.Direct3D9.DeviceType.Hardware, SlimDX.Direct3D9.Format.X8R8G8B8,
                deviceMode == DeviceMode.Fullscreen ? false : true, SlimDX.Direct3D9.MultisampleType.FourSamples))
            {
                SettingConverters.AntiAliasingConverter.MultiSampleTypes.Add(SlimDX.Direct3D9.MultisampleType.FourSamples);
            }

            if (d3d.CheckDeviceMultisampleType(0, SlimDX.Direct3D9.DeviceType.Hardware, SlimDX.Direct3D9.Format.X8R8G8B8,
                deviceMode == DeviceMode.Fullscreen ? false : true, SlimDX.Direct3D9.MultisampleType.EightSamples))
            {
                SettingConverters.AntiAliasingConverter.MultiSampleTypes.Add(SlimDX.Direct3D9.MultisampleType.EightSamples);
            }

            if (d3d.CheckDeviceMultisampleType(0, SlimDX.Direct3D9.DeviceType.Hardware, SlimDX.Direct3D9.Format.X8R8G8B8,
                deviceMode == DeviceMode.Fullscreen ? false : true, SlimDX.Direct3D9.MultisampleType.SixteenSamples))
            {
                SettingConverters.AntiAliasingConverter.MultiSampleTypes.Add(SlimDX.Direct3D9.MultisampleType.SixteenSamples);
            }

            SlimDX.Direct3D9.Capabilities caps = d3d.GetDeviceCaps(0, SlimDX.Direct3D9.DeviceType.Hardware);
            
            SlimDX.Direct3D9.FilterCaps c = caps.TextureFilterCaps;

            if((c & SlimDX.Direct3D9.FilterCaps.MinLinear) != 0 && (c & SlimDX.Direct3D9.FilterCaps.MagLinear) != 0)
            {
                SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Bilinear, new TextureFilters()
                {
                    TextureFilter = TextureFilterEnum.Bilinear,
                    AnisotropicLevel = 1,
                    TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Linear,
                    TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Linear,
                    TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Point
                });

                if ((c & SlimDX.Direct3D9.FilterCaps.MipLinear) != 0)
                    SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Trilinear, new TextureFilters()
                    {
                        TextureFilter = TextureFilterEnum.Trilinear,
                        AnisotropicLevel = 1,
                        TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Linear,
                        TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Linear,
                        TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                    });
            }
            if((c & SlimDX.Direct3D9.FilterCaps.MinAnisotropic) != 0 && (c & SlimDX.Direct3D9.FilterCaps.MagAnisotropic) != 0)
            {
                if ((c & SlimDX.Direct3D9.FilterCaps.MipLinear) != 0)
                {
                    if (caps.MaxAnisotropy >= 2)
                        SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Anisotropic2x, new TextureFilters()
                        {
                            TextureFilter = TextureFilterEnum.Anisotropic2x,
                            AnisotropicLevel = 2,
                            TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                        });
                    if (caps.MaxAnisotropy >= 4)
                        SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Anisotropic4x, new TextureFilters()
                        {
                            TextureFilter = TextureFilterEnum.Anisotropic4x,
                            AnisotropicLevel = 4,
                            TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                        });
                    if (caps.MaxAnisotropy >= 8)
                        SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Anisotropic8x, new TextureFilters()
                        {
                            TextureFilter = TextureFilterEnum.Anisotropic8x,
                            AnisotropicLevel = 8,
                            TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                        });
                    if (caps.MaxAnisotropy >= 16)
                        SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Anisotropic16x, new TextureFilters()
                        {
                            TextureFilter = TextureFilterEnum.Anisotropic16x,
                            AnisotropicLevel = 16,
                            TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                        });
                }
            }
            else if ((c & SlimDX.Direct3D9.FilterCaps.MinAnisotropic) != 0)
            {
                if ((c & SlimDX.Direct3D9.FilterCaps.MipLinear) != 0)
                {
                    if (caps.MaxAnisotropy >= 2)
                        SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Anisotropic2x, new TextureFilters()
                        {
                            TextureFilter = TextureFilterEnum.Anisotropic2x,
                            AnisotropicLevel = 2,
                            TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Linear,
                            TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                        });
                    if (caps.MaxAnisotropy >= 4)
                        SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Anisotropic4x, new TextureFilters()
                        {
                            TextureFilter = TextureFilterEnum.Anisotropic4x,
                            AnisotropicLevel = 4,
                            TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Linear,
                            TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                        });
                    if (caps.MaxAnisotropy >= 8)
                        SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Anisotropic8x, new TextureFilters()
                        {
                            TextureFilter = TextureFilterEnum.Anisotropic8x,
                            AnisotropicLevel = 8,
                            TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Linear,
                            TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                        });
                    if (caps.MaxAnisotropy >= 16)
                        SettingConverters.TextureFilteringConverter.TextureFilteringTypesDict.Add(TextureFilterEnum.Anisotropic16x, new TextureFilters()
                        {
                            TextureFilter = TextureFilterEnum.Anisotropic16x,
                            AnisotropicLevel = 16,
                            TextureFilterMin = SlimDX.Direct3D9.TextureFilter.Anisotropic,
                            TextureFilterMag = SlimDX.Direct3D9.TextureFilter.Linear,
                            TextureFilterMip = SlimDX.Direct3D9.TextureFilter.Linear
                        });
                }
            }
            else if ((c & SlimDX.Direct3D9.FilterCaps.MagAnisotropic) != 0)
            {
                throw new Exception("Weird setup, do something about it");
            }

            Application.Log("Vertex shader version: " + caps.VertexShaderVersion);
            Application.Log("Pixel shader version: " + caps.PixelShaderVersion);

            SettingConverters.PixelShaderVersion = caps.PixelShaderVersion;

            Graphics.Renderer.Results result = Graphics.Renderer.Results.OK;

            if (caps.PixelShaderVersion.Major < 3 || caps.VertexShaderVersion.Major < 3)
            {
                if (caps.PixelShaderVersion.Major == 2)
                {
                    Application.Log("Shader version is not recommended");
                    result = Graphics.Renderer.Results.VideoCardNotRecommended;
                }
                else
                {
                    Application.Log("Shader version not supported");
                    return Graphics.Renderer.Results.VideoCardNotSupported;
                }                
            }

            if ((caps.DeviceType & SlimDX.Direct3D9.DeviceType.Hardware) == 0)
            {
                Application.Log("The device is not a hardware device");
                return Graphics.Renderer.Results.VideoCardNotSupported;
            }

            if ((caps.DeviceCaps & SlimDX.Direct3D9.DeviceCaps.HWTransformAndLight) == 0)
            {
                Application.Log("Hardware vertex processing is not supported. Trying with sotfware vertex processing");
                SettingConverters.VertexProcessing = SlimDX.Direct3D9.CreateFlags.SoftwareVertexProcessing;
                result = Graphics.Renderer.Results.VideoCardNotRecommended;
            }
            else
                SettingConverters.VertexProcessing = SlimDX.Direct3D9.CreateFlags.HardwareVertexProcessing;

            SettingConverters.DepthBufferFormats = new List<SlimDX.Direct3D9.Format>();

            if (d3d.CheckDeviceFormat(0, SlimDX.Direct3D9.DeviceType.Hardware, SlimDX.Direct3D9.Format.X8R8G8B8, SlimDX.Direct3D9.Usage.DepthStencil,
                SlimDX.Direct3D9.ResourceType.Surface, SlimDX.Direct3D9.Format.D16))
            {
                SettingConverters.DepthBufferFormats.Add(SlimDX.Direct3D9.Format.D16);
            }
            else
            {
                Application.Log("D16 is not supported");
                return Graphics.Renderer.Results.VideoCardNotSupported;
            }

            if (d3d.CheckDeviceFormat(0, SlimDX.Direct3D9.DeviceType.Hardware, SlimDX.Direct3D9.Format.X8R8G8B8, SlimDX.Direct3D9.Usage.DepthStencil,
                SlimDX.Direct3D9.ResourceType.Surface, SlimDX.Direct3D9.Format.D24X8))
            {
                SettingConverters.DepthBufferFormats.Add(SlimDX.Direct3D9.Format.D24X8);
            }
            else
            {
                Application.Log("D24X8 is not supported");
            }

            if ((caps.PresentationIntervals & SlimDX.Direct3D9.PresentInterval.Immediate) == 0)
            {
                Application.Log("Presentation interval \"Immidiate\" is not supported");
                return Graphics.Renderer.Results.VideoCardNotSupported;
            }
            if ((caps.PresentationIntervals & SlimDX.Direct3D9.PresentInterval.One) == 0)
            {
                Application.Log("Presentation interval \"One\" is not supported");
                return Graphics.Renderer.Results.VideoCardNotSupported;
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select AdapterRAM from Win32_VideoController");

            foreach (ManagementObject mo in searcher.Get())
            {
                var ram = mo.Properties["AdapterRAM"].Value as UInt32?;

                if (ram.HasValue)
                {
                    VideoMemory = ((int)ram / 1048576);
                    Application.Log("Video memory: " + VideoMemory + " MB");
                }
            }

#if LOG_DEVICE_SETTINGS

            deviceSettingsLogFile.WriteLine("Graphics Card: " + d3d.Adapters[0].Details.Description);
            deviceSettingsLogFile.WriteLine("Driver Version: " + d3d.Adapters[0].Details.DriverVersion);

            deviceSettingsLogFile.WriteLine();

            deviceSettingsLogFile.WriteLine("MaxAnisotropy: " + caps.MaxAnisotropy);
            deviceSettingsLogFile.WriteLine("Texture Filters: " + caps.TextureFilterCaps);
            deviceSettingsLogFile.WriteLine("Multisample types: | ");
            foreach (SlimDX.Direct3D9.MultisampleType m in SettingConverters.AntiAliasingConverter.MultiSampleTypes)
                deviceSettingsLogFile.Write(m + " | ");

            d3d.Dispose();
            
            deviceSettingsLogFile.Close();
#endif

            return result;
        }

        //public static int VideoMemory = 0;

        public static int FindRecommendedVideoSettings()
        {
            if (SettingConverters.PixelShaderVersion.Major == 2)
                return 3;

            System.IO.StreamReader desktopVideoCardReader = new System.IO.StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.DesktopGraphicsCardsByRank)));
            System.IO.StreamReader laptopVideoCardReader = new System.IO.StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.LaptopGraphicsCardsByRank)));

            SlimDX.Direct3D9.Direct3D d3d = new SlimDX.Direct3D9.Direct3D();

            string currenGraphicsCard = d3d.Adapters[0].Details.Description;

            d3d.Dispose();

            currenGraphicsCard.ToLower();
            string[] currentGraphicsCardParts = currenGraphicsCard.Split(' ');

            List<string> currentGraphicsCardPartsList = currentGraphicsCardParts.ToList();

            List<string> toRemove = new List<string>();

            for (int n = 0; n < currentGraphicsCardPartsList.Count; n++)
            {
                if (currentGraphicsCardPartsList[n].Length == 0)
                    toRemove.Add(currentGraphicsCardPartsList[n]);
            }

            foreach (string s in toRemove)
            {
                if (currentGraphicsCardPartsList.Contains(s))
                    currentGraphicsCardPartsList.Remove(s);

            }

            currentGraphicsCardParts = currentGraphicsCardPartsList.ToArray();

            string graphicsCard = "";
            string bestDesktopGraphicsCard = "";
            string bestLaptopGraphicsCard = "";

            bool foundDesktopVideoCard = false;
            bool foundLaptopVideoCard = false;

            int bestDesktopNumber = 0;
            int nMathcesForDesktop = 0;
            int bestLaptopNumber = 0;
            int nMathcesForLaptop = 0;

            int desktopNumber = 0;
            while ((graphicsCard = desktopVideoCardReader.ReadLine()) != null)
            {
                graphicsCard = graphicsCard.ToLower();

                int k = 0;

                for (int j = 0; j < currentGraphicsCardParts.Length; j++)
                {
                    if (graphicsCard.Contains(currentGraphicsCardParts[j].ToLower()))
                        k++;
                }

                if (k > nMathcesForDesktop)
                {
                    nMathcesForDesktop = k;
                    bestDesktopNumber = desktopNumber;
                    bestDesktopGraphicsCard = graphicsCard;
                }

                desktopNumber++;
            }

            int laptopNumber = 0;
            while ((graphicsCard = laptopVideoCardReader.ReadLine()) != null)
            {
                graphicsCard = graphicsCard.ToLower();

                int k = 0;

                for (int m = 0; m < currentGraphicsCardParts.Length; m++)
                {
                    if (graphicsCard.Contains(currentGraphicsCardParts[m].ToLower()))
                        k++;
                }

                if (k >= nMathcesForLaptop)
                {
                    nMathcesForLaptop = k;
                    bestLaptopNumber = laptopNumber;
                    bestLaptopGraphicsCard = graphicsCard;
                }
                laptopNumber++;
            }

            if (nMathcesForLaptop > nMathcesForDesktop + 1)
            {
                foundLaptopVideoCard = true;
            }
            else
            {
                foundDesktopVideoCard = true;
            }

            if (foundDesktopVideoCard)
            {
                Application.Log("Found desktop graphics card: " + bestDesktopGraphicsCard);
                Application.Log("Card number in desktop-list: " + bestDesktopNumber);
                Application.Log("Matches desktop: " + nMathcesForDesktop);
                Application.Log("Matches laptop: " + nMathcesForLaptop);

                if (bestDesktopNumber < 25)
                {
                    return 0;
                }
                else if (bestDesktopNumber < 60)
                {
                    return 1;
                }
                else if (bestDesktopNumber < 100)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            else if (foundLaptopVideoCard)
            {
                Application.Log("Found laptop graphics card: " + bestLaptopGraphicsCard);
                Application.Log("Card number in laptop-list: " + bestLaptopNumber);
                Application.Log("Matches desktop: " + nMathcesForDesktop);
                Application.Log("Matches laptop: " + nMathcesForLaptop);

                if (bestLaptopNumber < 10)
                {
                    return 0;
                }
                else if (bestLaptopNumber < 30)
                {
                    return 1;
                }
                else if (bestLaptopNumber < 120)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            else
            {
                Application.Log("Graphics card not found in any of the two lists");
                if (VideoMemory > 1000)
                    return 0;
                else if (VideoMemory > 700)
                    return 1;
                else if (VideoMemory > 500)
                    return 2;
                else
                    return 3;
            }
        }


        public static int VideoMemory;
    }

    [TypeConverter(typeof(ExpandableObjectConverter)), JsonObject, Serializable]
    public class Settings
    {
        public SlimDX.Direct3D9.MultisampleType AntiAliasing { get; set; }
        
        public DeviceMode DeviceMode { get; set; }

        private Resolution resolution;
        public Resolution Resolution { get { return resolution; } set { resolution = value; } }

        public VerticalSyncMode VSync { get; set; }

        public bool PureDevice { get; set; }

        public Settings()
        {
            AntiAliasing = SlimDX.Direct3D9.MultisampleType.None;
            VSync = VerticalSyncMode.Off;
            PureDevice = true;
        }
    }
}
