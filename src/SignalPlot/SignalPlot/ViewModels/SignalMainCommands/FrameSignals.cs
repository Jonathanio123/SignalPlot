using CommunityToolkit.Mvvm.Input;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    public IRelayCommand FrameSignalsCommand { get; set; }

    private void FrameSignals()
    {
        _signalPlotManager.FrameAllSignals();
    }
}