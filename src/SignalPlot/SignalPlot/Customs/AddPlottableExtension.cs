using System.Collections.Generic;
using ScottPlot;
using SignalPlot.Customs.DataSources;
using SignalPlot.Customs.Plottables;

namespace SignalPlot.Customs;

internal static class AddPlottableExtension
{
    public static LiveScatter AddLiveScatter(this AddPlottable addPlottable, Plot plot, ILiveScatterSource data,
        Color? color = null)
    {
        if (addPlottable == null) throw new System.ArgumentNullException(nameof(addPlottable));
        Color nextColor = color ?? addPlottable.NextColor;
        LiveScatter liveScatter = new(data);
        liveScatter.LineStyle.Color = nextColor;
        liveScatter.MarkerStyle.Fill.Color = nextColor;
        plot.Plottables.Add(liveScatter);
        return liveScatter;
    }

    public static LiveScatter AddLiveScatter(this AddPlottable addPlottable, Plot plot, IList<double> xs,
        IList<double> ys, Color? color = null)
    {
        return addPlottable.AddLiveScatter(plot, new LiveScatterSource(xs, ys), color);
    }
}