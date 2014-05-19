using System;
using System.Collections.Generic;
using System.Globalization;

namespace Frost.SharpLanguageDetect.JDK {

    public sealed class UnicodeBlock : CharacterSubset {
        private static readonly Dictionary<string, UnicodeBlock> Map = new Dictionary<string, UnicodeBlock>();
        private static readonly int[] BlockStarts;
        private static readonly CultureInfo EnUsCulture = CultureInfo.GetCultureInfo(0x0409);
        private static readonly UnicodeBlock[] Blocks;

        //public static readonly UnicodeBlock THAI;
        public static readonly UnicodeBlock BASIC_LATIN;
        public static readonly UnicodeBlock LATIN_1_SUPPLEMENT;
        //public static readonly UnicodeBlock LATIN_EXTENDED_A;
        //public static readonly UnicodeBlock LATIN_EXTENDED_B;
        //public static readonly UnicodeBlock IPA_EXTENSIONS;
        //public static readonly UnicodeBlock SPACING_MODIFIER_LETTERS;
        //public static readonly UnicodeBlock COMBINING_DIACRITICAL_MARKS;
        //public static readonly UnicodeBlock GREEK;
        //public static readonly UnicodeBlock CYRILLIC;
        //public static readonly UnicodeBlock ARMENIAN;
        //public static readonly UnicodeBlock HEBREW;
        public static readonly UnicodeBlock ARABIC;
        //public static readonly UnicodeBlock DEVANAGARI;
        //public static readonly UnicodeBlock BENGALI;
        //public static readonly UnicodeBlock GURMUKHI;
        //public static readonly UnicodeBlock GUJARATI;
        //public static readonly UnicodeBlock ORIYA;
        //public static readonly UnicodeBlock TAMIL;
        //public static readonly UnicodeBlock TELUGU;
        //public static readonly UnicodeBlock KANNADA;
        //public static readonly UnicodeBlock MALAYALAM;
        //public static readonly UnicodeBlock LAO;
        //public static readonly UnicodeBlock TIBETAN;
        //public static readonly UnicodeBlock GEORGIAN;
        //public static readonly UnicodeBlock HANGUL_JAMO;
        public static readonly UnicodeBlock LATIN_EXTENDED_ADDITIONAL;
        //public static readonly UnicodeBlock GREEK_EXTENDED;
        public static readonly UnicodeBlock GENERAL_PUNCTUATION;
        //public static readonly UnicodeBlock SUPERSCRIPTS_AND_SUBSCRIPTS;
        //public static readonly UnicodeBlock CURRENCY_SYMBOLS;
        //public static readonly UnicodeBlock COMBINING_MARKS_FOR_SYMBOLS;
        //public static readonly UnicodeBlock LETTERLIKE_SYMBOLS;
        //public static readonly UnicodeBlock NUMBER_FORMS;
        //public static readonly UnicodeBlock ARROWS;
        //public static readonly UnicodeBlock MATHEMATICAL_OPERATORS;
        //public static readonly UnicodeBlock MISCELLANEOUS_TECHNICAL;
        //public static readonly UnicodeBlock CONTROL_PICTURES;
        //public static readonly UnicodeBlock OPTICAL_CHARACTER_RECOGNITION;
        //public static readonly UnicodeBlock ENCLOSED_ALPHANUMERICS;
        //public static readonly UnicodeBlock BOX_DRAWING;
        //public static readonly UnicodeBlock BLOCK_ELEMENTS;
        //public static readonly UnicodeBlock GEOMETRIC_SHAPES;
        //public static readonly UnicodeBlock MISCELLANEOUS_SYMBOLS;
        //public static readonly UnicodeBlock DINGBATS;
        //public static readonly UnicodeBlock CJK_SYMBOLS_AND_PUNCTUATION;
        public static readonly UnicodeBlock HIRAGANA;
        public static readonly UnicodeBlock KATAKANA;
        public static readonly UnicodeBlock BOPOMOFO;
        //public static readonly UnicodeBlock HANGUL_COMPATIBILITY_JAMO;
        //public static readonly UnicodeBlock KANBUN;
        //public static readonly UnicodeBlock ENCLOSED_CJK_LETTERS_AND_MONTHS;
        //public static readonly UnicodeBlock CJK_COMPATIBILITY;
        public static readonly UnicodeBlock CJK_UNIFIED_IDEOGRAPHS;
        public static readonly UnicodeBlock HANGUL_SYLLABLES;
        //public static readonly UnicodeBlock PRIVATE_USE_AREA;
        //public static readonly UnicodeBlock CJK_COMPATIBILITY_IDEOGRAPHS;
        //public static readonly UnicodeBlock ALPHABETIC_PRESENTATION_FORMS;
        //public static readonly UnicodeBlock ARABIC_PRESENTATION_FORMS_A;
        //public static readonly UnicodeBlock COMBINING_HALF_MARKS;
        //public static readonly UnicodeBlock CJK_COMPATIBILITY_FORMS;
        //public static readonly UnicodeBlock SMALL_FORM_VARIANTS;
        //public static readonly UnicodeBlock ARABIC_PRESENTATION_FORMS_B;
        //public static readonly UnicodeBlock HALFWIDTH_AND_FULLWIDTH_FORMS;
        //public static readonly UnicodeBlock SPECIALS;

