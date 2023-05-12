using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalPlot.Data.Models.Interfaces;

public interface IAnalogueSensorLine : IBaseSensorLine
{
    IReadOnlyList<IDataEntry<double>> DataEntries { get; }
    double MaxValue { get; }
    void AddEntry(IDataEntry<double> dataEntry);
}

public interface IDigitalSensorLine : IBaseSensorLine
{
    IReadOnlyList<IDataEntry<bool>> DataEntries { get; }
    void AddEntry(IDataEntry<bool> dataEntry);
}