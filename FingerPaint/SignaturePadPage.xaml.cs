﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FingerPaint.Request;
using Refit;
using SignaturePad.Forms;
using Xamarin.Forms;

namespace FingerPaint
{
    public partial class SignaturePadPage : ContentPage
    {
        public bool _saveImg;
        readonly string _host = "https://filesave.herokuapp.com";
        string fileId;
        List<IEnumerable<Point>> listUndo;
        public SignaturePadPage()
        {
            InitializeComponent();
            btnLoad.Clicked += BtnLoad_Clicked;
            btnSave.Clicked += BtnSave_ClickedAsync;
            listUndo = new List<IEnumerable<Point>>();
            btnSave.IsVisible = false;
            btnLoad.IsVisible = false;
            btnUndo.IsVisible = false;
            btnErase.IsVisible = false;
            btnReUndo.IsVisible = false;
            signatureView.PropertyChanged += SignatureView_PropertyChanged;
        }

        private void SignatureView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsBlank":
                    btnSave.IsVisible = !signatureView.IsBlank;
                    btnUndo.IsVisible = !signatureView.IsBlank;
                    btnErase.IsVisible = !signatureView.IsBlank;
                    break;
            }
        }
        private async void BtnLoad_Clicked(object sender, EventArgs e)
        {
            RequestGet request = new RequestGet();
            request.FileId = fileId;
            await LoadFileFormServe(request);
            imgSignature.IsVisible = true;
        }
        private async void BtnSave_ClickedAsync(object sender, System.EventArgs e)
        {
            Stream sigimage = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png);

            BinaryReader br = new BinaryReader(sigimage);
            br.BaseStream.Position = 0;
            Byte[] All = br.ReadBytes((int)sigimage.Length);
            byte[] image = (byte[])All;
            string base64 = Convert.ToBase64String(image);

            Random rnd = new Random();
            int id = rnd.Next();
            fileId = id.ToString();

            RequestSave request = new RequestSave()
            {
                FileId = fileId,
                FileName = "test.png",
                Base64File = base64
            };
            await PushFileToServer(request);

            signatureView.Clear();
            btnLoad.IsVisible = true;
        }

        async Task PushFileToServer(RequestSave request)
        {
            var apiReponse = RestService.For<IApiSaveFile>(_host);
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

            var apiReponse = RestService.For<IApiSaveFile>(_host);
            var reponse = await apiReponse.GetFile(request);
            if (reponse.Success == true)
            {
                imgSignature.Source = _host + reponse.LinkFile;
            }
        }

        void cbBlack_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                signatureView.StrokeColor = Color.Black;
                cbBlue.IsChecked = false;
                cbRed.IsChecked = false;
                cbGreen.IsChecked = false;
            }
        }

        void cbGreen_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                signatureView.StrokeColor = Color.Green;
                cbBlue.IsChecked = false;
                cbRed.IsChecked = false;
                cbBlack.IsChecked = false;
            }
        }

        void cbRed_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                signatureView.StrokeColor = Color.Red;
                cbBlue.IsChecked = false;
                cbBlack.IsChecked = false;
                cbGreen.IsChecked = false;
            }
        }

        void cbBlue_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                signatureView.StrokeColor = Color.Blue;
                cbBlack.IsChecked = false;
                cbRed.IsChecked = false;
                cbGreen.IsChecked = false;
            }
        }

        void btnSmall_Clicked(System.Object sender, System.EventArgs e)
        {
            signatureView.StrokeWidth = 2;
        }

        void btnNormal_Clicked(System.Object sender, System.EventArgs e)
        {
            signatureView.StrokeWidth = 5;
        }

        void btnBigger_Clicked(System.Object sender, System.EventArgs e)
        {
            signatureView.StrokeWidth = 10;
        }

        void btnUndo_Clicked(System.Object sender, System.EventArgs e)
        {
            btnReUndo.IsVisible = true;
            var strokes = signatureView.Strokes;
            signatureView.Strokes = RemoveStrokesLast(strokes);
        }

        public IEnumerable<IEnumerable<Point>> RemoveStrokesLast(IEnumerable<IEnumerable<Point>> strokes)
        {
            var ls = strokes.ToList();
            var a = ls.LastOrDefault();

            listUndo.Add(a);
            ls.Remove(a);
            return ls;
        }

        public IEnumerable<IEnumerable<Point>> ReUndoStrokesLast(IEnumerable<IEnumerable<Point>> strokes)
        {
            var ls = strokes.ToList();
            var a = listUndo.LastOrDefault();
            listUndo.Remove(a);
            if (listUndo.Count == 0)
                btnReUndo.IsVisible = false;
            ls.Add(a);
            return ls;
        }

        void btnReUndo_Clicked(System.Object sender, System.EventArgs e)
        {
            var strokes = signatureView.Strokes;
            signatureView.Strokes = ReUndoStrokesLast(strokes);
        }

        void btnErase_Clicked(System.Object sender, System.EventArgs e)
        {

        }
    }
}
