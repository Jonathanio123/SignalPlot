using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    [RelayCommand]
    private void Export()
    {
        NotImplementedFlyoutIsOpen = true;
    }
}