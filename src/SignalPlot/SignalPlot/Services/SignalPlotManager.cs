using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using OneOf.Types;
using ScottPlot;
using ScottPlot.Control;
using ScottPlot.Grids;
using ScottPlot.Plottables;
using ScottPlot.WinUI;
using SignalPlot.Customs;
using SignalPlot.Customs.DataSources;
using SignalPlot.Customs.Plottables;
using SignalPlot.Data.Infrastructure;
using SignalPlot.Data.Models;

namespace SignalPlot.Services;

public class SignalPlotManager : ISignalPlotManager
{
    private readonly IPlotControl _analoguePlot;
    private readonly IPlotControl _digitalPlot;
    private readonly object _plottablesLock = new();

    private Windows.UI.Color _currentBackgroundColor;

    public SignalPlotManager(IPlotControl analoguePlot, IPlotControl digitalPlot)
    {
        _analoguePlot = analoguePlot;
        _analoguePlot.Interaction.ContextMenuItems = new ContextMenuItem[] { };
        _analoguePlot.Plot.Axes.DateTimeTicks(Edge.Bottom);
        _analoguePlot.Plot.Legends.Clear();


        _digitalPlot = digitalPlot;
        _digitalPlot.Plot.Axes.DateTimeTicks(Edge.Bottom);
        //_digitalPlot.Plot.YAxes.ForEach(y => y.IsVisible = false);
        //_digitalPlot.Plot.XAxes.ForEach(x => x.IsVisible = false);
        _digitalPlot.Plot.Title.IsVisible = false;
        _digitalPlot.Plot.Grids.Clear();
        _digitalPlot.Plot.Legends.Clear();
        if (_digitalPlot is WinUIPlot winUIPlot)
            winUIPlot.Background.Opacity = 0;

        //var newInteraction = new Customs.LinkedInteraction(_analoguePlot, digitalPlot);
        //_digitalPlot.Replace(newInteraction);
        //_analoguePlot.Replace(newInteraction);
    }

    private IPlottable _GetSignal(Guid signalId)
    {
        lock (_plottablesLock)
        {
            foreach (var plottable in _digitalPlot.Plot.Plottables)
            {
                if (plottable.GetId().Value == signalId)
                    return plottable;
            }

            foreach (var plottable in _analoguePlot.Plot.Plottables)
            {
                if (plottable.GetId().Value == signalId)
                    return plottable;
            }
        }

        return null;
    }

    public OneOf<NotFound, Scatter> GetScatterSignal(Guid signalId)
    {
        var plottable = _GetSignal(signalId);
        if (plottable is Scatter scatter)
            return scatter;
        return new NotFound();
    }

    public OneOf<NotFound, BarPlot> GetBarSignal(Guid signalId)
    {
        var plottable = _GetSignal(signalId);
        if (plottable is BarPlot barPlot)
            return barPlot;
        return new NotFound();
    }

    public OneOf<NotFound, Scatter, BarPlot, LiveScatter> GetSignal(Guid signalId)
    {
        var scatterSignal = GetScatterSignal(signalId);
        var barSeries = GetBarSignal(signalId);
        var liveScatter = GetLiveScatterSignal(signalId);

        if (scatterSignal.IsT1 && barSeries.IsT1 && liveScatter.IsT1)
            throw new InvalidOperationException("Digital and Analogue signal found with same signal name");

        if (scatterSignal.IsT1)
            return scatterSignal.AsT1;

        if (barSeries.IsT1)
            return barSeries.AsT1;
        if (liveScatter.IsT1)
            return liveScatter.AsT1;

        return new NotFound();
    }

    public void ToggleSignalVisibility(Guid signalId)
    {
        var scatterSignal = GetScatterSignal(signalId);
        var barSeries = GetBarSignal(signalId);
        var liveScatter = GetLiveScatterSignal(signalId);


        if (scatterSignal.IsT1 && barSeries.IsT1)
            throw new InvalidOperationException("Digital and Analogue signal found with same signal name");

        if (scatterSignal.IsT1)
        {
            scatterSignal.AsT1.IsVisible = !scatterSignal.AsT1.IsVisible;
            _analoguePlot.Refresh();
            return;
        }
        else if (barSeries.IsT1)
        {
            barSeries.AsT1.IsVisible = !barSeries.AsT1.IsVisible;
            _digitalPlot.Refresh();
            return;
        }
        else if (liveScatter.IsT1)
        {
            liveScatter.AsT1.IsVisible = !liveScatter.AsT1.IsVisible;
            _analoguePlot.Refresh();
            return;
        }

        Debug.WriteLine("Warning no signal found! ToggleSignalVisibility(string signalName)");
    }

