using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel.Utils
{
    public static class Extensions
    {
        public static string ToCapitalCase(this string source)
        {
            return new StringUtils().ToCapitalCase(source);
        }
    }
}
