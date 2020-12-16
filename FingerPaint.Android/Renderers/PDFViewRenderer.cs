using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using FingerPaint.Controls.Renderers;
using FingerPaint.Droid.PdfLibrarys.Pdfs;
using FingerPaint.Droid.PdfLibrarys.Utilities;
using FingerPaint.Droid.PdfLibrarys.Views;
using FingerPaint.Droid.Renderers;
using Java.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FormControl = FingerPaint.Controls.Renderers;

[assembly: ExportRenderer(typeof(FormControl.PDFView), typeof(PDFViewRenderer))]
namespace FingerPaint.Droid.Renderers
{
    public class PDFViewRenderer : ViewRenderer<FormControl.PDFView, PdfView>
    {
        public PDFViewRenderer(Context context) : base(context)
        {}

        protected override void OnElementChanged(ElementChangedEventArgs<PDFView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var pdfView = new PdfView(Context, Element);
                    SetNativeControl(pdfView);
                    LoadFile(Element?.Uri);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case nameof(PDFView.Uri):
                    LoadFile(Element?.Uri);
                    break;
            }
        }

        private void LoadFile(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return;

            File file = Android.OS.Build.VERSION.SdkInt > BuildVersionCodes.M ? GetFileFromContentUri(uri) : new File(uri);
            if (!file.Exists())
            {
                Toast.MakeText(Context, "File not exist", ToastLength.Short).Show();
                return;
            }
            if (file != null && Control != null)
            {
                var pdfFile = new PdfFile(file);
                Control.SetPdfFile(pdfFile);
            }
        }

        public File GetFileFromContentUri(string url)
        {
            File file = null;
            System.IO.Stream inputStream = null;
            FileOutputStream outputStream = null;

            try
            {
                // inputStream = Resources.OpenRawResource(resource);
                inputStream = this.Context.ContentResolver.OpenInputStream(Android.Net.Uri.Parse(url));
                file = File.CreateTempFile("PDF", null);
                outputStream = new FileOutputStream(file);

                byte[] buffer = new byte[8 * 1024];
                int bytesRead;
                while ((bytesRead = inputStream.Read(buffer)) > 0)
                {
                    outputStream.Write(buffer, 0, bytesRead);
                }
            }
            catch (IOException ex)
            {
                LogUtils.Log(nameof(PDFViewRenderer), ex.Message);
            }
            finally
            {
                inputStream.Close();
                outputStream.Close();
            }
            return file;
        }
    }
}