    public void ToggleGridVisibility(bool refresh = true)
    {
        if (_analoguePlot.Plot.Grids.Any())
            _analoguePlot.Plot.Grids.Clear();
        else
            _analoguePlot.Plot.Grids.Add(new DefaultGrid(_analoguePlot.Plot.XAxis, _analoguePlot.Plot.YAxis));
        SetPlotsColor(_currentBackgroundColor);
        if (refresh)
            Refresh();
    }

    public void SetSignalVisibility(Guid signalId, bool signalVisible)
    {
        var scatterSignal = GetScatterSignal(signalId);
        var barSeries = GetBarSignal(signalId);


        if (scatterSignal.IsT1 && barSeries.IsT1)
            throw new InvalidOperationException("Digital and Analogue signal found with same signal name");

        if (scatterSignal.IsT1)
        {
            scatterSignal.AsT1.IsVisible = signalVisible;
            _analoguePlot.Refresh();
        }
        else if (barSeries.IsT1)
        {
            barSeries.AsT1.IsVisible = signalVisible;
            _digitalPlot.Refresh();
        }
    }


    public Scatter AddSignal(AnalogueSensorLine sensorLine)
    {
        var timeStamps = sensorLine.DataEntries.Select(dataEntry => dataEntry.Timestamp.ToOADate()).ToArray();
        var values = sensorLine.DataEntries.Select(dataEntry => dataEntry.Value).ToArray();

        var plotSignal = _analoguePlot.Plot.Add.Scatter(timeStamps, values);

        plotSignal.SetId(sensorLine.Id);
        plotSignal.Label = sensorLine.SignalName;
        plotSignal.MarkerStyle = MarkerStyle.None;

        return plotSignal;
    }

    public async Task AddSignalSourceAsync(IAnalogueSignalSource signalSource,
        CancellationToken cancellationToken = default)
    {
        await foreach (var dataEntry in signalSource.GetValuesAsyncEnumerable(cancellationToken).ConfigureAwait(false))
        {
            var liveScatter = GetLiveScatterSignal(signalSource.Id)
                .Match(
                    (_) =>
                    {
                        LiveScatter plotSignal;
                        lock (_plottablesLock)
                        {
                            plotSignal = _analoguePlot.Plot.Add.AddLiveScatter(_analoguePlot.Plot,
                                new LiveScatterSource(new List<double>() { },
                                    new List<double>() { }));

                            plotSignal.Label = signalSource.SignalName;
                            plotSignal.MarkerStyle = MarkerStyle.None;
                            plotSignal.SetId(signalSource.Id);
                        }

                        return plotSignal;
                    },
                    (scatter) => scatter);

            liveScatter.Data.AddPoint(new Coordinates(dataEntry.Timestamp.ToOADate(), dataEntry.Value));

            // Remove points older than 30 seconds
            //if ((dataEntry.Timestamp - TimeSpan.FromMinutes(0.5)).ToOADate() > liveScatter.Data.GetPoint(0).X)
            //liveScatter.Data.RemoveFirstPoint();
        }
    }

