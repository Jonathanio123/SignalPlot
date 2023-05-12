using CommunityToolkit.Mvvm.Input;
using SignalPlot.Data.Models;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    public IRelayCommand<OverviewSensorLine> AssignAxisCommand { get; set; }

    private void AssignAxis(OverviewSensorLine overviewSensorLine)
    {
        _signalPlotManager.AssignAxis(overviewSensorLine.SensorLine.Id, overviewSensorLine.Yaxis);
    }
}