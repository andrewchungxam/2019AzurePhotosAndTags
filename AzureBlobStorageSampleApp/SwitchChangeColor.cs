﻿using System;
using System.Linq;
using Xamarin.Forms;
using RoutingEffects = AzureBlobStorageSampleApp.Effects;


namespace AzureBlobStorageSampleApp.Effects
{
    public static class SwitchChangeColor
    {
        public static readonly BindableProperty FalseColorProperty = BindableProperty.CreateAttached("FalseColor", typeof(Color), typeof(SwitchChangeColor), Color.Transparent, propertyChanged: OnColorChanged);
        public static readonly BindableProperty TrueColorProperty = BindableProperty.CreateAttached("TrueColor", typeof(Color), typeof(SwitchChangeColor), Color.Transparent, propertyChanged: OnColorChanged);

        static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as Switch;
            if (control == null)
                return;

            var color = (Color)newValue;

            var attachedEffect = control.Effects.FirstOrDefault(e => e is SwitchChangeColorEffect);
            if (color != Color.Transparent && attachedEffect == null)
                control.Effects.Add(new SwitchChangeColorEffect());
            else if (color == Color.Transparent && attachedEffect != null)
                control.Effects.Remove(attachedEffect);
        }

        public static Color GetFalseColor(BindableObject view) =>
            (Color)view.GetValue(FalseColorProperty);

        public static void SetFalseColor(BindableObject view, string color) =>
            view.SetValue(FalseColorProperty, color);

        public static Color GetTrueColor(BindableObject view) =>
            (Color)view.GetValue(TrueColorProperty);

        public static void SetTrueColor(BindableObject view, Color color) =>
            view.SetValue(TrueColorProperty, color);
    }

    public class SwitchChangeColorEffect : RoutingEffect
    {
        //public SwitchChangeColorEffect()
        //    : base(EffectIds.SwitchChangeColor)

        //{
        //}

        //RoutingEffects.SwitchChangeColorEffect

        public SwitchChangeColorEffect() : base("MyCompany.SwitchChangeColor")
        {
        }
    }
}
