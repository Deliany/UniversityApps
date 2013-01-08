using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CryptographicAlgorithms.Cyphers
{
    public enum Alphabet
    {
        [StringValue(" abcdefghijklmnopqrstuvwxyz")]
        Latin,

        [StringValue(" абвгґдеєжзиіїйклмнопрстуфхцчшщьюя")]
        Ukrainian
    }

    public enum Cypher
    {
        ShiftCypher,
        SubstitutionCypher,
        VigenereCypher
    }
}
