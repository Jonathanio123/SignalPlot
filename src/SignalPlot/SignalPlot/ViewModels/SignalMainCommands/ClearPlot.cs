using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    private bool CanClearPlot()
    {
        return !StartReadingCommand.IsRunning && !ReadFileCommand.IsRunning;
    }
    [RelayCommand(CanExecute = nameof(CanClearPlot))]
    private void ClearPlot()
    {
        _signalPlotManager.ClearSignals();
        _signalOverviewVm.ClearSignals();
    }
}