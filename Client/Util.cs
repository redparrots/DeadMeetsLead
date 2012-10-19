using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Client
{
    static class Util
    {
        public static String KeyToString(Keys key)
        {
            switch (key)
            {
                case Keys.D1:
                    return "1";
                case Keys.D2:
                    return "2";
                case Keys.D3:
                    return "3";
                case Keys.D4:
                    return "4";
                case Keys.D5:
                    return "5";
                case Keys.D6:
                    return "6";
                case Keys.D7:
                    return "7";
                case Keys.D8:
                    return "8";
                case Keys.D9:
                    return "9";
                case Keys.D0:
                    return "0";
                case Keys.LButton:
                    return Locale.Resource.GenLeftMouse;
                case Keys.RButton:
                    return Locale.Resource.GenRightMouse;
                case Keys.MButton:
                    return Locale.Resource.GenMiddleMouse;
                case Keys.None:
                    return "";
            }
            return key.ToString();
        }

        public static string GetLocaleResourceString(Enum en)
        {
            return Common.StringLocalizationStorage.GetResourceString("Locale.Resource", new Locale.Resources().GetType().Assembly, en);
        }

        public static void StartBrowser(string url, string args)
        {
            StartBrowser(url + args);
        }
        public static void StartBrowser(string url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                var u = new Uri(url);
                var s = u.ToString();
                if (!u.IsFile)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(s);
                    }
                    catch (Exception ex)
                    {
                        Graphics.Application.Log("Couldn't open browser: ", ex.ToString());
                    }
                }
            }
        }
        public static void DownloadString(string url, Action<bool, string> output)
        {
            Graphics.Application.Log("Downloading string from: " + url);
            WebClient c = new WebClient();
            c.DownloadStringCompleted += new DownloadStringCompletedEventHandler((s, e) =>
            {
                if (e.Cancelled || e.Error != null)
                {
                    Graphics.Application.Log("Unable to read news", "Cancelled: " + e.Cancelled, "Error: " + e.Error);
                    output(false, Locale.Resource.ErrorUnableToConnect);
                }
                else
                {
                    output(true, e.Result);
                }
            });
            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
            {
                c.DownloadStringAsync(new Uri(url));
            });
        }
    }
}
