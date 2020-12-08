using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.AppModel;
using Prism;
using Prism.Services;
using Prism.Services.Dialogs;

namespace FingerPaint.ViewModels
{
    public abstract class ViewModelBase : BindableBase,
        IInitializeAsync,
        INavigatedAware,
        IDestructible,
        IPageLifecycleAware,
        IApplicationLifecycleAware
    {
        protected INavigationService _navigationService { get; set; }

        public ViewModelBase(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        #region Lifecycle aware
        public virtual Task InitializeAsync(INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        public virtual void OnResume()
        {
        }

        public virtual void OnSleep()
        {
        }

        public virtual void Destroy()
        {
        }
        #endregion

        #region Base Properties
        private string _title;
        /// <summary>
        /// Identifies the <see cref="P:Xamarin.Forms.Page.Title" /> property.
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isBusy;
        /// <summary>
        /// Marks the Page as busy. This will cause the platform specific global activity indicator to show a busy state.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
        #endregion
    }
}
