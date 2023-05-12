using System.Collections.Generic;
using System.Collections.ObjectModel;
using SignalPlot.Data.Models;

namespace SignalPlot.Data.Infrastructure.CsvSignalManager;

public partial class CsvSignalManager : ICsvSignalManager
{
    private static readonly ObservableCollection<AnalogueSensorLine> _analogueSensorLines = new();
    private static readonly ObservableCollection<DigitalSensorLine> _binarySensorLines = new();

    public IReadOnlyCollection<AnalogueSensorLine> GetAnalogueSensorLines() => _analogueSensorLines.AsReadOnly();

    public IReadOnlyCollection<DigitalSensorLine> GetBoolSensorLines() => _binarySensorLines.AsReadOnly();
}