using System;
using System.ComponentModel;
using FingerPaint.iOS.Renderers;
using Foundation;
using PdfKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using PdfViewF = FingerPaint.Extended.PdfView;

[assembly: ExportRenderer(typeof(PdfViewF), typeof(PdfViewRenderer))]
namespace FingerPaint.iOS.Renderers
{
    public class PdfViewRenderer : ViewRenderer<PdfViewF, PdfView>
    {
        public PdfViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PdfViewF> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                // init
                var nativeView = new PdfView();
                if (e.NewElement.Base64Data != null)
                {
                    var data = new NSData(e.NewElement.Base64Data, NSDataBase64DecodingOptions.IgnoreUnknownCharacters);
                    nativeView.Document = new PdfDocument(data);
                }
                SetNativeControl(nativeView);
            }
            if (e.OldElement != null)
            {
                // cleanup
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case nameof(PdfViewF.Base64Data):
                    var control = Control;
                    var element = sender as PdfViewF;
                    var data = new NSData(element.Base64Data, NSDataBase64DecodingOptions.IgnoreUnknownCharacters);
                    control.Document?.Dispose();
                    control.Document = new PdfDocument(data);
                    break;
                default:
                    break;
            }
        }
    }
}
