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
    public class SigningPageViewModel : ViewModelBase
    {
        private string urlPDF;
        public string UrlPDF
        {
            get { return urlPDF; }
            set { SetProperty(ref urlPDF, value); }
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
    }
}
