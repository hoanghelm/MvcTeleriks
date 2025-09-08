using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WIRS.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveSpecialCharacters(this string str)
        {
            string replacement = Regex.Replace(str, "/[\x00-\x1F\x7F]/u", "", RegexOptions.Compiled);

            replacement = Regex.Replace(str, @"[\n\u000B\u000C\r\u0085\u2028\u2029]", "");
            return replacement;
        }
    }
}