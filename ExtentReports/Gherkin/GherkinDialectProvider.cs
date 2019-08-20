using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AventStack.ExtentReports.Gherkin
{
    internal static class GherkinDialectProvider
    {
        private static Dictionary<string, GherkinKeywords> _dialects;
        private static GherkinKeywords _keywords;
        private static string _dialect;

        static GherkinDialectProvider()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtentReports.Resources.Languages.txt"))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                _dialects = serializer.Deserialize<Dictionary<string, GherkinKeywords>>(jsonReader);
            }
        }

        public static string DefaultDialect { get; } = "en";

        public static GherkinDialect Dialect { get; private set; }

        public static string Language
        {
            get
            {
                if (string.IsNullOrEmpty(_dialect))
                    _dialect = DefaultDialect;
                return _dialect;
            }
            set
            {
                _dialect = value;

                if (!_dialects.ContainsKey(_dialect))
                    throw new InvalidGherkinLanguageException(_dialect + " is not a valid Gherkin dialect");

                _keywords = _dialects[_dialect];
                Dialect = new GherkinDialect(_dialect, _keywords);
            }
        }
    }
}
