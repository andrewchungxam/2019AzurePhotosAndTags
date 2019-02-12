using Xamarin.Forms;
using System;

namespace AzureBlobStorageSampleApp
{
    public abstract class BaseContentPageWithPublicViewModel<T> : ContentPage where T : BaseViewModel, new()
    {
        protected BaseContentPageWithPublicViewModel()
        {
            BindingContext = ViewModel;
            BackgroundColor = ColorConstants.PageBackgroundColor;
            this.SetBinding(IsBusyProperty, nameof(ViewModel.IsInternetConnectionActive));
        }

        public T ViewModel { get; } = new T();
    }
}
