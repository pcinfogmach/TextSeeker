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
                    content = PdfTextExtractor(filePath);
                }
                else if (filePath.Contains("ToratEmetInstall")||filePath.Contains("ToratEmetUserData"))
                {
                    return File.ReadAllText(filePath, Encoding.GetEncoding("Windows-1255"));
                }
                else
                {
                    content = Toxy.ParserFactory.CreateText(new Toxy.ParserContext(filePath)).Parse();
                }
            }
            catch 
            {
                try
                {
                    var result = new TikaOnDotNet.TextExtraction.TextExtractor().Extract(filePath);
                    content = result.Text;
                }
                catch { }
                
            }
            return content.Trim();
        }

        static string PdfTextExtractor(string filePath)
        {
            try
            {
                var helper = new XpdfNet.XpdfHelper();
                return helper.ToText(filePath);
            }
            catch
            {
                return DocNetPdfTextExtractor(filePath);
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
