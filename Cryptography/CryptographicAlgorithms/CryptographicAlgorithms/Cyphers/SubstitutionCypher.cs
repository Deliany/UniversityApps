using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptographicAlgorithms.Cyphers
{
    class SubstitutionCypher
    {
        public Alphabet Alphabet { get; set; }

        public string Encrypt(string M, Dictionary<char,char> Key)
        {
            StringBuilder C = new StringBuilder();
            string characters = Alphabet.GetStringValue();

            for (int i = 0; i < M.Length; i++)
            {
                //if(!characters.Contains(M[i]))
                //{
                //    continue;
                //}
                C.Append(Key[M[i]]);
            }
            return C.ToString();
        }

        public string Decrypt(string C, Dictionary<char, char> Key)
        {
            return Encrypt(C, Key.ToDictionary(k => k.Value, k => k.Key));
        }
    }
}
