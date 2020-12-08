using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using FingerPaint.Models;
using FingerPaint.Utilities;
using Prism.Commands;
using Prism.Navigation;

namespace FingerPaint.ViewModels
{
    public class PDFListPageViewModel : ViewModelBase
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        bool _isRefreshing;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                SetProperty(ref _isRefreshing, value);
            }
        }

        //List<FilesPDFModel> _listOfPDF = new List<FilesPDFModel>();
        public IList<FilesPDFModel> ListOfPDF { get; private set; }

        public PDFListPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            MockDownloadPDF();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            IsBusy = false;
            //MockDownloadPDF();
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

        private DelegateCommand _pdfListRefreshCommand;
        public DelegateCommand PDFListRefreshCommand => _pdfListRefreshCommand ?? (_pdfListRefreshCommand = new DelegateCommand(async () => await PDFListRefreshCommandExecute()));

        private async Task PDFListRefreshCommandExecute()
        {
            try
            {
                await _navigationService.GoBackAsync();
                //MockDownloadPDF();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private DelegateCommand<FilesPDFModel> _itemSelectedCommand;
        public DelegateCommand<FilesPDFModel> SelectedItemCommand => _itemSelectedCommand ?? (_itemSelectedCommand = new DelegateCommand<FilesPDFModel>(ItemSelectedCommandExecute));
        private async void ItemSelectedCommandExecute(FilesPDFModel filesPDF)
        {
            if (IsBusy)
                return;
            _cts?.Cancel();
            IsBusy = true;
            var pr = new NavigationParameters();
            pr.Add(PageConstants.PDFList, filesPDF.ItemName);
            pr.Add(nameof(FilesPDFModel.Url), filesPDF.Url);
            await _navigationService.NavigateAsync(PageConstants.Sign, pr);
        }

        private void MockDownloadPDF()
        {
            try
            {
                // TODO: Temporary Mockup. Call API later
                ListOfPDF = new List<FilesPDFModel>();
                ListOfPDF.Add(new FilesPDFModel { ItemName = "UIWebView", Url = "http://developer.apple.com/iphone/library/documentation/UIKit/Reference/UIWebView_Class/UIWebView_Class.pdf" });
                ListOfPDF.Add(new FilesPDFModel { ItemName = "Signature", Url = "https://www.aphis.usda.gov/mrpbs/systems/fmmi/downloads/How_to_Insert_a_Digital_Signature_PDF_Doc.pdf" });
                ListOfPDF.Add(new FilesPDFModel { ItemName = "Adobe", Url = "https://www.adobe.com/support/products/enterprise/knowledgecenter/media/c4611_sample_explain.pdf" });
                ListOfPDF.Add(new FilesPDFModel { ItemName = "UNEC", Url = "https://unec.edu.az/application/uploads/2014/12/pdf-sample.pdf" });
            }
            catch (System.Threading.Tasks.TaskCanceledException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
