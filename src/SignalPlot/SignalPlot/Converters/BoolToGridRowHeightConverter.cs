using System;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SignalPlot.Converters;

internal class BoolToGridRowHeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not bool boolValue)
            throw new ArgumentException("Value must be of type bool");


        if (!boolValue) return new GridLength(0);

        double length = parameter switch
        {
            null => 1,
            string gridLength => double.TryParse(gridLength, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double doubleValueResult) ? doubleValueResult : 0,
            _ => throw new ArgumentException("Parameter must be of type double or string")
        };

        return new GridLength(length, GridUnitType.Star);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return null;
    }
}