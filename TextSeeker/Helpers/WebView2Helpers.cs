using Microsoft.Web.WebView2.Wpf;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TextSeeker.Helpers
{
    public static class WebView2Helpers
    {
        public static void NavigateTostring(WebView2 webView2, string input, string searchTerm, bool isRegexPattern)
        {
            if(!isRegexPattern) { Regex.Escape(input); }
            input = CreateHtmlPage(input, searchTerm);

            string htmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSeeker", "TextSeekerTextPreview.html");
            File.WriteAllText(htmlFilePath, input);
            
            webView2.Dispatcher.Invoke(() =>
            {
                webView2.Source = new Uri("about:blank");
                webView2.Source = new Uri(htmlFilePath);
            });
        }

        static string CreateHtmlPage(string input, string searchTerm)
        {
            return $@"<!DOCTYPE html>
<html lang=""he"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>RTL Justified Text Example</title>
    <style>
        .container {{
            margin: 0 auto; /* Center the content */
            text-align: justify; /* Justify the text */
        }}
    </style>
</head>
<body dir=""auto"">
    <div class=""container"">
      {Highlight(input, searchTerm)}
    </div>
</body>
<script>
 window.find('{searchTerm}')
</script>
</html>
";
        }

        static string Highlight(string input, string searchTerm)
        {
            string escapedTerm = Regex.Escape(searchTerm);
            string wildcardPattern = escapedTerm
                   .Replace("\\*", "[א-ת\"]+")
                   .Replace("\\?", ".");

            string[] searchTerms = searchTerm.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string term in searchTerms)
            {
                input = Regex.Replace(input, wildcardPattern, $"<mark>$&</mark>", RegexOptions.IgnoreCase);
            }
            return input;
        }
    }
}
