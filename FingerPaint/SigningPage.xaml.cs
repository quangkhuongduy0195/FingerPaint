using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using RectangleText = iText.Kernel.Geom.Rectangle;

namespace FingerPaint
{
    public partial class SigningPage : ContentPage
    {
        public SigningPage()
        {
            InitializeComponent();
            /*
             * var assembly = IntrospectionExtensions.GetTypeInfo(GetType()).Assembly;
            var path = "sbicfd.Logics.licenses.json";// no "-"
             */
        }

        public string PdfData { get; internal set; }

        public byte[] ImageBinary { get; internal set; }

        public string PdfFilePath { get; internal set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Viewer.Base64Data = PdfData;
        }

        void OnButtonSignClicked(System.Object sender, System.EventArgs e)
        {
            using (var pdfStream = new FileStream(PdfFilePath, FileMode.Open))
            {
                InsertImageIntoPdf(pdfStream, ImageBinary, 400, 50, 150, 50);
            }
        }

        public static void InsertImageIntoPdf(Stream pdfStream, byte[] imageBinary, float x, float y, float width, float height)
        {
            // get page
            var reader = new PdfReader(pdfStream);
            var writer = new PdfWriter(pdfStream);
            var pdfDocument = new PdfDocument(reader, writer);
            var page = pdfDocument.GetLastPage();

            // load image
            var imageData = ImageDataFactory.Create(imageBinary);

            // add image to canvas
            var canvas = new PdfCanvas(page);
            canvas.AddImageFittedIntoRectangle(imageData, new RectangleText(x, y, width, height), false);

            // close
            pdfDocument.Close();
        }
    }
}
