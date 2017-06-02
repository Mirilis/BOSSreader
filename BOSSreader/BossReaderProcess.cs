using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BOSSreader
{
    public class BossReaderProcess
    {
        private List<String> products { get; set; }
        private List<string> rangeData { get; set; }
        private List<Record> Records { get; set; }
        private string path { get; set; }
        private string Filename;
        
        public BossReaderProcess()
        {
            using (var sql = new ProductNamesContext())
            {
                products = sql.Products.Select(x => x.Name.Trim().ToUpper()).ToList();
                products.Add("000DAILYSCOPECHECK");

            }
            using (var sql = new BossDataContext())
            {
                rangeData = sql.BossRangeDatas.Select(x=>x.Product).ToList();
                Records = sql.Records.ToList();
            }
            path = @"\\newnas01\dir-grps\QUAL\Brinell Hardness Results\Production Results\Production Parts\Backup\";
            ProcessDirectory(path);
           
        }
        private void ProcessDirectory(string path)
        {
            if (!path.Contains("Errors")) 
            {
                var filelist = Directory.GetFiles(path).ToList();
                filelist.ForEach(x => ReadBOSSData(x));
                var dirList = Directory.GetDirectories(path).ToList();
                dirList.ForEach(x => ProcessDirectory(x));
            }
        }

        bool IsBossFile(string myPath)
        {
            if (!File.Exists(myPath))
            {
                return false;
            }
            else if (Path.GetExtension(myPath) != ".csv")
            {
                return false;
            }
            return true;
        }
        public bool ReadBOSSData(string myPath)
        {
            Filename = myPath;
            if (!IsBossFile(myPath))
            {
                return false;
            }
            var fileName = Path.GetFileNameWithoutExtension(myPath);

            if (fileName.Contains('-'))
            {
                _currentFileOrigin = fileName.Substring(fileName.Length -2, 2);

            }
            else _currentFileOrigin = "UN";

            int count = 0;
            bool IsAValidProduct = true;

            using (var sr = new StreamReader(myPath))
            {
                var countString = sr.ReadLine().Split(',')[1];
                var result = int.TryParse(countString, out count);
                if (!result)
                {
                    count = -1;

                }
                if (count > 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var s = sr.ReadLine();
                    }

                    var partRaw = sr.ReadLine();
                    var partCleaned = partRaw.Split(',')[1].Trim().RemoveSpecialCharacters().ToUpper();

                    if (!products.Contains(partCleaned))
                    {
                        IsAValidProduct = false;
                        Console.WriteLine(partCleaned + ": Not a Valid Product");
                    }


                    if (IsAValidProduct && count > 0)
                    {
                        sr.ReadLine(); sr.ReadLine();
                        var specificationRaw = sr.ReadLine().Split(',')[1];
                        var specificationCleaned = specificationRaw.Trim().RemoveSpecialCharacters().ToUpper();
                        var rangeRaw = sr.ReadLine().Split(',')[1];
                        
                        if (rangeRaw.Contains('-'))
                        {
                            var rangeLowRaw = rangeRaw.Substring(0, rangeRaw.IndexOf('-'));
                            var rangeHighRaw = rangeRaw.Substring(rangeRaw.IndexOf('-') + 1, rangeRaw.Length - (rangeRaw.IndexOf('-') + 1));
                            
                            int rangeLow, rangeHigh;
                            int.TryParse(rangeLowRaw, out rangeLow);
                            int.TryParse(rangeHighRaw, out rangeHigh);
                            
                            using (var sql = new BossDataContext())
                            {
                                if (!rangeData.Where(x => x == partCleaned).Any())
                                {
                                    if (!(partCleaned == "000DAILYSCOPECHECK"))
                                    {
                                        var rd = new BossRangeData();
                                        rd.Product = partCleaned;
                                        rd.Specification = specificationCleaned;
                                        rd.RangeLow = rangeLow;
                                        rd.RangeHigh = rangeHigh;
                                        sql.AddToBossRangeDatas(rd);
                                        sql.SaveChanges();
                                        rangeData.Add(partCleaned);
                                    }
                                }

                            }

                        }
                        else IsAValidProduct = false;
                        sr.ReadLine(); sr.ReadLine(); sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
                        Console.WriteLine(partCleaned);
                        while (!sr.EndOfStream)
                        {
                            LoadRecordIntoSQL(sr.ReadLine());
                        }
                        
                        
                        
                    }
                }
            }

            if (count < 1)
            {
                try
                {
                    File.Delete(myPath);
                }
                catch (Exception)
                {
                          File.Delete(myPath);
                }
                
            }
            else if (!IsAValidProduct)
            {
                var file = Path.GetFileName(myPath);
                File.Move(myPath, path + "/Errors/" + file);
            }
            else
            {
                if (!File.Exists(Path.ChangeExtension(myPath,".csv.001")))
                {
                    File.Move(myPath, Path.ChangeExtension(myPath,".csv.001"));
                }
                else
                {
                    File.Move(myPath, MakeUnique(Path.ChangeExtension(myPath, ".csv.001")));
                }
                
            }
            return true;


        }

        public string MakeUnique(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = Path.GetExtension(path);


            
            for (int i = 1; ; ++i)
            {
                if (!File.Exists(path))
                    return path;

                path = Path.Combine(dir, fileName + " (" + i + ") " + fileExt);
            }

        }

        private string _currentFileOrigin = string.Empty;

        private void LoadRecordIntoSQL(string p)
        {

            var newRecord = new Record();
            var record = p.Split(',').ToList();
            if (record.Count > 8)
            {
                record.ForEach(x => x = x.Trim());
                newRecord.Date = GetDate(record[1], record[2]);
                newRecord.BHNReading = GetBHN(record[7]);
                newRecord.CastDate = GetCastDate(record[8]);
                newRecord.Product = record[9].RemoveSpecialCharacters();
                if (newRecord.Product == "000DAILYSCOPECHECK")
                {
                    newRecord.Product = "SCOPE_CHECK_" + _currentFileOrigin;
                }
                newRecord.DateCast = DateCodes.Translate(newRecord.CastDate);
                DateTime t;
                var dateTest = DateTime.TryParse(newRecord.CastDate, out t);
                if (dateTest == true)
                {
                    newRecord.DateCast = t;
                }
                
                using (var sql = new BossDataContext())
                {
                    if (!Records.Where(x => x.Product == newRecord.Product && x.Date == newRecord.Date && x.BHNReading == newRecord.BHNReading && newRecord.CastDate == x.CastDate).Any())
                    {
                        Console.WriteLine(record[0] + " is a new record.  Adding to SQL.");
                        sql.AddToRecords(newRecord);
                        Records.Add(newRecord);
                        sql.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine(record[0] + " is a old record.  Skipping");
                    }
                }
            }
            
        }

        private string GetCastDate(string p)
        {
            return p;
        }

        private double GetDiameter(string p)
        {
            return p.GetDoubleFromString();
        }

        private double GetBHN(string p)
        {
            return p.GetDoubleFromString();
        }
        
        private DateTime GetDate(string p, string s)
        {
            DateTime Date;
            if (p.Contains('"'))
            {
                DateTime.TryParse(p.Substring(1, p.Length - 2), out Date);
            }
            else DateTime.TryParse(p, out Date);
       
            Date = Date.AddMinutes(GetTime(s));
            return Date;
            
        }

        private double GetTime(string s)
        {
            string[] P;
            if (s.Contains('"'))
            {
                P = s.Substring(1, s.Length - 2).Split(':');
            }
            else
            {
                P = s.Split(':');
            }
           
            double hours, minutes;
            double.TryParse(P[0], out hours);
            double.TryParse(P[1], out minutes);
            return hours * 60 + minutes;

        }




    }
}
