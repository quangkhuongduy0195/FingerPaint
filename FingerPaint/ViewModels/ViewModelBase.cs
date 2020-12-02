using Prism.Mvvm;
using Prism.Navigation;

namespace FingerPaint.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        protected INavigationService _navigationService { get; set; }

        public ViewModelBase(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
