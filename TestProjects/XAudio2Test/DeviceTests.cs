using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.XAudio2;
using SlimDX.Multimedia;

namespace XAudio2Test
{
    public class DeviceTests
    {
        public DeviceTests()
        {
            audioDevice = new XAudio2(XAudio2Flags.DebugEngine, ProcessorSpecifier.AnyProcessor);
            int deviceCount = audioDevice.DeviceCount;
            Console.WriteLine("Device count: " + deviceCount);
            for (int i = 0; i < deviceCount; i++)
            {
                Console.Write(FormatDeviceDetailsOutput(i, audioDevice.GetDeviceDetails(i)));
                //DeviceDetails dd = audioDevice.GetDeviceDetails(i);
                //Console.WriteLine("------------------------------------------------------------");
                //Console.WriteLine("#{0} - {1}", i, dd.DisplayName);
                //Console.WriteLine("Role: {0}", dd.Role.ToString());
                //Console.WriteLine("ID: {0}", dd.DeviceId);
                //Console.WriteLine("Output format: ");
                //Console.WriteLine(FormatOutputFormat(dd.OutputFormat));
            }
        }

        private static string FormatDeviceDetailsOutput(int deviceIndex, DeviceDetails details)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("------------------------------------------------------------");
            sb.AppendLine(String.Format("#{0} - {1}", deviceIndex, details.DisplayName));
            sb.AppendLine(String.Format("Role: {0}", details.Role.ToString()));
            sb.AppendLine(String.Format("ID: {0}", details.DeviceId));
            sb.AppendLine("Output format: ");

            WaveFormatExtensible format = details.OutputFormat;
            string pad = "\t";
            sb.AppendLine(String.Format("{0}ChanMask: {1}\tChannels: {2}", pad, format.ChannelMask, format.Channels));
            sb.AppendLine(String.Format("{0}BlockAlign: {1}\t\tSamplesPerBlock: {2}", pad, format.BlockAlignment, format.SamplesPerBlock));
            sb.AppendLine(String.Format("{0}BitsPerSample: {1}\tSamplesPerSecond: {2}", pad, format.BitsPerSample, format.SamplesPerSecond));
            sb.AppendLine(String.Format("{0}ValidBitsPerSample: {1}\tAvgBytesPerSecond: {2}", pad, format.ValidBitsPerSample, format.AverageBytesPerSecond));
            sb.AppendLine(String.Format("{0}Tag: {1}", pad, format.FormatTag));

            return sb.ToString();
        }

        //private static String FormatOutputFormat(WaveFormatExtensible format)
        //{
        //    string s = "";
        //    string pad = "\t";
        //    s += String.Format("{0}ChanMask: {1}\tChannels: {2}" + Environment.NewLine, pad, format.ChannelMask, format.Channels);
        //    s += String.Format("{0}BlockAlign: {1}\t\tSamplesPerBlock: {2}" + Environment.NewLine, pad, format.BlockAlignment, format.SamplesPerBlock);
        //    s += String.Format("{0}BitsPerSample: {1}\tSamplesPerSecond: {2}" + Environment.NewLine, pad, format.BitsPerSample, format.SamplesPerSecond);
        //    s += String.Format("{0}ValidBitsPerSample: {1}\tAvgBytesPerSecond: {2}" + Environment.NewLine, pad, format.ValidBitsPerSample, format.AverageBytesPerSecond);
        //    s += String.Format("{0}Tag: {1}" + Environment.NewLine, pad, format.FormatTag.ToString());
        //    return s;
        //}

        XAudio2 audioDevice;
    }
}
