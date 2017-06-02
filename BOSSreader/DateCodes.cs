using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BOSSreader
{
    public class DateCodes
    {
        public DateCodes(bool UseSQL)
        {
            using (var sql = new BossDataContext())
            {
                foreach (var item in sql.Records.ToList())
                {
                    Console.Write(item.ID+ " ");
                    if (item.DateCast == null)
                    {
                        item.DateCast = TranslateDateCode(item.CastDate);
                        Console.Write(item.DateCast + Environment.NewLine);    
                    }
                }
                Console.WriteLine("Ready to Upload");
                Console.ReadKey();
                sql.SaveChanges();
            }

        }
        public static DateTime? Translate(string p)
        {
            p = p.ToUpper().RemoveSpecialCharacters();
            return TranslateDateCode(p);
        }

        private static DateTime? TranslateDateCode(string p)
        {
            p = p.Trim();
            var hasLetters = Regex.Matches(p, @"[a-zA-Z]").Count;
            switch (hasLetters)
            {
                case 0:
                    return TryJulian(p);
                case 1: 
                    if (p.Length == 4)
	                {
                        return TryAlpha4(p);
	                }
                    else if (p.Length == 5)
	                {
                        return TryAlpha5(p);
	                }
                    break;
                case 2:
                    return TryAlpha4(p.Substring(0,4));
                case 6:
                    return TryNumeralkod(p);
                default:
                    break;
            }
            return null;
        }

        private static DateTime? TryNumeralkod(string p)
        {
            p = p.ToUpperInvariant();
            try
            {
                var s = "NUMERALKOD";
                string translation = null;
                foreach (var item in p)
                {
                    translation += s.IndexOf(item).ToString();
                }
                int day, month, year;
                int.TryParse(translation.Substring(0, 2), out day);
                int.TryParse(translation.Substring(2, 2), out month);
                int.TryParse(translation.Substring(4, 2), out year);
                return new DateTime(year + 2000, month, day);
            }
            catch
            {
                return null;
            }
        }

        private static DateTime? TryAlpha5(string p)
        {
            try
            {
                p = p.ToUpperInvariant();
                var s = " ABCDEFGHJKLM";
                int day, month, year;
                int.TryParse(p.Substring(0, 2), out day);
                month = s.IndexOf(p.Substring(2, 1));
                int.TryParse(p.Substring(3, 2), out year);
                year = year + 2000;
                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }

        private static DateTime? TryAlpha4(string p)
        {
            try
            {
                p = p.ToUpperInvariant();
                var s = " ABCDEFGHJKLM";
                int day, month, year;
                int.TryParse(p.Substring(0, 2), out day);
                month = s.IndexOf(p.Substring(2, 1));
                int.TryParse(p.Substring(3, 1), out year);
                var addyear = int.Parse(DateTime.Now.Year.ToString().Substring(0, 3)) * 10;
                year = year + addyear;
                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }

        private static DateTime? TryJulian(string p)
        {
            try
            {
                int day, year;
                int.TryParse(p.Substring(0, 1), out year);
                int.TryParse(p.Substring(1, 3), out day);

                var addyear = int.Parse(DateTime.Now.Year.ToString().Substring(0, 3)) * 10;
                var date = new DateTime(year + addyear, 1, 1).AddDays(day - 1);
                return date;
            }
            catch
            {
                return null;
            }

        }
    }
}
