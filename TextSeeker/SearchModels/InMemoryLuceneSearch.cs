using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.IO;
using System.Linq;
using TextSeeker.Helpers;
using static Lucene.Net.QueryParsers.Flexible.Core.Nodes.PathQueryNode;

namespace TextSeeker.SearchModels
{
    internal class InMemoryLuceneSearch
    {
        private RAMDirectory ramDirectory;
        private Analyzer analyzer;
        private CostumeQueryParser queryParser;

        public InMemoryLuceneSearch()
        {
            analyzer = new HebrewAnalyzer(LuceneVersion.LUCENE_48);
            queryParser = new CostumeQueryParser(LuceneVersion.LUCENE_48, "Content", analyzer);
        }

        public bool Search(string file, string queryText)
        {
            using (ramDirectory = new RAMDirectory())
            {
                IndexFile(file);

                var searcher = new IndexSearcher(DirectoryReader.Open(ramDirectory));
                var query = queryParser.ParseSpanQuery(queryText, 2);
                var fuzzyQuery = new FuzzyQuery(new Term("Content", queryText), 2); // Set the maxEdits to 1 for minor spelling variations

                var result = searcher.Search(query, 10).ScoreDocs;

                if (result.Length == 0)
                {
                    result = searcher.Search(fuzzyQuery, 10).ScoreDocs;
                }

                return result.Length > 0;
            }
        }

        public string GetFormattedSnippets(string filePath, string searchTerm)
        {
            string content = TextExtractor.ReadText(filePath);
            var snippets = new InMemoryLuceneSearch().GetSnippets(filePath, searchTerm);
            return "<li>..." + string.Join("...<p><li>...", snippets) + "...";
        }

        string[] GetSnippets(string file, string queryText)
        {
            using (ramDirectory = new RAMDirectory())
            {
                IndexFile(file);
                return GetResultSnippets(queryText, 2);
            }
        }

        void IndexFile(string file)
        {
            var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            using (var writer = new IndexWriter(ramDirectory, indexConfig))
            {
                string content = TextExtractor.ReadText(file);
                var doc = new Document
                {
                    new TextField("Content", content, Field.Store.YES),
                    new StringField("Path", file, Field.Store.YES)
                };

                writer.AddDocument(doc);
                writer.Flush(triggerMerge: true, applyAllDeletes: true);
            }
        }

        string[] GetResultSnippets(string queryText, int distanceBetweenWord)
        {
            var searcher = new IndexSearcher(DirectoryReader.Open(ramDirectory));
            var query = queryParser.ParseSpanQuery(queryText, distanceBetweenWord);
            var fuzzyQuery = new FuzzyQuery(new Term("Content", queryText), 2); // Set the maxEdits to 1 for minor spelling variations

            var result = searcher.Search(query, 10).ScoreDocs;
            bool isFuzzy = false;

            if (result.Length == 0)
            {
                result = searcher.Search(fuzzyQuery, 10).ScoreDocs;
                isFuzzy = true;
            }

            if (result.Length > 0)
            {
                if (isFuzzy) { return GetFragments(searcher, result[0].Doc, fuzzyQuery); }
                return GetFragments(searcher, result[0].Doc, query);
            }
            return Array.Empty<string>();
        }

        string[] GetFragments(IndexSearcher searcher, int docId, Query query)
        {
            var reader = searcher.IndexReader;
            var scorer = new QueryScorer(query);
            var fragmenter = new SimpleSpanFragmenter(scorer, 300); // 300 is the fragment size 

            var formatter = new SimpleHTMLFormatter("<mark>", "</mark>"); //define how to mark found keywords if left empty <b> tags is the default
            var highlighter = new Highlighter(formatter, scorer);
            highlighter.TextFragmenter = fragmenter;

            var content = searcher.Doc(docId).Get("Content");
            var tokenStream = TokenSources.GetAnyTokenStream(reader, docId, "Content", analyzer);
            var fragments = highlighter.GetBestTextFragments(tokenStream, content, false, 30); // 10 is the number of snippets

            return fragments.Where(fragment => fragment.Score > 0).Select(fragment => fragment.ToString()).ToList().ToArray();
        }
    }
}
