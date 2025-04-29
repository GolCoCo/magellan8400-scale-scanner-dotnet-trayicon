using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magellan8400ReaderTray.Models
{
    public class SettingMain : SettingsBase
    {
        public string _FolderPathScale {  get; set; }
        public string _FolderPathScanner { get; set; }

        public string _username { get; set; }
        public string _password { get; set; }
        public bool _rememeber { get; set; }

        public bool IsEmpty()
        {
            if (_username == "" || _password == "")
            {
                return true;
            }
            return false;
        }

        public SettingMain() {

            _username = "";
            _password = "";
            _rememeber = false;
            _FolderPathScale = "";
            _FolderPathScanner = "";
        }
    }
    public class SettingsBase
    {
        public void Save()
        {

            using (StreamWriter writer = new StreamWriter(this.GetType().Name + ".xml"))
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                xmlSerializer.Serialize(writer, this);
            }
        }

        public static T Load<T>() where T : SettingsBase
        {
            T result = null;
            if (File.Exists(typeof(T).Name + ".xml"))
            {
                using (StreamReader reader = new StreamReader(typeof(T).Name + ".xml"))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    result = (T)xmlSerializer.Deserialize(reader);
                }
            }
            return result;
        }
    }
}
