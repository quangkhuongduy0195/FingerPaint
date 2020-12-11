using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using FingerPaint.Models;
using FingerPaint.Utilities;
using Prism.Commands;
using Prism.Navigation;
using System.IO;
using System.Net;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;

namespace FingerPaint.ViewModels
{
    public class SigningPageViewModel : ViewModelBase
    {
        private string _urlPDF;
        public string UrlPDF
        {
            get { return _urlPDF; }
            set { SetProperty(ref _urlPDF, value); }
        }

        private string _localPDFUrl;
        public string LocalPDFUrl
        {
            get { return _localPDFUrl; }
            set { SetProperty(ref _localPDFUrl, value); }
        }

        public SigningPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public override Task InitializeAsync(INavigationParameters parameters)
        {
            if (parameters.ContainsKey(PageConstants.PDFList))
            {
                Title = parameters.GetValue<string>(PageConstants.PDFList);
            }

            return base.InitializeAsync(parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey(nameof(FilesPDFModel.Url)))
            {
                UrlPDF = parameters[nameof(FilesPDFModel.Url)].ToString();
            }
            else if (parameters.ContainsKey(nameof(PageConstants.Start)))
            {
                LocalPDFUrl = parameters[nameof(PageConstants.Start)].ToString(); 
            }
        }

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new DelegateCommand(async () => await GoBackCommandExecute()));

        private async Task GoBackCommandExecute()
        {
            try
            {
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private DelegateCommand _signCommand;
        public DelegateCommand SignCommand => _signCommand ?? (_signCommand = new DelegateCommand(async () => await SignCommandExecute()));

        private async Task SignCommandExecute()
        {
            try
            {
                //using (var pdfStream = CopyStreamedFileToLocalStorage(GetStreamFromUrl(UrlPDF)))
                //{
                //    var imageFileName = "TimCook_signature.png";
                //    var signatureRectangle = new Rectangle(400, 50, 150, 50);
                //    InsertSignatureImageIntoPDF(pdfStream, imageFileName, signatureRectangle);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private Stream GetStreamFromUrl(string url)
        {
            var request = WebRequest.Create(url);
            Stream responseStream = request.GetResponse().GetResponseStream();
            return responseStream;
        }

        private string _destinationPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CommonConst.PATH_STORAGE_FOLDER_PDF);
        private FileStream CopyStreamedFileToLocalStorage(Stream stream)
        {
            string filename = string.Empty;
            var fileStream = File.Create(System.IO.Path.Combine(_destinationPath, filename));
            stream.CopyTo(fileStream);
            return fileStream;
        }

        private void InsertSignatureImageIntoPDF(Stream pdfStream, string imageFileName, Rectangle signatureRectangle)
        {
            // get page
            var reader = new PdfReader(pdfStream);
            var writer = new PdfWriter(pdfStream);
            var pdfDocument = new PdfDocument(reader, writer);
            var page = pdfDocument.GetLastPage();
            var size = page.GetPageSize();
            Console.WriteLine("Page size rectangle X = " + size.GetX() + "; Y = " + size.GetY() + "; Width = " + size.GetWidth() + "; Height = " + size.GetHeight());

            // load image
            var imageData = ImageDataFactory.Create(imageFileName);

            // add image to canvas
            var canvas = new PdfCanvas(page);
            canvas.AddImageFittedIntoRectangle(imageData, signatureRectangle, false);

            // close
            pdfDocument.Close();
        }
    }
}
