using System;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

using Plugin.Media;
using Plugin.Media.Abstractions;

using AzureBlobStorageSampleApp.Shared;
using AzureBlobStorageSampleApp.Mobile.Shared;
using AzureBlobStorageSampleApp.Services;



using AsyncAwaitBestPractices.MVVM;
using AsyncAwaitBestPractices;
using Xamarin.Essentials;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using ZXing;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
//using ScannerHelperLibrary;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Cognitive.CustomVision.Prediction.Models;
using Microsoft.Cognitive.CustomVision.Prediction;

namespace AzureBlobStorageSampleApp
{
    public class AddPhotoViewModel : BaseViewModel
    {
        #region Constant Fields
        readonly WeakEventManager _noCameraFoundEventManager = new WeakEventManager();
        readonly WeakEventManager _savePhotoCompletedEventManager = new WeakEventManager();
        readonly WeakEventManager<string> _savePhotoFailedEventManager = new WeakEventManager<string>();
        readonly WeakEventManager _noCameraPickerFoundEventManager = new WeakEventManager();

        #endregion

        #region Fields
        ICommand _savePhotoCommand, _takePhotoCommand, _takeScanCommand;
        ICommand _getGeoLocationCommand;
        ICommand _getPhotoCommand;

        string _photoTitle, _pageTitle = PageTitles.AddPhotoPage;
        bool _isPhotoSaving;
        ImageSource _photoImageSource;
        PhotoBlobModel _photoBlob;
        ImageAnalysis analysis;
        #endregion

        #region Events
        public event EventHandler NoCameraFound
        {
            add => _noCameraFoundEventManager.AddEventHandler(value);
            remove => _noCameraFoundEventManager.RemoveEventHandler(value);
        }

        public event EventHandler NoCameraPickerFound
        {
            add => _noCameraPickerFoundEventManager.AddEventHandler(value);
            remove => _noCameraPickerFoundEventManager.RemoveEventHandler(value);
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

        public ICommand GetPhotoCommand => _getPhotoCommand ??
            (_getPhotoCommand = new AsyncCommand(ExecuteGetPhotoCommand, continueOnCapturedContext: false));

        public ICommand TakeScanCommand => _takeScanCommand ??
            (_takeScanCommand = new AsyncCommand(ExecuteTakeScanCommand, continueOnCapturedContext: false));


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


        string _barcodeString;
        public string BarcodeString
        {
            get => _barcodeString;
            set => SetProperty(ref _barcodeString, value, UpdatePageTilte);
        }

        string _descriptionCaptionOfImage;
        public string DescriptionCaptionOfImage
        {
            get => _descriptionCaptionOfImage;
            set => SetProperty(ref _descriptionCaptionOfImage, value, UpdatePageTilte);
        }

        string _tagsCombinedString;
        public string TagsCombinedString
        {
            get => _tagsCombinedString;
            set => SetProperty(ref _tagsCombinedString, value, UpdatePageTilte);
        }

        string _foregroundColor;
        public string ForegroundColor
        {
            get => _foregroundColor;
            set => SetProperty(ref _foregroundColor, value);
        }

       string _colorsCombinedString;
        public string ColorsCombinedString
        {
            get => _colorsCombinedString;
            set => SetProperty(ref _colorsCombinedString, value, UpdatePageTilte);
        }

        string _objectDescription;
        public string ObjectDescription
        {
            get => _objectDescription;
            set => SetProperty(ref _objectDescription, value, UpdatePageTilte);
        }

        List<string> _tagsListOfStrings;
        public List<string> TagsListOfStrings
        {
            get => _tagsListOfStrings;
            set => SetProperty(ref _tagsListOfStrings, value, UpdatePageTilte);
        }

        List<string> _colorsListOfStrings;
        public List<string> ColorsListOfStrings
        {
            get => _colorsListOfStrings;
            set => SetProperty(ref _colorsListOfStrings, value);
        }

        string _customVisionTagsCombinedString;
        public string CustomVisionTagsCombinedString
        {
            get => _customVisionTagsCombinedString;
            set => SetProperty(ref _customVisionTagsCombinedString, value, UpdatePageTilte);
        }

        Xamarin.Forms.Color _blueColor;
        public Xamarin.Forms.Color BlueColor
        {
            get => _blueColor;
            set => SetProperty(ref _blueColor, value);
        }

        Xamarin.Forms.Color _switchTrueColor;
        public Xamarin.Forms.Color SwitchTrueColor
        {
            get => _switchTrueColor;
            set => SetProperty(ref _switchTrueColor, value);
        }

        bool _isBarcode;
        public bool IsBarcode
        {
            get => _isBarcode;
            set => SetProperty(ref _isBarcode, value);
        }

        bool _isComputerVision;
        public bool IsComputerVision
        {
            get => _isComputerVision;
            set => SetProperty(ref _isComputerVision, value);
        }

        bool _isCustomVision;
        public bool IsCustomVision
        {
            get => _isCustomVision;
            set => SetProperty(ref _isCustomVision, value);
        } 

        bool _isPhotoGallery;
        public bool IsPhotoGallery
        {
            get => _isPhotoGallery;
            set => SetProperty(ref _isPhotoGallery, value);
        }  

        DateTimeOffset _photoCreatedDateTime;
        public DateTimeOffset PhotoCreatedDateTime
        {
            get => _photoCreatedDateTime;
            set => SetProperty(ref _photoCreatedDateTime, value);
        }  

        Xamarin.Essentials.Location _location;

        #endregion

        #region Methods

        //async Task ExecuteGetGeoLocationCommand()
        //{

        //    try
        //    {

        //            try
        //            {

        //                //Device.BeginInvokeOnMainThread(async () =>
        //                //{
        //                //    _location = await Geolocation.GetLastKnownLocationAsync();
        //                //});


        //                Device.BeginInvokeOnMainThread(() =>
        //                {
        //                    _location = Geolocation.GetLastKnownLocationAsync();
        //                });
        //                //var location1 = await Geolocation.GetLastKnownLocationAsync();
        //                //LastLocation = FormatLocation(location);'

        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"error: {ex}");
        //            }

