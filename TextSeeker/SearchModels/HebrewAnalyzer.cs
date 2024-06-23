using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextSeeker.SearchModels
{
    internal class HebrewAnalyzer : Analyzer
    {
        LuceneVersion version;
        public HebrewAnalyzer(LuceneVersion luceneVersion) 
        {
            version = luceneVersion;
        }
        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            var tokenizer = new WhitespaceTokenizer(version, reader);
            TokenStream filter = new HebrewTokenFilter(tokenizer); // Custom filter with cleaning logic
            return new TokenStreamComponents(tokenizer, filter);
        }

        sealed class HebrewTokenFilter : TokenFilter
        {
            private readonly ICharTermAttribute termAttr;
            private static readonly Regex TrimRegex = new Regex(@" \W+|\W+ ", RegexOptions.Compiled);

            public HebrewTokenFilter(TokenStream input) : base(input)
            {
                this.termAttr = AddAttribute<ICharTermAttribute>();
            }

            public sealed override bool IncrementToken()
            {
                if (m_input.IncrementToken())
                {
                    string token = termAttr.ToString();
                    string cleanedToken = CleanText(token);
                    termAttr.SetEmpty().Append(cleanedToken);
                    return true;
                }
                return false;
            }

            private string CleanText(string input)
            {
                input = Regex.Replace(input, @"\p{M}", ""); // Remove diacritics
                input = TrimRegex.Replace(input, " "); // Trim non-word characters
                return input;
            }
        }
    }
}
