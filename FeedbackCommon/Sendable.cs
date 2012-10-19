using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.Odbc;
using System.Net;
using System.Threading;
using Common;

namespace FeedbackCommon
{
    [Serializable]
    public abstract class SendableBase
    {
        protected void OnSendablePosted()
        {
            if (triggerStaticEvents && SendablePosted != null) SendablePosted(this);
        }
        protected void OnSendablePostingError(Exception e)
        {
            if (triggerStaticEvents && SendablePostingError != null) SendablePostingError(this, e);
        }
        public string GetDescription()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("__" + GetType().Name + "___");
            foreach (var v in GetType().GetProperties())
            {
                var val = v.GetValue(this, null);
                s.Append(v.Name).Append(": ").AppendLine(val != null ? val.ToString() : "null");
                if (val is SendableBase)
                    s.Append(((SendableBase)val).GetDescription()).AppendLine("---");
            }
            return s.ToString();
        }

        public static event Action<SendableBase> SendablePosted;
        public static event Action<SendableBase, Exception> SendablePostingError;
        public abstract string ParameterString { get; }
        public string LastSendURI { get; protected set; }
        protected bool triggerStaticEvents = true;
    }

    public class GenericSendable : Sendable<String>
    {
        public GenericSendable(string parameterString, bool triggerStaticEvents)
        {
            this.parameterString = parameterString;
            this.triggerStaticEvents = triggerStaticEvents;
        }
        public override string ParameterString { get { return parameterString; } }
        private string parameterString;
    }

    [Serializable]
    public abstract class Sendable<T> : SendableBase
    {
        protected void OnPosted()
        {
            if (Posted != null) Posted();
        }
        protected void OnPostingError(Exception ex)
        {
            if (PostingError != null) PostingError(ex);
        }

        /// <summary>
        /// Sends an HTTP-post to URI.
        /// </summary>
        /// <param name="uri">Destination address.</param>
        public void HttpPost(string uri)
        {
            LastSendURI = uri;
            WebRequest webRequest = WebRequest.Create(uri);
            //string ProxyString = 
            //   System.Configuration.ConfigurationManager.AppSettings
            //   [GetConfigKey("proxy")];
            //webRequest.Proxy = new WebProxy (ProxyString, true);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(ParameterString);
            webRequest.ContentLength = bytes.Length;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                Stream stream = null;
                try
                {
                    stream = webRequest.GetRequestStream();
                    stream.Write(bytes, 0, bytes.Length);
                    OnSendablePosted();
                    OnPosted();
                }
                catch (WebException ex)
                {
                    Console.Error.WriteLine("FeedbackError, WebException: " + ex.Message);
                    OnSendablePostingError(ex);
                    OnPostingError(ex);
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            });
        }

        protected string ToHttpString()
        {
            XmlFormatter formatter = new XmlFormatter();
            //BinaryFormatter formatter = new BinaryFormatter();

            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            stream.Close();

            byte[] b = stream.ToArray();
            return Uri.EscapeDataString(Convert.ToBase64String(b));
        }

        public static T FromHttpString(string s)
        {
            byte[] b2 = Convert.FromBase64String(Uri.UnescapeDataString(s));
            MemoryStream stream = new MemoryStream(b2);

            XmlFormatter formatter = new XmlFormatter();
            //BinaryFormatter formatter = new BinaryFormatter();
            T obj = (T)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

        public event Action Posted;
        public event Action<Exception> PostingError;
    }
}
