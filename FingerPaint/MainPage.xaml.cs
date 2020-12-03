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
using Xamarin.Forms;

namespace FingerPaint
{
    public partial class MainPage : ContentPage
    {
        public bool _saveImg;
        private Point[] points;
        string host = "https://filesave.herokuapp.com";
        string fileId;
        public MainPage()
        {
            InitializeComponent();
            btnLoad.Clicked += BtnLoad_Clicked;
            btnSave.Clicked += BtnSave_ClickedAsync;
            btnSave.IsVisible = false;
            btnLoad.IsVisible = false;
            signatureView.PropertyChanged += SignatureView_PropertyChanged;
        }

        private async void BtnLoad_Clicked(object sender, EventArgs e)
        {
            RequestGet request = new RequestGet();
            request.FileId = fileId;
            await LoadFileFormServe(request);
        }

        private void SignatureView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBlank":
                    btnSave.IsVisible = !signatureView.IsBlank;
                    break;
            }
        }

        private async void BtnSave_ClickedAsync(object sender, System.EventArgs e)
        {
            Stream sigimage = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png);

            BinaryReader br = new BinaryReader(sigimage);
            br.BaseStream.Position = 0;
            Byte[] All = br.ReadBytes((int)sigimage.Length);
            byte[] image = (byte[])All;
            var lenght = image.Length;
            string base64 = Convert.ToBase64String(image);
            var bytefrombase64 = Convert.FromBase64String(base64);
            var lenghtfrombase64 = bytefrombase64.Length;

            Random rnd = new Random();
            int id = rnd.Next();
            //fileId.Add(id.ToString());
            fileId = id.ToString();

            RequestSave request = new RequestSave()
            {
                FileId = fileId,
                FileName="test.png",
                Base64File = base64
            };
            await PushFileToServer(request);


            signatureView.Clear();
            btnLoad.IsVisible = true;
        }

        async Task PushFileToServer(RequestSave request)
        {
            var apiReponse = RestService.For<IApiSaveFile>(host);
            var reponse = await apiReponse.SaveFile(request);
            if (reponse.Success == true)
            {
                await DisplayAlert("Notice", "Save Image Success", "OK");
            }
            else
            {
                await DisplayAlert("Notice", "Save Image Fail", "OK");
            }
        }

        async Task LoadFileFormServe(RequestGet request)
        {

            var apiReponse = RestService.For<IApiSaveFile>(host);
            var reponse = await apiReponse.GetFile(request);
            if (reponse.Success == true)
            {
                imgSignature.Source = host + reponse.LinkFile;
            }
        }
    }
}
