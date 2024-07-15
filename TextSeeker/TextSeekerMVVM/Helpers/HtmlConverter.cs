using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jdk.nashorn.@internal.ir;
using Microsoft.Office.Interop.Word;
using org.openxmlformats.schemas.drawingml.x2006.main;

namespace TextSeeker.Helpers
{
    public static class HtmlConverter
    {
        public static string ConvertToHtml(string filePath)
        {
            string[] docxExtensions = {".doc", ".docm", ".docx", ".dotx", ".dotm", ".dot", ".odt", ".rtf"};
            string extension = Path.GetExtension(filePath);
            if (docxExtensions.Contains(extension)) { filePath = MsWordConverter(filePath); }
            return filePath;          
        }

        static string MsWordConverter(string filePath) 
        {
            string tempHtmlPath = Path.Combine(Path.GetTempPath(), filePath + "textSeekerPreview.html");
            if (File.Exists(tempHtmlPath)) { return tempHtmlPath; }
            Application wordApp = new Application();
            Document wordDoc = null;

            try
            {
                wordDoc = wordApp.Documents.Open(filePath);
                wordDoc.SaveAs2(tempHtmlPath, WdSaveFormat.wdFormatFilteredHTML);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occurred: " + ex.Message);
                string content = TextExtractor.ReadText(filePath);
                if (!string.IsNullOrEmpty(content)) { File.WriteAllText(tempHtmlPath, content); }
                else { return filePath; }
            }
            finally
            {
                if (wordDoc != null)
                {
                    wordDoc.Close(false);
                }
                wordApp.Quit(false);
            }
            return tempHtmlPath;
        }
    }
}
