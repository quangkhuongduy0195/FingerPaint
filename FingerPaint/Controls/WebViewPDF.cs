using System;
using Xamarin.Forms;

namespace FingerPaint.Controls
{
    public class WebViewPDF : WebView
    {
        /// <summary>
        /// Gets or sets remote the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }
        public static readonly BindableProperty UriProperty =
            BindableProperty.Create(propertyName: "Uri",
            returnType: typeof(string), declaringType: typeof(WebViewPDF),
            defaultValue: default(string), defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Gets or sets the local URI.
        /// </summary>
        /// <value>The URI.</value>
        public string LocalUri
        {
            get { return (string)GetValue(LocalUriProperty); }
            set { SetValue(LocalUriProperty, value); }
        }
        public static readonly BindableProperty LocalUriProperty =
            BindableProperty.Create(propertyName: "LocalUri",
            returnType: typeof(string), declaringType: typeof(WebViewPDF),
            defaultValue: default(string), defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:kbcfx.Views.Controls.Renderers.WebViewPDF"/> retry status.
        /// </summary>
        /// <value><c>true</c> if retry status; otherwise, <c>false</c>.</value>
        public bool RetryStatus
        {
            get { return (bool)GetValue(RetryStatusProperty); }
            set { SetValue(RetryStatusProperty, value); }
        }
        public static readonly BindableProperty RetryStatusProperty =
            BindableProperty.Create(propertyName: "RetryStatus",
                                    returnType: typeof(bool),
                                    declaringType: typeof(WebViewPDF),
                                    defaultValue: false,
                                    defaultBindingMode: BindingMode.OneWay);

        public event WebViewErrorHandler WebViewError;

        public new event WebViewNavigatedHandler Navigated;
        public new event WebViewNavigatingHandler Navigating;

        public virtual void OnNavigating(WebNavigatingEventArgs args)
        {
            Navigating?.Invoke(this, args);
        }

        public virtual void OnNavigated(WebNavigatedEventArgs args)
        {
            Navigated?.Invoke(this, args);
        }

        public void OnWebViewError(WebViewErorEventArgs args)
        {
            var eventHandler = WebViewError;
            if (eventHandler != null)
            {
                eventHandler(this, args);
            }
        }

        public WebViewPDF()
        {
        }
    }
}
