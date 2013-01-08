using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CryptographicAlgorithms.Cyphers
{
    class ShiftCypher
    {
        public Alphabet Alphabet { get; set; }

        public string Encrypt(string M, int Key)
        {
            StringBuilder C = new StringBuilder();

            string characters = Alphabet.GetStringValue();
            for (int i = 0; i < M.Length; i++)
            {
                //if(!characters.Contains(M[i]))
                //{
                //    continue;
                //}

                int newIndex = (characters.IndexOf(M[i]) + Key) % characters.Length;
                C.Append(characters[newIndex]);
            }
            return C.ToString();
        }

        public string Decrypt(string C, int Key)
        {
            return Encrypt(C, Alphabet.GetLength() - Key);
        }
    }
}
