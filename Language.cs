///////////////////////////////////////////////////////////////////
//   Ported to C# by Martin Kraner <martinkraner@outlook.com>    //
//   from https://code.google.com/p/language-detection/          //
///////////////////////////////////////////////////////////////////

namespace Frost.SharpLanguageDetect {

    /**
     * {@link Language} is to store the detected language.
     * {@link Detector#GetProbabilities()} returns an List&lt;T&gt; of {@link Language}s.
     *  
     * @see Detector#getProbabilities()
     * @author Nakatani Shuyo
     *
     */
    public class Language {

        public string LangCode { get; private set; }
        public double Probability { get; private set; }

        public Language(string langCode, double probability) {
            LangCode = langCode;
            Probability = probability;
        }

        public override string ToString() {
            return LangCode + ":" + Probability;
        }

    }

}