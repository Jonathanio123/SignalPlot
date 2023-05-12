using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SignalPlot.Data.Infrastructure;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SampleApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Window MainWindow { get; } = App.MainWindow;
        public ISignalSourceService SignalSourceService { get; set; } = new SignalMockers.RandomValuesSourceMocked(15, 0);

        public MainPage()
        {
            this.InitializeComponent();
            MainWindow.Title = "SignalPlot Component";
        }
    }
}