    public void AssignAxis(Guid signalId, string axis)
    {
        var plotSignal = _GetSignal(signalId);
        if (_analoguePlot.Plot.YAxes.Count() == 4)
        {
            if (axis == "Tertiary")
            {
                _analoguePlot.Plot.YAxes[3].IsVisible = true;
                _analoguePlot.Plot.YAxes[0].Range.Reset();
                _analoguePlot.Plot.YAxes[2].Range.Reset();
                _analoguePlot.Plot.YAxes[3].Range.Reset();
                plotSignal.Axes.YAxis = _analoguePlot.Plot.YAxes[3];
            }

            if (axis == "Secondary")
            {
                _analoguePlot.Plot.YAxes[2].IsVisible = true;
                _analoguePlot.Plot.YAxes[0].Range.Reset();
                _analoguePlot.Plot.YAxes[2].Range.Reset();
                _analoguePlot.Plot.YAxes[3].Range.Reset();
                plotSignal.Axes.YAxis = _analoguePlot.Plot.YAxes[2];
            }

            if (axis == "Primary")
            {
                _analoguePlot.Plot.YAxes[0].IsVisible = true;
                _analoguePlot.Plot.YAxes[0].Range.Reset();
                _analoguePlot.Plot.YAxes[2].Range.Reset();
                _analoguePlot.Plot.YAxes[3].Range.Reset();
                plotSignal.Axes.YAxis = _analoguePlot.Plot.YAxes[0];
            }

            AxesCheck();
            Refresh();
            return;
        }
        else
        {
            _analoguePlot.Plot.YAxes[0].Label.Text = "Primary";

            ScottPlot.Axis.StandardAxes.LeftAxis yAxis2 = new();
            yAxis2.Color(_analoguePlot.Plot.YAxes[0].FrameLineStyle.Color);
            yAxis2.Label.Text = "Secondary";
            yAxis2.IsVisible = false;
            _analoguePlot.Plot.YAxes.Add(yAxis2);

            ScottPlot.Axis.StandardAxes.LeftAxis yAxis3 = new();
            yAxis3.Color(_analoguePlot.Plot.YAxes[0].FrameLineStyle.Color);
            yAxis3.Label.Text = "Tertiary";
            yAxis3.IsVisible = false;
            _analoguePlot.Plot.YAxes.Add(yAxis3);

            if (axis == "Primary")
            {
                _analoguePlot.Plot.YAxes[0].IsVisible = true;
                plotSignal.Axes.YAxis = _analoguePlot.Plot.YAxes[0];
                _analoguePlot.Plot.YAxes[3].Range.Reset();
            }

            if (axis == "Secondary")
            {
                _analoguePlot.Plot.YAxes[2].IsVisible = true;
                plotSignal.Axes.YAxis = _analoguePlot.Plot.YAxes[2];
                _analoguePlot.Plot.YAxes[3].Range.Reset();
            }

            if (axis == "Tertiary")
            {
                _analoguePlot.Plot.YAxes[3].IsVisible = true;
                plotSignal.Axes.YAxis = _analoguePlot.Plot.YAxes[3];
                _analoguePlot.Plot.YAxes[3].Range.Reset();
            }

            AxesCheck();
            Refresh();
        }
    }

    public void AxesCheck()
    {
        var signals = _analoguePlot.Plot.Plottables;
        bool primary = false;
        bool secondary = false;
        bool tertiary = false;
        foreach (var signal in signals)
        {
            if (signal.Axes.YAxis.Equals(_analoguePlot.Plot.YAxes[0]))
            {
                primary = true;
            }

            if (signal.Axes.YAxis.Equals(_analoguePlot.Plot.YAxes[2]))
            {
                secondary = true;
            }

            if (signal.Axes.YAxis.Equals(_analoguePlot.Plot.YAxes[3]))
            {
                tertiary = true;
            }
        }

        if (!primary) _analoguePlot.Plot.YAxes[0].IsVisible = false;
        if (!secondary) _analoguePlot.Plot.YAxes[2].IsVisible = false;
        if (!tertiary) _analoguePlot.Plot.YAxes[3].IsVisible = false;
    }


    public BarPlot AddSignal(DigitalSensorLine sensorLine)
    {
        var signalsInPlot = ((double)_digitalPlot.Plot.Plottables.Count()) * 1.5;
        List<Bar> bars = sensorLine.DataEntries.Select(dataEntry => new Bar()
        {
            Position = dataEntry.Timestamp.ToOADate(),
            Value = signalsInPlot * -1 + Convert.ToDouble(dataEntry.Value),
            ValueBase = signalsInPlot * -1
        }).ToList();

        var barPlot = _digitalPlot.Plot.Add.Bar(bars);
        barPlot.SetId(sensorLine.Id);
        barPlot.Label = sensorLine.SignalName;
        foreach (var barPlotSeries in barPlot.Series)
        {
            barPlotSeries.Color = Colors.DarkGrey;
        }

        barPlot.LineStyle.Color = Colors.White;


        return barPlot;
    }


    public void ChangeSignalColor(Guid signalId, Windows.UI.Color color)
    {
        var sColor = Color.FromARGB(uint.Parse(color.ToString()[1..], NumberStyles.HexNumber));

        GetSignal(signalId).Switch(
            _ => { Debug.WriteLine($"Warning no signal found with signalId: {signalId}"); },
            scatterSignal =>
            {
                scatterSignal.LineStyle.Color = sColor;
                scatterSignal.MarkerStyle.Fill.Color = sColor;
                _analoguePlot.Refresh();
            },
            barPlot =>
            {
                barPlot.Series.FirstOrDefault()!.Color = sColor;
                _digitalPlot.Refresh();
            },
            liveScatter =>
            {
                liveScatter.LineStyle.Color = sColor;
                liveScatter.MarkerStyle.Fill.Color = sColor;
                _analoguePlot.Refresh();
            }
        );
    }

