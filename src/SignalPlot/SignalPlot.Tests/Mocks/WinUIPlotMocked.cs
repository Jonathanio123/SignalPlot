using ScottPlot;
using ScottPlot.Control;
using SkiaSharp;

namespace SignalPlot.Tests.Mocks
{
    internal class WinUiPlotMocked : IPlotControl
    {
        internal List<object> _refreshes = new(1);
        public void Refresh()
        {
            _refreshes.Add(new object());
        }

        public void Replace(Interaction interaction)
        {
            return;
        }

        public void ShowContextMenu(Pixel position)
        {
            return;
        }

        public WinUiPlotMocked()
        {
            Interaction = new(this);
        }

        public Plot Plot { get; } = new();
        public Interaction Interaction { get; }
        public GRContext? GRContext { get; } = null;
    }
}
