using System;
using System.Threading.Tasks;
using FingerPaint.Utilities;
using Prism.Commands;
using Prism.Navigation;

namespace FingerPaint.ViewModels
{
    public class StartPageViewModel : ViewModelBase
    {
        public StartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        private DelegateCommand _openFileCommand;
        public DelegateCommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new DelegateCommand(async () => await OpenFileCommandExecute()));

        private async Task OpenFileCommandExecute()
        {
            try
            {
                string testPDF = "HeaderFooter.pdf";
                var pr = new NavigationParameters();
                pr.Add(nameof(PageConstants.Start), testPDF);
                await _navigationService.NavigateAsync(PageConstants.Sign, pr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private DelegateCommand _downloadCommand;
        public DelegateCommand DownloadCommand => _downloadCommand ?? (_downloadCommand = new DelegateCommand(async () => await DownloadCommandExecute()));

        private async Task DownloadCommandExecute()
        {
            try
            {
                await _navigationService.NavigateAsync(PageConstants.PDFList); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
