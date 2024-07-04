using com.mchange.v2.c3p0.stmt;
using Microsoft.Web.WebView2.Wpf;
using sun.swing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TextSeeker.SearchModels;

namespace TextSeeker.Helpers
{
    public static class WebView2Helpers
    {
        public static void NavigateTostring(WebView2 webView2, string content)
        {
            content = CreateHtmlPage(content);
            //string htmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSeeker", "TextSeekerTextPreview.html");
            string htmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TextSeekerTextPreview.html");
            File.WriteAllText(htmlFilePath, content);
            
            webView2.Dispatcher.Invoke(() =>
            {
                webView2.Source = new Uri("about:blank");
                webView2.Source = new Uri(htmlFilePath);
            });
        }

        static string CreateHtmlPage(string input)
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
      {input}
    </div>
</body>
</html>
";
        }
    }
}
