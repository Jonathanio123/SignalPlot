using CommunityToolkit.Mvvm.Input;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    public IRelayCommand ToggleGridCommand { get; set; }

    private void ToggleGrid()
    {
        IsGridVisible = !IsGridVisible;
        _signalPlotManager.ToggleGridVisibility();
    }
}