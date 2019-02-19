using System;

using Xamarin.Forms;

namespace AzureBlobStorageSampleApp
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !((bool)value);

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !((bool)value);
    }

    public class AddBarcodeWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var barcode = (string)value;
            return $"Barcode: {barcode}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddCaptionWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Caption: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddColorWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Colors: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddObjectDescriptionWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Object description: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddTagsWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Tags: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }

    public class AddCustomVisionWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            if (value == null)
                return string.Empty;
            var valueWord = (string)value;
            return $"Custom vision tags: {valueWord}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { 
            throw new NotImplementedException();
        }
    }
}


