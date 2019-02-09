﻿using UIKit;
using Foundation;

namespace AzureBlobStorageSampleApp.iOS
{
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            //TODO
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();


#if DEBUG
            Xamarin.Calabash.Start();
#endif
            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
