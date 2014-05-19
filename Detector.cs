///////////////////////////////////////////////////////////////////
//   Ported to C# by Martin Kraner <martinkraner@outlook.com>    //
//   from https://code.google.com/p/language-detection/          //
///////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Frost.SharpLanguageDetect.JDK;
using Frost.SharpLanguageDetect.Util;
using Newtonsoft.Json;

namespace Frost.SharpLanguageDetect {

    /**
    * {@link Detector} class is to detect language from specified text. 
    * Its instance is able to be constructed via the factory class {@link DetectorFactory}.
    * <p>
    * After appending a target text to the {@link Detector} instance with {@link #append(Reader)} or {@link #append(string)},
    * the detector provides the language detection results for target text via {@link #detect()} or {@link #getProbabilities()}.
    * {@link #detect()} method returns a single language name which has the highest probability.
    * {@link #getProbabilities()} methods returns a list of multiple languages and their probabilities.
    * </p><p>  
    * The detector has some parameters for language detection.
    * See {@link #setAlpha(double)}, {@link #setMaxTextLength(int)} and {@link #setPriorMap(Dictionary)}.
    * </p>
     * 
    * <pre>
    * import java.util.List;
    * import com.cybozu.labs.langdetect.Detector;
    * import com.cybozu.labs.langdetect.DetectorFactory;
    * import com.cybozu.labs.langdetect.Language;
    * 
    * class LangDetectSample {
    *     public void init(string profileDirectory) throws LangDetectException {
    *         DetectorFactory.loadProfile(profileDirectory);
    *     }
    *     public string detect(string text) throws LangDetectException {
    *         Detector detector = DetectorFactory.create();
    *         detector.append(text);
    *         return detector.detect();
    *     }
    *     public List&lt;Language&gt; detectLangs(string text) throws LangDetectException {
    *         Detector detector = DetectorFactory.create();
    *         detector.append(text);
    *         return detector.getProbabilities();
    *     }
    * }
    * </pre>
    * 
    * <ul>
    * <li>4x faster improvement based on Elmer Garduno's code. Thanks!</li>
    * </ul>
    * 
    * @author Nakatani Shuyo
    * @see DetectorFactory
    */

    public class Detector {
        private const double ALPHA_DEFAULT = 0.5;
        private const double ALPHA_WIDTH = 0.05;

        private const int ITERATION_LIMIT = 1000;
        private const double PROBABILITY_THRESHOLD = 0.1;
        private const double CONV_THRESHOLD = 0.99999;
        private const int BASE_FREQ = 10000;
        private const string UNKNOWN_LANG = "unknown";
        private const int N_TRIAL = 7;

        private static readonly Regex URLRegex = new Regex(@"https?://[-_.?&~;+=/#0-9A-Za-z]+", RegexOptions.Compiled);
        private static readonly Regex MailRegex = new Regex(@"[-_.0-9A-Za-z]+@[-_0-9A-Za-z]+[-_.0-9A-Za-z]+", RegexOptions.Compiled);

        [JsonProperty]
        private readonly List<string> _langList;

        [JsonProperty]
        private readonly Dictionary<string, double[]> _wordLangProbMap;

        [JsonProperty]
        private double[] _langprob;

        private int _maxTextLength = 10000;
        private double[] _priorMap;
        private long? _seed;
        private StringBuilder _text;

        /**
        * Constructor.
        * Detector instance can be constructed via {@link DetectorFactory#create()}.
        * @param factory {@link DetectorFactory} instance (only DetectorFactory inside)
        */

        public Detector(DetectorFactory factory) {
            Alpha = ALPHA_DEFAULT;

            _wordLangProbMap = factory.WordLangProbMap;
            _langList = factory.Langlist;
            _text = new StringBuilder(_maxTextLength);
            _seed = factory.Seed;
        }

        /**
         * Set Verbose Mode(use for debug).
         */
        public bool Verbose { get; set; }

        /**
         * Set smoothing parameter.
         * The default value is 0.5(i.e. Expected Likelihood Estimate).
         * @param alpha the smoothing parameter
         */
        public double Alpha { get; set; }

        /**
         * Specify max size of target text to use for language detection.
         * The default value is 10000(10KB).
         * @param max_text_length the max_text_length to set
         */

        public int MaxTextLength {
            get { return _maxTextLength; }
            set { _maxTextLength = value; }
        }

        /**
         * Set prior information about language probabilities.
         * @param priorMap the priorMap to set
         * @throws LangDetectException 
         */

