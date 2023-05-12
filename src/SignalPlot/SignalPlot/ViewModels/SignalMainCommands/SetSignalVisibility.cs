using CommunityToolkit.Mvvm.Input;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    public IRelayCommand<string> SetSignalVisibilityCommand { get; private set; }

    private void SetSignalVisibility(string showSignalString)
    {
        var showSignal = bool.Parse(showSignalString);
        foreach (var sensorLine in _signalOverviewVm.OverviewSensorLines)
        {
            sensorLine.SignalVisible = showSignal;
            _signalPlotManager.SetSignalVisibility(sensorLine.SensorLine.Id, showSignal);
        }
    }
}