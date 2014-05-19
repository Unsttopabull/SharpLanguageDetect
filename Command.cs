using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Frost.SharpLanguageDetect.Util;
using Newtonsoft.Json;

namespace Frost.SharpLanguageDetect {

    /**
     * 
     * LangDetect Command Line Interface
     * <p>
     * This is a command line interface of Language Detection Library "LandDetect".
     * </p>
     * 
     * @author Nakatani Shuyo
     *
     */
    public class Command {

        /** smoothing default parameter (ELE) */
        private const double DEFAULT_ALPHA = 0.5;

        /** for Command line easy parser */
        private readonly Dictionary<string, string> _optWithValue = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();
        private readonly HashSet<string> _optWithoutValue = new HashSet<string>();
        private readonly List<string> _arglist = new List<string>();
        private bool _profilesLoaded;
        private static readonly string Filler;

        static Command() {
              Filler = string.Join("", Enumerable.Repeat("_", Console.BufferWidth));            
        }

        /**
         * Command line easy parser
         * @param args command line arguments
         */
        private void Parse(string[] args) {
            for (int i = 0; i < args.Length; ++i) {
                if (_optWithValue.ContainsKey(args[i])) {
                    string key = _optWithValue[args[i]];
                    _values[key] = args[i + 1];
                    ++i;
                }
                else if (args[i].StartsWith("-")) {
                    _optWithoutValue.Add(args[i]);
                }
                else {
                    _arglist.Add(args[i]);
                }
            }
        }

        private void AddOpt(string opt, string key, string value) {
            _optWithValue.Add(opt, key);
            _values.Add(key, value);
        }

        private string Get(string key) {
            return _values[key];
        }

        private long? Getlong(string key) {
            if(_values.ContainsKey(key)){
                string value = _values[key];
                if (value == null){
                    return null;
                }
                try {
                    return long.Parse(value);
                }
                catch (FormatException) {
                    return null;
                }
            }
            return null;
        }

        private double GetDouble(string key, double defaultValue) {
            try {
                if (_values.ContainsKey(key)) {
                    return Double.Parse(_values[key]);
                }
                return defaultValue;
            }
            catch (FormatException) {
                return defaultValue;
            }
        }

        private bool HasOpt(string opt) {
            return _optWithoutValue.Contains(opt);
        }

        /**
         * File search (easy glob)
         * @param directory directory path
         * @param pattern   searching file pattern with regular representation
         * @return matched file
         */
        private FileInfo SearchFile(DirectoryInfo directory, string pattern) {
            foreach(FileInfo file in directory.EnumerateFiles()) {
                string fileName = file.Name;
                if (Regex.IsMatch(fileName, pattern)) {
                    return file;
                }
            }
            return null;
        }

        /**
         * load profiles
         * @return false if load success
         */
        public bool LoadProfiles(string directory = null) {
            string profileDirectory = directory ?? Get("directory") + "/";
            try {
                DetectorFactory.LoadProfilesFromFolder(profileDirectory);
                long? seed = Getlong("seed");
                if (seed != null) {
                    DetectorFactory.SetSeed(seed.Value);
                }
                _profilesLoaded = true;
                return false;
            }
            catch (LangDetectException e) {
                Console.Error.WriteLine("ERROR: " + e.Message);
                _profilesLoaded = false;
                return true;
            }
        }

        /**
         * Generate Language Profile from Wikipedia Abstract Database File
         * 
         * <pre>
         * usage: --genprofile -d [abstracts directory] [language names]
         * </pre>
         * 
         */
        private void GenerateProfile() {
            DirectoryInfo directory = new DirectoryInfo(Get("directory"));
            foreach (string lang in _arglist) {
                FileInfo file = SearchFile(directory, lang + "wiki-.*-abstract\\.xml.*");
                if (file == null) {
                    Console.Error.WriteLine("Not Found abstract xml : lang = " + lang);
                    continue;
                }

                StreamWriter os = null;
                try {
                    LangProfile profile = LangProfile.LoadFromWikipediaAbstract(lang, file);
                    profile.OmitLessFrequent();

                    FileInfo profilePath = new FileInfo(Get("directory") + "/profiles/" + lang);

                    os = new StreamWriter(profilePath.OpenWrite());
                    using (JsonWriter jsw = new JsonTextWriter(os)) {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Error += (sender, args) => {
                            // only log an error once
                            if (args.CurrentObject == args.ErrorContext.OriginalObject) {
                                WriteExceptionOnConsole(args.ErrorContext.Error);
                            }
                        };

                        serializer.Serialize(jsw, profile);
                    }
                }
                catch (IOException e) {
                    WriteExceptionOnConsole(e);
                }
                catch (LangDetectException e) {
                    WriteExceptionOnConsole(e);
                }
                finally {
                    if (os != null) {
                        os.Close();
                    }
                }
            }
        }

        /**
         * Generate Language Profile from Text File
         * 
         * <pre>
         * usage: --genprofile-text -l [language code] [text file path]
         * </pre>
         * 
         */
        private void GenerateProfileFromText() {
            if (_arglist.Count != 1) {
                Console.Error.WriteLine("Need to specify text file path");
                return;
            }
            FileInfo file = new FileInfo(_arglist[0]);
            if (!file.Exists) {
                Console.Error.WriteLine("Need to specify existing text file path");
                return;
            }

            string lang = Get("lang");
            if (lang == null) {
                Console.Error.WriteLine("Need to specify langage code(-l)");
                return;
            }

            StreamWriter os = null;
            try {
                LangProfile profile = LangProfile.LoadFromText(lang, file);
                profile.OmitLessFrequent();

                FileInfo profilePath = new FileInfo(lang);

                os = new StreamWriter(profilePath.OpenWrite());
                using (JsonWriter jsw = new JsonTextWriter(os)) {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Error += (sender, args) => {
                        // only log an error once
                        if (args.CurrentObject == args.ErrorContext.OriginalObject) {
                            Console.Error.WriteLine(args.ErrorContext.Error.StackTrace);
                        }
                    };

                    serializer.Serialize(jsw, profile);
                }
            }
            catch (IOException e) {
                WriteExceptionOnConsole(e);
            }
            catch (LangDetectException e) {
                WriteExceptionOnConsole(e);
            }
            finally {
                if (os != null) {
                    os.Close();
                }
            }
        }

