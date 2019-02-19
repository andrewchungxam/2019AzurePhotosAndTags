using System;

using Xamarin.Forms;

using AzureBlobStorageSampleApp.Shared;
using AzureBlobStorageSampleApp.Mobile.Shared;

namespace AzureBlobStorageSampleApp
{
    public class PhotoListPage : BaseContentPage<PhotoListViewModel>
    {
        #region Constant Fields
        readonly ListView _photosListView;
        readonly ToolbarItem _addPhotosButton;

        SearchBar searchBar;


        #endregion

        #region Constructors
        public PhotoListPage()
        {

            searchBar = new SearchBar
            {
                Placeholder = "Enter search term",
                //SearchCommand = new Command(() => { Console.WriteLine($"Search command"); })
                BackgroundColor = Color.Wheat,

            };

            searchBar.SetBinding(SearchBar.SearchCommandProperty, nameof(ViewModel.SearchCommand));
            searchBar.SetBinding(SearchBar.TextProperty, nameof(ViewModel.SearchString));   

            _addPhotosButton = new ToolbarItem
            {
                Text = "+",
                AutomationId = AutomationIdConstants.AddPhotoButton
            };
            _addPhotosButton.Clicked += HandleAddContactButtonClicked;

            ToolbarItems.Add(_addPhotosButton);

            _photosListView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(PhotoViewCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = Color.Transparent,
                AutomationId = AutomationIdConstants.PhotoListView,
                SeparatorVisibility = SeparatorVisibility.None
            };
            _photosListView.ItemSelected += HandleItemSelected;
            _photosListView.SetBinding(ListView.IsRefreshingProperty, nameof(ViewModel.IsRefreshing));
            _photosListView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.AllPhotosList));
            _photosListView.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.RefreshCommand));

            //#TODO - modifying size of cells
            _photosListView.HasUnevenRows = true;




            Title = PageTitles.PhotoListPage;

            var stackLayout = new StackLayout();
            stackLayout.Children.Add(searchBar);
            stackLayout.Children.Add(_photosListView);


            var relativeLayout = new RelativeLayout();
            //relativeLayout.Children.Add(searchBar, _photosListView,

            relativeLayout.Children.Add(stackLayout,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0),
                                       Constraint.RelativeToParent(parent => parent.Width),
                                       Constraint.RelativeToParent(parent => parent.Height));

            Content = relativeLayout;
        }

        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(_photosListView.BeginRefresh);
        }

        void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = sender as ListView;
            var selectedPhoto = e?.SelectedItem as PhotoModel;

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (selectedPhoto != null)
                {
                    await Navigation.PushAsync(new PhotoDetailsPage(selectedPhoto));
                    listView.SelectedItem = null;
                }
            });
        }

        void HandleAddContactButtonClicked(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () => await Navigation.PushModalAsync(new BaseNavigationPage(new AddPhotoPage())));
        #endregion
    }
}
