using System;
using Windows.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace SignalPlot.Converters;

// https://stackoverflow.com/a/3309904
public class ColorToSolidColorBrushValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (null == value) return null;

        // For a more sophisticated converter, check also the targetType and react accordingly..
        if (value is Color)
            return new SolidColorBrush((Color)value);
        // You can support here more source types if you wish
        // For the example I throw an exception

        Type type = value.GetType();
        throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
            return null;

        if (value is SolidColorBrush)
            return ((SolidColorBrush)value).Color;

        throw new InvalidOperationException("Unsupported type [" + value.GetType().Name +
                                            "], ColorToSolidColorBrushValueConverter.ConvertBack()");
    }
}