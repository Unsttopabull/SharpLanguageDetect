///////////////////////////////////////////////////////////////////
//   Ported to C# by Martin Kraner <martinkraner@outlook.com>    //
//   from https://code.google.com/p/language-detection/          //
///////////////////////////////////////////////////////////////////

using System.Text;

namespace Frost.SharpLanguageDetect.Util {

    /**
    * {@link TagExtractor} is a class which extracts inner texts of specified tag.
    * Users don't use this class directly.
    * @author Nakatani Shuyo
    */
    public class TagExtractor {

        private readonly string _target;
        private readonly int _threshold;
        private StringBuilder _buf;
        private string _tag;
        private int _count;

        public TagExtractor(string tag, int threshold) {
            _target = tag;
            _threshold = threshold;
            _count = 0;
            Clear();
        }

        public int Count() {
            return _count;
        }

        public void Clear() {
            _buf = new StringBuilder();
            _tag = null;
        }

        public void SetTag(string tag) {
            _tag = tag;
        }

        public void Add(string line) {
            if (_tag == _target && line != null) {
                _buf.Append(line);
            }
        }

        public void CloseTag(LangProfile profile) {
            if (profile != null && _tag == _target && _buf.Length > _threshold) {
                NGram gram = new NGram();
                for (int i = 0; i < _buf.Length; ++i) {
                    gram.AddChar(_buf[i]);
                    for (int n = 1; n <= NGram.N_GRAM; ++n) {
                        profile.Add(gram[n]);
                    }
                }
                ++_count;
            }
            Clear();
        }

    }

}