        public void SetPriorityMap(Dictionary<string, double> priorMap) {
            _priorMap = new double[_langList.Count];
            double sump = 0;
            for (int i = 0; i < _priorMap.Length; ++i) {
                string lang = _langList[i];
                if (priorMap.ContainsKey(lang)) {
                    double p = priorMap[lang];
                    if (p < 0) {
                        throw new LangDetectException(ErrorCode.InitParamError, "Priority probability must be non-negative.");
                    }
                    _priorMap[i] = p;
                    sump += p;
                }
            }
            if (sump <= 0) {
                throw new LangDetectException(ErrorCode.InitParamError, "More one of prior probability must be non-zero.");
            }

            for (int i = 0; i < _priorMap.Length; ++i) {
                _priorMap[i] /= sump;
            }
        }


        /**
         * Append the target text for language detection.
         * This method read the text from specified input reader.
         * If the total size of target text exceeds the limit size specified by {@link Detector#setMaxTextLength(int)},
         * the rest is cut down.
         * 
         * @param reader the input reader (BufferedReader as usual)
         * @throws IOException Can't read the reader.
         */

        public void Append(StreamReader reader) {
            char[] buf = new char[_maxTextLength / 2];
            while (_text.Length < _maxTextLength && !reader.EndOfStream) {
                int length = reader.Read(buf, 0, buf.Length);
                Append(new string(buf, 0, length));
            }
        }

        /**
         * Append the target text for language detection.
         * If the total size of target text exceeds the limit size specified by {@link Detector#setMaxTextLength(int)},
         * the rest is cut down.
         * 
         * @param text the target text to append
         */

        public void Append(string text) {
            text = URLRegex.Replace(text, " ");
            text = MailRegex.Replace(text, " ");
            char pre = (char) 0;
            for (int i = 0; i < text.Length && i < _maxTextLength; ++i) {
                char c = NGram.Normalize(text[i]);
                if (c != ' ' || pre != ' ') {
                    _text.Append(c);
                }
                pre = c;
            }
        }

        /**
         * Cleaning text to detect
         * (eliminate URL, e-mail address and Latin sentence if it is not written in Latin alphabet)
         */

        private void CleaningText() {
            int latinCount = 0, nonLatinCount = 0;
            for (int i = 0; i < _text.Length; ++i) {
                char c = _text[i];
                if (c <= 'z' && c >= 'A') {
                    ++latinCount;
                } 
                    //safe by-ref comparison of Unicode blocks as they are cached and readonly (same ref everytime)
                else if (c >= '\u0300' && UnicodeBlock.Of(c) != UnicodeBlock.LATIN_EXTENDED_ADDITIONAL) {
                    ++nonLatinCount;
                }
            }
            if (latinCount * 2 < nonLatinCount) {
                StringBuilder textWithoutLatin = new StringBuilder();
                for (int i = 0; i < _text.Length; ++i) {
                    char c = _text[i];
                    if (c > 'z' || c < 'A') {
                        textWithoutLatin.Append(c);
                    }
                }
                _text = textWithoutLatin;
            }
        }

        /**
         * Detect language of the target text and return the language name which has the highest probability.
         * @return detected language name which has most probability.
         * @throws LangDetectException 
         *  code = ErrorCode.CantDetectError : Can't detect because of no valid features in text
         */

        public string Detect() {
            List<Language> probabilities = GetProbabilities();
            return (probabilities.Count > 0)
                       ? probabilities[0].LangCode
                       : UNKNOWN_LANG;
        }

        /**
         * Get language candidates which have high probabilities
         * @return possible languages list (whose probabilities are over PROB_THRESHOLD, ordered by probabilities descendently
         * @throws LangDetectException 
         *  code = ErrorCode.CantDetectError : Can't detect because of no valid features in text
         */

        public List<Language> GetProbabilities() {
            if (_langprob == null) {
                DetectBlock();
            }

            List<Language> list = SortProbability(_langprob);
            return list;
        }

        /**
         * @throws LangDetectException 
         * 
         */

        private void DetectBlock() {
            CleaningText();
            List<string> ngrams = ExtractNGrams();
            if (ngrams.Count == 0) {
                throw new LangDetectException(ErrorCode.CantDetectError, "no features in text");
            }

            _langprob = new double[_langList.Count];

            GaussianRandom rand = _seed.HasValue
                                      ? new GaussianRandom((int) _seed.Value)
                                      : new GaussianRandom();

            //Random rand = new Random();
            //if (_seed.HasValue) {
            //    rand.setSeed(_seed.Value);
            //}

            for (int t = 0; t < N_TRIAL; ++t) {
                double[] prob = InitProbability();
                double alpha = Alpha + rand.NextGaussian() * ALPHA_WIDTH;

                for (int i = 0;; ++i) {
                    int r = rand.Next(ngrams.Count);
                    //int r = rand.nextInt(ngrams.Count);
                    UpdateLangProb(prob, ngrams[r], alpha);

                    if (i % 5 == 0) {
                        if (NormalizeProb(prob) > CONV_THRESHOLD || i >= ITERATION_LIMIT) {
                            break;
                        }
                        if (Verbose) {
                            Console.WriteLine(@"> [{0}]", string.Join(", ", SortProbability(prob).Select(l => l.ToString()).ToArray()));
                        }
                    }
                }

                for (int j = 0; j < _langprob.Length; ++j) {
                    _langprob[j] += prob[j] / N_TRIAL;
                }

                if (Verbose) {
                    Console.WriteLine(@"==> [{0}]", string.Join(", ", SortProbability(prob).Select(l => l.ToString()).ToArray()));
                }
            }
        }

