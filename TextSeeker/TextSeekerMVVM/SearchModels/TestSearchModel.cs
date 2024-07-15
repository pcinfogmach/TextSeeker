using Docnet.Core.Models;
using Docnet.Core;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextSeeker.Helpers;
using System.Threading;
using TextSeeker.TreeModels;

namespace TextSeeker.SearchModels
{
    public static class TestSearchModel
    {
        public enum SearchType { ContainsSearch, LooseSearch, RegexSearch, TamperedRegexSearch}

        public static bool Search(TreeNode file, string searchTerm, SearchType searchType, CancellationToken cancellationToken)
        {
            if ((file.Path.ToLower().EndsWith(".pdf") && PdfTextSearch(file.Path, searchTerm, searchType, cancellationToken)) ||
                IsSearchMatch(TextExtractor.ReadText(file.Path), searchTerm, searchType))
            {
                file.DateLastModified = File.GetLastWriteTime(file.Path);
                return true;
            }
            return false;
        }

        static bool PdfTextSearch(string filePath, string searchTerm, SearchType searchType, CancellationToken cancellationToken)
        {
            using (var docReader = DocLib.Instance.GetDocReader(filePath, new PageDimensions()))
            {
                int pageCount = docReader.GetPageCount();
                bool matchFound = false;

                Parallel.For(0, pageCount, (i, loopState) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        loopState.Stop();
                    }

                    var pageReader = docReader.GetPageReader(i);
                    if (IsSearchMatch(pageReader.GetText(), searchTerm, searchType))
                    {
                        matchFound = true;
                        loopState.Stop();
                    }
                    pageReader.Dispose();
                });

                return matchFound;
            }
        }

        static bool IsSearchMatch(string content, string searchTerm, SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.ContainsSearch:
                    return content.StringContains(searchTerm.Split(' '), searchTerm.Length);
                case SearchType.LooseSearch:
                    // Implement loose search logic
                    break;
                case SearchType.RegexSearch:
                    return Regex.IsMatch(content, searchTerm);
                case SearchType.TamperedRegexSearch:
                    // Implement tampered regex search logic
                    break;
                default:
                    return false;
            }
            return false;
        }
    }
}
