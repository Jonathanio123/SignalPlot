using System.Collections.Specialized;
using System.Linq;
using Windows.Foundation;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI.Controls.Primitives;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SignalPlot.Data.Models;
using SignalPlot.ViewModels;
using SignalPlot.ViewModels.SignalOverview;

namespace SignalPlot;

public sealed partial class SignalOverview : UserControl
{
    public const string DisplayNameHeader = OverviewSensorLine.DisplayNameHeader;
    public const string VisibleHeader = OverviewSensorLine.VisibleHeader;
    public const string SignalNameHeader = OverviewSensorLine.SignalNameHeader;
    public const string SourceHeader = OverviewSensorLine.SourceHeader;
    public const string SignalTypeHeader = OverviewSensorLine.SignalTypeHeader;
    public const string UnitHeader = OverviewSensorLine.UnitHeader;
    public const string ColorHeader = OverviewSensorLine.ColorHeader;
    public const string AxisHeader = OverviewSensorLine.AxisHeader;


    public SignalOverview()
    {
        InitializeComponent();


        SensorListView.PointerPressed += SensorListView_PointerReleased;
        Loaded += (sender, args) =>
        {
            _vm.OverviewSensorLines.CollectionChanged += OverviewSensorLinesOnCollectionChanged;
            SensorListView_ScaleHeaders(SensorListView, DataGridLengthUnitType.SizeToHeader);
            _vm.DataGrid_SelectedItems = SensorListView.SelectedItems;

            // Init right click menu
            _rightClickMenuFlyout.Items.Add(new MenuFlyoutItem()
                { Text = "Toggle signals", Command = new RelayCommand(_vm.ToggleSelectedSignalVisibility) });
            _rightClickMenuFlyout.Items.Add(new MenuFlyoutItem()
            {
                Text = "Hide signals", Command = new RelayCommand<bool>(_vm.SetSelectedVisibility),
                CommandParameter = false
            });
            _rightClickMenuFlyout.Items.Add(new MenuFlyoutItem()
            {
                Text = "Show signals", Command = new RelayCommand<bool>(_vm.SetSelectedVisibility),
                CommandParameter = true
            });
            _rightClickMenuFlyout.Items.Add(new MenuFlyoutItem()
                { Text = "Delete signals", Command = new RelayCommand(_vm.DeleteSelectedSignals) });
        };
    }

    public readonly SearchFlyout SearchFlyout = new SearchFlyout();
    private readonly MenuFlyout _rightClickMenuFlyout = new();


    private void SensorListView_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint(sender as UIElement).Properties;
        if (properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
        {
            OnRightTapped(e.GetCurrentPoint(SensorListView).Position);
        }
    }

    private void OnRightTapped(Point pointerPosition)
    {
        if (!(SensorListView.SelectedItems.Count > 0))
            return;

        _rightClickMenuFlyout.ShowAt(SensorListView, pointerPosition);
    }


    private void OverviewSensorLinesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null) return;
        SensorListView_ScaleHeaders(SensorListView, DataGridLengthUnitType.SizeToCells);
    }

    private void SensorListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _vm.OnSelectionChanged((sender as DataGrid)?.SelectedItems);
    }

    private void SensorListView_OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != Windows.System.VirtualKey.Space || !(SensorListView.SelectedItems.Count > 0))
            return;

        _vm.ToggleSelectedSignalVisibility();
    }

    private static void SensorListView_ScaleHeaders(DataGrid sensorListView, DataGridLengthUnitType sizeTo)
    {
        foreach (var dataGridColumn in sensorListView.Columns)
        {
            if (dataGridColumn.Header is ColorHeader) continue;
            dataGridColumn.Width = new DataGridLength(0, sizeTo);
        }
    }

    private void AutoSuggestBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var filterBy = SearchFlyout.SelectedItem;
        _vm.FilterSignals(args.QueryText, filterBy);
    }

    private void AutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        _vm.SearchBoxTextChanged(sender.Text);
    }

    private void SearchByComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SearchFlyout.UpdatePlaceHolderText(e.AddedItems[0].ToString());
    }

    private void ResetGrid_OnClick(object sender, RoutedEventArgs e)
    {
        SensorListView_ScaleHeaders(SensorListView,
            _vm.DataGridSensorLines.Any() ? DataGridLengthUnitType.SizeToCells : DataGridLengthUnitType.SizeToHeader);

        _vm.ResetGridContent();
    }

    #region _vm

    public static readonly DependencyProperty _vmProperty = DependencyProperty.Register(
        nameof(_vm), typeof(SignalOverviewVM), typeof(SignalOverview), new PropertyMetadata(default(SignalOverviewVM)));

    public SignalOverviewVM _vm
    {
        get { return (SignalOverviewVM)GetValue(_vmProperty); }
        set { SetValue(_vmProperty, value); }
    }

    #endregion
}