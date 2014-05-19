///////////////////////////////////////////////////////////////////
//   Ported to C# by Martin Kraner <martinkraner@outlook.com>    //
//   from https://code.google.com/p/language-detection/          //
///////////////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.IO;

namespace Frost.SharpLanguageDetect {
    interface ILanguageDetector {
        bool Verbose {get; set;}
        double Alpha { get; set; }
        int MaxTextLength {get; set;}

        void SetPriorityMap(Dictionary<string, double> priorMap);
        void Append(StreamReader reader);
        void Append(string text);

        string Detect();

        List<Language> GetProbabilities();
    }
}
