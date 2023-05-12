using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SignalPlot.Data.Enums;
using SignalPlot.Data.Models;

namespace SignalPlot.Data.Infrastructure.CsvSignalManager;

public partial class CsvSignalManager : ICsvSignalManager
{
    [Obsolete("This method is obsolete, use ReadSignalsFromLines instead")]
    public async Task ReadSignalsFromLinesAsync(IAsyncEnumerable<string> lines)
    {
        _analogueSensorLines.Clear();
        _binarySensorLines.Clear();

        var readingUnitType = SignalType.NotSett;
        await foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                readingUnitType = SignalType.NotSett;
                continue;
            }

            if (line.Contains("DisplayName"))
            {
                ReadHeaderLine(line, ref readingUnitType);
                continue;
            }

            AddDataEntry(line, ref readingUnitType);
        }

        Debug.WriteLine("Finished converting logs to SignalLists");
    }

    // This is the one that's actually used
    // TODO: Add a test for ReadSignalsFromLines
    // Because this one removes digital values it should have its own test
    public void ReadSignalsFromLines(ICollection<string> lines)
    {
        _analogueSensorLines.Clear();
        _binarySensorLines.Clear();

        var readingUnitType = SignalType.NotSett;
        var lastDataEntry = new DataEntry<bool>(DateTime.UnixEpoch, false);
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                AddLastDataEntry(lastDataEntry, ref readingUnitType);
                readingUnitType = SignalType.NotSett;
                continue;
            }

            if (line.Contains("DisplayName"))
            {
                ReadHeaderLine(line, ref readingUnitType);
                lastDataEntry = new DataEntry<bool>(DateTime.UnixEpoch, false);
                continue;
            }

            AddDataEntry(line, ref readingUnitType, ref lastDataEntry);
        }

        Debug.WriteLine("Finished converting logs to SignalLists");
    }


    private static void AddLastDataEntry(DataEntry<bool> lastDataEntry, ref SignalType readingUnitType)
    {
        if (readingUnitType != SignalType.Digital) return;

        var sensorLine = _binarySensorLines.Last();
        if (!sensorLine.DataEntries.Any()) return;

        if (sensorLine.DataEntries.Last().Equals(lastDataEntry) || lastDataEntry.Timestamp.Equals(DateTime.UnixEpoch))
            return;
        sensorLine.AddEntry(lastDataEntry);
    }

    private static void AddDataEntry(string line, ref SignalType readingUnitType, ref DataEntry<bool> lastDataEntry)
    {
        var values = line.Split(',');
        var date = DateTime.ParseExact(values[0], "dd:MM:yyyy HH:mm:ss:fff",
            CultureInfo.InvariantCulture);

        if (readingUnitType == SignalType.Digital)
        {
            var value = Convert.ToBoolean(Convert.ToInt16(values[1]));
            var sensorLine = _binarySensorLines.Last();

            // This is a hacky way to remove duplicate entries to improve performance
            // Is it acceptable to remove duplicate entries?
            // There can be multiple digital points with the same timestamp but different values
            // See SignalPlotData2.csv rows
            // 21:02:2023 14:01:36:560	1			
            // 21:02:2023 14:01:36:560	0			
            // 21:02:2023 14:01:36:560	0			

            var isNotFirstEntry = !lastDataEntry.Timestamp.Equals(DateTime.UnixEpoch);
            var isNewEntryDifferentValue = lastDataEntry.Value != value;
            var data = new DataEntry<bool>(date, value);

            if (isNotFirstEntry)
            {
                if (isNewEntryDifferentValue)
                {
                    sensorLine.AddEntry(lastDataEntry);
                    sensorLine.AddEntry(data);
                }

                lastDataEntry = data;
                return;
            }

            sensorLine.AddEntry(data);
            lastDataEntry = data;
            return;
        }
        else if (readingUnitType == SignalType.Analog)
        {
            var value = double.Parse(values[1], NumberStyles.AllowDecimalPoint |
                                                NumberStyles.AllowExponent |
                                                NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture);

            var data = new DataEntry<double>(date, value);
            var sensorLine = _analogueSensorLines.Last();
            sensorLine.AddEntry(data);
        }
    }

    #region Old version with no duplicate removal

    private static void AddDataEntry(string line, ref SignalType readingUnitType)
    {
        var values = line.Split(',');
        var date = DateTime.ParseExact(values[0], "dd:MM:yyyy HH:mm:ss:fff",
            CultureInfo.InvariantCulture);

        if (readingUnitType == SignalType.Digital)
        {
            var value = Convert.ToBoolean(Convert.ToInt16(values[1]));
            var sensorLine = _binarySensorLines.Last();
            var data = new DataEntry<bool>(date, value);
            sensorLine.AddEntry(data);
        }
        else if (readingUnitType == SignalType.Analog)
        {
            var value = double.Parse(values[1], NumberStyles.AllowDecimalPoint |
                                                NumberStyles.AllowExponent |
                                                NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture);

            var data = new DataEntry<double>(date, value);
            var sensorLine = _analogueSensorLines.Last();
            sensorLine.AddEntry(data);
        }
    }

    #endregion

    private static void ReadHeaderLine(string line, ref SignalType readingUnitType)
    {
        var lineSplit = line.Split(',');
        var displayname_line = lineSplit[0].Split(": ");
        var signalname_line = lineSplit[1].Split(": ");
        var source_line = lineSplit[2].Split(": ");
        var unit_line = lineSplit[3].Split(": ");
        var signaltype_line = lineSplit[4].Split(": ");
        var displayName = displayname_line[1].Trim();
        var signalName = signalname_line[1].Trim();
        var source = source_line[1].Trim();
        var unit = unit_line[1].Trim();
        var signalType = signaltype_line[1].Trim() switch
        {
            nameof(SignalType.Digital) => SignalType.Digital,
            nameof(SignalType.Analog) => SignalType.Analog,
            nameof(SignalType.EventLog) => SignalType.EventLog,
            _ => throw new ArgumentOutOfRangeException("SignalType not recognised")
        };


        if (signalType == SignalType.Digital)
        {
            readingUnitType = SignalType.Digital;
            _binarySensorLines.Add(new DigitalSensorLine(displayName, signalName, source, unit, signalType));
        }
        else if (signalType == SignalType.Analog)
        {
            readingUnitType = SignalType.Analog;
            _analogueSensorLines.Add(new AnalogueSensorLine(displayName, signalName, source, unit, signalType));
        }
        else if (signalType == SignalType.EventLog)
        {
            Debug.WriteLine("EventLog not implemented yet");
            readingUnitType = SignalType.EventLog;
        }
    }
}