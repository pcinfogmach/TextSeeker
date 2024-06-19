using Docnet.Core.Models;
using Docnet.Core;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using org.apache.tika;
using com.sun.tools.doclets.formats.html;
using System;

namespace TextSeeker.Helpers
{
    internal class TextExtractor
    {
        public static string ReadText(string filePath)
        {
            string content = string.Empty;
            try
            {
                filePath = filePath.ToLower();
                if (filePath.EndsWith(".pdf"))
                {
                    content = DocNetPdfTextExtractor(filePath);
                }
                //else if (filePath.EndsWith(".txt"))
                //{
                //    return File.ReadAllText(filePath);
                //}
                else
                {
                    content = Toxy.ParserFactory.CreateText(new Toxy.ParserContext(filePath)).Parse();
                }
            }
            catch (System.NotSupportedException)
            {
                var textExtractor = new TikaOnDotNet.TextExtraction.TextExtractor();
                var result = textExtractor.Extract(filePath);
                content = result.Text;
            }
            catch (Exception ex)
            {
                content = string.Empty; 
            }
            return content.Trim();
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
