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

        readonly Label _descriptionCaptionLabel;
        readonly Label _tagsStringLabel;
        readonly Label _colorLabel;
        readonly Label _objectDescription;
        readonly Label _customVisionTagsStringLabel;

        readonly Switch _scannerSwitch;
        readonly Switch _computerVisionSwitch;
        readonly Switch _customVisionSwitch;


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
                BackgroundColor = Color.White,
                TextColor = ColorConstants.TextColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _geoLabel.SetBinding(Label.TextProperty, nameof(ViewModel.GeoString));

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
                TextColor = ColorConstants.TextColor
            };

            _takePhotoButton.SetBinding(Button.CommandProperty, nameof(ViewModel.TakePhotoCommand));

            //TODO - only one Command binding per Button
            //_takePhotoButton.SetBinding(Button.CommandProperty, nameof(ViewModel.GetGeoLocationCommand));

            _takePhotoButton.SetBinding(IsEnabledProperty, new Binding(nameof(ViewModel.IsPhotoSaving), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsPhotoSaving));


            _scanLabel = new Label
            {
                BackgroundColor = Color.White,
                TextColor = ColorConstants.TextColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _scanLabel.SetBinding(Label.TextProperty, nameof(ViewModel.BarcodeString));


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

            _scannerSwitch.SetBinding(SwitchChangeColor.TrueColorProperty, nameof(ViewModel.SwitchTrueColor));

            ViewModel.SwitchTrueColor = ColorConstants.NavigationBarBackgroundColor;



            _computerVisionSwitch = new Switch() { };

            _photoImage = new CachedImage();
            _photoImage.SetBinding(CachedImage.SourceProperty, nameof(ViewModel.PhotoImageSource));

            _takeScanButton = new Button
            {
                Text = "Scan barcode",
                BackgroundColor = ColorConstants.NavigationBarBackgroundColor,
                TextColor = ColorConstants.TextColor
            };

            //_takeScanButton.SetBinding(Button.CommandProperty, nameof(ViewModel.TakeScanCommand));

            _takeScanButton.Clicked += async delegate
            {
                var customScanPage = new CustomScanPage();
                await Navigation.PushAsync(customScanPage);
            };

            //_takePhotoButton.SetBinding(IsEnabledProperty, new Binding(nameof(ViewModel.IsPhotoSaving), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsPhotoSaving));

            _descriptionCaptionLabel = new Label
            {
                BackgroundColor = Color.White,
                TextColor = ColorConstants.TextColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _descriptionCaptionLabel.SetBinding(Label.TextProperty, nameof(ViewModel.DescriptionCaptionOfImage));

            _colorLabel = new Label
            {
                BackgroundColor = Color.White,
                TextColor = ColorConstants.TextColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _colorLabel.SetBinding(Label.TextProperty, nameof(ViewModel.ForegroundColor));

            _objectDescription = new Label
            {
                BackgroundColor = Color.White,
                TextColor = ColorConstants.TextColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _objectDescription.SetBinding(Label.TextProperty, nameof(ViewModel.ObjectDescription));

            _tagsStringLabel = new Label
            {
                BackgroundColor = Color.White,
                TextColor = ColorConstants.TextColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            _tagsStringLabel.SetBinding(Label.TextProperty, nameof(ViewModel.TagsCombinedString));


           _customVisionTagsStringLabel = new Label
           {
               BackgroundColor = Color.White,
               TextColor = ColorConstants.TextColor,
               HorizontalOptions = LayoutOptions.FillAndExpand,
           };

            _customVisionTagsStringLabel.SetBinding(Label.TextProperty, nameof(ViewModel.CustomVisionTagsCombinedString));



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




            Children = {
                    _photoImage,
                    _photoTitleEntry,
                    _geoLabel,
                    _scanLabel,
                    _descriptionCaptionLabel,
                    _objectDescription,
                    _tagsStringLabel,
                    _customVisionTagsStringLabel,
                    _takePhotoButton,
                    _takeScanButton,
                    _scannerSwitch,
                    _computerVisionSwitch,
                    //_computerVisionSwitch,
                    //_customVisionSwitch,
                    activityIndicator
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
    }
}
