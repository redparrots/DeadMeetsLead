using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace Common
{
    public class ResourceStringAttribute : Attribute
    {
        public ResourceStringAttribute(string key)
        {
            ResourceKey = key;
        }
        public string ResourceKey;
    }

    public class StringLocalizationStorage
    {
        public static string GetResourceString(string resourceName, System.Reflection.Assembly assembly, Enum en)
        {
            Type type = en.GetType();
            System.Reflection.MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = Attribute.GetCustomAttributes(memInfo[0], typeof(ResourceStringAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager(resourceName, assembly);
                    return resourceManager.GetString(((ResourceStringAttribute)attrs[0]).ResourceKey);
                }
            }

            return en.ToString();
        }

        static Regex reg = new Regex(@"\$\((\w+)\)");
        public String GetString(String inputString)
        {
            if (stringStorage.Count == 0 || string.IsNullOrEmpty(inputString))
                return "???";

            string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            Dictionary<String, String> extraLanguage = null;
            if (lang != "en" && stringStorage.ContainsKey(lang))
                extraLanguage = stringStorage[lang];

            inputString = reg.Replace(inputString, (match) => 
            {
                string variable = match.Groups[1].Value;
                if (extraLanguage != null && extraLanguage.ContainsKey(variable))
                    return extraLanguage[variable];
                else if (stringStorage["en"].ContainsKey(variable))
                    return stringStorage["en"][variable];
                else
                    return match.Value;
            });

            return inputString;
        }
        
        public String GetValue(String key)
        {
            var dict = stringStorage[System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName];
            if (dict.ContainsKey(key))
                return dict[key];
            return stringStorage["en"][key];
        }

        public List<string> GetLanguages()
        {
            return stringStorage.Keys.ToList();
        }
        public void WriteLanguageToStream(string language, Stream stream)
        {
            using (var w = new System.Resources.ResXResourceWriter(stream))
            {
                foreach (var pair in stringStorage[language])
                    w.AddResource(pair.Key, pair.Value);
            }
        }

        public void AddLanguage(string language, Stream data)
        {
            var dict = new Dictionary<string, string>();
            var reader = new System.Resources.ResXResourceReader(data);
            foreach (System.Collections.DictionaryEntry de in reader)
                dict[(string)de.Key] = (string)de.Value;
            stringStorage[language] = dict;
        }

        Dictionary<String, Dictionary<String, String>> stringStorage = new Dictionary<String, Dictionary<string, string>>
        {
            { "en", new Dictionary<String, String>() },
            { "ru", new Dictionary<String, String>() }
        };

        public Dictionary<String, Dictionary<String, String>> StringStorage { get { return stringStorage; } set { stringStorage = value; } }
    }
}
