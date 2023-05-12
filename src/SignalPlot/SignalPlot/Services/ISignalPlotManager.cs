using System;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using OneOf.Types;
using ScottPlot.Plottables;
using SignalPlot.Customs.Plottables;
using SignalPlot.Data.Infrastructure;
using SignalPlot.Data.Models;

namespace SignalPlot.Services;

public interface ISignalPlotManager
{
    /// <summary>
    /// Gets a <see cref="Scatter"/> plottable by id
    /// </summary>
    /// <param name="signalId"></param>
    /// <returns>Either <see cref="NotFound"/> or <see cref="Scatter"/> wrapped in <see cref="OneOf{T0, T1}"/></returns>
    public OneOf<NotFound, Scatter> GetScatterSignal(Guid signalId);
    /// <summary>
    /// Gets a <see cref="BarPlot"/> plottable by id
    /// </summary>
    /// <param name="signalId"></param>
    /// <returns>Either <see cref="NotFound"/> or <see cref="BarPlot"/> wrapped in <see cref="OneOf{T0, T1}"/></returns>
    public OneOf<NotFound, BarPlot> GetBarSignal(Guid signalId);
    /// <summary>
    /// Gets a <see cref="LiveScatter"/> plottable by id
    /// </summary>
    /// <param name="signalId"></param>
    /// <returns>Either <see cref="NotFound"/> or <see cref="LiveScatter"/> wrapped in <see cref="OneOf{T0, T1}"/></returns>
    public OneOf<NotFound, LiveScatter> GetLiveScatterSignal(Guid signalId);
    /// <summary>
    /// Gets a plottable by id
    /// </summary>
    /// <param name="signalId"></param>
    /// <returns><see cref="OneOf{T0,T1,T2,T3}"/> where generics are <see cref="NotFound"/>, <see cref="Scatter"/>, <see cref="BarPlot"/> or <see cref="LiveScatter"/> in that order</returns>
    public OneOf<NotFound, Scatter, BarPlot, LiveScatter> GetSignal(Guid signalId);
    /// <summary>
    /// Toggles signal by id
    /// </summary>
    /// <param name="signalId"></param>
    public void ToggleSignalVisibility(Guid signalId);
    /// <summary>
    /// Toggles the analogue plot's grid. The <paramref name="refresh"/> parameter can set to false to disable refreshing after execution
    /// </summary>
    /// <param name="refresh"></param>
    public void ToggleGridVisibility(bool refresh = true);
    /// <summary>
    /// Sets the visibility of a specific signal by id. <c>true</c> will show that signal and <c>false</c> will hide that signal
    /// </summary>
    /// <param name="signalId"></param>
    /// <param name="signalVisible"></param>
    public void SetSignalVisibility(Guid signalId, bool signalVisible);
    public void AssignAxis(Guid signalId, string size);
    /// <summary>
    /// Plots the sensorLine. <para>Note: Method does not call <see cref="Refresh"/> after execution</para>
    /// </summary>
    /// <param name="sensorLine"></param>
    /// <returns>The <see cref="Scatter"/> plottable that was added</returns>
    public Scatter AddSignal(AnalogueSensorLine sensorLine);

    /// <summary>
    /// Add a subscribed signal source to the plot that will continually add new data to the plot until cancelled
    /// </summary>
    /// <exception cref="System.OperationCanceledException"></exception>
    /// <exception cref="System.InvalidOperationException"></exception>
    /// <param name="signalSource"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><para><see cref="Task"/></para></returns>
    public Task AddSignalSourceAsync(IAnalogueSignalSource signalSource, CancellationToken cancellationToken = default);
    /// <summary>
    /// Plots the sensorLine. <para>Note: Method does not call <see cref="Refresh"/> after execution</para>
    /// </summary>
    /// <param name="sensorLine"></param>
    /// <returns>The <see cref="BarPlot"/> plottable that was added</returns>
    public BarPlot AddSignal(DigitalSensorLine sensorLine);
    /// <summary>
    /// Changes the specific plottable's color with <paramref name="color"/>
    /// </summary>
    /// <param name="signalId"></param>
    /// <param name="color"></param>
    public void ChangeSignalColor(Guid signalId, Windows.UI.Color color);
    /// <summary>
    /// Refreshes both analogue plot and digital plot
    /// </summary>
    public void Refresh();
    /// <summary>
    /// Frames all visible plottables
    /// </summary>
    /// <param name="refresh"></param>
    public void FrameAllSignals(bool refresh = true);
    /// <summary>
    /// Removes all <see cref="ScottPlot.IPlottable"/>(s) from analogue plot and digital plot. And resets YAxes
    /// </summary>
    /// <param name="refresh"></param>
    public void ClearSignals(bool refresh = true);
    /// <summary>
    /// Colors the plots
    /// </summary>
    /// <param name="background"></param>
    public void SetPlotsColor(Windows.UI.Color background);
    /// <summary>
    /// Sets the boldness of a specific signal by id. <c>true</c> => bold and <c>false</c> => normal
    /// </summary>
    /// <param name="signalId"></param>
    /// <param name="selected"></param>
    public void ChangeSignalBoldness(Guid signalId, bool selected);

    public bool TogglePlotInteraction();
    public bool PlotsAreInteractive { get; }
}