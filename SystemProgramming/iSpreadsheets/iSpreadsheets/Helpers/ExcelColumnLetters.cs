using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iSpreadsheets.Helpers
{
    /// <summary>
    /// Class for converting between integers and spreadsheet column letters.
    /// </summary>
    static class SSColumns
    {
        private const string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Converts an integer value to the
        /// </summary>
        /// <param name="value">Integer to convert</param>
        public static string ToString(int value)
        {
            StringBuilder builder = new StringBuilder();
            do
            {
                if (builder.Length > 0)
                    value--;
                builder.Insert(0, _letters[value % _letters.Length]);
                value /= _letters.Length;
            } while (value > 0);

            return builder.ToString();
        }

        /// <summary>
        /// Converts spreadsheet column letters to their corresponding
        /// integer value.
        /// </summary>
        /// <param name="s">String to parse</param>
        public static int Parse(string s)
        {
            int result;
            if (!TryParse(s, out result))
                throw new ArgumentException();
            return result;
        }

        /// <summary>
        /// Converts spreadsheet column letters to their corresponding
        /// integer value.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="result">The resulting integer value</param>
        public static bool TryParse(string s, out int result)
        {
            // Normalize input
            s = s.Trim().ToUpper();

            int pos = 0;
            result = 0;

            // Use lookup table to parse string
            while (pos < s.Length && !Char.IsWhiteSpace(s[pos]))
            {
                if (pos > 0)
                    result++;

                string digit = s.Substring(pos, 1);
                int i = _letters.IndexOf(digit, System.StringComparison.Ordinal);
                if (i >= 0)
                {
                    result *= _letters.Length;
                    result += i;
                    pos++;
                }
                else
                {
                    // Invalid character encountered
                    return false;
                }
            }
            // Return true if any characters processed
            return (pos > 0);
        }
    }
}
