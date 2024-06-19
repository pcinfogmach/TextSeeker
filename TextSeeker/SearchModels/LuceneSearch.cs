using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Documents;
using System;
using System.IO;
using TextSeeker.Helpers;
using Lucene.Net.Search;
using System.Collections.Generic;
using Lucene.Net.QueryParsers;
using System.Linq;
using com.drew.metadata;
using sun.swing;
using System.Threading.Tasks;

namespace TextSeeker.SearchModels
{
    internal class LuceneSearch
    {
        string indexPath;
        Analyzer analyzer;
        public LuceneSearch()
        {
            indexPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSeeker", "TextSeekerIndex");
            if(!System.IO.Directory.Exists(indexPath)) { System.IO.Directory.CreateDirectory(indexPath); }
            analyzer = new WhitespaceAnalyzer();
        }

        public async void IndexFiles(List<string> files)
        {
            await Task.Run(() =>
            {
                using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
                using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                   
                    foreach (string file in files)
                    {
                        string content = TextExtractor.ReadText(file);
                        var doc = new Document();
                        doc.Add(new Field("Path", file, Field.Store.YES, Field.Index.ANALYZED));
                        //doc.Add(new Field("DateModified", File.GetLastWriteTime(file).ToString(), Field.Store.YES, Field.Index.NO));
                        doc.Add(new Field("Content", content, Field.Store.YES, Field.Index.ANALYZED));
                        writer.AddDocument(doc);
                    }
                    writer.Optimize();
                    writer.Commit();
                }
            });
        }

        public void RemoveFiles(List<string> files) 
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Path", analyzer);
                parser.AllowLeadingWildcard = true;
                foreach (string file in files)
                {
                    Query query = parser.Parse(file);
                    writer.DeleteDocuments(query);
                }
                writer.Commit();
            }
        }

        public List<TreeNode> Search(string queryText, List<TreeNode> checkedTreeNodes)
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            using (var searcher = new IndexSearcher(directory, true))
            {
                var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Content", analyzer);
                var query = parser.Parse(queryText);

                var scoreDocs = searcher.Search(query, int.MaxValue).ScoreDocs;

                List<TreeNode> results = new List<TreeNode>();
                foreach (var scoreDoc in scoreDocs)
                {
                    var path = searcher.Doc(scoreDoc.Doc).Get("Path");
                    results.Add(checkedTreeNodes.FirstOrDefault(node => node.Path ==  path));
                }
                return results;
            }
        }
    }
}
