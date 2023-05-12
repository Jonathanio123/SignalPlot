using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SignalPlot.Data.Models;
using SignalPlot.ViewModels.SignalOverview;

namespace SignalPlot.ViewModels;

public partial class SignalOverviewVM : ObservableObject
{
    /// <summary>
    /// Backing field should rarely be cleared, only when the user wants to permanently delete a signal
    /// </summary>
    public readonly ObservableCollection<OverviewSensorLine> OverviewSensorLines = new();

    /// <summary>
    /// Selected items in the DataGrid, mirrors the <see cref="DataGrid_SelectedItems"/> <seealso cref="IList"/> which is bound to DataGrid's selectedItems property
    /// </summary>
    public List<OverviewSensorLine> SelectedItems = new();

    /// <summary>
    /// Bound to DataGrids selectedItems property on Loaded +=
    /// </summary>
    internal IList DataGrid_SelectedItems;

    /// <summary>
    /// The actual items in the DataGrid, this list is bound to DataGrids <c>ItemSource</c> property
    /// </summary>
    public ObservableCollection<OverviewSensorLine> DataGridSensorLines;

    // This does not work for some reason
    //[ObservableProperty] private bool notImplementedFlyoutIsOpenn;
    private bool _notImplementedFlyoutIsOpen;

    public bool NotImplementedFlyoutIsOpen
    {
        get => _notImplementedFlyoutIsOpen;
        set => SetProperty(ref _notImplementedFlyoutIsOpen, value);
    }

    public SignalOverviewVM()
    {
        DataGridSensorLines = new ObservableCollection<OverviewSensorLine>(OverviewSensorLines);
        OverviewSensorLines.CollectionChanged += (sender, args) =>
        {
            if (args.NewItems is null) return;
            foreach (OverviewSensorLine item in args.NewItems)
                _propertyChangedHandlers.ForEach(action => item.PropertyChanged += action);
            
        };
    }

    public void AddSensorLine(OverviewSensorLine sensorLine)
    {
        if (DataGridSensorLines.Any(x => x.SensorLine.Id == sensorLine.SensorLine.Id))
            return;
        if (OverviewSensorLines.Any(x => x.SensorLine.Id == sensorLine.SensorLine.Id))
            return;
        DataGridSensorLines.Add(sensorLine);
        OverviewSensorLines.Add(sensorLine);
    }

    public void ClearSignal(OverviewSensorLine sensorLine)
    {
        OverviewSensorLines.Remove(sensorLine);
        SelectedItems.Remove(sensorLine);
        DataGridSensorLines.Remove(sensorLine);
        DataGrid_SelectedItems.Remove(sensorLine);
    }

    public void ClearSignals()
    {
        OverviewSensorLines.Clear();
        SelectedItems.Clear();
        DataGridSensorLines.Clear();
    }

    internal void OnSelectionChanged(IList selectedItemsGeneric)
    {
        var selectedItems = selectedItemsGeneric.Cast<OverviewSensorLine>().ToList();
        foreach (var overviewSensorLine in selectedItems)
        {
            if (!SelectedItems.Contains(overviewSensorLine))
                SelectedItems.Add(overviewSensorLine);
        }

        var itemsToRemove = new List<OverviewSensorLine>();
        foreach (var dataGridSelectedItem in SelectedItems)
        {
            if (!selectedItems.Contains(dataGridSelectedItem))
                itemsToRemove.Add(dataGridSelectedItem);
        }

        SelectedItems.RemoveAll(itemsToRemove.Contains);
        SelectionChanged();
    }

    public void SetSelectedVisibility(bool visibility)
    {
        foreach (var selectedItem in SelectedItems)
        {
            selectedItem.SignalVisible = visibility;
        }
    }

    public void DeleteSelectedSignals()
    {
        NotImplementedFlyoutIsOpen = true;
    }

    private void SelectionChanged()
    {
        foreach (var sensorLine in DataGridSensorLines)
        {
            sensorLine.SignalSelected = SelectedItems.Contains(sensorLine);
        }
    }

    public void ToggleSelectedSignalVisibility()
    {
        foreach (OverviewSensorLine sensorLine in SelectedItems)
            sensorLine.SignalVisible = !sensorLine.SignalVisible;
    }

    public void FilterSignals(string queryText, string filterBy)
    {
        var queriedSignals = new ObservableCollection<OverviewSensorLine>();
        foreach (var overviewSensorLine in DataGridSensorLines)
            switch (filterBy)
            {
                case "All":
                {
                    if (StringContains(overviewSensorLine.SensorLine.DisplayName, queryText) ||
                        StringContains(overviewSensorLine.SensorLine.Source, queryText) ||
                        StringContains(overviewSensorLine.SensorLine.SignalName, queryText) ||
                        StringContains(overviewSensorLine.SensorLine.SignalType.ToString(), queryText) ||
                        StringContains(overviewSensorLine.SensorLine.Unit, queryText))
                        queriedSignals.Add(overviewSensorLine);
                    break;
                }
                case OverviewSensorLine.DisplayNameHeader:
                {
                    if (StringContains(overviewSensorLine.SensorLine.DisplayName, queryText))
                        queriedSignals.Add(overviewSensorLine);
                    break;
                }
                case OverviewSensorLine.SourceHeader:
                {
                    if (StringContains(overviewSensorLine.SensorLine.Source, queryText))
                        queriedSignals.Add(overviewSensorLine);
                    break;
                }
                case OverviewSensorLine.SignalNameHeader:
                {
                    if (StringContains(overviewSensorLine.SensorLine.SignalName, queryText))
                        queriedSignals.Add(overviewSensorLine);
                    break;
                }
                case OverviewSensorLine.SignalTypeHeader:
                {
                    if (StringContains(overviewSensorLine.SensorLine.SignalType.ToString(), queryText))
                        queriedSignals.Add(overviewSensorLine);
                    break;
                }
                case OverviewSensorLine.UnitHeader:
                {
                    if (StringContains(overviewSensorLine.SensorLine.Unit, queryText))
                        queriedSignals.Add(overviewSensorLine);
                    break;
                }
            }

        DataGridSensorLines.Clear();
        SelectedItems.Clear();
        foreach (var overviewSensorLine in queriedSignals)
        {
            DataGridSensorLines.Add(overviewSensorLine);
            DataGrid_SelectedItems?.Add(overviewSensorLine);
        }
    }

    public void SearchBoxTextChanged(string newText)
    {
        if (!string.IsNullOrWhiteSpace(newText)) return;
        ResetGridContent();
    }

    public void ResetGridContent()
    {
        DataGridSensorLines.Clear();
        SelectedItems.Clear();
        foreach (var overviewSensorLine in OverviewSensorLines)
        {
            overviewSensorLine.SignalSelected = false;
            DataGridSensorLines.Add(overviewSensorLine);
        }
    }

    private static bool StringContains(string stringToCheck, string isInString)
    {
        return stringToCheck.ToLower().Trim().Contains(isInString.ToLower().Trim());
    }

    private readonly List<PropertyChangedEventHandler> _propertyChangedHandlers = new();

    public void AddPropertyChangedHandler(PropertyChangedEventHandler f)
    {
        _propertyChangedHandlers.Add(f);
        foreach (var overviewSensorLine in OverviewSensorLines)
            overviewSensorLine.PropertyChanged += f;
    }
}