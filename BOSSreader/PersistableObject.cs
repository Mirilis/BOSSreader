using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace BOSSreader
{
    public class PersistableObject 
    {
        public static T Load<T>(string fileName) where T : PersistableObject, new()
        {

            var path = @"c:\BossReader\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + fileName;
            T result = default(T);

            if (File.Exists(path))
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    result = new XmlSerializer(typeof(T)).Deserialize(stream) as T;
                }
            }
            else
            {
               result = new T();
            }
            return result;
        }

        public void Save<T>(string fileName) where T : PersistableObject
        {
            var path = @"c:\BossReader\" + fileName;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (FileStream stream = new FileStream(path, FileMode.CreateNew))
            {
                new XmlSerializer(typeof(T)).Serialize(stream, this);
            }
        }
    }
}

