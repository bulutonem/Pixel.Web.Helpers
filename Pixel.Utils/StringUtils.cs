using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Utils
{
    public class StringUtils
    {
        public string ToCapitalCase(string source)
        {
            string cultureCode = "en-US";
            return ToCapitalCase(source, cultureCode);
        }
        public string ToCapitalCase(string source, string cultureCode)
        {
            TextInfo textInfo = new CultureInfo(cultureCode, false).TextInfo;
            return textInfo.ToTitleCase(source);
        }
    }
}
