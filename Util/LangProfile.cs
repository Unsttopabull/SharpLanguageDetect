///////////////////////////////////////////////////////////////////
//   Ported to C# by Martin Kraner <martinkraner@outlook.com>    //
//   from https://code.google.com/p/language-detection/          //
///////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

#pragma warning disable 1591

namespace Frost.SharpLanguageDetect.Util {

    /**
     * {@link LangProfile} is a Language Profile Class.
     * Users don't use this class directly.
     * 
     * @author Nakatani Shuyo
     */
    public class LangProfile : ILanguageProfile {
        private const int MINIMUM_FREQ = 2;
        private const int LESS_FREQ_RATIO = 100000;

        [NonSerialized]
        private static readonly Regex Regex1 = new Regex("^[A-Za-z]$", RegexOptions.Compiled);

        [NonSerialized]
        private static readonly Regex Regex2 = new Regex(".*[A-Za-z].*", RegexOptions.Compiled);

        [JsonProperty("n_words")]
        public readonly int[] _nWords = new int[NGram.N_GRAM];

        [JsonProperty("freq")]
        private Dictionary<string, int> _frequency;

        public Dictionary<string, int> Frequency {
            get { return _frequency;  } 
            private set { _frequency = value; }
        }

        [JsonProperty("name")]
        public string Name { get; private set; }

        public int[] NWords { get { return _nWords; } }

        public LangProfile() {
        }

        /**
        * Normal Constructor
        * @param name language name
        */
        public LangProfile(string name) {
            Frequency = new Dictionary<string, int>();
            Name = name;
        }

        public LangProfile(string name, int[] nWords) {
            Name = name;
            _nWords = nWords;
        }

        /**
        * Add n-gram to profile
        * @param gram
        */
        public void Add(string gram) {
            if (Name == null || gram == null) {
                return; // Illegal
            }

            int len = gram.Length;
            if (len < 1 || len > NGram.N_GRAM) {
                return; // Illegal
            }

            ++NWords[len - 1];
            if (Frequency.ContainsKey(gram)) {
                Frequency[gram]++;
            }
            else {
                Frequency.Add(gram, 1);
            }
        }

        /**
        * Eliminate below less frequency n-grams and noise Latin alphabets
        */
        public void OmitLessFrequent() {
            if (Name == null) {
                return; // Illegal
            }

            int threshold = NWords[0] / LESS_FREQ_RATIO;
            if (threshold < MINIMUM_FREQ) {
                threshold = MINIMUM_FREQ;
            }

            IEnumerable<string> keys = Frequency.Keys;
            List<string> keyToRemove = new List<string>();

            int roman = 0;
            foreach (string key in keys) {
                int count = Frequency[key];
                if (count <= threshold) {
                    NWords[key.Length - 1] -= count;

                    //Frequency.Remove(key); //exception
                    keyToRemove.Add(key);
                }
                else {
                    if (Regex1.IsMatch(key)) {
                        roman += count;
                    }
                }
            }

            foreach (string key in keyToRemove) {
                Frequency.Remove(key);
            }

            keyToRemove.Clear();

            // roman check
            if (roman < NWords[0] / 3) {
                IEnumerable<string> keys2 = Frequency.Keys;
                foreach (string key in keys2) {
                    if (Regex2.IsMatch(key)) {
                        NWords[key.Length - 1] -= Frequency[key];

                        keyToRemove.Add(key);
                        //Frequency.Remove(key); //Exception
                    }
                }

                foreach (string key in keyToRemove) {
                    Frequency.Remove(key);
                }
            }
        }

        /// <summary>Load Wikipedia abstract database file and generate its language profile</summary>
        /// <param name="lang">Target language name.</param>
        /// <param name="file">File target database file path.</param>
        /// <returns>Language profile instance parsed from the database.</returns>
        /// <exception cref="LangDetectException">Throws if the file is not an XML file or can't be opened.</exception>
        public static LangProfile LoadFromWikipediaAbstract(string lang, FileInfo file) {
            LangProfile profile = new LangProfile(lang);

            try {
                using (StreamReader sr = file.Name.EndsWith(".gz") ? new StreamReader(new GZipStream(file.OpenRead(), CompressionMode.Decompress)) : new StreamReader(file.OpenRead())) {
                    TagExtractor tagExtractor = new TagExtractor("abstract", 100);

                    using (XmlReader reader = XmlReader.Create(sr)) {
                        while (reader.Read()) {
                            switch (reader.NodeType) {
                                case XmlNodeType.Element:
                                    tagExtractor.SetTag(reader.LocalName);
                                    break;
                                case XmlNodeType.Text:
                                    tagExtractor.Add(reader.Value);
                                    break;
                                case XmlNodeType.EndElement:
                                    tagExtractor.CloseTag(profile);
                                    break;
                            }
                        }
                        Console.WriteLine(@"{0}:{1}", lang, tagExtractor.Count());
                    }
                }
            }
            catch (XmlException) {
                throw new LangDetectException(ErrorCode.TrainDataFormatError, string.Format("Training database file '{0}' is an invalid XML.", file.Name));
            }
            catch (Exception) {
                throw new LangDetectException(ErrorCode.CantOpenTrainData, string.Format("Can't open training database file '{0}'", file.Name));
            }
            return profile;
        }

        /// <summary>Load text file with UTF-8 and generate its language profile</summary>
        /// <param name="lang">target language name</param>
        /// <param name="file">file target file path</param>
        /// <returns>Language profile instance</returns>
        /// <exception cref="LangDetectException">Throws when the training database file can't be opened.</exception>
        public static LangProfile LoadFromText(string lang, FileInfo file) {
            LangProfile profile = new LangProfile(lang);

            StreamReader sr = null;
            try {
                sr = new StreamReader(file.OpenRead(), Encoding.UTF8);

                int count = 0;
                while (!sr.EndOfStream) {
                    string line = sr.ReadLine();
                    NGram gram = new NGram();
                    if (line != null) {
                        foreach (char ch in line) {
                            gram.AddChar(ch);
                            for (int n = 1; n <= NGram.N_GRAM; ++n) {
                                profile.Add(gram[n]);
                            }
                        }
                    }
                    ++count;
                }
                Console.WriteLine(lang + ":" + count);
            }
            catch (IOException) {
                throw new LangDetectException(ErrorCode.CantOpenTrainData, "Can't open training database file '" + file.Name + "'");
            }
            finally {
                if (sr != null) {
                    sr.Close();
                }
            }
            return profile;
        }
    }

}