        //       //var location = await Geolocation.GetLastKnownLocationAsync();
        //        //var location = await Geolocation.GetLocationAsync();   

        //        if (_location != null)
        //        {
        //            Console.WriteLine($"Latitude: {_location.Latitude}, Longitude: {_location.Longitude}, Altitude: {_location.Altitude}");
        //        } else
        //        {
        //            Console.WriteLine($"Exiting geolocation");
        //            //return;
        //        }

        //        //this.GeoString = "Paramus, NJ";

        //        //var lat = 47.673988;
        //        //var lon = -122.121513;

        //        //var placemarks = Task.Run(async () => await Geocoding.GetPlacemarksAsync(lat, lon)).Result;

        //        var lat = 47.673988;
        //        var lon = -122.121513;
            
        //        var placemarks = Task.Run(async () => await Geocoding.GetPlacemarksAsync(_location.Latitude, _location.Longitude)).Result;


        //        var placemark = placemarks?.FirstOrDefault();

        //        if (placemark != null)
        //        {
        //            //var geocodeAddress =
        //            //$"AdminArea:       {placemark.AdminArea}\n" +
        //            //$"CountryCode:     {placemark.CountryCode}\n" +
        //            //$"CountryName:     {placemark.CountryName}\n" +
        //            //$"FeatureName:     {placemark.FeatureName}\n" +
        //            //$"Locality:        {placemark.Locality}\n" +
        //            //$"PostalCode:      {placemark.PostalCode}\n" +
        //            //$"SubAdminArea:    {placemark.SubAdminArea}\n" +
        //            //$"SubLocality:     {placemark.SubLocality}\n" +
        //            //$"SubThoroughfare: {placemark.SubThoroughfare}\n" +
        //            //$"Thoroughfare:    {placemark.Thoroughfare}\n";

