using CommunityToolkit.Mvvm.ComponentModel;
using SignalPlot.Data.Models;

namespace SignalPlot.ViewModels.SignalOverview;

public partial class SearchFlyout : ObservableObject
{
    internal string[] ComboBoxSource = new[]
    {
        "All", OverviewSensorLine.DisplayNameHeader, OverviewSensorLine.SignalNameHeader,
        OverviewSensorLine.SourceHeader, OverviewSensorLine.SignalTypeHeader, OverviewSensorLine.UnitHeader
    };

    [ObservableProperty] private string placeHolderText = "Search All";

    public SearchFlyout()
    {
    }

    // Having this be an [ObservableProperty] connected to .SelectedItem does not work
    // for some reason, assuming .SelectedItem cannot be bound to
    public string SelectedItem { get; private set; } = "All";

    internal void UpdatePlaceHolderText(string selectedItem)
    {
        SelectedItem = selectedItem;
        PlaceHolderText = $"Search {SelectedItem}...";
    }
}