        //[Obsolete]
        //public static readonly UnicodeBlock SURROGATES_AREA;

        //public static readonly UnicodeBlock SYRIAC;
        //public static readonly UnicodeBlock THAANA;
        //public static readonly UnicodeBlock SINHALA;
        //public static readonly UnicodeBlock MYANMAR;
        //public static readonly UnicodeBlock ETHIOPIC;
        //public static readonly UnicodeBlock CHEROKEE;
        //public static readonly UnicodeBlock UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS;
        //public static readonly UnicodeBlock OGHAM;
        //public static readonly UnicodeBlock RUNIC;
        //public static readonly UnicodeBlock KHMER;
        //public static readonly UnicodeBlock MONGOLIAN;
        //public static readonly UnicodeBlock BRAILLE_PATTERNS;
        //public static readonly UnicodeBlock CJK_RADICALS_SUPPLEMENT;
        //public static readonly UnicodeBlock KANGXI_RADICALS;
        //public static readonly UnicodeBlock IDEOGRAPHIC_DESCRIPTION_CHARACTERS;
        public static readonly UnicodeBlock BOPOMOFO_EXTENDED;
        //public static readonly UnicodeBlock CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A;
        //public static readonly UnicodeBlock YI_SYLLABLES;
        //public static readonly UnicodeBlock YI_RADICALS;
        //public static readonly UnicodeBlock CYRILLIC_SUPPLEMENTARY;
        //public static readonly UnicodeBlock TAGALOG;
        //public static readonly UnicodeBlock HANUNOO;
        //public static readonly UnicodeBlock BUHID;
        //public static readonly UnicodeBlock TAGBANWA;
        //public static readonly UnicodeBlock LIMBU;
        //public static readonly UnicodeBlock TAI_LE;
        //public static readonly UnicodeBlock KHMER_SYMBOLS;
        //public static readonly UnicodeBlock PHONETIC_EXTENSIONS;
        //public static readonly UnicodeBlock MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A;
        //public static readonly UnicodeBlock SUPPLEMENTAL_ARROWS_A;
        //public static readonly UnicodeBlock SUPPLEMENTAL_ARROWS_B;
        //public static readonly UnicodeBlock MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B;
        //public static readonly UnicodeBlock SUPPLEMENTAL_MATHEMATICAL_OPERATORS;
        //public static readonly UnicodeBlock MISCELLANEOUS_SYMBOLS_AND_ARROWS;
        //public static readonly UnicodeBlock KATAKANA_PHONETIC_EXTENSIONS;
        //public static readonly UnicodeBlock YIJING_HEXAGRAM_SYMBOLS;
        //public static readonly UnicodeBlock VARIATION_SELECTORS;
        //public static readonly UnicodeBlock LINEAR_B_SYLLABARY;
        //public static readonly UnicodeBlock LINEAR_B_IDEOGRAMS;
        //public static readonly UnicodeBlock AEGEAN_NUMBERS;
        //public static readonly UnicodeBlock OLD_ITALIC;
        //public static readonly UnicodeBlock GOTHIC;
        //public static readonly UnicodeBlock UGARITIC;
        //public static readonly UnicodeBlock DESERET;
        //public static readonly UnicodeBlock SHAVIAN;
        //public static readonly UnicodeBlock OSMANYA;
        //public static readonly UnicodeBlock CYPRIOT_SYLLABARY;
        //public static readonly UnicodeBlock BYZANTINE_MUSICAL_SYMBOLS;
        //public static readonly UnicodeBlock MUSICAL_SYMBOLS;
        //public static readonly UnicodeBlock TAI_XUAN_JING_SYMBOLS;
        //public static readonly UnicodeBlock MATHEMATICAL_ALPHANUMERIC_SYMBOLS;
        //public static readonly UnicodeBlock CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B;
        //public static readonly UnicodeBlock CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT;
        //public static readonly UnicodeBlock TAGS;
        //public static readonly UnicodeBlock VARIATION_SELECTORS_SUPPLEMENT;
        //public static readonly UnicodeBlock SUPPLEMENTARY_PRIVATE_USE_AREA_A;
        //public static readonly UnicodeBlock SUPPLEMENTARY_PRIVATE_USE_AREA_B;
        //public static readonly UnicodeBlock HIGH_SURROGATES;
        //public static readonly UnicodeBlock HIGH_PRIVATE_USE_SURROGATES;
        //public static readonly UnicodeBlock LOW_SURROGATES;

