using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using FingerPaint.Utilities;
using UIKit;
using WebKit;

namespace FingerPaint.iOS.Utility
{
    public static class WebViewUtility
    {
        /// <summary>
        /// Scaleses the page to fit.
        /// </summary>
        /// <param name="userContentController">User content controller.</param>
        public static void ScalesPageToFit(this WKUserContentController userContentController)
        {
            NSString jScript = new NSString("var meta = document.createElement('meta'); " +
                                                    "meta.setAttribute('name', 'viewport'); " +
                                                    "meta.setAttribute('content', 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no'); " +
                                                    "document.getElementsByTagName('head')[0].appendChild(meta);");
            WKUserScript wkUScript = new WKUserScript(jScript, WKUserScriptInjectionTime.AtDocumentEnd, true);
            userContentController.AddUserScript(wkUScript);
        }

        /// <summary>
        /// Shows the alert.
        /// </summary>
        /// <returns>The alert.</returns>
        /// <param name="message">Message.</param>
        /// <param name="btnClose">Button close.</param>
        /// <param name="btnRetry">Button retry.</param>
        public static Task<int> ShowAlert(string message, string btnClose, string btnRetry)
        {
            var tcs = new TaskCompletionSource<int>();
            var alert = new UIAlertView
            {
                Title = null,
                Message = message,
                CancelButtonIndex = 0
            };

            alert.AddButton(btnClose);
            if (btnRetry != null)
            {
                alert.AddButton(btnRetry);
            }
            alert.Clicked += (s, e) => tcs.TrySetResult((int)e.ButtonIndex);
            alert.Show();
            return tcs.Task;
        }
    }
}
