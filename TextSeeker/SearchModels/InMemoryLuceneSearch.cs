using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using org.bouncycastle.asn1.cmp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TextSeeker.Helpers;

namespace TextSeeker.SearchModels
{
    internal class InMemoryLuceneSearch
    {
        private RAMDirectory ramDirectory;
        private Analyzer analyzer;
        private QueryParser queryParser;

        public InMemoryLuceneSearch()
        {
            ramDirectory = new RAMDirectory();
            analyzer = new HebrewAnalyzer(LuceneVersion.LUCENE_48);
            queryParser = new CostumeQueryParser(LuceneVersion.LUCENE_48, "Content", analyzer);
        }

        public Tuple<bool, float> Execute(string file, string queryText)
        {
            IndexFile(file);
            return Search(queryText);
        }

        void IndexFile(string file)
        {
            var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            using (var writer = new IndexWriter(ramDirectory, indexConfig))
            {
                string content = TextExtractor.ReadText(file);
                var doc = new Document
                    {
                        new TextField("Content", content, Field.Store.YES)
                    };

                writer.AddDocument(doc);
                writer.Flush(triggerMerge: true, applyAllDeletes: true);
            }
        }

        Tuple<bool, float> Search(string queryText)
        {
            var searcher = new IndexSearcher(DirectoryReader.Open(ramDirectory));
            var query = queryParser.Parse(queryText);

            var result = searcher.Search(query, 1).ScoreDocs;
            if (result.Length > 0)
            {
                var score = result[0].Score; 
                return Tuple.Create(true, score);
            }
            return Tuple.Create(false, 0f); 
        }

    }
}
