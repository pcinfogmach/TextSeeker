using com.drew.metadata;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.QueryParsers.Flexible.Standard;
using Lucene.Net.QueryParsers.Simple;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Search.VectorHighlight;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TextSeeker.Helpers;
using TextSeeker.TextSeekerMVVM.Helpers;
using TextSeeker.TextSeekerMVVM.SearchModels;
using TextSeeker.TreeModels;

namespace TextSeeker.SearchModels
{
    internal class IndexedLuceneSearch
    {
        string indexPath;
        Analyzer analyzer;
        private CostumeQueryParser parser;
        Regex idRegex = new Regex(@"\W", RegexOptions.Compiled);

        public IndexedLuceneSearch()
        {
            //indexPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSeeker", "TextSeekerIndex");
            indexPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TextSeekerIndex");
            if (!System.IO.Directory.Exists(indexPath)) { System.IO.Directory.CreateDirectory(indexPath); }
            analyzer = new HebrewAnalyzer(LuceneVersion.LUCENE_48);
            parser = new CostumeQueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_48, "Content", analyzer);
        }

        public void IndexFiles(List<string> files)
        {
            using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            {
                var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
                using (var writer = new IndexWriter(directory, indexConfig))
                {
                    Parallel.ForEach(files, (file) =>
                    {
                        string id = idRegex.Replace(file, "");
                        string content = TextExtractor.ReadText(file).RemoveEmptyLines();
                        var doc = new Document
                        {
                            new StringField("Path", file, Field.Store.YES),
                            new StringField("Id", id, Field.Store.YES),
                            new TextField("Content", content, Field.Store.YES)
                        };
                        writer.UpdateDocument(new Term("Id", id), doc);  // Update the document if it exists, otherwise add it
                    });
                    writer.Flush(triggerMerge: true, applyAllDeletes: true);
                }
            }
        }

        public void RemoveFiles(List<string> files)
        {
            try
            {
                using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
                {
                    var indexConfig = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, analyzer);
                    using (var writer = new IndexWriter(directory, indexConfig))
                    {
                        var parser = new SimpleQueryParser(analyzer, "Id");
                        Parallel.ForEach(files, (file) =>
                        {
                            string id = idRegex.Replace(file, "");
                            Query query = parser.Parse(id);
                            writer.DeleteDocuments(query);
                        });
                        writer.Flush(triggerMerge: false, applyAllDeletes: false);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public List<ResultItem> Search(string queryText, List<TreeNode> checkedTreeNodes)
        {
            List<ResultItem> results = new List<ResultItem>();
            try
            {
                using (var directory = FSDirectory.Open(new DirectoryInfo(indexPath)))
                {
                    var searcher = new IndexSearcher(DirectoryReader.Open(directory));
                    Query query = parser.ParseSpanQuery(queryText, 2);
                    var topDocs = searcher.Search(query, int.MaxValue);
                    if (topDocs.ScoreDocs.Length == 0)
                    {
                        query = new FuzzyQuery(new Term("Content", queryText), 2);
                        topDocs = searcher.Search(query, int.MaxValue);
                    }

                    foreach (var scoreDoc in topDocs.ScoreDocs)
                    {
                        var path = searcher.Doc(scoreDoc.Doc).Get("Path");
                        var result = checkedTreeNodes.FirstOrDefault(node => node.Path == path);
                        if (result != null)
                        {
                            var snippets = LuceneFragmentor.GetFragments(searcher, scoreDoc.Doc, query, analyzer);
                            foreach (var snippet in snippets)
                            {
                                results.Add(new ResultItem
                                {
                                    TreeNode = result,
                                    Snippet = snippet
                                });                          
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return results;
        }


    }
}
