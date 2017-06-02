using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOSSreader
{
    public static class StringExtension
    {
        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static double GetDoubleFromString(this string str)
        {
            double d;
            double.TryParse(str, out d);
            return d;
        }

    }
}