        static UnicodeBlock() {
            #region Unicode Blocks

            BASIC_LATIN = new UnicodeBlock("BASIC_LATIN", new[] {
                "Basic Latin",
                "BasicLatin"
            });

            LATIN_1_SUPPLEMENT = new UnicodeBlock("LATIN_1_SUPPLEMENT", new[] {
                "Latin-1 Supplement",
                "Latin-1Supplement"
            });

            //LATIN_EXTENDED_A = new UnicodeBlock("LATIN_EXTENDED_A", new[] {
            //    "Latin Extended-A",
            //    "LatinExtended-A"
            //});

            //LATIN_EXTENDED_B = new UnicodeBlock("LATIN_EXTENDED_B", new[] {
            //    "Latin Extended-B",
            //    "LatinExtended-B"
            //});

            //IPA_EXTENSIONS = new UnicodeBlock("IPA_EXTENSIONS", new[] {
            //    "IPA Extensions",
            //    "IPAExtensions"
            //});

            //SPACING_MODIFIER_LETTERS = new UnicodeBlock("SPACING_MODIFIER_LETTERS", new[] {
            //    "Spacing Modifier Letters",
            //    "SpacingModifierLetters"
            //});

            //COMBINING_DIACRITICAL_MARKS = new UnicodeBlock("COMBINING_DIACRITICAL_MARKS", new[] {
            //    "Combining Diacritical Marks",
            //    "CombiningDiacriticalMarks"
            //});

            //GREEK = new UnicodeBlock("GREEK", new[] {
            //    "Greek and Coptic",
            //    "GreekandCoptic"
            //});

            //CYRILLIC = new UnicodeBlock("CYRILLIC");
            //ARMENIAN = new UnicodeBlock("ARMENIAN");
            //HEBREW = new UnicodeBlock("HEBREW");
            //ARABIC = new UnicodeBlock("ARABIC");
            //DEVANAGARI = new UnicodeBlock("DEVANAGARI");
            //BENGALI = new UnicodeBlock("BENGALI");
            //GURMUKHI = new UnicodeBlock("GURMUKHI");
            //GUJARATI = new UnicodeBlock("GUJARATI");
            //ORIYA = new UnicodeBlock("ORIYA");
            //TAMIL = new UnicodeBlock("TAMIL");
            //TELUGU = new UnicodeBlock("TELUGU");
            //KANNADA = new UnicodeBlock("KANNADA");
            //MALAYALAM = new UnicodeBlock("MALAYALAM");
            //THAI = new UnicodeBlock("THAI");
            //LAO = new UnicodeBlock("LAO");
            //TIBETAN = new UnicodeBlock("TIBETAN");
            //GEORGIAN = new UnicodeBlock("GEORGIAN");

            //HANGUL_JAMO = new UnicodeBlock("HANGUL_JAMO", new[] {
            //    "Hangul Jamo",
            //    "HangulJamo"
            //});

            LATIN_EXTENDED_ADDITIONAL = new UnicodeBlock("LATIN_EXTENDED_ADDITIONAL", new[] {
                "Latin Extended Additional",
                "LatinExtendedAdditional"
            });

            //GREEK_EXTENDED = new UnicodeBlock("GREEK_EXTENDED", new[] {
            //    "Greek Extended",
            //    "GreekExtended"
            //});

            GENERAL_PUNCTUATION = new UnicodeBlock("GENERAL_PUNCTUATION", new[] {
                "General Punctuation",
                "GeneralPunctuation"
            });

            //SUPERSCRIPTS_AND_SUBSCRIPTS = new UnicodeBlock("SUPERSCRIPTS_AND_SUBSCRIPTS", new[] {
            //    "Superscripts and Subscripts",
            //    "SuperscriptsandSubscripts"
            //});

            //CURRENCY_SYMBOLS = new UnicodeBlock("CURRENCY_SYMBOLS", new[] {
            //    "Currency Symbols",
            //    "CurrencySymbols"
            //});

            //COMBINING_MARKS_FOR_SYMBOLS = new UnicodeBlock("COMBINING_MARKS_FOR_SYMBOLS", new[] {
            //    "Combining Diacritical Marks for Symbols",
            //    "CombiningDiacriticalMarksforSymbols",
            //    "Combining Marks for Symbols",
            //    "CombiningMarksforSymbols"
            //});

            //LETTERLIKE_SYMBOLS = new UnicodeBlock("LETTERLIKE_SYMBOLS", new[] {
            //    "Letterlike Symbols",
            //    "LetterlikeSymbols"
            //});

            //NUMBER_FORMS = new UnicodeBlock("NUMBER_FORMS", new[] {
            //    "Number Forms",
            //    "NumberForms"
            //});

            //ARROWS = new UnicodeBlock("ARROWS");

            //MATHEMATICAL_OPERATORS = new UnicodeBlock("MATHEMATICAL_OPERATORS", new[] {
            //    "Mathematical Operators",
            //    "MathematicalOperators"
            //});

            //MISCELLANEOUS_TECHNICAL = new UnicodeBlock("MISCELLANEOUS_TECHNICAL", new[] {
            //    "Miscellaneous Technical",
            //    "MiscellaneousTechnical"
            //});

            //CONTROL_PICTURES = new UnicodeBlock("CONTROL_PICTURES", new[] {
            //    "Control Pictures",
            //    "ControlPictures"
            //});

            //OPTICAL_CHARACTER_RECOGNITION = new UnicodeBlock("OPTICAL_CHARACTER_RECOGNITION", new[] {
            //    "Optical Character Recognition",
            //    "OpticalCharacterRecognition"
            //});

            //ENCLOSED_ALPHANUMERICS = new UnicodeBlock("ENCLOSED_ALPHANUMERICS", new[] {
            //    "Enclosed Alphanumerics",
            //    "EnclosedAlphanumerics"
            //});

            //BOX_DRAWING = new UnicodeBlock("BOX_DRAWING", new[] {
            //    "Box Drawing",
            //    "BoxDrawing"
            //});

            //BLOCK_ELEMENTS = new UnicodeBlock("BLOCK_ELEMENTS", new[] {
            //    "Block Elements",
            //    "BlockElements"
            //});

            //GEOMETRIC_SHAPES = new UnicodeBlock("GEOMETRIC_SHAPES", new[] {
            //    "Geometric Shapes",
            //    "GeometricShapes"
            //});

            //MISCELLANEOUS_SYMBOLS = new UnicodeBlock("MISCELLANEOUS_SYMBOLS", new[] {
            //    "Miscellaneous Symbols",
            //    "MiscellaneousSymbols"
            //});

            //DINGBATS = new UnicodeBlock("DINGBATS");

            //CJK_SYMBOLS_AND_PUNCTUATION = new UnicodeBlock("CJK_SYMBOLS_AND_PUNCTUATION", new[] {
            //    "CJK Symbols and Punctuation",
            //    "CJKSymbolsandPunctuation"
            //});

            HIRAGANA = new UnicodeBlock("HIRAGANA");
            KATAKANA = new UnicodeBlock("KATAKANA");
            BOPOMOFO = new UnicodeBlock("BOPOMOFO");

            //HANGUL_COMPATIBILITY_JAMO = new UnicodeBlock("HANGUL_COMPATIBILITY_JAMO", new[] {
            //    "Hangul Compatibility Jamo",
            //    "HangulCompatibilityJamo"
            //});

            //KANBUN = new UnicodeBlock("KANBUN");

            //ENCLOSED_CJK_LETTERS_AND_MONTHS = new UnicodeBlock("ENCLOSED_CJK_LETTERS_AND_MONTHS", new[] {
            //    "Enclosed CJK Letters and Months",
            //    "EnclosedCJKLettersandMonths"
            //});

            //CJK_COMPATIBILITY = new UnicodeBlock("CJK_COMPATIBILITY", new[] {
            //    "CJK Compatibility",
            //    "CJKCompatibility"
            //});

            CJK_UNIFIED_IDEOGRAPHS = new UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS", new[] {
                "CJK Unified Ideographs",
                "CJKUnifiedIdeographs"
            });

