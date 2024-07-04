using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.VectorHighlight;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextSeeker.Helpers;

namespace TextSeeker.SearchModels
{
    internal class IndexedLuceneSearch
    {
        string indexPath;
        Analyzer analyzer;

        public IndexedLuceneSearch()
        {
            //indexPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSeeker", "TextSeekerIndex");
            indexPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TextSeekerIndex");
            if (!System.IO.Directory.Exists(indexPath)) {  System.IO.Directory.CreateDirectory(indexPath); }
            analyzer = new HebrewAnalyzer(LuceneVersion.LUCENE_48);
        }

        public void IndexFiles(List<string> files)
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            {
                var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
                using (var writer = new IndexWriter(directory, indexConfig))
                {
                    foreach (string file in files)
                    {
                        string content = TextExtractor.ReadText(file);
                        var doc = new Document
                        {
                            new StringField("Path", file, Field.Store.YES),
                            new TextField("Content", content, Field.Store.YES)
                        };
                      
                        var term = new Term("Path", file); // Create a term to search for the existing document by its path

                        writer.UpdateDocument(term, doc);  // Update the document if it exists, otherwise add it
                    }

                    writer.Flush(triggerMerge: true, applyAllDeletes: true);
                }
            }
        }

        public void RemoveFiles(List<string> files)
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            {
                var indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, analyzer);
                using (var writer = new IndexWriter(directory, indexConfig))
                {
                    var parser = new QueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, "Path", analyzer);

                    foreach (string file in files)
                    {
                        Query query = parser.Parse(file);
                        writer.DeleteDocuments(query);
                    }
                    writer.Flush(triggerMerge: false, applyAllDeletes: false);
                }
            }
        }

        public List<TreeNode> Search(string queryText, List<TreeNode> checkedTreeNodes)
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            {
                var searcher = new IndexSearcher(DirectoryReader.Open(directory));
                var parser = new CostumeQueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, "Content", analyzer);

                var query = parser.Parse(queryText);

                var topDocs = searcher.Search(query, int.MaxValue);

                List<TreeNode> results = new List<TreeNode>();
                foreach (var scoreDoc in topDocs.ScoreDocs)
                {
                    var path = searcher.Doc(scoreDoc.Doc).Get("Path");
                    var result = checkedTreeNodes.FirstOrDefault(node => node.Path == path);
                    results.Add(result) ;
                }
                return results;
            }
        }
    }
}
