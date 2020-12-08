using System;
using Xamarin.Forms;

namespace FingerPaint.Controls
{
    public delegate void WebViewNavigatedHandler(object sender, WebNavigatedEventArgs args);
    public delegate void WebViewNavigatingHandler(object sender, WebNavigatingEventArgs args);
    public delegate void WebViewErrorHandler(object sender, WebViewErorEventArgs args);
    public delegate void WebViewFileHandler(object sender, string args);

    public class WebViewHelper
    {
        public WebViewHelper()
        {
        }
    }

    public class WebViewErorEventArgs
    {
        public WebViewErorMode ErrorMode { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public object BindingContext { get; set; }
    }

    public enum WebViewErorMode
    {
        Timeout,
        NoInternet,
        ErrorCommon,
        SessionTimeout
    }
}