        //            var geocodeAddress = $"Location: {placemark.Locality}, {placemark.AdminArea}";

        //            //Console.WriteLine(geocodeAddress);
        //            this.GeoString = geocodeAddress;
        //        }
        //    }
        //    catch (FeatureNotSupportedException fnsEx)
        //    {
        //        // Feature not supported on device
        //        //return $"Feature not supported: {fnsEx}";
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exception that may have occurred in geocoding
        //        //return $"Error: {ex}";
        //    }

        //}

        async Task StampDateTime()
        {

            try
            {
                this.PhotoCreatedDateTime = DateTimeOffset.UtcNow;
            }
            catch (Exception ex)
            {

            }
        }


            async Task ExecuteGetGeoLocationCommand()
        {

            try
            {
                ////MOVE TO SEPERATE COMMAND
                //await this.StampDateTime();

                var locationFromPhone = await GetLocationFromPhone().ConfigureAwait(false);

                if (locationFromPhone is null)
                    return;

                _location = locationFromPhone;

                if (_location != null)
                {
                    Console.WriteLine($"Latitude: {_location.Latitude}, Longitude: {_location.Longitude}, Altitude: {_location.Altitude}");
                } else
                {
                    Console.WriteLine($"Exiting geolocation");
                }

                var lat = 47.673988;
                var lon = -122.121513;
            
                var placemarks = Task.Run(async () => await Geocoding.GetPlacemarksAsync(_location.Latitude, _location.Longitude)).Result;


                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    var geocodeAddress = $"Location: {placemark.Locality}, {placemark.AdminArea}";
                    this.GeoString = geocodeAddress;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                //return await $"Error: {fnsEx}";
            }
            catch (Exception ex)
            {
                //return await $"Error: {ex}";
            }
        }


