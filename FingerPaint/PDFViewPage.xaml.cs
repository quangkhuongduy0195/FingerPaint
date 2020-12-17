using System;
using System.Collections.Generic;
using Plugin.FilePicker;
using Xamarin.Forms;

namespace FingerPaint
{
    public partial class PDFViewPage : ContentPage
    {
        public PDFViewPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
            {
                var file = await CrossFilePicker.Current.PickFile();
                pdfView.Uri = file.FilePath;
            });
        }


        async void OnClickButtonSelectFile(System.Object sender, System.EventArgs e)
        {
            var file = await CrossFilePicker.Current.PickFile();
            pdfView.Uri = file.FilePath;

            pdfView.CurrentScale = 1.5;
        }
    }
}
