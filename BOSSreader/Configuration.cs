using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOSSreader
{
    [Serializable]
    public class Configuration : PersistableObject
    {
        private string bossFilePath = @"c:\Mt92vw\wmt92v.exe";
        private string bossProcessName = "wmt92v";
        private string brinellDataFilePath = @"\\newnas01\dir-grps\QUAL\Brinell Hardness Results\Production Results\Production Parts\Line1";
        private string uploadDataFilePath = @"\\newnas01\dir-grps\QUAL\Brinell Hardness Results\Production Results\Production Parts\Backup";
        private string appendtoFile = "-L1";
        private string cleanHeaderFilePath = @"\\newnas01\dir-grps\QUAL\Brinell Hardness Results\Production Results\Production Parts\FileStore";
        //private string brinellDataFilePath = @"D:\BHN\Line1";
        //private string uploadDataFilePath = @"D:\BHN\Backup";
        //private string appendtoFile = "-L1";
        //private string cleanHeaderFilePath = @"D:\BHN\FileStore";

        public string CleanHeaderFilePath
        {
            get
            {
                return cleanHeaderFilePath;
            }
            set
            {
                cleanHeaderFilePath = value;
            }
        }
        public string AppendToFile
        {
            get
            {
                return appendtoFile;
            }
            set
            {
                appendtoFile = value;
            }
        }
        public string BossFilePath
        {
            get
            {
                return bossFilePath;
            }
            set
            {
                bossFilePath = value;
            }
        }

        public string UploadDataFilePathWithDate
        {
            get
            {
                var date = DateTime.Now.ToString("yyMMdd");
                return uploadDataFilePath + "/" + date;
            }
        }
        public string UploadDataFilePath
        {
            get
            {
              
                return uploadDataFilePath;
            }
            set
            {
                uploadDataFilePath = value;
            }
        }

        public string BossProcessName
        {
            get
            {
                return bossProcessName;
            }
            set
            {
                bossProcessName = value;
            }
        }
        public string BrinellDataFilePath
        {
            get
            {
                return brinellDataFilePath;
            }
            set
            {
                brinellDataFilePath = value;
            }


        }
    }
}
