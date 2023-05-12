using System.Runtime.CompilerServices;
using SignalPlot.Data.Enums;
using SignalPlot.Data.Models;
using SignalPlot.ViewModels;
using Xunit.Sdk;

namespace SignalPlot.Tests.ViewModels;

public class SignalOverviewVMTests
{
    [Fact]
    public void AddSignalTest()
    {
        var signalOverviewVM = new SignalOverviewVM();
        var sensorLine =
            new OverviewSensorLine(new AnalogueSensorLine("Test", "Test", "Test", "Test", SignalType.Analog));
        signalOverviewVM.AddSensorLine(sensorLine);
        Assert.Contains(sensorLine, signalOverviewVM.DataGridSensorLines);
        Assert.Contains(sensorLine, signalOverviewVM.OverviewSensorLines);
    }

    private SignalOverviewVM CreateSignalOverviewVM(int signalsToAdd, int signalSelected = 0)
    {
        if (signalSelected > signalsToAdd)
            throw new XunitException("signalSelected cannot be greater than signalsToAdd");

        var signalOverviewVM = new SignalOverviewVM();
        for (int i = 0; i < signalsToAdd; i++)
        {
            var sensorLine =
                new OverviewSensorLine(new AnalogueSensorLine($"#{i}", $"#{i}", "Test", "Test", SignalType.Analog));
            signalOverviewVM.AddSensorLine(sensorLine);
        }

        if (signalsToAdd > 0)
        {
            var selectedItems = new List<OverviewSensorLine>();
            for (int i = 0; i < signalsToAdd; i++)
            {
                selectedItems.Add(signalOverviewVM.DataGridSensorLines[i]);
            }

            signalOverviewVM.OnSelectionChanged(selectedItems);
        }

        return signalOverviewVM;
    }

    [Fact]
    public void OnSelectionChangedTest()
    {
        var signalOverviewVM = new SignalOverviewVM();
        var sensorLineToBeRemoved =
            new OverviewSensorLine(new AnalogueSensorLine("Test", "Test", "Test", "Test", SignalType.Analog));
        var sensorLineToBeAdded =
            new OverviewSensorLine(new AnalogueSensorLine("Test", "Test", "Test", "Test", SignalType.Analog));
        signalOverviewVM.AddSensorLine(sensorLineToBeAdded);
        signalOverviewVM.AddSensorLine(sensorLineToBeRemoved);


        signalOverviewVM.OnSelectionChanged(new List<OverviewSensorLine>
            { sensorLineToBeAdded, sensorLineToBeRemoved });
        signalOverviewVM.OnSelectionChanged(new List<OverviewSensorLine> { sensorLineToBeAdded });


        Assert.Contains(sensorLineToBeAdded, signalOverviewVM.SelectedItems);
        Assert.DoesNotContain(sensorLineToBeRemoved, signalOverviewVM.SelectedItems);
    }


    [Fact]
    public void SetSelectedVisibility()
    {
        // Arrange
        var signalOverviewVm = CreateSignalOverviewVM(5, 2);

        // Act
        signalOverviewVm.SetSelectedVisibility(true);

        // Assert
        Assert.All(signalOverviewVm.SelectedItems, x => Assert.True(x.SignalVisible));
    }

    [Fact]
    public void ToggleSelectedSignalVisibility()
    {
        // Arrange
        var signalOverviewVm = CreateSignalOverviewVM(5, 2);
        var selectedSignals = signalOverviewVm.SelectedItems;

        // Act
        signalOverviewVm.ToggleSelectedSignalVisibility();

        // Assert
        Assert.All(signalOverviewVm.SelectedItems, selectedItems => Assert.False(selectedItems.SignalVisible));
        Assert.All(signalOverviewVm.SelectedItems.Where(s => !signalOverviewVm.DataGridSensorLines.Contains(s)),
            unselectedItems => Assert.True(unselectedItems.SignalVisible));
    }


    [Fact]
    public void FilterSignals_ByAll_Single()
    {
        var signalOverviewVM = CreateSignalOverviewVM(10, 2);

        signalOverviewVM.FilterSignals("1", "All");

        Assert.Single(signalOverviewVM.DataGridSensorLines);
    }


    [Fact]
    public void FilterSignals_BySource_LowerCase_Spaces()
    {
        var signalOverviewVM = CreateSignalOverviewVM(10, 5);

        signalOverviewVM.FilterSignals(" test ", "Source");


        Assert.Equal(10, signalOverviewVM.DataGridSensorLines.Count);
    }

    [Fact]
    public void ResetGridContent()
    {
        var signalOverviewVM = CreateSignalOverviewVM(10, 5);
        signalOverviewVM.FilterSignals("1", "All");


        signalOverviewVM.ResetGridContent();

        Assert.All(signalOverviewVM.DataGridSensorLines, line => Assert.False(line.SignalSelected));
        Assert.Empty(signalOverviewVM.SelectedItems);
    }
}