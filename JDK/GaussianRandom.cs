using System;

namespace Frost.SharpLanguageDetect.JDK {

    public class GaussianRandom : Random {
        private bool _haveNextGaussian;
        private double _nextNextGaussian;

        public GaussianRandom() {
        }

        public GaussianRandom(int seed) : base(seed) {
        }

        public double NextGaussian() {
            if (_haveNextGaussian) {
                _haveNextGaussian = false;
                return _nextNextGaussian;
            }

            double num1;
            double num2;
            double d;
            do {
                num1 = 2.0 * NextDouble() - 1.0;
                num2 = 2.0 * NextDouble() - 1.0;
                d = num1 * num1 + num2 * num2;
            } while (d >= 1.0 || Math.Abs(d) < 0.001);

            double num3 = Math.Sqrt(-2.0 * Math.Log(d) / d);
            _nextNextGaussian = num2 * num3;
            _haveNextGaussian = true;
            return num1 * num3;
        }
    }

}