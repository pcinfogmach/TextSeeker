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

namespace TextSeeker.SearchModels
{
    internal class LuceneSearch
    {
        string indexPath;
        Analyzer analyzer;
        public LuceneSearch()
        {
            indexPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSeekerTreeNodes.json");
            if(!System.IO.Directory.Exists(indexPath)) { System.IO.Directory.CreateDirectory(indexPath); }
            analyzer = new WhitespaceAnalyzer();
        }

        void AddFolder(string[] files)
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (string file in files)
                {
                    string content = TextExtractor.ReadText(file);
                    var doc = new Document();
                    doc.Add(new Field("Path", file, Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("Content", content, Field.Store.YES, Field.Index.ANALYZED));
                    writer.AddDocument(doc);
                }
                writer.Optimize();
            }
        }

        public List<TreeNode> Search(string queryText, List<TreeNode> checkedTreeNodes)
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            using (var searcher = new IndexSearcher(directory, true))
            {
                var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "content", analyzer);
                var query = parser.Parse(queryText);

                var scoreDocs = searcher.Search(query, int.MaxValue).ScoreDocs;
                var filePathOrder = scoreDocs
                    .Select((scoreDoc, index) => new { Path = searcher.Doc(scoreDoc.Doc).Get("Path"), Order = index })
                    .ToDictionary(x => x.Path, x => x.Order);

                return checkedTreeNodes
                    .Where(treeNode => filePathOrder.ContainsKey(treeNode.Path))
                    .OrderBy(treeNode => filePathOrder[treeNode.Path])
                    .ToList();
            }
        }
    }
}
