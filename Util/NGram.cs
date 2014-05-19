///////////////////////////////////////////////////////////////////
//   Ported to C# by Martin Kraner <martinkraner@outlook.com>    //
//   from https://code.google.com/p/language-detection/          //
///////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using Frost.SharpLanguageDetect.JDK;
using Frost.SharpLanguageDetect.Properties;
using StringBuilder = System.Text.StringBuilder;

namespace Frost.SharpLanguageDetect.Util {

    /**
    * 
    * Users don't use this class directly.
    * @author Nakatani Shuyo
    */
    public class NGram {

        public const int N_GRAM = 3;
        private static readonly string Latin1Excluded = Resources.NGram_LATIN1_EXCLUDE;
        private static readonly Dictionary<char, char> CjkMap = new Dictionary<char, char>();

        private bool _capitalword;
        private StringBuilder _grams;

        static NGram() {
            foreach (string cjkList in CjkClass) {
                char representative = cjkList[0];
                foreach (char ch in cjkList) {
                    CjkMap.Add(ch, representative);
                }
            }
        }

        /**
        * 
        */
        public NGram() {
            _grams = new StringBuilder(" ");
            _capitalword = false;
        }

        /**
        * @param ch
        */
        public void AddChar(char ch) {
            ch = Normalize(ch);
            char lastchar = _grams[_grams.Length - 1];

            if (lastchar == ' ') {
                _grams = new StringBuilder(" ");
                _capitalword = false;
                if (ch == ' ') {
                    return;
                }
            }
            else if (_grams.Length >= N_GRAM) {
                _grams.Remove(0, 1);
            }

            _grams.Append(ch);

            if (char.IsUpper(ch)) {
                if (char.IsUpper(lastchar)) {
                    _capitalword = true;
                }
            }
            else {
                _capitalword = false;
            }
        }

        /**
        * Get n-Gram
        * @param n length of n-gram
        * @return n-Gram string (null if it is invalid)
        */
        public string this[int n] {
            get {
                if (_capitalword) {
                    return null;
                }

                int len = _grams.Length;
                if (n < 1 || n > 3 || len < n) {
                    return null;
                }

                if (n == 1) {
                    char ch = _grams[len - 1];

                    return (ch == ' ')
                        ? null
                        : char.ToString(ch);
                }
                return _grams.ToString().Substring(len - n, n);
            }
        }

        /**
        * char Normalization
        * @param ch
        * @return Normalized character
        */
        public static char Normalize(char ch) {
            UnicodeBlock block = UnicodeBlock.Of(ch);

            //comparing by-ref is ok
            if (block == UnicodeBlock.BASIC_LATIN) {
                if (ch < 'A' || (ch < 'a' && ch > 'Z') || ch > 'z') {
                    ch = ' ';
                }
            }
            else if (block == UnicodeBlock.LATIN_1_SUPPLEMENT) {
                if (Latin1Excluded.IndexOf(ch) >= 0) {
                    ch = ' ';
                }
            }
            else if (block == UnicodeBlock.GENERAL_PUNCTUATION) {
                ch = ' ';
            }
            else if (block == UnicodeBlock.ARABIC) {
                if (ch == '\u06cc') {
                    ch = '\u064a';
                }
            }
            else if (block == UnicodeBlock.LATIN_EXTENDED_ADDITIONAL) {
                if (ch >= '\u1ea0') {
                    ch = '\u1ec3';
                }
            }
            else if (block == UnicodeBlock.HIRAGANA) {
                ch = '\u3042';
            }
            else if (block == UnicodeBlock.KATAKANA) {
                ch = '\u30a2';
            }
            else if (block == UnicodeBlock.BOPOMOFO || block == UnicodeBlock.BOPOMOFO_EXTENDED) {
                ch = '\u3105';
            }
            else if (block == UnicodeBlock.CJK_UNIFIED_IDEOGRAPHS) {
                if (CjkMap.ContainsKey(ch)) {
                    ch = CjkMap[ch];
                }
            }
            else if (block == UnicodeBlock.HANGUL_SYLLABLES) {
                ch = '\uac00';
            }
            return ch;
        }

        #region CJK CJK Kanji Normalization Mapping

