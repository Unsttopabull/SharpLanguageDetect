using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Frost.SharpLanguageDetect.Util {

    /**
     * {@link LangProfile} is a Language Profile Class.
     * Users don't use this class directly.
     * 
     * @author Nakatani Shuyo
     */
    public class LangProfile {

        private const int MINIMUM_FREQ = 2;
        private const int LESS_FREQ_RATIO = 100000;
        public Dictionary<string, int> freq = new Dictionary<string, int>();
        public int[] n_words = new int[NGram.N_GRAM];
        public string name;
        public static readonly Regex Regex1 = new Regex("^[A-Za-z]$", RegexOptions.Compiled);
        public static readonly Regex Regex2 = new Regex(".*[A-Za-z].*", RegexOptions.Compiled);

        /**
        * Constructor for JSONIC 
        */
        public LangProfile() {
        }

        /**
        * Normal Constructor
        * @param name language name
        */
        public LangProfile(string name) {
            this.name = name;
        }

        /**
        * Add n-gram to profile
        * @param gram
        */
        public void add(string gram) {
            if (name == null || gram == null) {
                return; // Illegal
            }

            int len = gram.Length;
            if (len < 1 || len > NGram.N_GRAM) {
                return; // Illegal
            }

            ++n_words[len - 1];
            if (freq.ContainsKey(gram)) {
                freq.Add(gram, freq[gram] + 1);
                //freq[gram]++;
            }
            else {
                freq.Add(gram, 1);
            }
        }

        /**
        * Eliminate below less frequency n-grams and noise Latin alphabets
        */
        public void omitLessFreq() {
            if (name == null) {
                return; // Illegal
            }

            int threshold = n_words[0] / LESS_FREQ_RATIO;
            if (threshold < MINIMUM_FREQ) {
                threshold = MINIMUM_FREQ;
            }

            IEnumerable<string> keys = freq.Keys;
            int roman = 0;
            for (Iterator<string> i = keys.iterator(); i.hasNext();) {
                string key = i.next();
                int count = freq[key];
                if (count <= threshold) {
                    n_words[key.Length - 1] -= count;
                    i.remove();
                }
                else {
                    if (Regex1.IsMatch(key)) {
                        roman += count;
                    }
                }
            }

            // roman check
            if (roman < n_words[0] / 3) {
                IEnumerable<string> keys2 = freq.Keys;
                for (Iterator<string> i = keys2.iterator(); i.hasNext();) {
                    string key = i.next();
                    if (Regex2.IsMatch(key)) {
                        n_words[key.Length - 1] -= freq[key];
                        i.remove();
                    }
                }
            }
        }

    }

}