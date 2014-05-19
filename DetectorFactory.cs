///////////////////////////////////////////////////////////////////
//   Ported to C# by Martin Kraner <martinkraner@outlook.com>    //
//   from https://code.google.com/p/language-detection/          //
///////////////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Frost.SharpLanguageDetect.Profiles;
using Frost.SharpLanguageDetect.Util;
using Newtonsoft.Json;

namespace Frost.SharpLanguageDetect {

    /**
     * Language Detector Factory Class
     * 
     * This class manages an initialization and constructions of {@link Detector}. 
     * 
     * Before using language detection library, 
     * load profiles with {@link DetectorFactory#LoadProfilesFromFolder(string)} method
     * and set initialization parameters.
     * 
     * When the language detection,
     * construct Detector instance via {@link DetectorFactory#Create()}.
     * See also {@link Detector}'s sample code.
     * 
     * <ul>
     * <li>4x faster improvement based on Elmer Garduno's code. Thanks!</li>
     * </ul>
     * 
     * @see Detector
     * @author Nakatani Shuyo
     * @author Martin Kraner
     */
    public class DetectorFactory {

        private static readonly DetectorFactory Instance = new DetectorFactory();
        public readonly List<string> Langlist;
        public long? Seed;
        public readonly Dictionary<string, double[]> WordLangProbMap;

        private DetectorFactory() {
            WordLangProbMap = new Dictionary<string, double[]>();
            Langlist = new List<string>();
        }

        public static void LoadStaticProfiles() {
            const int NUM_PROFILES = 54;

            AddProfile(AfrikaansLangProfile.Instance,  0, NUM_PROFILES);
            AddProfile(AlbanianLangProfile.Instance,   1, NUM_PROFILES);
            AddProfile(ArabicLangProfile.Instance,     2, NUM_PROFILES);
            AddProfile(BengaliLangProfile.Instance,    3, NUM_PROFILES);
            AddProfile(BulgarianLangProfile.Instance,  4, NUM_PROFILES);
            AddProfile(CroatianLangProfile.Instance,   5, NUM_PROFILES);
            AddProfile(CzechLangProfile.Instance,      6, NUM_PROFILES);
            AddProfile(DanishLangProfile.Instance,     7, NUM_PROFILES);
            AddProfile(DutchLangProfile.Instance,      8, NUM_PROFILES);
            AddProfile(EnglishLangProfile.Instance,    9, NUM_PROFILES);
            AddProfile(EstonianLangProfile.Instance,   10, NUM_PROFILES);
            AddProfile(FinnishLangProfile.Instance,    11, NUM_PROFILES);
            AddProfile(FrenchLangProfile.Instance,     12, NUM_PROFILES);
            AddProfile(GermanLangProfile.Instance,     13, NUM_PROFILES);
            AddProfile(GreekLangProfile.Instance,      14, NUM_PROFILES);
            AddProfile(GujaratiLangProfile.Instance,   15, NUM_PROFILES);
            AddProfile(HebrewLangProfile.Instance,     16, NUM_PROFILES);
            AddProfile(HindiLangProfile.Instance,      17, NUM_PROFILES);
            AddProfile(HungarianLangProfile.Instance,  18, NUM_PROFILES);
            AddProfile(IndonesianLangProfile.Instance, 19, NUM_PROFILES);
            AddProfile(ItalianLangProfile.Instance,    20, NUM_PROFILES);
            AddProfile(JapaneseLangProfile.Instance,   21, NUM_PROFILES);
            AddProfile(KannadaLangProfile.Instance,    22, NUM_PROFILES);
            AddProfile(KoreanLangProfile.Instance,     23, NUM_PROFILES);
            AddProfile(LatvianLangProfile.Instance,    24, NUM_PROFILES);
            AddProfile(LithuanianLangProfile.Instance, 25, NUM_PROFILES);
            AddProfile(MacedonianLangProfile.Instance, 26, NUM_PROFILES);
            AddProfile(MalayalamLangProfile.Instance,  27, NUM_PROFILES);
            AddProfile(MarathiLangProfile.Instance,    28, NUM_PROFILES);
            AddProfile(NepaliLangProfile.Instance,     29, NUM_PROFILES);
            AddProfile(NorwegianLangProfile.Instance,  30, NUM_PROFILES);
            AddProfile(PersianLangProfile.Instance,    31, NUM_PROFILES);
            AddProfile(PolishLangProfile.Instance,     32, NUM_PROFILES);
            AddProfile(PortugueseLangProfile.Instance, 33, NUM_PROFILES);
            AddProfile(PunjabiLangProfile.Instance,    34, NUM_PROFILES);
            AddProfile(RomanianLangProfile.Instance,   35, NUM_PROFILES);
            AddProfile(RussianLangProfile.Instance,    36, NUM_PROFILES);
            AddProfile(SerbianLangProfile.Instance,    37, NUM_PROFILES);
            AddProfile(SimplifiedChineseLangProfile.Instance, 38, NUM_PROFILES);
            AddProfile(SlovakLangProfile.Instance,     39, NUM_PROFILES);
            AddProfile(SloveneLangProfile.Instance,    40, NUM_PROFILES);
            AddProfile(SomaliLangProfile.Instance,     41, NUM_PROFILES);
            AddProfile(SpanishLangProfile.Instance,    42, NUM_PROFILES);
            AddProfile(SwahiliLangProfile.Instance,    43, NUM_PROFILES);
            AddProfile(SwedishLangProfile.Instance,    44, NUM_PROFILES);
            AddProfile(TagalogLangProfile.Instance,    45, NUM_PROFILES);
            AddProfile(TamilLangProfile.Instance,      46, NUM_PROFILES);
            AddProfile(TeluguLangProfile.Instance,     47, NUM_PROFILES);
            AddProfile(ThaiLangProfile.Instance,       48, NUM_PROFILES);
            AddProfile(TraditionalChineseLangProfile.Instance, 49, NUM_PROFILES);
            AddProfile(TurkishLangProfile.Instance,    50, NUM_PROFILES);
            AddProfile(UkrainianLangProfile.Instance,  51, NUM_PROFILES);
            AddProfile(UrduLangProfile.Instance,       52, NUM_PROFILES);
            AddProfile(VietnameseLangProfile.Instance, 53, NUM_PROFILES);
        }