    public void Refresh()
    {
        lock (_plottablesLock)
        {
            _analoguePlot.Refresh();
            _digitalPlot.Refresh();
        }
    }

    public void FrameAllSignals(bool refresh = true)
    {
        lock (_plottablesLock)
        {
            _analoguePlot.Plot.AutoScaleVisible(true);
            _digitalPlot.Plot.AutoScaleVisible(true);
        }

        if (refresh)
            Refresh();
    }

    public void ClearSignals(bool refresh = true)
    {
        _analoguePlot.Plot.Clear();
        _digitalPlot.Plot.Clear();
        var leftAxis = _analoguePlot.Plot.YAxes[0];
        leftAxis.Label.Text = "";
        var rightAxis = _analoguePlot.Plot.YAxes[1];
        _analoguePlot.Plot.YAxes.Clear();
        _analoguePlot.Plot.YAxes.Add(leftAxis);
        _analoguePlot.Plot.YAxes.Add(rightAxis);
        if (refresh)
            Refresh();
    }

    public void ChangeSignalBoldness(Guid signalId, bool selected)
    {
        GetSignal(signalId).Switch(
            _ => { Debug.WriteLine($"Warning no signal found with signalId: {signalId}"); },
            scatterSignal =>
            {
                scatterSignal.LineStyle.Width = selected ? 2f : 1f;
                _analoguePlot.Refresh();
            },
            barSignal =>
            {
                Debug.WriteLine("Digital signal boldness on select not implemented");
                _digitalPlot.Refresh();
            },
            liveScatterSignal =>
            {
                liveScatterSignal.LineStyle.Width = selected ? 2f : 1f;
                _analoguePlot.Refresh();
            }
        );
    }

    public void SetPlotsColor(Windows.UI.Color background)
    {
        _currentBackgroundColor = background;
        var backgroundStringHex = background.ToString();
        var lightModeEnabled = backgroundStringHex == "#FFFFFFFF";
        _analoguePlot.Plot.Style.Background(Color.FromHex(backgroundStringHex), Color.FromHex(backgroundStringHex));

        if (lightModeEnabled) // Set Custom white color to match the rest of the app
            _analoguePlot.Plot.Style.Background(Color.FromHex("f3f3f3"), Color.FromHex(backgroundStringHex));

        _digitalPlot.Plot.Style.Background(Color.FromARGB(16777215), Color.FromARGB(16777215)); // Transparent
        _digitalPlot.Plot.Style.ColorAxes(Color.FromARGB(16777215));
        _digitalPlot.Plot.Style.ColorGrids(Color.FromARGB(16777215), Color.FromARGB(16777215));
        ColorAxesAndGrids(_analoguePlot, lightModeEnabled);
        var axes = _analoguePlot.Plot.YAxes;
        var color = _analoguePlot.Plot.YAxes[0].FrameLineStyle.Color;
        foreach (var axis in axes)
        {
            axis.Label.Font.Color = color;
            axis.FrameLineStyle.Color = color;
            axis.MajorTickColor = color;
            axis.MinorTickColor = color;
        }

        Refresh();
    }

    public OneOf<NotFound, LiveScatter> GetLiveScatterSignal(Guid signalId)
    {
        var plottable = _GetSignal(signalId);
        if (plottable is LiveScatter liveScatter)
            return liveScatter;
        return new NotFound();
    }

    private static void ColorAxesAndGrids(IPlotControl winUiPlot, bool lightModeEnabled)
    {
        if (lightModeEnabled)
        {
            winUiPlot.Plot.Style.ColorAxes(Colors.Black);
            winUiPlot.Plot.Style.ColorGrids(Colors.Grey.WithOpacity(), Colors.Grey.WithOpacity());
        }
        else // darkModeEnabled
        {
            winUiPlot.Plot.Style.ColorAxes(Colors.WhiteSmoke);
            winUiPlot.Plot.Style.ColorGrids(Colors.White.WithOpacity(0.5), Colors.White.WithOpacity(0.5));
        }
    }

    public bool PlotsAreInteractive { get; private set; } = true;

    public bool TogglePlotInteraction()
    {
        if (PlotsAreInteractive)
        {
            _analoguePlot.Interaction.Actions = PlotActions.NonInteractive();
            _digitalPlot.Interaction.Actions = PlotActions.NonInteractive();
            PlotsAreInteractive = false;
        }
        else
        {
            _analoguePlot.Interaction.Actions = PlotActions.Standard();
            _digitalPlot.Interaction.Actions = PlotActions.Standard();
            PlotsAreInteractive = true;
        }
        return PlotsAreInteractive;
    }
}