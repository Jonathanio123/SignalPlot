using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SignalPlot.Views;

public sealed partial class NotImplementedFlyout : UserControl
{
    public NotImplementedFlyout()
    {
        this.InitializeComponent();
    }

    internal static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(NotImplementedFlyout), new PropertyMetadata("Not implemented"));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    internal static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description), typeof(string), typeof(NotImplementedFlyout), new PropertyMetadata("This feature is not implemented yet."));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    internal static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen), typeof(bool), typeof(NotImplementedFlyout), new PropertyMetadata(default(bool)));

    public bool IsOpen
    {
        get { return (bool)GetValue(IsOpenProperty); }
        set { SetValue(IsOpenProperty, value); }
    }
}