            HANGUL_SYLLABLES = new UnicodeBlock("HANGUL_SYLLABLES", new[] {
                "Hangul Syllables",
                "HangulSyllables"
            });

            //PRIVATE_USE_AREA = new UnicodeBlock("PRIVATE_USE_AREA", new[] {
            //    "Private Use Area",
            //    "PrivateUseArea"
            //});

            //CJK_COMPATIBILITY_IDEOGRAPHS = new UnicodeBlock("CJK_COMPATIBILITY_IDEOGRAPHS", new[] {
            //    "CJK Compatibility Ideographs",
            //    "CJKCompatibilityIdeographs"
            //});

            //ALPHABETIC_PRESENTATION_FORMS = new UnicodeBlock("ALPHABETIC_PRESENTATION_FORMS", new[] {
            //    "Alphabetic Presentation Forms",
            //    "AlphabeticPresentationForms"
            //});

            //ARABIC_PRESENTATION_FORMS_A = new UnicodeBlock("ARABIC_PRESENTATION_FORMS_A", new[] {
            //    "Arabic Presentation Forms-A",
            //    "ArabicPresentationForms-A"
            //});

            //COMBINING_HALF_MARKS = new UnicodeBlock("COMBINING_HALF_MARKS", new[] {
            //    "Combining Half Marks",
            //    "CombiningHalfMarks"
            //});

            //CJK_COMPATIBILITY_FORMS = new UnicodeBlock("CJK_COMPATIBILITY_FORMS", new[] {
            //    "CJK Compatibility Forms",
            //    "CJKCompatibilityForms"
            //});

            //SMALL_FORM_VARIANTS = new UnicodeBlock("SMALL_FORM_VARIANTS", new[] {
            //    "Small Form Variants",
            //    "SmallFormVariants"
            //});

            //ARABIC_PRESENTATION_FORMS_B = new UnicodeBlock("ARABIC_PRESENTATION_FORMS_B", new[] {
            //    "Arabic Presentation Forms-B",
            //    "ArabicPresentationForms-B"
            //});

