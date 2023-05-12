using System;
using System.Runtime.CompilerServices;
using ScottPlot;
using SignalPlot.Data.Models.Interfaces;

namespace SignalPlot.Customs;

public class PlottableId
{
    public PlottableId()
    {
        Value = Guid.Empty;
    }

    public PlottableId(Guid id)
    {
        Value = id;
    }

    public Guid Value { get; }

    public override string ToString()
    {
        return Value.ToString();
    }

    public bool Equals(PlottableId other)
    {
        return other is not null && Value.Equals(other.Value);
    }

    public override int GetHashCode() => Value.GetHashCode();
}

public static class PlottableExtensions
{
    private static readonly ConditionalWeakTable<IPlottable, PlottableId> _data = new();

    public static PlottableId GetId(this IPlottable plottable)
    {
        return _data.TryGetValue(plottable, out var id) ? id : new PlottableId();
    }

    public static void SetId(this IPlottable plottable, Guid id)
    {
        if (!_data.TryAdd(plottable, new PlottableId(id)))
            throw new InvalidOperationException("Duplicate Id");
    }
}