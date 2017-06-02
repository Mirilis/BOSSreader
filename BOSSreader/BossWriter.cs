using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BOSSreader
{
    public class BossWriter
    {
        public BossWriter()
        {
            Configuration config;
            using (var sql = new BossDataContext())
            {
                config = Configuration.Load<Configuration>("config.xml");
                var dataList = sql.BossRangeDatas.ToList();
                var path = config.CleanHeaderFilePath;
                foreach (var item in dataList)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var filename = path + "/" +item.Product.TrimEnd(' ') + ".csv";
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }

                    using (StreamWriter SW = new StreamWriter(filename))
                    {
                        Console.WriteLine("Refreshing "+ filename  + "in FileStore.");
                        SW.Write(new BHNHeader(item).ToString());
                    }
                }
                
            }
           
      
        }
    }

}


