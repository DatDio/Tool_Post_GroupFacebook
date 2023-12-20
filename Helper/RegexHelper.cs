using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tool_Facebook.Helper
{
    public class RegexHelper
    {
        public static string GetValueFromGroup(string regex, string content, int group = 1)
        {
            var match = Regex.Match(content, regex);
            if (match.Success)
            {
                return match.Groups[group].Value;
            }
            return "";
        }
    }
}
