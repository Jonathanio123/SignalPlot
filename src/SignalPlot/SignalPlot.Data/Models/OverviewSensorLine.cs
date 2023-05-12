using Windows.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using SignalPlot.Data.Models.Interfaces;
using Windows.ApplicationModel.Email.DataProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SignalPlot.Data.Enums;

namespace SignalPlot.Data.Models;

public sealed partial class OverviewSensorLine : ObservableObject
{
    public const string DisplayNameHeader = "Display Name";
    public const string VisibleHeader = "Visible";
    public const string SignalNameHeader = "Signal";
    public const string SourceHeader = "Source";
    public const string ColorHeader = "Color";
    public const string SignalTypeHeader = "Type";
    public const string UnitHeader = "Unit";
    public const string AxisHeader = "Axis";
    [ObservableProperty] private string yaxis = "Primary";
    [ObservableProperty] private bool signalSelected;
    [ObservableProperty] private bool signalVisible;
    [ObservableProperty] private Color color = Colors.Transparent;
    public IBaseSensorLine SensorLine { get; set; }

    public bool IsAnalogue => SensorLine.SignalType == SignalType.Analog;

    public OverviewSensorLine(IBaseSensorLine sensorLine, Color color)
    {
        SignalSelected = false;
        SignalVisible = true;
        SensorLine = sensorLine;
        Color = color;
    }

    public OverviewSensorLine(IBaseSensorLine sensorLine)
    {
        SignalSelected = false;
        SignalVisible = true;
        SensorLine = sensorLine;
    }


    public OverviewSensorLine()
    {
        SignalVisible = false;
        SensorLine = null;
    }
}