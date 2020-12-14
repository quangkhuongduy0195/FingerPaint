using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Viewer.Base64Data = PdfData;
        }
    }
}