        /**
         * Load profiles from specified directory.
         * This method must be called once before language detection.
         *  
         * @param profileDirectory profile directory path
         * @throws LangDetectException  Can't open profiles(error code = {@link ErrorCode#FileLoadError})
         *                              or profile's format is wrong (error code = {@link ErrorCode#FormatError})
         */
        public static void LoadProfilesFromFolder(string profileDirectory) {
            DirectoryInfo dirInfo = new DirectoryInfo(profileDirectory);
            LoadProfilesFromFolder(dirInfo);
        }

        /**
         * Load profiles from specified directory.
         * This method must be called once before language detection.
         *  
         * @param profileDirectory profile directory path
         * @throws LangDetectException  Can't open profiles(error code = {@link ErrorCode#FileLoadError})
         *                              or profile's format is wrong (error code = {@link ErrorCode#FormatError})
         */
        public static void LoadProfilesFromFolder(DirectoryInfo profileDirectory) {
            FileInfo[] listFiles = profileDirectory.GetFiles();
            if (listFiles == null) {
                throw new LangDetectException(ErrorCode.NeedLoadProfileError, "Not found profile: " + profileDirectory);
            }

            int index = 0;
            int langsize = listFiles.Length;
            foreach (FileInfo file in listFiles) {
                if (file.Name.StartsWith(".")) {
                    continue;
                }

                StreamReader sr = null;
                try {
                    //UTF-8
                    sr = file.OpenText();

                    using (JsonReader jsr = new JsonTextReader(sr)) {
                        string fileName = file.Name;

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Error += (sender, args) => {
                            throw new LangDetectException(ErrorCode.FormatError, string.Format("Language profile format error in '{0}'.", fileName));
                        };

                        LangProfile profile = serializer.Deserialize<LangProfile>(jsr);
                        AddProfile(profile, index, langsize);

                        ++index;
                    }
                }
                catch (IOException) {
                    throw new LangDetectException(ErrorCode.FileLoadError, string.Format("Can't open \"{0}\".", file.Name));
                }
                finally {
                    if (sr != null) {
                        sr.Close();
                    }
                }
            }
        }

        public static void LoadProfilesFromFile(string filePath) {
            try {
                JsonSerializer jsr = new JsonSerializer();
                LangProfile[] langProfiles = jsr.Deserialize<LangProfile[]>(new JsonTextReader(File.OpenText("join_min.js")));

                int langSize = langProfiles.Length;
                for (int i = 0; i < langSize; i++) {
                    AddProfile(langProfiles[i], i, langSize);
                }
            }
            catch (IOException) {
                throw new LangDetectException(ErrorCode.FileLoadError, string.Format("Can't open \"{0}\".", filePath));
            }
        }

        /**
         * @param profile The profile to load
         * @param index  The profile index
         * @param langsize The number of profiles
         * @throws LangDetectException Throws if the profile is a duplicate of an already loaded profile
         */
        private static void AddProfile(ILanguageProfile profile, int index, int langsize) {
            string lang = profile.Name;
            if (Instance.Langlist.Contains(lang)) {
                throw new LangDetectException(ErrorCode.DuplicateLangError, string.Format("Duplicate language profile: \"{0}\"", lang));
            }
            Instance.Langlist.Add(lang);
            foreach (string word in profile.Frequency.Keys) {
                if (!Instance.WordLangProbMap.ContainsKey(word)) {
                    Instance.WordLangProbMap.Add(word, new double[langsize]);
                }
                int length = word.Length;
                if (length >= 1 && length <= 3) {
                    double prob = (double) profile.Frequency[word] / profile.NWords[length - 1];
                    Instance.WordLangProbMap[word][index] = prob;
                }
            }
        }

        /**
         * for only Unit Test
         */
        public /*internal*/ static void Clear() {
            Instance.Langlist.Clear();
            Instance.WordLangProbMap.Clear();
        }

        /**
         * Construct Detector instance
         * 
         * @return Detector instance
         * @throws LangDetectException 
         */
        public static Detector Create() {
            return CreateDetector();
        }

        /**
         * Construct Detector instance with smoothing parameter 
         * 
         * @param alpha smoothing parameter (default value = 0.5)
         * @return Detector instance
         * @throws LangDetectException 
         */
        public static Detector Create(double alpha) {
            Detector detector = CreateDetector();
            detector.Alpha = alpha;
            return detector;
        }

        private static Detector CreateDetector() {
            if (Instance.Langlist.Count == 0) {
                throw new LangDetectException(ErrorCode.NeedLoadProfileError, "need to load profiles");
            }
            Detector detector = new Detector(Instance);
            return detector;
        }

        public static void SetSeed(long seed) {
            Instance.Seed = seed;
        }

        public static ReadOnlyCollection<string> GetLangList() {
            return Instance.Langlist.AsReadOnly();
        }

    }

}