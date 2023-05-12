using Microsoft.UI.Xaml.Controls;

namespace SignalPlot.Views;

public sealed partial class SignalsSelectorDialog : ContentDialog
{
    public SignalsSelectorDialog()
    {
        InitializeComponent();
        Width = 1200;
        Height = 1200;
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
    }
}