        /**
        * CJK Kanji Normalization Mapping
        */
        private static readonly string[] CjkClass = {
            Resources.NGram_KANJI_1_0,
            Resources.NGram_KANJI_1_2,
            Resources.NGram_KANJI_1_4,
            Resources.NGram_KANJI_1_8,
            Resources.NGram_KANJI_1_11,
            Resources.NGram_KANJI_1_12,
            Resources.NGram_KANJI_1_13,
            Resources.NGram_KANJI_1_14,
            Resources.NGram_KANJI_1_16,
            Resources.NGram_KANJI_1_18,
            Resources.NGram_KANJI_1_22,
            Resources.NGram_KANJI_1_27,
            Resources.NGram_KANJI_1_29,
            Resources.NGram_KANJI_1_31,
            Resources.NGram_KANJI_1_35,
            Resources.NGram_KANJI_2_0,
            Resources.NGram_KANJI_2_1,
            Resources.NGram_KANJI_2_4,
            Resources.NGram_KANJI_2_9,
            Resources.NGram_KANJI_2_10,
            Resources.NGram_KANJI_2_11,
            Resources.NGram_KANJI_2_12,
            Resources.NGram_KANJI_2_13,
            Resources.NGram_KANJI_2_15,
            Resources.NGram_KANJI_2_16,
            Resources.NGram_KANJI_2_18,
            Resources.NGram_KANJI_2_21,
            Resources.NGram_KANJI_2_22,
            Resources.NGram_KANJI_2_23,
            Resources.NGram_KANJI_2_28,
            Resources.NGram_KANJI_2_29,
            Resources.NGram_KANJI_2_30,
            Resources.NGram_KANJI_2_31,
            Resources.NGram_KANJI_2_32,
            Resources.NGram_KANJI_2_35,
            Resources.NGram_KANJI_2_36,
            Resources.NGram_KANJI_2_37,
            Resources.NGram_KANJI_2_38,
            Resources.NGram_KANJI_3_1,
            Resources.NGram_KANJI_3_2,
            Resources.NGram_KANJI_3_3,
            Resources.NGram_KANJI_3_4,
            Resources.NGram_KANJI_3_5,
            Resources.NGram_KANJI_3_8,
            Resources.NGram_KANJI_3_9,
            Resources.NGram_KANJI_3_11,
            Resources.NGram_KANJI_3_12,
            Resources.NGram_KANJI_3_13,
            Resources.NGram_KANJI_3_15,
            Resources.NGram_KANJI_3_16,
            Resources.NGram_KANJI_3_18,
            Resources.NGram_KANJI_3_19,
            Resources.NGram_KANJI_3_22,
            Resources.NGram_KANJI_3_23,
            Resources.NGram_KANJI_3_27,
            Resources.NGram_KANJI_3_29,
            Resources.NGram_KANJI_3_30,
            Resources.NGram_KANJI_3_31,
            Resources.NGram_KANJI_3_32,
            Resources.NGram_KANJI_3_35,
            Resources.NGram_KANJI_3_36,
            Resources.NGram_KANJI_3_37,
            Resources.NGram_KANJI_3_38,
            Resources.NGram_KANJI_4_0,
            Resources.NGram_KANJI_4_9,
            Resources.NGram_KANJI_4_10,
            Resources.NGram_KANJI_4_16,
            Resources.NGram_KANJI_4_17,
            Resources.NGram_KANJI_4_18,
            Resources.NGram_KANJI_4_22,
            Resources.NGram_KANJI_4_24,
            Resources.NGram_KANJI_4_28,
            Resources.NGram_KANJI_4_34,
            Resources.NGram_KANJI_4_39,
            Resources.NGram_KANJI_5_10,
            Resources.NGram_KANJI_5_11,
            Resources.NGram_KANJI_5_12,
            Resources.NGram_KANJI_5_13,
            Resources.NGram_KANJI_5_14,
            Resources.NGram_KANJI_5_18,
            Resources.NGram_KANJI_5_26,
            Resources.NGram_KANJI_5_29,
            Resources.NGram_KANJI_5_34,
            Resources.NGram_KANJI_5_39,
            Resources.NGram_KANJI_6_0,
            Resources.NGram_KANJI_6_3,
            Resources.NGram_KANJI_6_9,
            Resources.NGram_KANJI_6_10,
            Resources.NGram_KANJI_6_11,
            Resources.NGram_KANJI_6_12,
            Resources.NGram_KANJI_6_16,
            Resources.NGram_KANJI_6_18,
            Resources.NGram_KANJI_6_20,
            Resources.NGram_KANJI_6_21,
            Resources.NGram_KANJI_6_22,
            Resources.NGram_KANJI_6_23,
            Resources.NGram_KANJI_6_25,
            Resources.NGram_KANJI_6_28,
            Resources.NGram_KANJI_6_29,
            Resources.NGram_KANJI_6_30,
            Resources.NGram_KANJI_6_32,
            Resources.NGram_KANJI_6_34,
            Resources.NGram_KANJI_6_35,
            Resources.NGram_KANJI_6_37,
            Resources.NGram_KANJI_6_39,
            Resources.NGram_KANJI_7_0,
            Resources.NGram_KANJI_7_3,
            Resources.NGram_KANJI_7_6,
            Resources.NGram_KANJI_7_7,
            Resources.NGram_KANJI_7_9,
            Resources.NGram_KANJI_7_11,
            Resources.NGram_KANJI_7_12,
            Resources.NGram_KANJI_7_13,
            Resources.NGram_KANJI_7_16,
            Resources.NGram_KANJI_7_18,
            Resources.NGram_KANJI_7_19,
            Resources.NGram_KANJI_7_20,
            Resources.NGram_KANJI_7_21,
            Resources.NGram_KANJI_7_23,
            Resources.NGram_KANJI_7_25,
            Resources.NGram_KANJI_7_28,
            Resources.NGram_KANJI_7_29,
            Resources.NGram_KANJI_7_32,
            Resources.NGram_KANJI_7_33,
            Resources.NGram_KANJI_7_35,
            Resources.NGram_KANJI_7_37,
        };

        #endregion
    }

}