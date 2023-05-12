using CommunityToolkit.Mvvm.Input;
using SignalPlot.Data.Models;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    public IRelayCommand<OverviewSensorLine> ToggleSignalVisibilityCommand { get; set; }

    private void ToggleSignalVisibility(OverviewSensorLine overviewSensorLine)
    {
        _signalPlotManager.ToggleSignalVisibility(overviewSensorLine.SensorLine.Id);
    }
}