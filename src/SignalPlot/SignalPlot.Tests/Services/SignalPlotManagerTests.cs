using Windows.UI;
using ScottPlot;
using SignalPlot.Data.Enums;
using SignalPlot.Data.Models;
using SignalPlot.Services;
using SignalPlot.Tests.Mocks;
using SignalPlot.Customs;
using OneOf.Types;
using OneOf;
using ScottPlot.Plottables;
using SignalPlot.Customs.Plottables;
using Color = Windows.UI.Color;

namespace SignalPlot.Tests.Services;

public class SignalPlotManagerTests
{
    private ISignalPlotManager InitSignalPlotManager() =>
        new SignalPlotManager(new WinUiPlotMocked(), new WinUiPlotMocked());

    private AnalogueSensorLine CreateAnalogueSensorLine(int points, TimeSpan timeBetweenPoints, int minValue,
        int maxValue)
    {
        var rnd = new Random();
        var sensorLine = new AnalogueSensorLine("Test", "Test", "Test", "Test", SignalType.Analog);
        for (int i = 0; i < points; i++)
        {
            var vale = (double)rnd.Next(minValue, maxValue);
            var time = DateTime.Now.Add(timeBetweenPoints.Multiply(i));
            sensorLine.AddEntry(new DataEntry<double>(time, vale));
        }

        return sensorLine;
    }



    [Fact]
    public void ClearSignals_NoScattersShouldBeFound()
    {
        var signalPlotManager = InitSignalPlotManager();
        var signalIds = new List<Guid>();
        for (int i = 0; i < 10; i++)
        {
            var sensorLine = CreateAnalogueSensorLine(100+i, TimeSpan.FromSeconds(1+i), 0, 100);
            signalIds.Add(sensorLine.Id);
            signalPlotManager.AddSignal(sensorLine);
        }

        signalPlotManager.ClearSignals();

        var signalsFound = 0;
        for (int i = 0; i < 10; i++)
        {
            if (!signalPlotManager.GetScatterSignal(signalIds[i]).IsT0)
                signalsFound++;
        }


        Assert.Equal(0, signalsFound);
    }

    [Fact]
    public void ChangeSignalColor_ChangeToWhite()
    {
        var signalPlotManager = InitSignalPlotManager();
        var sensorLine = CreateAnalogueSensorLine(100, TimeSpan.FromSeconds(1), 0, 100);
        signalPlotManager.AddSignal(sensorLine);
        Windows.UI.Color newColor = Color.FromArgb(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue);

        signalPlotManager.ChangeSignalColor(sensorLine.Id, newColor);


        var result = signalPlotManager.GetSignal(sensorLine.Id);
        Assert.True(result.AsT1.LineStyle.Color == ScottPlot.Colors.White);

    }

    [Fact]
    public void GetSignal_ScatterFound()
    {
        var signalPlotManager = InitSignalPlotManager();
        var sensorLine = CreateAnalogueSensorLine(100, TimeSpan.FromSeconds(1), 0, 100);
        signalPlotManager.AddSignal(sensorLine);


        var result = signalPlotManager.GetSignal(sensorLine.Id);
        var signal = result.AsT1;


        Assert.Equal(sensorLine.Id, signal.GetId().Value);
    }
}