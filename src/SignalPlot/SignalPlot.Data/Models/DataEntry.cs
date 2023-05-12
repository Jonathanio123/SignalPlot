using System;
using System.Diagnostics;
using SignalPlot.Data.Models.Interfaces;

namespace SignalPlot.Data.Models;

public sealed record DataEntry<T> : IDataEntry<T>
{
    public DateTime Timestamp { get; }
    public T Value { get; }

    /// <summary>
    /// T must be bool or double.
    /// </summary>
    public DataEntry(DateTime timestamp, T value)
    {
        if (typeof(T) != typeof(bool) && typeof(T) != typeof(double))
            throw new ArgumentException("Type must be bool or double");

        Timestamp = timestamp;
        Value = value;
    }

    public override string ToString()
    {
        return $"TimeStamp: {Timestamp} | Value: {Value}";
    }

    public bool Equals(DataEntry<T> other)
    {
        if (other is null)
            return false;


        if (ReferenceEquals(this, other))
            return true;

        if (this.GetType() != other.GetType())
        {
            return false;
        }

        return (Timestamp.Equals(other.Timestamp) && Value.Equals(other.Value));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Timestamp, Value);
    }
}