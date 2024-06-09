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
            input = Regex.Replace(input, $"({searchTerm})", @"<mark>$1</mark>");
            var visiblity = webView2.Visibility;

            webView2.Visibility = System.Windows.Visibility.Collapsed;

            string htmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSeekerTextPreview.html");
            File.WriteAllText(htmlFilePath, input);
            webView2.Dispatcher.Invoke(() =>
            {
                webView2.Source = new Uri("about:blank");
                webView2.Source = new Uri(htmlFilePath);
            });

            webView2.NavigationCompleted += async (s, e) =>
            {
                string script = $@"
                        document.documentElement.style.direction = 'rtl';
                        document.documentElement.style.textAlign = 'justify';
                        document.documentElement.style.margin = '10px';
                        window.find('{searchTerm}')
                    ";
                await webView2.CoreWebView2.ExecuteScriptAsync(script);
                webView2.Visibility = visiblity;
            };
        }
    }
}
