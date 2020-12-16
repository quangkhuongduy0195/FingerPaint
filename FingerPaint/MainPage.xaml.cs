using FingerPaint.Request;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Refit;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.IO;
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
            btnPdfPad.Clicked += BtnPdfPad_ClickedAsync;
            btnSignPad.Clicked += BtnSignPad_ClickedAsync;
        }

        private async void BtnSignPad_ClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new SignaturePadPage()));
        }

        private async void BtnPdfPad_ClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NavigationPage(new PDFViewPage()));
        }
    }
}
