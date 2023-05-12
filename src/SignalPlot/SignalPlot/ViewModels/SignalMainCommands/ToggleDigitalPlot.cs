using CommunityToolkit.Mvvm.Input;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{

    [RelayCommand]
    private void ToggleDigitalPlot()
    {
        IsDigitalVisible = !IsDigitalVisible;
    }
}

