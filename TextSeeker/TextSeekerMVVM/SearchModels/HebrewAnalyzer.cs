﻿using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
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
            var tokenizer = new StandardTokenizer(version, reader);
            TokenStream filter = new HebrewTokenFilter(tokenizer);
            filter = new LowerCaseFilter(version, filter);
            filter = new StopFilter(version, filter, StopAnalyzer.ENGLISH_STOP_WORDS_SET);
            return new TokenStreamComponents(tokenizer, filter);
        }

        sealed class HebrewTokenFilter : TokenFilter
        {
            private readonly ICharTermAttribute termAttr;

            public HebrewTokenFilter(TokenStream input) : base(input)
            {
                this.termAttr = AddAttribute<ICharTermAttribute>();
            }

            public sealed override bool IncrementToken()
            {
                if (m_input.IncrementToken())
                {
                    string token = termAttr.ToString();
                    string cleanedToken = Regex.Replace(token, @"\p{M}", "");

                    if (!string.Equals(token, cleanedToken))
                    {
                        termAttr.SetEmpty().Append(cleanedToken);
                    }

                    return true;
                }
                return false;
            }
        }
    }
}