        async Task<Xamarin.Essentials.Location> GetLocationFromPhone()
        {

            var locationTaskCompletionSource = new TaskCompletionSource<Xamarin.Essentials.Location>();
            Device.BeginInvokeOnMainThread( async () =>
                {
                    locationTaskCompletionSource.SetResult(await Geolocation.GetLastKnownLocationAsync());
                }   
            );

            return await locationTaskCompletionSource.Task;
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

        //BarcodeDecoding barcode;
        async Task ExecuteGetPhotoCommand()
        {
            await this.StampDateTime();
             
            var mediaFile = await GetStoredMediaFileFromCamera().ConfigureAwait(false);

            if (mediaFile is null)
                return;

            var tempByteArray = ConvertStreamToByteArrary(mediaFile.GetStream());

            PhotoBlob = new PhotoBlobModel
            {
                Image = ConvertStreamToByteArrary(mediaFile.GetStream())
            };


            if (IsComputerVision || IsCustomVision)
            {
                //TODO
                this.GetGeoLocationCommand.Execute(null);
            }

            if (IsComputerVision)
            {
                //COMPUTER VISION
                IList<VisualFeatureTypes> visFeatures = new List<VisualFeatureTypes>() {
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Color, VisualFeatureTypes.Categories, VisualFeatureTypes.Color, VisualFeatureTypes.Faces, VisualFeatureTypes.Objects, VisualFeatureTypes.ImageType, VisualFeatureTypes.Description
                };
     
                //TODO
                var client = new ComputerVisionService();
                using (var photoStream = mediaFile.GetStream())
                {
                    //ImageAnalysis analysis = client.AnalyzeImageAsync(photoStream);
                    //ImageAnalysis analysis = await client.computerVisionClient.AnalyzeImageInStreamAsync(photoStream);    //AnalyzeImageInStreamAsync(photoStream);

                    analysis = await client.computerVisionClient.AnalyzeImageInStreamAsync(photoStream, visFeatures);                                                                                                //DisplayResults (analysis, photoStream);
                    DisplayResults(analysis);
                }
            }
        
            if (IsCustomVision)
            {
               //CUSTOM VISION
                var tagList = this.GetBestTagList(mediaFile);
                DisplayCustomVisionResults(tagList);
            }
        }



        async Task ExecuteTakePhotoCommand()
        {
            await this.StampDateTime();

            //DELETE
            //var mediaFile = await GetStoredMediaFileFromCamera().ConfigureAwait(false);

            var mediaFile = await GetMediaFileFromCamera().ConfigureAwait(false);

            if (mediaFile is null)
                return;

            var tempByteArray = ConvertStreamToByteArrary(mediaFile.GetStream());

            PhotoBlob = new PhotoBlobModel
            {
                Image = ConvertStreamToByteArrary(mediaFile.GetStream())
            };


            if (IsComputerVision || IsCustomVision)
            {
                //TODO
                this.GetGeoLocationCommand.Execute(null);
            }

            if (IsComputerVision)
            {
                //COMPUTER VISION
                IList<VisualFeatureTypes> visFeatures = new List<VisualFeatureTypes>() {
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Color, VisualFeatureTypes.Categories, VisualFeatureTypes.Color, VisualFeatureTypes.Faces, VisualFeatureTypes.Objects, VisualFeatureTypes.ImageType, VisualFeatureTypes.Description
                };
     
                //TODO
                var client = new ComputerVisionService();
                using (var photoStream = mediaFile.GetStream())
                {
                    //ImageAnalysis analysis = client.AnalyzeImageAsync(photoStream);
                    //ImageAnalysis analysis = await client.computerVisionClient.AnalyzeImageInStreamAsync(photoStream);    //AnalyzeImageInStreamAsync(photoStream);

                    analysis = await client.computerVisionClient.AnalyzeImageInStreamAsync(photoStream, visFeatures);                                                                                                //DisplayResults (analysis, photoStream);
                    DisplayResults(analysis);
                }
            }
        
            if (IsCustomVision)
            {
               //CUSTOM VISION
                var tagList = this.GetBestTagList(mediaFile);
                DisplayCustomVisionResults(tagList);
            }

            //TODO
            //var barcodeScannerService = new BarcodeScannerServiceLib();
            //var stringBarcode = barcodeScannerService.JustDecodeBarcode(PhotoBlob.Image);
            //var stringBarcode = barcodeScannerService.DecodeBarcode(PhotoBlob.Image);

            //var barcodeScannerService = new BarcodeScannerService();
            //var byteArray = this.DoConvertMediaFileToByteArray(mediaFile);
            //var stringBarcode = barcodeScannerService.DecodeBarcode(byteArray);


            //System.Drawing.Bitmap(filename);

            //int hi = 5;

            //barcode = new BarcodeDecoding();

            //var aditionalHints = new KeyValuePair<DecodeHintType, object>(key: DecodeHintType.PURE_BARCODE, value: "TRUE");

            //var result = barcode.Decode(file: "image_to_read", format: BarcodeFormat.QR_CODE, aditionalHints: new[] { aditionalHints });

            //Label to show the text decoded
            //QrResult.Text = result.Text;

            //var qrRest = result.Text;

        }

        // Display the most relevant caption for the image
        private void DisplayResults(ImageAnalysis analysis)
        {
            Console.WriteLine("Test image 1");
            //Console.WriteLine(analysis.Description.Captions[0].Text + "\n");

            this.DescriptionCaptionOfImage = analysis.Description.Captions.FirstOrDefault()?.Text ?? "";

            this.ForegroundColor = analysis.Color?.DominantColorForeground ?? ""; //.FirstOrDefault()?.Text ?? "";

            //this.ColorsListOfStrings = analysis.Color.Select(t => t.DominantColors).ToList();
            this.ColorsListOfStrings = analysis.Color.DominantColors.ToList();


            var newStringBuilder1 = new StringBuilder();

            //foreach (var metaData in result.ResultMetadata)

            //foreach (var colorName in analysis.Tags.Select(t => t.Name))
            foreach (var colorName in analysis.Color.DominantColors.Select(x=>x.ToLower()).ToList())
            {
                newStringBuilder1.Append($"#{colorName} ");
            }

            var combinedTagString1 = newStringBuilder1.ToString();
            var trimCombinedString1 = combinedTagString1.Trim();

            this.ColorsCombinedString = trimCombinedString1;




            this.ObjectDescription = analysis.Objects.FirstOrDefault()?.ObjectProperty ?? "";  //.Text ?? "";

            this.TagsListOfStrings = analysis.Tags.Select(t => t.Name).ToList();
            //this.TagsCombinedString = analysis.Tags.Select(t => t.Name).ToString();

            //TagsCombinedString
            var newStringBuilder = new StringBuilder();

            //foreach (var metaData in result.ResultMetadata)

            foreach (var tagName in analysis.Tags.Select(t => t.Name))
            {
                newStringBuilder.Append($"#{tagName} ");
            }

            var combinedTagString = newStringBuilder.ToString();
            var trimCombinedString = combinedTagString.Trim();

            this.TagsCombinedString = trimCombinedString;

            //TagsListOfStrings
        }

        // Display the most relevant caption for the image
        private void DisplayCustomVisionResults(IEnumerable<ImageTagPredictionModel> tagList)
        {
            StringBuilder stringOfTags = new StringBuilder();

            if (tagList == null)
                stringOfTags.Append($"No tags found");
            else
                //stringOfTags.Append($"Custom tags: ");
            foreach (var tagItem in tagList)
            {
                stringOfTags.Append($"#{tagItem.Tag} ");
                //Console.WriteLine($"\t{c.TagName}: {c.Probability:P1}");
            }

            var combinedTagString = stringOfTags.ToString();
            var trimCombinedString = combinedTagString.Trim();

            this.CustomVisionTagsCombinedString = trimCombinedString;

        }

        private IEnumerable<ImageTagPredictionModel> GetBestTagList(MediaFile file)
        {
            using (var stream = file.GetStream())
            {
                //var predictImagePredictions = _endpoint.PredictImage(CustomVisionService.ProjectId, stream).Predictions;

                var _endpoint = new CustomVisionService()._endpoint;
                var predictImagePredictions = _endpoint.PredictImage(CustomVisionService.ProjectId, stream).Predictions;
                var orderedPredictions = predictImagePredictions.OrderByDescending(p => p.Probability).Where(p => p.Probability > CustomVisionService.ProbabilityThreshold);

                return orderedPredictions;
            }
        }


        private Task ExecuteTakeScanCommand()
        {
            //    var customScanPage = new CustomScanPage();
            //    //    await Navigation.PushAsync(customScanPage);
            throw new NotImplementedException();
        }


        byte[] ConvertStreamToByteArrary(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        //https://stackoverflow.com/questions/33947138/convert-image-into-byte-array-in-xamarin-forms
        byte[] DoConvertMediaFileToByteArray(MediaFile mediaFile)
        {

            byte[] imageByte;
            Stream imageStream = null;

            //var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            //{ Name = "pic.jpg" });
            //if (file == null) return;

            imageStream = mediaFile.GetStream();
            BinaryReader br = new BinaryReader(imageStream);
            return imageByte = br.ReadBytes((int)imageStream.Length);

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

        async Task<MediaFile> GetStoredMediaFileFromCamera()
        {
            await CrossMedia.Current.Initialize().ConfigureAwait(false);

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                //OnNoCameraFound();
                OnNoCameraPickerFound();
                return null;
            }

            var mediaFileTCS = new TaskCompletionSource<MediaFile>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                mediaFileTCS.SetResult(await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    //DefaultCamera = CameraDevice.Rear
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
        void OnNoCameraPickerFound() => _noCameraPickerFoundEventManager.HandleEvent(this, EventArgs.Empty, nameof(OnNoCameraPickerFound));
        void OnSavePhotoCompleted() => _savePhotoCompletedEventManager.HandleEvent(this, EventArgs.Empty, nameof(SavePhotoCompleted));
        #endregion
    }
}
