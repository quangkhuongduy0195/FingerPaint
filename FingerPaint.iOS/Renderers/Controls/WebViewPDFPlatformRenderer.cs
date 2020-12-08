using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FingerPaint.Controls;
using FingerPaint.DTO;
using FingerPaint.iOS.Renderers.Controls;
using FingerPaint.iOS.Utility;
using FingerPaint.Utilities;
using Foundation;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebViewPDF), typeof(WebViewPDFPlatformRenderer))]
namespace FingerPaint.iOS.Renderers.Controls
{
    public class WebViewPDFPlatformRenderer : ViewRenderer<WebViewPDF, WKWebView>, IWKNavigationDelegate, IWKUIDelegate
    {
        WKWebView _wkWebViewPDF;
        private object _bindingContextCache;
        private bool _isDead = false;

        protected override void OnElementChanged(ElementChangedEventArgs<WebViewPDF> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var config = new WKWebViewConfiguration();
                    config.UserContentController.RemoveAllUserScripts();
                    config.UserContentController.ScalesPageToFit();
                    _wkWebViewPDF = new WKWebView(Frame, config)
                    {
                        NavigationDelegate = this,
                        UIDelegate = this,
                        Opaque = false,
                        BackgroundColor = UIColor.Clear,
                    };

                    SetNativeControl(_wkWebViewPDF);
                }

