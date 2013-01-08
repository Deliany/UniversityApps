using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{
    static class SubscriptExtension
    {
        public static string Subscript(this string normal)
        {
            if (normal == null) return normal;

            var res = new StringBuilder();
            foreach (var c in normal)
            {
                char c1 = c;

                // I'm not quite sure if char.IsDigit(c) returns true for, for example, '³',
                // so I'm using the safe approach here
                if (c >= '0' && c <= '9')
                {
                    // 0x208x is the unicode offset of the subscripted number characters
                    c1 = (char)(c - '0' + 0x2080);
                }
                res.Append(c1);
            }

            return res.ToString();
        }
    }
}