            //HALFWIDTH_AND_FULLWIDTH_FORMS = new UnicodeBlock("HALFWIDTH_AND_FULLWIDTH_FORMS", new[] {
            //    "Halfwidth and Fullwidth Forms",
            //    "HalfwidthandFullwidthForms"
            //});

            //SPECIALS = new UnicodeBlock("SPECIALS");
            //SURROGATES_AREA = new UnicodeBlock("SURROGATES_AREA");
            //SYRIAC = new UnicodeBlock("SYRIAC");
            //THAANA = new UnicodeBlock("THAANA");
            //SINHALA = new UnicodeBlock("SINHALA");
            //MYANMAR = new UnicodeBlock("MYANMAR");
            //ETHIOPIC = new UnicodeBlock("ETHIOPIC");
            //CHEROKEE = new UnicodeBlock("CHEROKEE");

            //UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS = new UnicodeBlock("UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS", new[] {
            //    "Unified Canadian Aboriginal Syllabics",
            //    "UnifiedCanadianAboriginalSyllabics"
            //});

            //OGHAM = new UnicodeBlock("OGHAM");
            //RUNIC = new UnicodeBlock("RUNIC");
            //KHMER = new UnicodeBlock("KHMER");
            //MONGOLIAN = new UnicodeBlock("MONGOLIAN");

            //BRAILLE_PATTERNS = new UnicodeBlock("BRAILLE_PATTERNS", new[] {
            //    "Braille Patterns",
            //    "BraillePatterns"
            //});

            //CJK_RADICALS_SUPPLEMENT = new UnicodeBlock("CJK_RADICALS_SUPPLEMENT", new[] {
            //    "CJK Radicals Supplement",
            //    "CJKRadicalsSupplement"
            //});

            //KANGXI_RADICALS = new UnicodeBlock("KANGXI_RADICALS", new[] {
            //    "Kangxi Radicals",
            //    "KangxiRadicals"
            //});

            //IDEOGRAPHIC_DESCRIPTION_CHARACTERS = new UnicodeBlock("IDEOGRAPHIC_DESCRIPTION_CHARACTERS", new[] {
            //    "Ideographic Description Characters",
            //    "IdeographicDescriptionCharacters"
            //});

            BOPOMOFO_EXTENDED = new UnicodeBlock("BOPOMOFO_EXTENDED", new[] {
                "Bopomofo Extended",
                "BopomofoExtended"
            });

            //CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A = new UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A", new[] {
            //    "CJK Unified Ideographs Extension A",
            //    "CJKUnifiedIdeographsExtensionA"
            //});

            //YI_SYLLABLES = new UnicodeBlock("YI_SYLLABLES", new[] {
            //    "Yi Syllables",
            //    "YiSyllables"
            //});

            //YI_RADICALS = new UnicodeBlock("YI_RADICALS", new[] {
            //    "Yi Radicals",
            //    "YiRadicals"
            //});

            //CYRILLIC_SUPPLEMENTARY = new UnicodeBlock("CYRILLIC_SUPPLEMENTARY", new[] {
            //    "Cyrillic Supplementary",
            //    "CyrillicSupplementary"
            //});

            //TAGALOG = new UnicodeBlock("TAGALOG");
            //HANUNOO = new UnicodeBlock("HANUNOO");
            //BUHID = new UnicodeBlock("BUHID");
            //TAGBANWA = new UnicodeBlock("TAGBANWA");
            //LIMBU = new UnicodeBlock("LIMBU");

            //TAI_LE = new UnicodeBlock("TAI_LE", new[] {
            //    "Tai Le",
            //    "TaiLe"
            //});

            //KHMER_SYMBOLS = new UnicodeBlock("KHMER_SYMBOLS", new[] {
            //    "Khmer Symbols",
            //    "KhmerSymbols"
            //});

            //PHONETIC_EXTENSIONS = new UnicodeBlock("PHONETIC_EXTENSIONS", new[] {
            //    "Phonetic Extensions",
            //    "PhoneticExtensions"
            //});

            //MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A = new UnicodeBlock("MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A", new[] {
            //    "Miscellaneous Mathematical Symbols-A",
            //    "MiscellaneousMathematicalSymbols-A"
            //});

            //SUPPLEMENTAL_ARROWS_A = new UnicodeBlock("SUPPLEMENTAL_ARROWS_A", new[] {
            //    "Supplemental Arrows-A",
            //    "SupplementalArrows-A"
            //});

            //SUPPLEMENTAL_ARROWS_B = new UnicodeBlock("SUPPLEMENTAL_ARROWS_B", new[] {
            //    "Supplemental Arrows-B",
            //    "SupplementalArrows-B"
            //});

            //MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B = new UnicodeBlock("MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B", new[] {
            //    "Miscellaneous Mathematical Symbols-B",
            //    "MiscellaneousMathematicalSymbols-B"
            //});

            //SUPPLEMENTAL_MATHEMATICAL_OPERATORS = new UnicodeBlock("SUPPLEMENTAL_MATHEMATICAL_OPERATORS", new[] {
            //    "Supplemental Mathematical Operators",
            //    "SupplementalMathematicalOperators"
            //});