        /**
         * Initialize the map of language probabilities.
         * If there is the specified prior map, use it as initial map.
         * @return initialized map of language probabilities
         */

        private double[] InitProbability() {
            double[] prob = new double[_langList.Count];
            if (_priorMap != null) {
                for (int i = 0; i < prob.Length; ++i) {
                    prob[i] = _priorMap[i];
                }
            }
            else {
                for (int i = 0; i < prob.Length; ++i) {
                    prob[i] = 1.0 / _langList.Count;
                }
            }
            return prob;
        }

        /**
         * Extract n-grams from target text
         * @return n-grams list
         */

        private List<string> ExtractNGrams() {
            List<string> list = new List<string>();
            NGram ngram = new NGram();
            for (int i = 0; i < _text.Length; ++i) {
                ngram.AddChar(_text[i]);

                for (int n = 1; n <= NGram.N_GRAM; ++n) {
                    string w = ngram[n];
                    if (w != null && _wordLangProbMap.ContainsKey(w)) {
                        list.Add(w);
                    }
                }
            }
            return list;
        }

        /**
         * update language probabilities with N-gram string(N=1,2,3)
         * @param word N-gram string
         * @param prob probabiliy array
         * @param alpha the smoothing parameter
         */

        private bool UpdateLangProb(double[] prob, string word, double alpha) {
            if (word == null || !_wordLangProbMap.ContainsKey(word)) {
                return false;
            }

            double[] langProbMap = _wordLangProbMap[word];
            if (Verbose) {
                Console.WriteLine(@"{0}({1}):{2}", word, UnicodeEncode(word), WordProbTostring(langProbMap));
            }

            double weight = alpha / BASE_FREQ;
            for (int i = 0; i < prob.Length; ++i) {
                prob[i] *= weight + langProbMap[i];
            }
            return true;
        }

        private string WordProbTostring(double[] prob) {
            StringBuilder formatter = new StringBuilder();
            for (int j = 0; j < prob.Length; ++j) {
                double p = prob[j];
                if (p >= 0.00001) {
                    formatter.Append(string.Format(" {0}:{1:F5}", _langList[j], p));
                }
            }
            return formatter.ToString();
        }

        /**
         * normalize probabilities and check convergence by the maximun probability
         * @return maximum of probabilities
         */

        private static double NormalizeProb(double[] prob) {
            double maxp = 0, sump = 0;

            for (int i = 0; i < prob.Length; ++i) {
                sump += prob[i];
            }

            for (int i = 0; i < prob.Length; ++i) {
                double p = prob[i] / sump;
                if (maxp < p) {
                    maxp = p;
                }
                prob[i] = p;
            }
            return maxp;
        }

        /**
         * @param probabilities Dictionary
         * @return lanugage candidates order by probabilities descendently
         */

        private List<Language> SortProbability(double[] prob) {
            List<Language> langs = new List<Language>();
            for (int j = 0; j < prob.Length; ++j) {
                double probability = prob[j];

                if (probability > PROBABILITY_THRESHOLD) {
                    int langCount = langs.Count;
                    for (int i = 0; i <= langCount; ++i) {
                        if (i == langCount || langs[i].Probability < probability) {
                            langs.Add(new Language(_langList[j], probability));
                            break;
                        }
                    }
                }
            }
            return langs;
        }

        /**
         * unicode encoding (for verbose mode)
         * @param word
         * @return
         */

        private static string UnicodeEncode(string word) {
            StringBuilder buf = new StringBuilder();
            foreach (char ch in word) {
                if (ch >= '\u0080') {
                    //to HEX string
                    string st = (0x10000 + (int) ch).ToString("X4");
                    while (st.Length < 4) {
                        st = "0" + st;
                    }

                    buf.Append("\\u")
                       .Append(st.Substring(1, 4)); //.Append(st.subSequence(1, 5));
                }
                else {
                    buf.Append(ch);
                }
            }
            return buf.ToString();
        }
    }

}