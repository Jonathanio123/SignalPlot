using System;

namespace SignalPlot.Data.Models.Interfaces;

public interface IDataEntry<out T>
{
    public DateTime Timestamp { get; }
    public T Value { get; }
}