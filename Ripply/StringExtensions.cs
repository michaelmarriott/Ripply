using System;
using System.Collections.Generic;
using System.Text;

namespace Ripply
{
    public  static class StringExtensions
    {
        public static string RemoveNewLines(this string value)
        {
            return value.Replace("\n", "");
        }
    }
}