            //MISCELLANEOUS_SYMBOLS_AND_ARROWS = new UnicodeBlock("MISCELLANEOUS_SYMBOLS_AND_ARROWS", new[] {
            //    "Miscellaneous Symbols and Arrows",
            //    "MiscellaneousSymbolsandArrows"
            //});

            //KATAKANA_PHONETIC_EXTENSIONS = new UnicodeBlock("KATAKANA_PHONETIC_EXTENSIONS", new[] {
            //    "Katakana Phonetic Extensions",
            //    "KatakanaPhoneticExtensions"
            //});

            //YIJING_HEXAGRAM_SYMBOLS = new UnicodeBlock("YIJING_HEXAGRAM_SYMBOLS", new[] {
            //    "Yijing Hexagram Symbols",
            //    "YijingHexagramSymbols"
            //});

            //VARIATION_SELECTORS = new UnicodeBlock("VARIATION_SELECTORS", new[] {
            //    "Variation Selectors",
            //    "VariationSelectors"
            //});

            //LINEAR_B_SYLLABARY = new UnicodeBlock("LINEAR_B_SYLLABARY", new[] {
            //    "Linear B Syllabary",
            //    "LinearBSyllabary"
            //});

            //LINEAR_B_IDEOGRAMS = new UnicodeBlock("LINEAR_B_IDEOGRAMS", new[] {
            //    "Linear B Ideograms",
            //    "LinearBIdeograms"
            //});

            //AEGEAN_NUMBERS = new UnicodeBlock("AEGEAN_NUMBERS", new[] {
            //    "Aegean Numbers",
            //    "AegeanNumbers"
            //});

            //OLD_ITALIC = new UnicodeBlock("OLD_ITALIC", new[] {
            //    "Old Italic",
            //    "OldItalic"
            //});

            //GOTHIC = new UnicodeBlock("GOTHIC");
            //UGARITIC = new UnicodeBlock("UGARITIC");
            //DESERET = new UnicodeBlock("DESERET");
            //SHAVIAN = new UnicodeBlock("SHAVIAN");
            //OSMANYA = new UnicodeBlock("OSMANYA");

            //CYPRIOT_SYLLABARY = new UnicodeBlock("CYPRIOT_SYLLABARY", new[] {
            //    "Cypriot Syllabary",
            //    "CypriotSyllabary"
            //});

            //BYZANTINE_MUSICAL_SYMBOLS = new UnicodeBlock("BYZANTINE_MUSICAL_SYMBOLS", new[] {
            //    "Byzantine Musical Symbols",
            //    "ByzantineMusicalSymbols"
            //});

            //MUSICAL_SYMBOLS = new UnicodeBlock("MUSICAL_SYMBOLS", new[] {
            //    "Musical Symbols",
            //    "MusicalSymbols"
            //});

            //TAI_XUAN_JING_SYMBOLS = new UnicodeBlock("TAI_XUAN_JING_SYMBOLS", new[] {
            //    "Tai Xuan Jing Symbols",
            //    "TaiXuanJingSymbols"
            //});

            //MATHEMATICAL_ALPHANUMERIC_SYMBOLS = new UnicodeBlock("MATHEMATICAL_ALPHANUMERIC_SYMBOLS", new[] {
            //    "Mathematical Alphanumeric Symbols",
            //    "MathematicalAlphanumericSymbols"
            //});

            //CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B = new UnicodeBlock("CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B", new[] {
            //    "CJK Unified Ideographs Extension B",
            //    "CJKUnifiedIdeographsExtensionB"
            //});

            //CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT = new UnicodeBlock("CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT", new[] {
            //    "CJK Compatibility Ideographs Supplement",
            //    "CJKCompatibilityIdeographsSupplement"
            //});

            //TAGS = new UnicodeBlock("TAGS");
            //VARIATION_SELECTORS_SUPPLEMENT = new UnicodeBlock("VARIATION_SELECTORS_SUPPLEMENT", new[] {
            //    "Variation Selectors Supplement",
            //    "VariationSelectorsSupplement"
            //});

            //SUPPLEMENTARY_PRIVATE_USE_AREA_A = new UnicodeBlock("SUPPLEMENTARY_PRIVATE_USE_AREA_A", new[] {
            //    "Supplementary Private Use Area-A",
            //    "SupplementaryPrivateUseArea-A"
            //});

            //SUPPLEMENTARY_PRIVATE_USE_AREA_B = new UnicodeBlock("SUPPLEMENTARY_PRIVATE_USE_AREA_B", new[] {
            //    "Supplementary Private Use Area-B",
            //    "SupplementaryPrivateUseArea-B"
            //});

            //HIGH_SURROGATES = new UnicodeBlock("HIGH_SURROGATES", new[] {
            //    "High Surrogates",
            //    "HighSurrogates"
            //});

