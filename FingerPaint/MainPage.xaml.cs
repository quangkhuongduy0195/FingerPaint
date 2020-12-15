using FingerPaint.Request;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Refit;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FingerPaint
{
    public partial class MainPage : ContentPage
    {
        public bool _saveImg;
        readonly string _host = "https://filesave.herokuapp.com";
        string fileId;
        List<IEnumerable<Point>> listUndo;
        public MainPage()
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
            //await LoadFileFormServe(request);
            await LoadFileFromDisk(request);
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
            //await PushFileToServer(request);
            await SaveFileToDisk(request);
            signatureView.Clear();
            btnLoad.IsVisible = true;


            // push
            var pdfBase64 = string.Empty;
            var assembly = IntrospectionExtensions.GetTypeInfo(GetType()).Assembly;
            var path = assembly.GetManifestResourceNames().FirstOrDefault(arg => arg != null && arg.Contains("Table"));
            using (var stream = assembly.GetManifestResourceStream(path))
            {
                var bytes = ReadToEnd(stream);
                pdfBase64 = Convert.ToBase64String(bytes);
            }
            var signingPage = new SigningPage();
            signingPage.PdfData = pdfBase64;
            signingPage.ImageBinary = image;
            await Navigation.PushAsync(signingPage);
        }

        private byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        async Task SaveFileToDisk(RequestSave request)
        {
            var existed = Application.Current.Properties.ContainsKey(request.FileId);
            if (!existed)
                Application.Current.Properties.Add(request.FileId, null);
            Application.Current.Properties[request.FileId] = request.Base64File;
            await Application.Current.SavePropertiesAsync();
            // verify
            var saved = Application.Current.Properties[request.FileId];
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

        async Task LoadFileFromDisk(RequestGet request)
        {
            var rootDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(rootDir, request.FileId);
            var existed = File.Exists(path);
            if (existed)
                File.Delete(path);
            var base64 = Application.Current.Properties[request.FileId].ToString();
            var byteArray = System.Convert.FromBase64String(base64);
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                /*
                 * Not using WriteAllBytesAsync
                 * b/c WriteAllBytesAsync support by .NET standard 2.1
                 * and not working in Native Platform
                 */
                File.WriteAllBytes(path, memoryStream.ToArray());
            }
            imgSignature.Source = path;
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