                //_bindingContextCache = Element.BindingContext;
                //Element.BindingContextChanged += OnBindingContextChanged;
            }
        }

        //private void OnBindingContextChanged(object sender, EventArgs e)
        //{
        //    if (Element.BindingContext != null)
        //        _bindingContextCache = Element.BindingContext;
        //}

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Uri" && (Element as WebViewPDF).Uri != null
               || e.PropertyName == "RetryStatus" && (Element as WebViewPDF).RetryStatus)
            {
                try
                {
                    var customWebView = Element as WebViewPDF;
                    if (customWebView == null)
                        return;

                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    customWebView.OnNavigating(new WebNavigatingEventArgs(WebNavigationEvent.NewPage, null, ""));
                    //});


                    DownloadFile(customWebView.Uri, customWebView);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _isDead = true;
            if (disposing)
            {
                Control?.RemoveFromSuperview();
                StopTimer();
                // Clear cache
                NSUrlCache.SharedCache.RemoveAllCachedResponses();

                _bindingContextCache = null;
                //if (Element != null)
                //    Element.BindingContextChanged -= OnBindingContextChanged;
            }
            if (Directory.Exists(_destinationpath))
            {
                try
                {
                    foreach (var item in Directory.GetFiles(_destinationpath))
                    {
                        File.Delete(item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            base.Dispose(disposing);
        }

        private string _destinationpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CommonConst.PATH_STORAGE_FOLDER_PDF);
        public void DownloadFile(string url, WebViewPDF customWebView)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                //WebRequest request = WebRequest.Create(url);
                // set timeout request 60s. WebRequest.TimeoutProperty is in miliseconds
                //request.Timeout = CommonConst.TIME_OUT_WEBVIEW_IN_MILISECONDS;

                NSUrl nsURL = new NSUrl(url);
                NSUrlRequest request = new NSUrlRequest(nsURL);

                string filename = string.Empty;
                if (!Directory.Exists(_destinationpath))
                {
                    Directory.CreateDirectory(_destinationpath);
                }

                try
                {
                    _wkWebViewPDF.LoadRequest(request);

                    /*using (WebResponse response = request.GetResponse())
                    {
                        string contentType = response.Headers["Content-Type"];

                        // Success case
                        if (CommonConst.HTTP_RESPONSE_HEADER_CONTENT_TYPE_PDF.Equals(contentType))
                        {
                            string path = response.Headers["Content-Disposition"];

                            var responseStream = response.GetResponseStream();
                            // set Read write file Timeout 60s. ReadTimeout, WriteTimeout are in miliseconds
                            responseStream.ReadTimeout = CommonConst.TIME_OUT_WEBVIEW_IN_MILISECONDS;
                            responseStream.WriteTimeout = CommonConst.TIME_OUT_WEBVIEW_IN_MILISECONDS;

                            using (var fileStream = File.Create(System.IO.Path.Combine(_destinationpath, filename)))
                            {
                                responseStream.CopyTo(fileStream);
                            }

                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                var pathPdf = Path.Combine(_destinationpath, filename);
                                var assetPath = new NSUrl(pathPdf, true);
                                _wkWebViewPDF.LoadFileUrl(assetPath, assetPath);

                                await Task.Delay(500);
                                customWebView.OnNavigated(new WebNavigatedEventArgs(WebNavigationEvent.NewPage, null, pathPdf, WebNavigationResult.Success));
                            });
                        }
                        else
                        {
                            using (Stream stream = response.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                                String responseString = reader.ReadToEnd();
                                customWebView.RetryStatus = false;

                                try
                                {
                                    CommonResponseDTO commonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonResponseDTO>(responseString);

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        var pathPdf = Path.Combine(_destinationpath, filename);
                                        var assetPath = new NSUrl(pathPdf, true);
                                        _wkWebViewPDF.LoadFileUrl(assetPath, assetPath);
                                        customWebView.OnNavigated(new WebNavigatedEventArgs(WebNavigationEvent.NewPage, null, pathPdf, WebNavigationResult.Failure));
                                    });

                                    if (commonResponse != null)
                                    {
                                        HandleWebViewError(WebViewErorMode.ErrorCommon, commonResponse.Message);
                                    }
                                    else
                                    {
                                        HandleWebViewError(WebViewErorMode.ErrorCommon, CommonConst.MessageRetry);
                                    }
                                }
                                catch
                                {
                                    //handle error webException default when error ocurs 
                                    throw new WebException();
                                }
                            }
                        }
                    }*/
                }
                catch (WebException ex)
                {
                    customWebView.RetryStatus = false;
                    switch (ex.Status)
                    {
                        case WebExceptionStatus.ConnectionClosed:
                        case WebExceptionStatus.ConnectFailure:
                            HandleWebViewError(WebViewErorMode.NoInternet, "");
                            break;
                        case WebExceptionStatus.Timeout:
                            HandleWebViewError(WebViewErorMode.Timeout, "");
                            break;
                        default:
                            HandleWebViewError(WebViewErorMode.ErrorCommon, CommonConst.ErrorCommonWebviewPDF);
                            break;
                    }
                }
            });
        }

        void HandleWebViewError(WebViewErorMode webViewErorMode, string message)
        {
            //LogCommon.LogDevService("WebViewRendererIOS: HandleError");
            (Element as WebViewPDF)?.OnWebViewError(new WebViewErorEventArgs
            {
                ErrorMode = webViewErorMode,
                Message = message,
                BindingContext = _bindingContextCache
            });
        }

        private NSUrlRequest nSUrlRequest = new NSUrlRequest();
        private NSTimer _timer;
        private readonly int timeout = CommonConst.TIME_OUT_WEBVIEW_IN_MILISECONDS / 1000;

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Invalidate();
                _timer.Dispose();
                _timer = null;
            }
        }

        [Export("webView:decidePolicyForNavigationResponse:decisionHandler:")]
        public void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
        {
            try
            {
                var response = (NSHttpUrlResponse)navigationResponse.Response;
                var contentType = response.AllHeaderFields[FromObject("Content-Type")]?.ToString() ?? "";
                if (!CommonConst.HTTP_RESPONSE_HEADER_CONTENT_TYPE_PDF.Equals(contentType))
                {
                    decisionHandler?.Invoke(WKNavigationResponsePolicy.Cancel);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            decisionHandler?.Invoke(WKNavigationResponsePolicy.Allow);
        }

        [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        public void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            var urlRequest = navigationAction?.Request?.Url?.ToString() ?? "";
            var urlMain = (navigationAction?.Request?.MainDocumentURL?.Scheme
                               + "://"
                               + navigationAction?.Request?.MainDocumentURL?.Host
                               + navigationAction?.Request?.MainDocumentURL?.Path) ?? "";

            // link user click
            if (urlRequest.Contains(urlMain))
            {
                nSUrlRequest = navigationAction?.Request;
            }
            decisionHandler?.Invoke(WKNavigationActionPolicy.Allow);
        }

        [Export("webView:didFinishNavigation:")]
        public async void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            StopTimer();
            //string path = await SavePdf(nSUrlRequest?.Url?.ToString());
            Element?.OnNavigated(new WebNavigatedEventArgs(WebNavigationEvent.NewPage, null, nSUrlRequest?.Url?.ToString(), WebNavigationResult.Success));
        }

        [Export("webView:didFailNavigation:withError:")]
        public async void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            await ShowError(webView, error);
        }

        [Export("webView:didFailProvisionalNavigation:withError:")]
        public async void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            await ShowError(webView, error);
        }

        private async Task ShowError(WKWebView webView, NSError error)
        {
            Element?.OnNavigated(new WebNavigatedEventArgs(WebNavigationEvent.NewPage, null, nSUrlRequest?.Url?.ToString(), WebNavigationResult.Failure));
            StopTimer();
            var enumErr = Enum.Parse(typeof(NSUrlError), error.Code.ToString());
            switch (enumErr)
            {
                case NSUrlError.Cancelled:
                    break;
                case NSUrlError.TimedOut:
                    var result = await WebViewUtility.ShowAlert(CommonConst.ErrorCommonWebviewPDF, CommonConst.ButtonClose, CommonConst.ButtonRetry);
                    if (result == 1 && !_isDead) // retry
                    {
                        webView?.LoadRequest(nSUrlRequest);
                    }
                    break;
                case NSUrlError.NotConnectedToInternet:
                case NSUrlError.CannotConnectToHost:
                case NSUrlError.NetworkConnectionLost:
                    var result1 = await WebViewUtility.ShowAlert(CommonConst.MessageRetry, CommonConst.ButtonClose, CommonConst.ButtonRetry);
                    if (result1 == 1 && !_isDead) // retry
                    {
                        webView?.LoadRequest(nSUrlRequest);
                    }
                    break;
                default:
                    CommonResponseDTO commonResponse;
                    try
                    {
                        NSData data = NSUrlConnection.SendSynchronousRequest(nSUrlRequest, out NSUrlResponse _response, out NSError _error);
                        commonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonResponseDTO>(data?.ToString() ?? "");
                    }
                    catch
                    {
                        //handle error when DeserializeObject failed
                        commonResponse = null;
                    }
                    await WebViewUtility.ShowAlert(commonResponse?.Message ?? CommonConst.ErrorCommonWebviewPDF, CommonConst.ButtonClose, null);
                    break;
            }
        }
    }
}
