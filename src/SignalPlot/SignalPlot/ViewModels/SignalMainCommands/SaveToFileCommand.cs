using CommunityToolkit.Mvvm.Input;
using SignalPlot.Views;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{

    [RelayCommand]
    public void SaveToFile()
    {
        NotImplementedFlyoutIsOpen = true;
    }
}