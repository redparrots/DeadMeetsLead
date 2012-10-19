using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Graphics;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Client
{
    public static class Serialization
    {
        public static void SerializeCommonXmlFormatter(string filename, object o)
        {
            Application.Log("Trying to save (CommonXmlFormatter) " + filename);
            Common.XmlFormatter formatter = new Common.XmlFormatter { Binder = ClientXmlFormatterBinder.Instance };

            var tmpFilename = Path.GetTempFileName();
            Application.Log("Using temp filename " + tmpFilename);
            var f = System.IO.File.Open(tmpFilename, FileMode.Create);
            formatter.Serialize(f, o);
            f.Close();
            File.Delete(filename);
            File.Move(tmpFilename, filename);
            Application.Log("Save complete (CommonXmlFormatter) " + filename);
        }
        public static void SerializeXmlSerializer(String filename, object o)
        {
            XmlSerializer s = new XmlSerializer(o.GetType());

            var tmpSettings = Path.GetTempFileName();
            var f = System.IO.File.Open(tmpSettings, FileMode.Create);
            s.Serialize(f, o);
            f.Close();
            File.Delete(filename);
            File.Move(tmpSettings, filename);
        }
        public static void SerializeDataContractSerializer(String filename, object o)
        {
            DataContractSerializer s = new DataContractSerializer(o.GetType());

            var tmpFilename = Path.GetTempFileName();
            var f = System.IO.File.Open(tmpFilename, FileMode.Create);
            s.WriteObject(f, o);
            f.Close();
            File.Delete(filename);
            File.Move(tmpFilename, filename);
        }
        public static string SerializeJSON(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                });
        }
        public static void SerializeJSON(String filename, object o)
        {
            Application.Log("Trying to save (JSON) " + filename);
            string text = SerializeJSON(o);
            
            var tmpFilename = Path.GetTempFileName();
            Application.Log("Using temp filename " + tmpFilename);
            File.WriteAllText(tmpFilename, text);
            File.Delete(filename);
            File.Move(tmpFilename, filename);
            Application.Log("Save complete (JSON) " + filename);
        }
        public static void SerializeBSON(String filename, object o)
        {
            Application.Log("Trying to save (BSON) " + filename);
            
            var tmpFilename = Path.GetTempFileName();
            Application.Log("Using temp filename " + tmpFilename);
            using (var fs = File.OpenWrite(tmpFilename))
            {
                var serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                };

                var writer = new BsonWriter(fs);
                serializer.Serialize(writer, o);
            }

            File.Delete(filename);
            File.Move(tmpFilename, filename);
            Application.Log("Save complete (BSON) " + filename);
        }
        public static object TryDeserializeXmlFormatter(String filename)
        {
            object obj = null;
            Application.Log("Trying to load (XmlFormatter) " + filename);
            if (!File.Exists(filename))
            {
                Application.Log("No such file: " + filename);
            }
            else
            {
                try
                {
                    Common.XmlFormatter formatter = new Common.XmlFormatter { Binder = ClientXmlFormatterBinder.Instance };
                    var f = System.IO.File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    obj = formatter.Deserialize(f);
                    f.Close();
                    Application.Log("Load completed (XmlFormatter) " + filename);
                }
                catch (Exception e)
                {
                    Application.Log("Error loading " + filename + ": ", e.ToString());
                }
            }
            return obj;
        }
        public static object TryDeserializeJSON(String filename, Type type)
        {
            object obj = null;
            Application.Log("Trying to load (JSON) " + filename);
            if (!File.Exists(filename))
            {
                Application.Log("No such file: " + filename);
            }
            else
            {
                try
                {
                    string text = File.ReadAllText(filename);
                    obj = JsonConvert.DeserializeObject(text, type, new JsonSerializerSettings
                    {
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        TypeNameHandling = TypeNameHandling.Objects,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full,
                        ContractResolver = new DefaultContractResolver
                        {
                            DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                        },
                        Binder = ClientXmlFormatterBinder.Instance
                    });
                    Application.Log("Load completed (JSON) " + filename);
                }
                catch (Exception e)
                {
                    Application.Log("Error loading " + filename + ": ", e.ToString());
                }
            }
            return obj;
        }
    }
}
