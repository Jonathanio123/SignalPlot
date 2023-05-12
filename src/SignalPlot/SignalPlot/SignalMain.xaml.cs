using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SignalPlot.Data.Infrastructure;
using SignalPlot.ViewModels;

namespace SignalPlot;

public sealed partial class SignalMain : Page
{
    public SignalMain()
    {
        InitializeComponent();
        _vm = new SignalMainViewModel(WinUiPlot, DigitalWinUiPlot, _signalOverviewVm);
        Loaded += (object sender, RoutedEventArgs e) =>
        {
            _vm.SetMainWindow(MainWindow);
            _vm.SetSignalSourceService(SignalSourceService);
        };
    }

    public Window MainWindow { get; set; }
    public SignalMainViewModel _vm { get; private set; }
    public SignalOverviewVM _signalOverviewVm { get; private set; } = new();
    public ISignalSourceService SignalSourceService { get; set; }
}