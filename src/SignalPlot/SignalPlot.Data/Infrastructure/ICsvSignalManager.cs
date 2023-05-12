using System.Collections.Generic;
using System.Threading.Tasks;
using SignalPlot.Data.Models;

namespace SignalPlot.Data.Infrastructure;

public partial interface ICsvSignalManager
{
    public IReadOnlyCollection<AnalogueSensorLine> GetAnalogueSensorLines();
    public IReadOnlyCollection<DigitalSensorLine> GetBoolSensorLines();
    public Task ReadSignalsFromLinesAsync(IAsyncEnumerable<string> lines);
    public void ReadSignalsFromLines(ICollection<string> lines);
}