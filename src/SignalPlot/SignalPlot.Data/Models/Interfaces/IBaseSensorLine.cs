using System;
using SignalPlot.Data.Enums;

namespace SignalPlot.Data.Models.Interfaces;

public interface IBaseSensorLine
{
    /// <summary>
    /// Unique identifier that is given to his sensor line
    /// </summary>
    public Guid Id { get;  }
    /// <summary>
    /// Sensor line display name
    /// </summary>
    public string DisplayName { get; init; }
    /// <summary>
    /// Sensor line signal name, typically has the most verbose description
    /// </summary>
    public string SignalName { get; init; }
    /// <summary>
    /// Source identifier
    /// </summary>
    public string Source { get; init; }
    /// <summary>
    /// Unit e.g. ml/min <para>Note: as this is a string value it can contain anything</para>
    /// </summary>
    public string Unit { get; init; }
    /// <summary>
    /// Every sensor line is given a signalType to decide how values should be handled
    /// <para>See <see cref="Enums.SignalType"/> for possible values</para>
    /// </summary>
    public SignalType SignalType { get; init; }
}