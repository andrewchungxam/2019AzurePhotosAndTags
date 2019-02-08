using System;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

using Plugin.Media;
using Plugin.Media.Abstractions;

using AzureBlobStorageSampleApp.Shared;
using AzureBlobStorageSampleApp.Mobile.Shared;
using AsyncAwaitBestPractices.MVVM;
using AsyncAwaitBestPractices;
using Xamarin.Essentials;
using System.Linq;

namespace AzureBlobStorageSampleApp
{
    public class AddPhotoViewModel : BaseViewModel
    {
        #region Constant Fields
        readonly WeakEventManager _noCameraFoundEventManager = new WeakEventManager();
        readonly WeakEventManager _savePhotoCompletedEventManager = new WeakEventManager();
        readonly WeakEventManager<string> _savePhotoFailedEventManager = new WeakEventManager<string>();
        #endregion

        #region Fields
        ICommand _savePhotoCommand, _takePhotoCommand;
        ICommand _getGeoLocationCommand;

        string _photoTitle, _pageTitle = PageTitles.AddPhotoPage;
        bool _isPhotoSaving;
        ImageSource _photoImageSource;
        PhotoBlobModel _photoBlob;
        #endregion

        #region Events
        public event EventHandler NoCameraFound
        {
            add => _noCameraFoundEventManager.AddEventHandler(value);
            remove => _noCameraFoundEventManager.RemoveEventHandler(value);
        }

        public event EventHandler SavePhotoCompleted
        {
            add => _savePhotoCompletedEventManager.AddEventHandler(value);
            remove => _savePhotoCompletedEventManager.RemoveEventHandler(value);
        }

