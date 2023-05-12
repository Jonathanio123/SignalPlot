using System;
using System.Collections.Generic;
using System.Linq;
using SignalPlot.Data.Enums;
using SignalPlot.Data.Models.Interfaces;

namespace SignalPlot.Data.Models;

public sealed class DigitalSensorLine : BaseSensorLine, IDigitalSensorLine
{
    public IReadOnlyList<IDataEntry<bool>> DataEntries => _dataEntries.AsReadOnly();
    private readonly List<IDataEntry<bool>> _dataEntries = new();

    public DigitalSensorLine(string displayName, string signalName, string source, string unit, SignalType signalType,
        Guid id = default)
    {
        Id = Guid.Empty == id ? Guid.NewGuid() : id;
        DisplayName = displayName;
        SignalName = signalName;
        Source = source;
        Unit = unit;
        SignalType = signalType;
    }

    public void AddEntry(IDataEntry<bool> dataEntry)
    {
        _dataEntries.Add(dataEntry);
    }
}

public sealed class AnalogueSensorLine : BaseSensorLine, IAnalogueSensorLine
{
    public IReadOnlyList<IDataEntry<double>> DataEntries => _dataEntries.AsReadOnly();
    private readonly List<IDataEntry<double>> _dataEntries = new();

    private bool _maxValueInitialized;
    private double _maxValue;

    public double MaxValue
    {
        get
        {
            if (_maxValueInitialized) return _maxValue;

            _maxValue = DataEntries.Max(x => x.Value);
            _maxValueInitialized = true;
            return _maxValue;
        }
    }

    public AnalogueSensorLine(string displayName, string signalName, string source, string unit, SignalType signalType,
        Guid id = default)
    {
        Id = Guid.Empty == id ? Guid.NewGuid() : id;
        DisplayName = displayName;
        SignalName = signalName;
        Source = source;
        Unit = unit;
        SignalType = signalType;
    }

    public void AddEntry(IDataEntry<double> dataEntry)
    {
        _dataEntries.Add(dataEntry);
        _maxValueInitialized = false;
    }
}

public abstract class BaseSensorLine : IBaseSensorLine
{
    public Guid Id { get; init; } = Guid.Empty;
    public string DisplayName { get; init; } = String.Empty;
    public string SignalName { get; init; } = String.Empty;
    public string Source { get; init; } = String.Empty;
    public string Unit { get; init; } = String.Empty;

    public SignalType SignalType { get; init; } = SignalType.NotSett;
}