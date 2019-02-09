using Android.OS;
using Android.App;
using Android.Runtime;
using Android.Content.PM;

using Plugin.Permissions;

using ZXing;
using ZXing.Net;
using ZXing.Net.Mobile;

//TODO - REQUIRED?
//[assembly: UsesPermission(Android.Manifest.Permission.Flashlight)]
namespace AzureBlobStorageSampleApp.Android
{
    [Activity(Label = "AzureBlobStorageSampleApp.Android", Icon = "@mipmap/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState); // add this line to your code, it may also be called: bundle

            //#TODO
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags("FastRenderers_Experimental");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            LoadApplication(new App());
        }

    }
}
