using System;
using System.Linq;
using System.Threading.Tasks;
using AzureBlobStorageSampleApp.Mobile.Shared;
using AzureBlobStorageSampleApp.Scanner;
using FFImageLoading.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;
using AzureBlobStorageSampleApp.Effects;

namespace AzureBlobStorageSampleApp
{
    public class AddPhotoPage : BaseContentPageWithPublicViewModel<AddPhotoViewModel>
    {
        #region Constant Fields
        readonly ToolbarItem _saveToobarItem, _cancelToolbarItem;
        readonly Entry _photoTitleEntry;
        readonly CachedImage _photoImage;
        readonly Button _takePhotoButton;
        readonly Label _geoLabel;
        readonly Button _takeScanButton;
        readonly Label _scanLabel;

        readonly Button _getPhotoGalleryButton;
        readonly Switch _photoGallerySwitch;
        readonly Label _photoGalleryLabel;

        readonly Label _descriptionCaptionLabel;
        readonly Label _tagsStringLabel;
        readonly Label _colorLabel;
        readonly Label _objectDescription;
        readonly Label _customVisionTagsStringLabel;

        readonly Switch _scannerSwitch;
        readonly Switch _computerVisionSwitch;
        readonly Switch _customVisionSwitch;

        readonly Label _scannerSwitchLabel;
        readonly Label _computerVisionSwitchLabel;
        readonly Label _customVisionSwitchLabel;

        readonly Label _dateTimeLabel;

        readonly string _geoString;
        readonly string _generalCognitiveServices;
        readonly string _entitiesCognitiveServices;

        #endregion

        #region Constructors
        public AddPhotoPage()
        {
            ViewModel.NoCameraFound += HandleNoCameraFound;
            ViewModel.SavePhotoCompleted += HandleSavePhotoCompleted;
            ViewModel.SavePhotoFailed += HandleSavePhotoFailed;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            { 
                ViewModel.IsInternetConnectionActive = true;
            }
            else 
            { 
               ViewModel.IsInternetConnectionActive = false;

            }

            _photoTitleEntry = new Entry
            {
                Placeholder = "Title",
                BackgroundColor = Color.White,
                TextColor = ColorConstants.TextColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ReturnType = ReturnType.Go
            };

            _photoTitleEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.PhotoTitle));
            _photoTitleEntry.SetBinding(Entry.ReturnCommandProperty, nameof(ViewModel.TakePhotoCommand));

