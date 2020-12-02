using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation;

namespace FingerPaint.ViewModels
{
    public class StartPageViewModel : ViewModelBase
    {
        public StartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
