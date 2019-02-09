using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;

using AzureBlobStorageSampleApp;
using AzureBlobStorageSampleApp.Android;
using Xamarin.Forms;

using ZXing;
using ZXing.Common;
using ZXing.Net;
using ZXing.Net.Mobile;


[assembly: Xamarin.Forms.Dependency(typeof(ImageHelper))]
//namespace AzureBlobStorageSampleApp.Android.InterfaceImplementation
namespace AzureBlobStorageSampleApp.Android
{
    public class ImageHelper : IImageHelper
    {
        Context context;

        public ImageHelper()
        {
            context = Xamarin.Forms.Forms.Context;
        }

        public BinaryBitmap GetBinaryBitmap(string imageName)
        {
            throw new NotImplementedException();
        }

        //public BinaryBitmap GetBinaryBitmap(byte[] image)
        public BinaryBitmap GetBinaryBitmap()
        {
            //uncomment the line below to use the image that is passed instead of a raw image.
            //Bitmap bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);

            //Bitmap bitmap = BitmapFactory.DecodeStream(context.Resources.OpenRawResource(global::Android.Resource.Raw.static_qr_code_without_logo));
            Bitmap bitmap = BitmapFactory.DecodeStream(context.Resources.Assets.Open("static_qr_code_without_logo"));

            //Bitmap bitmap = BitmapFactory.DecodeStream(context.Resources.OpenRawResource(Uri.("));

            byte[] rgbBytes = GetRgbBytes(bitmap);
            var bin = new HybridBinarizer(new RGBLuminanceSource(rgbBytes, bitmap.Width, bitmap.Height));
            var i = new BinaryBitmap(bin);

            return i;
        }

        public BinaryBitmap GetBinaryBitmap(byte[] image)
        {
            //uncomment the line below to use the image that is passed instead of a raw image.
            //Bitmap bitmap = BitmapFactory.DecodeByteArray(image, 0, image.Length);

            //Bitmap bitmap = BitmapFactory.DecodeStream(context.Resources.OpenRawResource(global::Android.Resource.Raw.static_qr_code_without_logo));

            Bitmap bitmap = BitmapFactory.DecodeStream(context.Resources.Assets.Open("static_qr_code_without_logo"));

            //Bitmap bitmap = BitmapFactory.DecodeStream(context.Resources.OpenRawResource(Uri.("));

            byte[] rgbBytes = GetRgbBytes(bitmap);
            var bin = new HybridBinarizer(new RGBLuminanceSource(rgbBytes, bitmap.Width, bitmap.Height));
            var i = new BinaryBitmap(bin);

            return i;
        }

        private byte[] GetRgbBytes(Bitmap image)
        {
            var rgbBytes = new List<byte>();
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    //https://forums.xamarin.com/discussion/69869/android-content-and-android-graphics-missing-an-assembly-reference
                    //works with global - does not work without it
                    var c = new global::Android.Graphics.Color(image.GetPixel(x, y));

                    rgbBytes.AddRange(new[] { c.R, c.G, c.B });
                }
            }
            return rgbBytes.ToArray();
        }
    }
}
