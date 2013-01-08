using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace iSpreadsheets.Helpers
{
    public static class RegexReplacer
    {
        private const string PatternForMatchRange = @"(?<Range>(?<Column1>[a-zA-Z]{1,3})(?<Row1>\d{1,3})\s*:\s*(?<Column2>[a-zA-Z]{1,3})(?<Row2>\d{1,3}))";

        public static string ReplaceRangeOperators(string input)
        {
            return Regex.Replace(input, PatternForMatchRange, m => ReplaceNamedGroup(input, m));
        }

        private static string ReplaceNamedGroup(string input, Match m)
        {
            var capt = m.Groups["Range"].Captures.OfType<Capture>().FirstOrDefault();

            if (capt == null)
                return m.Value;

            var col1 = SSColumns.Parse(m.Groups["Column1"].Value);
            var row1 = int.Parse(m.Groups["Row1"].Value);
            var col2 = SSColumns.Parse(m.Groups["Column2"].Value);
            var row2 = int.Parse(m.Groups["Row2"].Value);

            var colFrom = col1 < col2 ? col1 : col2;
            var colTo = colFrom == col1 ? col2 : col1;

            var rowFrom = row1 < row2 ? row1 : row2;
            var rowTo = rowFrom == row1 ? row2 : row1;

            StringBuilder extendedRange = new StringBuilder();
            for (int i = colFrom; i <= colTo; i++)
            {
                for (int j = rowFrom; j <= rowTo; j++)
                {
                    extendedRange.Append(SSColumns.ToString(i) + j);
                    if (i != colTo || j != rowTo)
                    {
                        extendedRange.Append(",");
                    }
                }
            }

            var sb = new StringBuilder(input);
            sb.Remove(capt.Index, capt.Length);
            sb.Insert(capt.Index, extendedRange.ToString());

            return sb.ToString();
        }
    }
}
