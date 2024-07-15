using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.IO;
using System.Windows;

namespace TextSeeker.Themes
{
    internal class WebViewX : WebView2
    {
        public WebViewX()
        {
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await EnsureCoreWebView2Async(null);
            CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
        }

        private void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            ExecuteScriptAsync("document.dir = 'auto';");
        }

        

        public void SetStringContent(string content)
        {
           
            string htmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TextSeekerTempView.html");
            File.WriteAllText(htmlFilePath, content);

            this.Dispatcher.Invoke(() =>
            {
                this.Source = new Uri("about:blank");
                this.Source = new Uri(htmlFilePath);
            });
        }
    }
}
