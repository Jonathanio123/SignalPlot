using System.Linq;
using ScottPlot;
using ScottPlot.Axis;
using ScottPlot.Rendering;

namespace SignalPlot.Customs;

internal static class PlotExtensions
{
    /// <summary>
    /// Plot extensions to auto scale only visible plottables
    /// </summary>
    /// <param name="plot"></param>
    /// <param name="tight"></param>
    internal static void AutoScaleVisible(this Plot plot, bool tight = false)
    {
        if (!plot.Plottables.Any(p => p.IsVisible))
            return;
        // reset limits for all axes
        plot.XAxes.ForEach(xAxis => xAxis.Range.Reset());
        plot.YAxes.ForEach(yAxis => yAxis.Range.Reset());

        // assign default axes to plottables without axes
        Common.ReplaceNullAxesWithDefaults(plot);

        // expand all axes by the limits of each plot
        foreach (IPlottable plottable in plot.Plottables.Where(p => p.IsVisible))
        {
            AutoScale(plot, plottable.Axes.XAxis, plottable.Axes.YAxis, tight);
        }
    }

    private static void AutoScale(Plot plot, IXAxis xAxis, IYAxis yAxis, bool tight = false)
    {
        // reset limits only for these axes
        xAxis.Range.Reset();
        yAxis.Range.Reset();

        // assign default axes to plottables without axes
        Common.ReplaceNullAxesWithDefaults(plot);

        // expand all axes by the limits of each plot
        foreach (IPlottable plottable in plot.Plottables.Where(p => p.IsVisible))
        {
            AxisLimits limits = plottable.GetAxisLimits();
            plottable.Axes.YAxis.Range.Expand(limits.Rect.YRange);
            plottable.Axes.XAxis.Range.Expand(limits.Rect.XRange);
        }

        // apply margins
        if (!tight)
        {
            plot.XAxes.ForEach(xAxis => xAxis.Range.ZoomFrac(plot.Margins.ZoomFracX));
            plot.YAxes.ForEach(yAxis => yAxis.Range.ZoomFrac(plot.Margins.ZoomFracY));
        }
    }
}