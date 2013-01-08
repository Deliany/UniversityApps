using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptographicAlgorithms.Cyphers
{
    class VigenereCypher
    {
        public Alphabet Alphabet { get; set; }

        public string Encrypt(string M, string Key)
        {
            StringBuilder C = new StringBuilder();
            string characters = Alphabet.GetStringValue();

            for (int i = 0; i < M.Length; i++)
            {
                //if (!characters.Contains(M[i]))
                //{
                //    continue;
                //}
                int newIndex = characters.IndexOf(M[i]) + characters.IndexOf(Key[i % Key.Length]);
                C.Append(characters[newIndex % characters.Length]);
            }
            return C.ToString();
        }

        public string Decrypt(string C, string Key)
        {
            string characters = Alphabet.GetStringValue();
            StringBuilder M = new StringBuilder();

            for (int i = 0; i < C.Length; i++)
            {
                int newIndex = characters.IndexOf(C[i]) - characters.IndexOf(Key[i % Key.Length]) + characters.Length;
                M.Append(characters[newIndex % characters.Length]);
            }

            return M.ToString();
        }
    }
}