            //HIGH_PRIVATE_USE_SURROGATES = new UnicodeBlock("HIGH_PRIVATE_USE_SURROGATES", new[] {
            //    "High Private Use Surrogates",
            //    "HighPrivateUseSurrogates"
            //});

            //LOW_SURROGATES = new UnicodeBlock("LOW_SURROGATES", new[] {
            //    "Low Surrogates",
            //    "LowSurrogates"
            //});

            #endregion

            #region Block Starts

            BlockStarts = new[] {
                0,
                128,
                256,
                384,
                592,
                688,
                768,
                880,
                1024,
                1280,
                1328,
                1424,
                1536,
                1792,
                1872,
                1920,
                1984,
                2304,
                2432,
                2560,
                2688,
                2816,
                2944,
                3072,
                3200,
                3328,
                3456,
                3584,
                3712,
                3840,
                4096,
                4256,
                4352,
                4608,
                4992,
                5024,
                5120,
                5760,
                5792,
                5888,
                5920,
                5952,
                5984,
                6016,
                6144,
                6320,
                6400,
                6480,
                6528,
                6624,
                6656,
                7424,
                7552,
                7680,
                7936,
                8192,
                8304,
                8352,
                8400,
                8448,
                8528,
                8592,
                8704,
                8960,
                9216,
                9280,
                9312,
                9472,
                9600,
                9632,
                9728,
                9984,
                10176,
                10224,
                10240,
                10496,
                10624,
                10752,
                11008,
                11264,
                11904,
                12032,
                12256,
                12272,
                12288,
                12352,
                12448,
                12544,
                12592,
                12688,
                12704,
                12736,
                12784,
                12800,
                13056,
                13312,
                19904,
                19968,
                40960,
                42128,
                42192,
                44032,
                55216,
                55296,
                56192,
                56320,
                57344,
                63744,
                64256,
                64336,
                65024,
                65040,
                65056,
                65072,
                65104,
                65136,
                65280,
                65520,
                65536,
                65664,
                65792,
                65856,
                66304,
                66352,
                66384,
                66432,
                66464,
                66560,
                66640,
                66688,
                66736,
                67584,
                67648,
                118784,
                119040,
                119296,
                119552,
                119648,
                119808,
                120832,
                131072,
                173792,
                194560,
                195104,
                917504,
                917632,
                917760,
                918000,
                983040,
                1048576
            };

            #endregion

            #region Blocks