            _geoLabel = new Label
            {
                //BackgroundColor = Color.White,
                //TextColor = ColorConstants.TextColor,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _geoLabel.SetBinding(Label.TextProperty, nameof(ViewModel.GeoString));

            _dateTimeLabel = new Label
            {
                //BackgroundColor = Color.White,
                //TextColor = ColorConstants.TextColor,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _dateTimeLabel.SetBinding(Label.TextProperty, nameof(ViewModel.PhotoCreatedDateTime), BindingMode.Default, new DateTimeOffSetToString());

            //ViewModel.GeoString = "Paramus, NJ";

            //var lat = 47.673988;
            //var lon = -122.121513;

            //var placemarks = Task.Run(async () => await Geocoding.GetPlacemarksAsync(lat, lon)).Result;

            //var placemark = placemarks?.FirstOrDefault();

            //if (placemark != null)
            //{
            //    var geocodeAddress =
            //        $"AdminArea:       {placemark.AdminArea}\n" +
            //        $"CountryCode:     {placemark.CountryCode}\n" +
            //        $"CountryName:     {placemark.CountryName}\n" +
            //        $"FeatureName:     {placemark.FeatureName}\n" +
            //        $"Locality:        {placemark.Locality}\n" +
            //        $"PostalCode:      {placemark.PostalCode}\n" +
            //        $"SubAdminArea:    {placemark.SubAdminArea}\n" +
            //        $"SubLocality:     {placemark.SubLocality}\n" +
            //        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
            //        $"Thoroughfare:    {placemark.Thoroughfare}\n";

            //    //Console.WriteLine(geocodeAddress);
            //    ViewModel.GeoString = geocodeAddress;
            //}

            _takePhotoButton = new Button
            {
                Text = "Take Photo",
                BackgroundColor = ColorConstants.NavigationBarBackgroundColor,
                //TextColor = ColorConstants.TextColor
                TextColor = Color.White,
            };

            _takePhotoButton.SetBinding(Button.CommandProperty, nameof(ViewModel.TakePhotoCommand));

            //TODO - only one Command binding per Button
            //_takePhotoButton.SetBinding(Button.CommandProperty, nameof(ViewModel.GetGeoLocationCommand));

            //_takePhotoButton.SetBinding(IsEnabledProperty, new Binding(nameof(ViewModel.IsPhotoSaving), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsPhotoSaving));
            _takePhotoButton.SetBinding(Button.IsVisibleProperty, nameof(ViewModel.IsBarcode), BindingMode.Default, new InverseBooleanConverter());


            _getPhotoGalleryButton = new Button
            { 
                Text = "Pick Photo",
                //BackgroundColor = ColorConstants.NavigationBarBackgroundColor,
                //TextColor = ColorConstants.TextColor    
                BackgroundColor = Color.White,
                TextColor = ColorConstants.NavigationBarBackgroundColor,                        
            };

            _getPhotoGalleryButton.SetBinding(Button.CommandProperty, nameof(ViewModel.GetPhotoCommand));
            _getPhotoGalleryButton.SetBinding(Button.IsVisibleProperty, nameof(ViewModel.IsPhotoGallery));

            _scanLabel = new Label
            {
                //BackgroundColor = Color.White,
                //TextColor = ColorConstants.TextColor,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _scanLabel.SetBinding(Label.TextProperty, nameof(ViewModel.BarcodeString), BindingMode.Default, new AddBarcodeWordConverter());

            _scannerSwitch = new Switch
            {
                //BackgroundColor = ColorConstants.NavigationBarBackgroundColor,
                //effects:SwitchChangeColor.FalseColor="#AAEE00",
                //effects:SwitchChangeColor.TrueColor="Blue",
               //IsToggled = false,
                





            };
            //_computerVisionSwitch,
            //_customVisionSwitch,


            //_scannerSwitch.SetBinding(SwitchChangeColor.TrueColorProperty, nameof(Color.Blue));



            //            _scannerSwitch.Effects.Add(Effect.Resolve("MyCompany.SwitchChangeColor"));

            //_scannerSwitch.SetBinding(SwitchChangeColor.TrueColorProperty, nameof(Color.Red));

            _scannerSwitch.Effects.Add(Effect.Resolve("MyCompany.SwitchChangeColorEffect"));

            //_scannerSwitch.SetBinding(SwitchChangeColor.FalseColorProperty, nameof(ViewModel.BlueColor));

            //ViewModel.BlueColor = Color.Blue;


//CHANGE BACK
            _scannerSwitch.SetBinding(SwitchChangeColor.TrueColorProperty, nameof(ViewModel.SwitchTrueColor));
            _scannerSwitch.SetBinding(Switch.IsToggledProperty , nameof(ViewModel.IsBarcode));

            _computerVisionSwitch = new Switch() { };
            _computerVisionSwitch.Effects.Add(Effect.Resolve("MyCompany.SwitchChangeColorEffect"));
            _computerVisionSwitch.SetBinding(SwitchChangeColor.TrueColorProperty, nameof(ViewModel.SwitchTrueColor));
            _computerVisionSwitch.SetBinding(Switch.IsToggledProperty , nameof(ViewModel.IsComputerVision));


            _customVisionSwitch = new Switch() { };
            _customVisionSwitch.Effects.Add(Effect.Resolve("MyCompany.SwitchChangeColorEffect"));
            _customVisionSwitch.SetBinding(SwitchChangeColor.TrueColorProperty, nameof(ViewModel.SwitchTrueColor));
            _customVisionSwitch.SetBinding(Switch.IsToggledProperty , nameof(ViewModel.IsCustomVision));

                        _computerVisionSwitch.SetBinding(Switch.IsVisibleProperty, nameof(ViewModel.IsInternetConnectionActive) );

            _customVisionSwitch.SetBinding(Switch.IsVisibleProperty, nameof(ViewModel.IsInternetConnectionActive) );


            _photoGallerySwitch = new Switch() { };
            _photoGallerySwitch.Effects.Add(Effect.Resolve("MyCompany.SwitchChangeColorEffect"));
            _photoGallerySwitch.SetBinding(SwitchChangeColor.TrueColorProperty, nameof(ViewModel.SwitchTrueColor));
            _photoGallerySwitch.SetBinding(Switch.IsToggledProperty , nameof(ViewModel.IsPhotoGallery));


            ViewModel.SwitchTrueColor = ColorConstants.NavigationBarBackgroundColor;


            _scannerSwitchLabel = new Label(){ 
                Text = "Barcode Reader",
                //TextColor = ColorConstants.TextColor,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
                };

            _photoGalleryLabel = new Label(){ 
                Text = "Pick Photo",
                //TextColor = ColorConstants.TextColor,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
                };

            _computerVisionSwitchLabel = new Label(){ 
                Text = "Vision AI",
                //TextColor = ColorConstants.TextColor,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
                };

            _computerVisionSwitchLabel.SetBinding(Label.IsVisibleProperty, nameof(ViewModel.IsInternetConnectionActive) );

            _customVisionSwitchLabel = new Label(){ 
                Text = "Custom AI",
                //TextColor = ColorConstants.TextColor,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
             };
            _customVisionSwitchLabel.SetBinding(Label.IsVisibleProperty, nameof(ViewModel.IsInternetConnectionActive) );


            Grid gridLayout = new Grid()
            {
            };

            gridLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            gridLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            gridLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            gridLayout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            gridLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            gridLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


            gridLayout.Children.Add(_scannerSwitchLabel, 1, 0);
            gridLayout.Children.Add(_photoGalleryLabel, 1, 1);
            gridLayout.Children.Add(_computerVisionSwitchLabel, 1, 2);
            gridLayout.Children.Add(_customVisionSwitchLabel, 1, 3);

            gridLayout.Children.Add(_scannerSwitch, 0, 0);
            gridLayout.Children.Add(_photoGallerySwitch, 0, 1);
            gridLayout.Children.Add(_computerVisionSwitch, 0, 2);
            gridLayout.Children.Add(_customVisionSwitch, 0, 3);

            _photoImage = new CachedImage();
            _photoImage.SetBinding(CachedImage.SourceProperty, nameof(ViewModel.PhotoImageSource));

            _takeScanButton = new Button
            {
                Text = "Scan barcode + Take photo",
                BackgroundColor = ColorConstants.NavigationBarBackgroundColor,
                //TextColor = ColorConstants.TextColor,
                TextColor = Color.White,
            };

            _takeScanButton.SetBinding(Button.IsVisibleProperty, nameof(ViewModel.IsBarcode) );


            //_takeScanButton.SetBinding(Button.CommandProperty, nameof(ViewModel.TakeScanCommand));

            _takeScanButton.Clicked += async delegate
            {
                var customScanPage = new CustomScanPage();
                await Navigation.PushAsync(customScanPage);
            };

            //_takePhotoButton.SetBinding(IsEnabledProperty, new Binding(nameof(ViewModel.IsPhotoSaving), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsPhotoSaving));

            _descriptionCaptionLabel = new Label
            {
                //BackgroundColor = Color.White,
                //TextColor = ColorConstants.TextColor,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _descriptionCaptionLabel.SetBinding(Label.TextProperty, nameof(ViewModel.DescriptionCaptionOfImage), BindingMode.Default, new AddCaptionWordConverter());
            _descriptionCaptionLabel.SetBinding(Label.IsVisibleProperty, nameof(ViewModel.IsComputerVision));


            _colorLabel = new Label
            {
                //BackgroundColor = Color.White,
                //TextColor = ColorConstants.TextColor,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            //_colorLabel.SetBinding(Label.TextProperty, nameof(ViewModel.ForegroundColor), BindingMode.Default, new AddColorWordConverter());

            _colorLabel.SetBinding(Label.TextProperty, nameof(ViewModel.ColorsCombinedString), BindingMode.Default, new AddColorWordConverter());
            _colorLabel.SetBinding(Label.IsVisibleProperty, nameof(ViewModel.IsComputerVision));

            _objectDescription = new Label
            {
                //BackgroundColor = Color.White,
                //TextColor = ColorConstants.TextColor,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _objectDescription.SetBinding(Label.TextProperty, nameof(ViewModel.ObjectDescription), BindingMode.Default, new AddObjectDescriptionWordConverter());
            _objectDescription.SetBinding(Label.IsVisibleProperty, nameof(ViewModel.IsComputerVision));


            _tagsStringLabel = new Label
            {
                //BackgroundColor = Color.White,
                //TextColor = ColorConstants.TextColor,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _tagsStringLabel.SetBinding(Label.TextProperty, nameof(ViewModel.TagsCombinedString), BindingMode.Default, new AddTagsWordConverter());
            _tagsStringLabel.SetBinding(Label.IsVisibleProperty, nameof(ViewModel.IsComputerVision));

           _customVisionTagsStringLabel = new Label
           {
               //BackgroundColor = Color.White,
               //TextColor = ColorConstants.TextColor,
               //HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
           };

            _customVisionTagsStringLabel.SetBinding(Label.TextProperty, nameof(ViewModel.CustomVisionTagsCombinedString), BindingMode.Default, new AddCustomVisionWordConverter());
            _customVisionTagsStringLabel.SetBinding(Label.IsVisibleProperty, nameof(ViewModel.IsCustomVision));

            _saveToobarItem = new ToolbarItem
            {
                Text = "Save",
                Priority = 0,
                AutomationId = AutomationIdConstants.AddPhotoPage_SaveButton,
            };

            _saveToobarItem.SetBinding(MenuItem.CommandProperty, nameof(ViewModel.SavePhotoCommand));
            ToolbarItems.Add(_saveToobarItem);

            _cancelToolbarItem = new ToolbarItem
            {
                Text = "Cancel",
                Priority = 1,
                AutomationId = AutomationIdConstants.CancelButton
            };
            _cancelToolbarItem.Clicked += HandleCancelToolbarItemClicked;

            ToolbarItems.Add(_cancelToolbarItem);

            var activityIndicator = new ActivityIndicator();
            activityIndicator.SetBinding(IsVisibleProperty, nameof(ViewModel.IsPhotoSaving));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsPhotoSaving));

            this.SetBinding(TitleProperty, nameof(ViewModel.PhotoTitle));

            Padding = new Thickness(20);

            var stackLayout = new StackLayout
            {
                Spacing = 20,

                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,

                //Children = {
                //    _photoImage,
                //    _photoTitleEntry,
                //    _takePhotoButton,
                //    activityIndicator
                //}




            //Children = {
                //    _photoImage,
                //    _photoTitleEntry,
                //    _scanLabel,
                //    _geoLabel,
                //    _descriptionCaptionLabel,
                //    _objectDescription,
                //    _colorLabel,
                //    _tagsStringLabel,
                //    _customVisionTagsStringLabel,
                //    _takePhotoButton,
                //    _takeScanButton,
                //    //_scannerSwitch,
                //    //_computerVisionSwitch,
                //    //_customVisionSwitch,
                //    gridLayout,
                //    activityIndicator
                //}

            Children = {
                    _photoImage,
                    _photoTitleEntry,
                    _scanLabel,
                    _geoLabel,
                    _dateTimeLabel,
                    _takePhotoButton,
                    _takeScanButton,
                    _getPhotoGalleryButton,
                    gridLayout,
                    activityIndicator,
                    _descriptionCaptionLabel,
                    _objectDescription,
                    _colorLabel,
                    _tagsStringLabel,
                    _customVisionTagsStringLabel,

                }

            };

            Content = new ScrollView { Content = stackLayout };
        }
        #endregion

        #region Methods
        void HandleSavePhotoCompleted(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Photo Saved", string.Empty, "OK");
                ClosePage();
            });
        }

        void HandleCancelToolbarItemClicked(object sender, EventArgs e)
        {
            if (!ViewModel.IsPhotoSaving)
                ClosePage();
        }

        void HandleSavePhotoFailed(object sender, string errorMessage) => DisplayErrorMessage(errorMessage);

        void HandleNoCameraFound(object sender, EventArgs e) => DisplayErrorMessage("No Camera Found");

        void DisplayErrorMessage(string message) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", message, "Ok"));

        void ClosePage() =>
            Device.BeginInvokeOnMainThread(async () => await Navigation.PopModalAsync());
        #endregion

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }


        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            { 
                ViewModel.IsInternetConnectionActive = true;
            }
            else
            { 
                ViewModel.IsInternetConnectionActive = false;
            }

        }
    }
}
