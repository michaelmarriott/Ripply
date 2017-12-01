using System;

namespace Ripply.Extensions
{
    public  static class StringExtensions
    {
        public static string RemoveNewLines(this string value)
        {
            return value.Replace("\n", "");
        }
    }
}