        /**
         * Language detection test for each file (--detectlang option)
         * 
         * <pre>
         * usage: --detectlang -d [profile directory] -a [alpha] -s [seed] [test file(s)]
         * </pre>
         * 
         */
        private void DetectLang(Encoding enc = null) {
            if (!_profilesLoaded && LoadProfiles()) {
                return;
            }

            foreach (string filename in _arglist) {
                DetectOneFile(filename, enc);
            }
        }

        private void DetectOneFile(string filename, Encoding enc) {
            StreamReader sr  = null;
            try {
                sr = (enc != null)
                    ? new StreamReader(filename, enc)
                    : new StreamReader(filename);

                Detector detector = DetectorFactory.Create(GetDouble("alpha", DEFAULT_ALPHA));
                if (HasOpt("--debug")) {
                    detector.Verbose = true;
                }
                detector.Append(sr);

                Console.WriteLine(filename + ":");
                foreach (Language language in detector.GetProbabilities()) {
                    Console.WriteLine("\t" + language);
                }     
            }
            catch (IOException e) {
                WriteExceptionOnConsole(e);
            }
            catch (LangDetectException e) {
                WriteExceptionOnConsole(e);
            }
            finally {
                if (sr != null) {
                    sr.Close();
                }
            }
        }

        /**
         * Batch Test of Language Detection (--batchtest option)
         * 
         * <pre>
         * usage: --batchtest -d [profile directory] -a [alpha] -s [seed] [test data(s)]
         * </pre>
         * 
         * The format of test data(s):
         * <pre>
         *   [correct language name]\t[text body for test]\n
         * </pre>
         *  
         */
        private void BatchTest() {
            if (LoadProfiles()) {
                return;
            }

            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (string filename in _arglist) {
                StreamReader sr = null;
                try {
                    sr = new StreamReader(filename, Encoding.UTF8);
                    while (!sr.EndOfStream){
                        string line = sr.ReadLine();
                        int idx = line.IndexOf('\t');
                        if (idx <= 0) {
                            continue;
                        }
                        string correctLang = line.Substring(0, idx);
                        string text = line.Substring(idx + 1);

                        Detector detector = DetectorFactory.Create(GetDouble("alpha", DEFAULT_ALPHA));
                        detector.Append(text);
                        string lang = "";

                        try {
                            lang = detector.Detect();
                        }
                        catch (Exception e) {
                            WriteExceptionOnConsole(e);
                        }

                        if (!result.ContainsKey(correctLang)) {
                            result.Add(correctLang, new List<string>());
                        }
                        result[correctLang].Add(lang);

                        if (HasOpt("--debug")) {
                            Console.WriteLine(correctLang + "," + lang + "," + (text.Length > 100 ? text.Substring(0, 100) : text));
                        }
                    }

                }
                catch (IOException e) {
                    WriteExceptionOnConsole(e);
                }
                catch (LangDetectException e) {
                    WriteExceptionOnConsole(e);
                }
                finally {
                    if (sr != null){
                        sr.Close();
                    }
                }

                List<string> langlist = new List<string>(result.Keys);
                langlist.Sort();

                int totalCount = 0, totalCorrect = 0;
                foreach (string lang in langlist) {
                    Dictionary<string, int> resultCount = new Dictionary<string, int>();
                    int count = 0;
                    List<string> list = result[lang];
                    foreach (string detectedLang in list) {
                        ++count;
                        if (resultCount.ContainsKey(detectedLang)) {
                            resultCount.Add(detectedLang, resultCount[detectedLang] + 1);
                            //resultCount[detectedLang]++;
                        }
                        else {
                            resultCount.Add(detectedLang, 1);
                        }
                    }
                    int correct = resultCount.ContainsKey(lang) ? resultCount[lang] : 0;
                    double rate = correct / (double) count;

                    Console.WriteLine("{0} ({1:D}/{2:D}={3:F2}): {4}", lang, correct, count, rate, resultCount);
                    totalCorrect += correct;
                    totalCount += count;
                }
                Console.WriteLine("total: {0:D}/{1:D} = {2:F3}", totalCorrect, totalCount, totalCorrect / (double) totalCount);
            }

        }

        private static void WriteExceptionOnConsole(Exception e) {
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine(Filler);
            Console.Error.WriteLine(e.StackTrace);            
        }

        /**
         * Command Line Interface
         * @param args command line arguments
         */
        public static void Main(string[] args) {
            Command command = new Command();
            command.AddOpt("-d", "directory", "./");
            command.AddOpt("-a", "alpha", "" + DEFAULT_ALPHA);
            command.AddOpt("-s", "seed", null);
            command.AddOpt("-l", "lang", null);
            command.Parse(args);

            if (command.HasOpt("--genprofile")) {
                command.GenerateProfile();
            }
            else if (command.HasOpt("--genprofile-text")) {
                command.GenerateProfileFromText();
            }
            else if (command.HasOpt("--detectlang")) {
                command.DetectLang();
            }
            else if (command.HasOpt("--batchtest")) {
                command.BatchTest();
            }

            Console.WriteLine("----------------END---------------");
            //Console.Read();
        }
    }

}