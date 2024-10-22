﻿using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using TextSeeker.Helpers;
using TextSeeker.TextSeekerMVVM.SearchModels;
using TextSeeker.Themes;

namespace TextSeeker.Themes
{
    internal class ResultsWebView : WebView2
    {

        public static readonly DependencyProperty ResultProperty =
                    DependencyProperty.Register("Result", typeof(ResultItem), typeof(ResultsWebView), new PropertyMetadata(new ResultItem(), OnResultChanged));

        public ResultItem Result
        {
            get { return (ResultItem)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        bool isRunning;

        private static void OnResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ResultsWebView;
            control?.LoadResult();
        }

        public async void LoadResult()
        {
            if (Result == null) { return; }
            this.Visibility = Visibility.Hidden;
            Source = new Uri("about:blank");
            Source = new Uri(HtmlConverter.ConvertToHtml(Result.TreeNode.Path));
            if (Result.TreeNode.Name.ToLower().EndsWith(".pdf")) { this.Visibility = Visibility.Visible; return; }
            await EnsureCoreWebView2Async(null);

            CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;                       
        }

        private void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            FindSnippet();
            ExecuteScriptAsync("document.dir = 'auto';");
            this.Visibility  = Visibility.Visible;
            CoreWebView2.DOMContentLoaded -= CoreWebView2_DOMContentLoaded;
        }

        async void FindSnippet()
        {
            string snippet = Regex.Replace(Result.Snippet, @"</?mark>", "");
            string markedText = Regex.Match(Result.Snippet, @"<mark>(.*?)</mark>").Value;
            markedText = Regex.Replace(markedText, @"</?mark>", "");

            var lines = snippet.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string lineContainingMarkedText = lines.FirstOrDefault(line => line.Contains(markedText));
            if (lineContainingMarkedText != null)
            {
                await FindTextAsync(lineContainingMarkedText);
            }
        }

        private async Task FindTextAsync(string searchTerm)
        {
            string script = $@"
            const targetString = `{searchTerm}`;
            const content = document.body.innerHTML;
            const index = content.indexOf(targetString);

            if (index !== -1) {{
               const highlightedText = content.substring(0, index) + 
                                '<span style=""background-color:lightgray"">' + `{Result.Snippet}` + 
                                '</span>' + content.substring(index + targetString.length);

                document.body.innerHTML = highlightedText;
                document.querySelector('mark').scrollIntoView({{ block: ""center"" }});
            }} else {{
                window.find(`{searchTerm}`);
            }}";

            await ExecuteScriptAsync(script);

        }
    }
}

