/*using System.ComponentModel;
using SignalPlot.Data.Infrastructure.CsvSignalManager;
using SignalPlot.Data.Models;

namespace SignalPlot.Data.Tests
{
    public static class CsvSignalManagerTests
    {
        private static async Task<CsvSignalManager> InitCsvSignalManager(string path)
        {
            _ = path ?? throw new NullReferenceException();
            var signalManger = new CsvSignalManager();

            var lines = System.IO.File.ReadLinesAsync(path);

            await signalManger.ReadSignalsFromLinesAsync(lines);
            return signalManger;
        }

        [Fact]
        public static async void ReadAnalogueSignalsFromLinesAsyncTests_CheckIfCorrectDisplayNames()
        {
            foreach (var signalCsv in Constants.SignalCsv)
            {
                var signalManger = await InitCsvSignalManager(signalCsv.CsvFilePath);

                Assert.All(signalManger.GetAnalogueSensorLines(),
                    sensorLine => Assert.Contains(sensorLine.DisplayName, signalCsv.AnalogueDisplayNames));
            }
        }

        [Fact]
        public static async void ReadBinarySignalsFromLinesAsyncTests_CheckIfCorrectDisplayNames()
        {
            foreach (var signalCsv in Constants.SignalCsv)
            {
                var signalManger = await InitCsvSignalManager(signalCsv.CsvFilePath);

                Assert.All(signalManger.GetBoolSensorLines(),
                    sensorLine => Assert.Contains(sensorLine.DisplayName, signalCsv.DigitalDisplayNames));
            }
        }


        // TODO: Update ReadSignalsFromLinesAsyncTests_DataEntriesCount test
        // Currently uses old implementation of ReadSignalsFromLinesAsync
        // Should use ReadSignalsFromLines
        [Fact(Skip = "Needs to be rewritten")]
        public static async void ReadSignalsFromLinesAsyncTests_DataEntriesCount()
        {
            foreach (var signalCsv in Constants.SignalCsv)
            {
                var signalManger = await InitCsvSignalManager(signalCsv.CsvFilePath);


                Assert.All(signalManger.GetBoolSensorLines(),
                    sensorLine => Assert.True(sensorLine.DataEntries.Count == signalCsv.NumberOfDigitalDataEntries,
                        $"Digital data entries count is not correct for {sensorLine.SignalName}\n" +
                        $"{sensorLine.DataEntries.Count} != {signalCsv.NumberOfDigitalDataEntries}"));

                Assert.All(signalManger.GetAnalogueSensorLines(),
                    sensorLine => Assert.True(sensorLine.DataEntries.Count ==
                                              signalCsv.NumberOfAnalogueDataEntries[sensorLine.DisplayName]));
            }
        }

        // TODO: Add tests for signal names
        [Fact]
        public static async void ReadSignalsFromLinesAsyncTests_SignalNames()
        {
            foreach (var signalCsv in Constants.SignalCsv)
            {
                var signalManger = await InitCsvSignalManager(signalCsv.CsvFilePath);

                Assert.All(signalManger.GetBoolSensorLines(),
                    sensorLine => Assert.Contains(sensorLine.SignalName, signalCsv.SignalNames));

                Assert.All(signalManger.GetAnalogueSensorLines(),
                    sensorLine => Assert.Contains(sensorLine.SignalName, signalCsv.SignalNames));
            }
        }
    }
}*/