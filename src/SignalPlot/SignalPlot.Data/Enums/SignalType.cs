using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalPlot.Data.Enums;

public enum SignalType
{
    /// <summary>
    /// Not set, meaning unknown
    /// </summary>
    NotSett,
    /// <summary>
    /// This is an digital type meaning only values 0 and 1
    /// </summary>
    Digital,
    /// <summary>
    /// This is an analogue type meaning values include all rational numbers
    /// </summary>
    Analog,
    /// <summary>
    /// TBW
    /// </summary>
    EventLog
}