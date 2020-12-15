using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.FilePicker;
using Xamarin.Forms;

namespace FingerPaint
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
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
