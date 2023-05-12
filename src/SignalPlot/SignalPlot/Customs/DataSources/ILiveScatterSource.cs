using System.Collections.Generic;
using System.Linq;
using ScottPlot;
using ScottPlot.DataSources;

namespace SignalPlot.Customs.DataSources;

public interface ILiveScatterSource : IHasAxisLimits
{
    /// <summary>
    /// Locks the data Xs and Ys before retrieving list
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<Coordinates> GetScatterPoints();

    /// <summary>
    /// Locks the data Xs and Ys before adding point
    /// </summary>
    /// <param name="point"></param>
    /// <param name="addThenRemove">If set to true, will also remove a point</param>
    public void AddPoint(Coordinates point, bool addThenRemove = false);

    public Coordinates GetPoint(int index);

    public bool RemovePoint(double yValue);
    public void RemoveFirstPoint();
}

public class LiveScatterSource : ILiveScatterSource
{
    private readonly IList<double> Xs;
    private readonly IList<double> Ys;
    private static readonly object ListLock = new();

    public LiveScatterSource(IList<double> xs, IList<double> ys)
    {
        lock (ListLock)
        {
            Xs = xs;
            Ys = ys;
        }
    }

    public CoordinateRange GetLimitsX()
    {
        lock (ListLock)
        {
            return new CoordinateRange(Xs.Min(), Xs.Max());
        }
    }

    public CoordinateRange GetLimitsY()
    {
        lock (ListLock)
        {
            return new CoordinateRange(Ys.Min(), Ys.Max());
        }
    }

    public AxisLimits GetLimits()
    {
        lock (ListLock)
        {
            return new AxisLimits(new CoordinateRange(Xs.Min(), Xs.Max()), new CoordinateRange(Ys.Min(), Ys.Max()));
        }
    }

    public IReadOnlyList<Coordinates> GetScatterPoints()
    {
        lock (ListLock)
        {
            return Xs.Zip(Ys, (x, y) => new Coordinates(x, y)).ToArray();
        }
    }

    public void AddPoint(Coordinates point, bool addThenRemove = false)
    {
        lock (ListLock)
        {
            Xs.Add(point.X);
            Ys.Add(point.Y);

            if (addThenRemove)
            {
                Xs.RemoveAt(0);
                Ys.RemoveAt(0);
            }
        }
    }

    public Coordinates GetPoint(int index)
    {
        lock (ListLock)
        {
            return new Coordinates(Xs[index], Ys[index]);
        }
    }

    public bool RemovePoint(double xValue)
    {
        lock (ListLock)
        {
            for (var i = 0; i < Ys.Count; i++)
                if (Xs[i].Equals(xValue))
                {
                    Ys.RemoveAt(i);
                    Xs.RemoveAt(i);
                    return true;
                }
            return false;
        }
    }

    public void RemoveFirstPoint()
    {
        lock (ListLock)
        {
            Xs.RemoveAt(0);
            Ys.RemoveAt(0);
        }
    }
}