            Blocks = new[] {
                BASIC_LATIN,
                LATIN_1_SUPPLEMENT,
                null, //LATIN_EXTENDED_A,
                null, //LATIN_EXTENDED_B,
                null, //IPA_EXTENSIONS,
                null, //SPACING_MODIFIER_LETTERS,
                null, //COMBINING_DIACRITICAL_MARKS,
                null, //GREEK,
                null, //CYRILLIC,
                null, //CYRILLIC_SUPPLEMENTARY,
                null, //ARMENIAN,
                null, //HEBREW,
                ARABIC,
                null, //SYRIAC,
                null,
                null, //THAANA,
                null,
                null, //DEVANAGARI,
                null, //BENGALI,
                null, //GURMUKHI,
                null, //GUJARATI,
                null, //ORIYA,
                null, //TAMIL,
                null, //TELUGU,
                null, //KANNADA,
                null, //MALAYALAM,
                null, //SINHALA,
                null, //THAI,
                null, //LAO,
                null, //TIBETAN,
                null, //MYANMAR,
                null, //GEORGIAN,
                null, //HANGUL_JAMO,
                null, //ETHIOPIC,
                null,
                null, //CHEROKEE,
                null, //UNIFIED_CANADIAN_ABORIGINAL_SYLLABICS,
                null, //OGHAM,
                null, //RUNIC,
                null, //TAGALOG,
                null, //HANUNOO,
                null, //BUHID,
                null, //TAGBANWA,
                null, //KHMER,
                null, //MONGOLIAN,
                null,
                null, //LIMBU,
                null, //TAI_LE,
                null,
                null, //KHMER_SYMBOLS,
                null,
                null, //PHONETIC_EXTENSIONS,
                null,
                LATIN_EXTENDED_ADDITIONAL,
                null, //GREEK_EXTENDED,
                GENERAL_PUNCTUATION,
                null, //SUPERSCRIPTS_AND_SUBSCRIPTS,
                null, //CURRENCY_SYMBOLS,
                null, //COMBINING_MARKS_FOR_SYMBOLS,
                null, //LETTERLIKE_SYMBOLS,
                null, //NUMBER_FORMS,
                null, //ARROWS,
                null, //MATHEMATICAL_OPERATORS,
                null, //MISCELLANEOUS_TECHNICAL,
                null, //CONTROL_PICTURES,
                null, //OPTICAL_CHARACTER_RECOGNITION,
                null, //ENCLOSED_ALPHANUMERICS,
                null, //BOX_DRAWING,
                null, //BLOCK_ELEMENTS,
                null, //GEOMETRIC_SHAPES,
                null, //MISCELLANEOUS_SYMBOLS,
                null, //DINGBATS,
                null, //MISCELLANEOUS_MATHEMATICAL_SYMBOLS_A,
                null, //SUPPLEMENTAL_ARROWS_A,
                null, //BRAILLE_PATTERNS,
                null, //SUPPLEMENTAL_ARROWS_B,
                null, //MISCELLANEOUS_MATHEMATICAL_SYMBOLS_B,
                null, //SUPPLEMENTAL_MATHEMATICAL_OPERATORS,
                null, //MISCELLANEOUS_SYMBOLS_AND_ARROWS,
                null,
                null, //CJK_RADICALS_SUPPLEMENT,
                null, //KANGXI_RADICALS,
                null,
                null, //IDEOGRAPHIC_DESCRIPTION_CHARACTERS,
                null, //CJK_SYMBOLS_AND_PUNCTUATION,
                HIRAGANA,
                KATAKANA,
                BOPOMOFO,
                null, //HANGUL_COMPATIBILITY_JAMO,
                null, //KANBUN,
                BOPOMOFO_EXTENDED,
                null,
                null, //KATAKANA_PHONETIC_EXTENSIONS,
                null, //ENCLOSED_CJK_LETTERS_AND_MONTHS,
                null, //CJK_COMPATIBILITY,
                null, //CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A,
                null, //YIJING_HEXAGRAM_SYMBOLS,
                CJK_UNIFIED_IDEOGRAPHS,
                null, //YI_SYLLABLES,
                null, //YI_RADICALS,
                null,
                HANGUL_SYLLABLES,
                null,
                null, //HIGH_SURROGATES,
                null, //HIGH_PRIVATE_USE_SURROGATES,
                null, //LOW_SURROGATES,
                null, //PRIVATE_USE_AREA,
                null, //CJK_COMPATIBILITY_IDEOGRAPHS,
                null, //ALPHABETIC_PRESENTATION_FORMS,
                null, //ARABIC_PRESENTATION_FORMS_A,
                null, //VARIATION_SELECTORS,
                null,
                null, //COMBINING_HALF_MARKS,
                null, //CJK_COMPATIBILITY_FORMS,
                null, //SMALL_FORM_VARIANTS,
                null, //ARABIC_PRESENTATION_FORMS_B,
                null, //HALFWIDTH_AND_FULLWIDTH_FORMS,
                null, //SPECIALS,
                null, //LINEAR_B_SYLLABARY,
                null, //LINEAR_B_IDEOGRAMS,
                null, //AEGEAN_NUMBERS,
                null,
                null, //OLD_ITALIC,
                null, //GOTHIC,
                null,
                null, //UGARITIC,
                null,
                null, //DESERET,
                null, //SHAVIAN,
                null, //OSMANYA,
                null,
                null, //CYPRIOT_SYLLABARY,
                null,
                null, //BYZANTINE_MUSICAL_SYMBOLS,
                null, //MUSICAL_SYMBOLS,
                null,
                null, //TAI_XUAN_JING_SYMBOLS,
                null,
                null, //MATHEMATICAL_ALPHANUMERIC_SYMBOLS,
                null,
                null, //CJK_UNIFIED_IDEOGRAPHS_EXTENSION_B,
                null,
                null, //CJK_COMPATIBILITY_IDEOGRAPHS_SUPPLEMENT,
                null,
                null, //TAGS,
                null,
                null, //VARIATION_SELECTORS_SUPPLEMENT,
                null,
                null, //SUPPLEMENTARY_PRIVATE_USE_AREA_A,
                null, //SUPPLEMENTARY_PRIVATE_USE_AREA_B,
            };

            #endregion
        }

        #region Constructors

        private UnicodeBlock(string name) : base(name) {
            Map.Add(name.ToUpper(EnUsCulture), this);
        }

        private UnicodeBlock(string name, IEnumerable<string> obj1) : this(name) {
            if (obj1 == null) {
                return;
            }

            foreach (string block in obj1) {
                Map.Add(block.ToUpper(EnUsCulture), this);
            }
        }

        private UnicodeBlock(string name, string obj1) : this(name) {
            Map.Add(obj1.ToUpper(EnUsCulture), this);
        }

        #endregion

        public static UnicodeBlock Of(char c) {
            return Of((int) c);
        }

        public static UnicodeBlock Of(int codePoint) {
            if (codePoint < 0 || codePoint > 1114111) {
                throw new ArgumentException(@"Invalid code point.", "codePoint");
            }

            int num1 = 0;
            int num2 = BlockStarts.Length;
            int index = num2 / 2;
            while (num2 - num1 > 1) {
                if (codePoint >= BlockStarts[index]) {
                    num1 = index;
                }
                else {
                    num2 = index;
                }
                index = (num2 + num1) / 2;
            }
            return Blocks[index];
        }

        public static UnicodeBlock ForName(string blockName) {
            string upper = blockName.ToUpper(EnUsCulture);
            if (Map.ContainsKey(upper)) {
                return Map[upper];
            }
            throw new ArgumentException();
        }
    }

}