        public event EventHandler<string> SavePhotoFailed
        {
            add => _savePhotoFailedEventManager.AddEventHandler(value);
            remove => _savePhotoFailedEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Properties
        public ICommand TakePhotoCommand => _takePhotoCommand ??
            (_takePhotoCommand = new AsyncCommand(ExecuteTakePhotoCommand, continueOnCapturedContext: false));

        public ICommand SavePhotoCommand => _savePhotoCommand ??
            (_savePhotoCommand = new AsyncCommand(() => ExecuteSavePhotoCommand(PhotoBlob, PhotoTitle), continueOnCapturedContext: false));


        public ICommand GetGeoLocationCommand => _getGeoLocationCommand ??
            (_getGeoLocationCommand = new AsyncCommand(ExecuteGetGeoLocationCommand, continueOnCapturedContext: false));

        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public bool IsPhotoSaving
        {
            get => _isPhotoSaving;
            set => SetProperty(ref _isPhotoSaving, value);
        }

        public string PhotoTitle
        {
            get => _photoTitle;
            set => SetProperty(ref _photoTitle, value, UpdatePageTilte);
        }

        public ImageSource PhotoImageSource
        {
            get => _photoImageSource;
            set => SetProperty(ref _photoImageSource, value);
        }

        PhotoBlobModel PhotoBlob
        {
            get => _photoBlob;
            set => SetProperty(ref _photoBlob, value, UpdatePhotoImageSource);
        }

        string _geoString;
        string _generalCognitiveServices;
        string _entitiesCognitiveServices;

        public string GeoString
        {
            get => _geoString;
            set => SetProperty(ref _geoString, value);
        }

        public string GeneralCognitiveServices
        {
            get => _generalCognitiveServices;
            set => SetProperty(ref _generalCognitiveServices, value);
        }

        public string EntitiesCognitiveServices
        {
            get => _entitiesCognitiveServices;
            set => SetProperty(ref _entitiesCognitiveServices, value);
        }

        #endregion

        #region Methods

        async Task ExecuteGetGeoLocationCommand()
        {
            try
            {
                //this.GeoString = "Paramus, NJ";

                var lat = 47.673988;
                var lon = -122.121513;

                var placemarks = Task.Run(async () => await Geocoding.GetPlacemarksAsync(lat, lon)).Result;

                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    //var geocodeAddress =
                    //$"AdminArea:       {placemark.AdminArea}\n" +
                    //$"CountryCode:     {placemark.CountryCode}\n" +
                    //$"CountryName:     {placemark.CountryName}\n" +
                    //$"FeatureName:     {placemark.FeatureName}\n" +
                    //$"Locality:        {placemark.Locality}\n" +
                    //$"PostalCode:      {placemark.PostalCode}\n" +
                    //$"SubAdminArea:    {placemark.SubAdminArea}\n" +
                    //$"SubLocality:     {placemark.SubLocality}\n" +
                    //$"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                    //$"Thoroughfare:    {placemark.Thoroughfare}\n";

                    var geocodeAddress = $"Location: {placemark.Locality}, {placemark.AdminArea}";

                    //Console.WriteLine(geocodeAddress);
                    this.GeoString = geocodeAddress;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
                //return $"Feature not supported: {fnsEx}";
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
                //return $"Error: {ex}";
            }
        }


        async Task ExecuteSavePhotoCommand(PhotoBlobModel photoBlob, string photoTitle)
        {
            if (IsPhotoSaving)
                return;

            //#TODO - Uncomment - when blob storage requires authorization
            //if (string.IsNullOrWhiteSpace(BackendConstants.PostPhotoBlobFunctionKey))
            //{
            //    OnSavePhotoFailed("Invalid Azure Function Key");
            //    return;
            //}

            if (string.IsNullOrWhiteSpace(photoTitle))
            {
                OnSavePhotoFailed("Title Cannot Be Empty");
                return;
            }

            IsPhotoSaving = true;

            try
            {
                var photo = await APIService.PostPhotoBlob(photoBlob, photoTitle).ConfigureAwait(false);

                if (photo is null)
                {
                    OnSavePhotoFailed("Error Uploading Photo");
                }
                else
                {
                    await PhotoDatabase.SavePhoto(photo).ConfigureAwait(false);
                    OnSavePhotoCompleted();
                }
            }
            catch (Exception e)
            {
                OnSavePhotoFailed(e.Message);
            }
            finally
            {
                IsPhotoSaving = false;
            }
        }

        async Task ExecuteTakePhotoCommand()
        {
            var mediaFile = await GetMediaFileFromCamera().ConfigureAwait(false);

            if (mediaFile is null)
                return;

            PhotoBlob = new PhotoBlobModel
            {
                Image = ConvertStreamToByteArrary(mediaFile.GetStream())
            };

            //TODO
            this.GetGeoLocationCommand.Execute(null);
        }

        byte[] ConvertStreamToByteArrary(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        async Task<MediaFile> GetMediaFileFromCamera()
        {
            await CrossMedia.Current.Initialize().ConfigureAwait(false);

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                OnNoCameraFound();
                return null;
            }

            var mediaFileTCS = new TaskCompletionSource<MediaFile>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                mediaFileTCS.SetResult(await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    DefaultCamera = CameraDevice.Rear
                }));
            });

            return await mediaFileTCS.Task;
        }

        void UpdatePageTilte()
        {
            if (string.IsNullOrWhiteSpace(PhotoTitle))
                PageTitle = PageTitles.AddPhotoPage;
            else
                PageTitle = PhotoTitle;
        }

        void UpdatePhotoImageSource() =>
            PhotoImageSource = ImageSource.FromStream(() => new MemoryStream(PhotoBlob.Image));

        void OnSavePhotoFailed(string errorMessage) => _savePhotoFailedEventManager.HandleEvent(this, errorMessage, nameof(SavePhotoFailed));
        void OnNoCameraFound() => _noCameraFoundEventManager.HandleEvent(this, EventArgs.Empty, nameof(NoCameraFound));
        void OnSavePhotoCompleted() => _savePhotoCompletedEventManager.HandleEvent(this, EventArgs.Empty, nameof(SavePhotoCompleted));
        #endregion
    }
}
