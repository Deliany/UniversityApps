using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;
using CryptographicAlgorithms.Cyphers;

namespace CryptographicAlgorithms.Analyzer
{
    public class FrequencyAnalysis
    {
        public Alphabet Alphabet { get; set; }

        public SerializableDictionary<char, double> LatinTable { get; private set; }
        //private readonly SerializableDictionary<char, int> _latinLetterCount; 

        public SerializableDictionary<char, double> UkrainianTable { get; private set; }
        //private readonly SerializableDictionary<char, int> _ukrainianLetterCount;

        //private int _textLength;

        public FrequencyAnalysis()
        {
            LatinTable = new SerializableDictionary<char, double>();
            //_latinLetterCount = new SerializableDictionary<char, int>();
            UkrainianTable = new SerializableDictionary<char, double>();
            //_ukrainianLetterCount = new SerializableDictionary<char, int>();
        }

        private void Initialize()
        {
            if(LatinTable.Count == 0)
            {
                foreach (var letter in Alphabet.Latin.GetStringValue())
                {
                    LatinTable.Add(letter, 0);
                    //_latinLetterCount.Add(letter, 0);
                }
            }
            if (UkrainianTable.Count == 0)
            {
                foreach (var letter in Alphabet.Ukrainian.GetStringValue())
                {
                    UkrainianTable.Add(letter, 0);
                    //_ukrainianLetterCount.Add(letter, 0);
                }
            }
        }

        public void Analyze(string text)
        {
            Initialize();
            if(text == string.Empty)
            {
                return;
            }

            var lettersCount = new Dictionary<char, int>();
            foreach (var letter in Alphabet.GetStringValue())
            {
                    lettersCount.Add(letter, 0);
            }
            foreach (var symbol in text)
            {
                if (lettersCount.ContainsKey(symbol))
                {
                    lettersCount[symbol] += 1;
                }
            }


            int textLength = Regex.Replace(text, "[^" + Alphabet.GetStringValue() + "]", "").Length;

            //_textLength += textLength;
            switch (Alphabet)
            {
                case Alphabet.Latin:

                    foreach (var character in lettersCount)
                    {
                        //_latinLetterCount[character.Key] += character.Value;
                        LatinTable[character.Key] = (double) character.Value*100/textLength;
                    }
                    break;
                case Alphabet.Ukrainian:
                    foreach (var character in lettersCount)
                    {
                        //_ukrainianLetterCount[character.Key] += character.Value;
                        UkrainianTable[character.Key] = (double) character.Value*100/textLength;
                    }
                    break;
            }
        }

        public void Serialize()
        {
            if (File.Exists("db.xml"))
            {
                File.Delete("db.xml");
            }
            using (Stream textWriter = File.Open("db.xml", FileMode.OpenOrCreate))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(textWriter, this);
            }
        }

        public FrequencyAnalysis Deserialize()
        {
            File.Copy("db.xml", "db_backup.xml", true);
            FrequencyAnalysis analyzer = new FrequencyAnalysis();
            using (Stream textReader = File.Open("db.xml", FileMode.Open))
            {
                XmlSerializer deserializer = new XmlSerializer(this.GetType());
                analyzer = (FrequencyAnalysis)deserializer.Deserialize(textReader);
            }
            return analyzer;
        }
    }
}
