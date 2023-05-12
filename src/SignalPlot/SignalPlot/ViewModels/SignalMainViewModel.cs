using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.ViewManagement;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using ScottPlot.Control;
using ScottPlot.WinUI;
using SignalPlot.Data.Infrastructure;
using SignalPlot.Data.Infrastructure.CsvSignalManager;
using SignalPlot.Data.Models;
using SignalPlot.Services;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel : ObservableObject
{
    internal Window _window;
    private readonly ICsvSignalManager _csvSignalManager = new CsvSignalManager();
    private ISignalSourceService _signalSourceService = null; // Add service
    private readonly ISignalPlotManager _signalPlotManager;
    private readonly UISettings _uiSettings = new();
    private SignalOverviewVM _signalOverviewVm { get; init; }
    public bool IsGridVisible { get; private set; } = true;
    [ObservableProperty] private bool isDigitalVisible = false;

    // This does not work for some reason
    //[ObservableProperty] private bool notImplementedFlyoutIsOpenn;
    private bool _notImplementedFlyoutIsOpen;

    public bool NotImplementedFlyoutIsOpen
    {
        get => _notImplementedFlyoutIsOpen;
        set => SetProperty(ref _notImplementedFlyoutIsOpen, value);
    }

    public SignalMainViewModel(IPlotControl winUiPlot, IPlotControl digitalWinUiPlot, SignalOverviewVM signalOverviewVm)
    {
        _signalOverviewVm = signalOverviewVm;
        _signalPlotManager = new SignalPlotManager(winUiPlot, digitalWinUiPlot);
        // Bug: This event is not always triggered
        _uiSettings.ColorValuesChanged += ColorValuesChanged;
        _signalPlotManager.SetPlotsColor(_uiSettings.GetColorValue(UIColorType.Background));
        _signalOverviewVm.AddPropertyChangedHandler((sender, args) =>
        {
            var overviewSensorLine = (OverviewSensorLine)sender;
            if (overviewSensorLine is not null)
                switch (args.PropertyName)
                {
                    case nameof(OverviewSensorLine.Color):
                        _signalPlotManager.ChangeSignalColor(overviewSensorLine.SensorLine.Id,
                            overviewSensorLine.Color);
                        break;
                    case nameof(OverviewSensorLine.SignalSelected):
                        _signalPlotManager.ChangeSignalBoldness(overviewSensorLine.SensorLine.Id,
                            overviewSensorLine.SignalSelected);
                        break;
                    case nameof(OverviewSensorLine.SignalVisible):
                        _signalPlotManager.ToggleSignalVisibility(overviewSensorLine.SensorLine.Id);
                        break;
                    case nameof(OverviewSensorLine.Yaxis):
                        _signalPlotManager.AssignAxis(overviewSensorLine.SensorLine.Id, overviewSensorLine.Yaxis);
                        break;
                }
            else
                throw new NullReferenceException("OverviewSensorLine/sender is null");
        });

        ToggleSignalVisibilityCommand = new RelayCommand<OverviewSensorLine>(ToggleSignalVisibility);
        AssignAxisCommand = new RelayCommand<OverviewSensorLine>(AssignAxis);
        FrameSignalsCommand = new RelayCommand(FrameSignals);
        SetSignalVisibilityCommand = new RelayCommand<string>(SetSignalVisibility);
        ToggleGridCommand = new RelayCommand(ToggleGrid);
    }


    public void SetMainWindow(Window window)
    {
        if (_window is not null) throw new InvalidOperationException("_window can only be set once");
        _ = window ?? throw new ArgumentNullException(nameof(window));
        _window = window;
    }

    public void SetSignalSourceService(ISignalSourceService signalSourceService)
    {
        if (_signalSourceService is not null)
            throw new InvalidOperationException("_signalSourceService can only be set once");
        _ = signalSourceService ?? throw new ArgumentNullException(nameof(signalSourceService));
        _signalSourceService = signalSourceService;
    }

    private void ColorValuesChanged(UISettings sender, object args)
    {
        _signalPlotManager.SetPlotsColor(sender.GetColorValue(UIColorType.Background));
    }
}