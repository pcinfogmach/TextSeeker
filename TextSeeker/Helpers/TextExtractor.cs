using Docnet.Core.Models;
using Docnet.Core;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using org.apache.tika;
using com.sun.tools.doclets.formats.html;

namespace TextSeeker.Helpers
{
    internal class TextExtractor
    {
        public static string ReadText(string filePath)
        {
            try
            {
                filePath = filePath.ToLower();
                if (filePath.EndsWith(".pdf"))
                {
                    return DocNetPdfTextExtractor(filePath);
                }
                //else if (filePath.EndsWith(".txt"))
                //{
                //    return File.ReadAllText(filePath);
                //}
                else
                {
                    return Toxy.ParserFactory.CreateText(new Toxy.ParserContext(filePath)).Parse();
                }
            }
            catch (System.NotSupportedException ex)
            {
                var textExtractor = new TikaOnDotNet.TextExtraction.TextExtractor();
                var result = textExtractor.Extract(filePath);
                return result.Text;
            }
            catch 
            {
                return string.Empty; 
            }
        }

        static string DocNetPdfTextExtractor(string filePath)
        {
            var pageTextDictionary = new ConcurrentDictionary<int, string>();
            using (var docReader = DocLib.Instance.GetDocReader(filePath, new PageDimensions()))
            {
                int pageCount = docReader.GetPageCount();
                Docnet.Core.Readers.IPageReader pageReader = null;
                Parallel.For(0, pageCount, i =>
                {
                    pageReader = docReader.GetPageReader(i);
                    pageTextDictionary[i] = pageReader.GetText();
                });
                pageReader.Dispose();
            }

            StringBuilder stb = new StringBuilder();

            for (int i = 0; i < pageTextDictionary.Count; i++)
            {
                stb.AppendLine(pageTextDictionary[i]);
            }

            return stb.ToString();
        }
    }
}
