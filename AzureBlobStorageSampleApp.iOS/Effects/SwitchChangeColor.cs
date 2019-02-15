using AzureBlobStorageSampleApp.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using AzureBlobStorageSampleApp.Effects.iOS;

using PlatformEffects = AzureBlobStorageSampleApp.Effects.iOS;
using RoutingEffects = AzureBlobStorageSampleApp.Effects;


//[assembly: ExportEffect(typeof(PlatformEffects.SwitchChangeColor), nameof(RoutingEffects.SwitchChangeColorEffect))]
using System.Runtime.CompilerServices;


[assembly: ResolutionGroupName("MyCompany")]
//[assembly: ExportEffect(typeof(PlatformEffects.SwitchChangeColor), "SwitchChangeColor")]
[assembly: ExportEffect(typeof(PlatformEffects.SwitchChangeColor), nameof(RoutingEffects.SwitchChangeColorEffect))]
namespace AzureBlobStorageSampleApp.Effects.iOS
{
    public class SwitchChangeColor : PlatformEffect
    {
        Color trueColor;
        Color falseColor;

        protected override void OnAttached()
        {
            var uiSwitch = Control as UISwitch;
            if (uiSwitch == null)
                return;

            trueColor = (Color)Element.GetValue(RoutingEffects.SwitchChangeColor.TrueColorProperty);
            falseColor = (Color)Element.GetValue(RoutingEffects.SwitchChangeColor.FalseColorProperty);

            //trueColor = Color.FromHex("E3553D");
            //falseColor = Color.White;

            if (falseColor != Color.Transparent)
            {
                uiSwitch.TintColor = falseColor.ToUIColor();
                uiSwitch.Layer.CornerRadius = 16;
                uiSwitch.BackgroundColor = falseColor.ToUIColor();
            }

            if (trueColor != Color.Transparent)
                uiSwitch.OnTintColor = trueColor.ToUIColor();
        }

        protected override void OnDetached()
        {
        }
    }
}