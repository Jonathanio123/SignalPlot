#nullable enable
using System.Collections.Generic;
using System.Linq;
using ScottPlot;
using ScottPlot.Axis;
using ScottPlot.Extensions;
using SignalPlot.Customs.DataSources;
using SkiaSharp;

namespace SignalPlot.Customs.Plottables;

public class LiveScatter : IPlottable
{
    public string? Label { get; set; }
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = ScottPlot.Axis.Axes.Default;
    public LineStyle LineStyle { get; set; } = new();
    public MarkerStyle MarkerStyle { get; set; } = MarkerStyle.Default;

    /// <summary>
    /// LiveScatter does not have a legend item
    /// </summary>
    /// <returns>An empty Enumerable</returns>
    public IEnumerable<LegendItem> LegendItems => Enumerable.Empty<LegendItem>();

    public ILiveScatterSource Data { get; }

    public LiveScatter(ILiveScatterSource data)
    {
        Data = data;
    }

    public AxisLimits GetAxisLimits() => Data.GetLimits();

    public void Render(SKSurface surface)
    {
        IEnumerable<Pixel> pixels = Data.GetScatterPoints().Select(x => Axes.GetPixel(x));

        using SKPaint paint = new() { IsAntialias = true };
        LineStyle.ApplyToPaint(paint);

        using SKPath path = new();
        path.MoveTo(pixels.First().X, pixels.First().Y);
        foreach (Pixel pixel in pixels) path.LineTo(pixel.X, pixel.Y);
        surface.Canvas.DrawPath(path, paint);

        MarkerStyle.Render(surface.Canvas, pixels);
    }
}