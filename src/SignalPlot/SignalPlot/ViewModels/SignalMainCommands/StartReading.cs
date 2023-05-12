using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Microsoft.UI;
using ScottPlot.DataSources;
using SignalPlot.Data.Infrastructure;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    private bool _autoFrame = true;

    public bool AutoFrame
    {
        get => _autoFrame;
        set => SetProperty(ref _autoFrame, value);
    }

    private bool CanStartReading()
        => !ReadFileCommand.IsRunning;


    [RelayCommand(IncludeCancelCommand = true, CanExecute = nameof(CanStartReading))]
    private async Task StartReadingAsync(CancellationToken cancellationToken = default)
    {
        if (_signalPlotManager.PlotsAreInteractive)
            _signalPlotManager.TogglePlotInteraction();

        var sourcesToRead = new List<IAnalogueSignalSource>();
        try
        {
            var sources = await _signalSourceService.ListAvailableSourcesAsync(cancellationToken);


            foreach (var source in sources)
            {
                var sensorLine = await _signalSourceService.GetAnalogueSourceAsync(source, cancellationToken);
                sourcesToRead.Add(sensorLine);
                _signalOverviewVm.AddSensorLine(new(sensorLine));
            }
        }
        catch (TaskCanceledException)
        {
            _signalOverviewVm.ClearSignals();
            sourcesToRead.ForEach(async s => await s.DisposeAsync());
            _signalPlotManager.TogglePlotInteraction();
            return;
        }


        foreach (var signalSource in sourcesToRead)
#pragma warning disable CS4014
            Task.Run(async () =>
#pragma warning restore CS4014
            {
                try
                {
                    await _signalPlotManager.AddSignalSourceAsync(signalSource, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"An exception in reader for {signalSource.SignalName}" +
                                    $"\t{e}");
                }
                finally
                {
                    await signalSource.DisposeAsync();
                }
            }, CancellationToken.None);


        // Reduces the chance of it crashing on start...
        // As the above tasks need to start before we can start refreshing
        await Task.Delay(1000, cancellationToken);


        // This is a hack to get the colors to show up in the datagrid
        foreach (var liveSignalLines in sourcesToRead)
        {
            var result = _signalPlotManager.GetLiveScatterSignal(liveSignalLines.Id);

            var signal = result.Match(_ => null,
                found => found);
            if (signal is null) continue;

            foreach (var dataGridSensorLine in _signalOverviewVm.DataGridSensorLines)
            {
                if (dataGridSensorLine.SensorLine.Id != liveSignalLines.Id ||
                    dataGridSensorLine.Color != Colors.Transparent) continue;
                ScottPlot.Color sColor = signal.LineStyle.Color;
                dataGridSensorLine.Color =
                    Windows.UI.Color.FromArgb(sColor.Alpha, sColor.Red, sColor.Green, sColor.Blue);
                break;
            }
        }

        _signalPlotManager.TogglePlotInteraction();


        // This creates a task that will refresh the plot every 100ms
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(100, CancellationToken.None);
            if (AutoFrame) _signalPlotManager.FrameAllSignals(false);
            _signalPlotManager.Refresh();
        }


        Debug.WriteLine("Stopped reading live signal");
    }
}