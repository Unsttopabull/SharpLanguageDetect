using System;

namespace Frost.SharpLanguageDetect.JDK {

    public class CharacterSubset {
        private readonly string _name;

        protected CharacterSubset(string name) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }
            _name = name;
        }

        public override sealed bool Equals(object obj) {
            return this == obj;
        }

        public override sealed int GetHashCode() {
            return _name.GetHashCode();
        }

        public override sealed string ToString() {
            return _name;
        }
    }

}