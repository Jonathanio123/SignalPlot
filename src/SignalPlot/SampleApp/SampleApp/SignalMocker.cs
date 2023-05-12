using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ScottPlot;
using SignalPlot.Data.Enums;
using SignalPlot.Data.Infrastructure;
using SignalPlot.Data.Models;
using SignalPlot.Data.Models.Interfaces;

namespace SampleApp;

internal abstract class SignalMockers
{
    internal interface IValueGenerator
    {
        public Task<double> GetNextValue(CancellationToken cancellationToken = default);
        public Task<DataEntry<double>> GetNextDataEntryAsync(CancellationToken cancellationToken = default);
    }

    public class SinusValueGenerator : IValueGenerator
    {
        private const int _period = 75;
        private static double[] _sinusValues = Array.Empty<double>();
        private readonly int _maxDelay = 110;
        private readonly double _maxNoise = 0;
        private readonly int _minDelay = 16;
        private readonly double _minNoise = 0;

        private readonly Random _random = new();
        private int index = 0;

        public SinusValueGenerator()
        {
            _sinusValues = Generate.NoisySin(_random, _period, 0);
        }


        public async Task<double> GetNextValue(CancellationToken cancellationToken = default)
        {
            await Task.Delay(_random.Next(_minDelay, _maxDelay), cancellationToken);
            return _GetNextValue();
        }

        public Task<DataEntry<double>> GetNextDataEntryAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private double _GetNextValue()
        {
            if (index == _sinusValues.Length - 1)
                index = 0;
            index++;
            return _sinusValues[index - 1];
        }
    }

    public class DoubleValueGenerator : IValueGenerator
    {
        private readonly int _maxDelay = 110;
        private readonly int _maxDelta = 10;
        private readonly int _maxNoise = 5;
        private readonly int _maxValue = 100;
        private readonly int _minDelay = 16;
        private readonly int _minDelta = -10;
        private readonly int _minNoise = -5;
        private readonly int _minValue = 0;

        private readonly Random _random = new();
        private int _currentValue = 0;

        public DoubleValueGenerator()
        {
        }

        public async Task<double> GetNextValue(CancellationToken cancellationToken = default)
        {
            var delta = _random.Next(_minDelta, _maxDelta);
            var noise = _random.Next(_minNoise, _maxNoise);
            _currentValue += delta + noise;
            if (_currentValue > _maxValue)
                _currentValue = _maxValue;
            else if (_currentValue < _minValue) _currentValue = _minValue;
            await Task.Delay(_random.Next(_minDelay, _maxDelay), CancellationToken.None);
            return _currentValue;
        }

        public async Task<DataEntry<double>> GetNextDataEntryAsync(CancellationToken cancellationToken = default)
        {
            return new DataEntry<double>(DateTime.Now, await GetNextValue(cancellationToken));
        }
    }

    public class PreGeneratedValuesGenerator : IValueGenerator
    {
        private readonly int _maxDelta = 10;
        private readonly int _maxNoise = 5;
        private readonly int _maxValue = 100;
        private readonly int _minDelta = -10;
        private readonly int _minNoise = -5;
        private readonly int _minValue = 0;
        private int _currentValue = 0;


        private readonly Random _random;
        private readonly DateTime[] _timeStamps;
        private readonly double[] _values;
        private int _index = 0;

        public PreGeneratedValuesGenerator(int seed = 0, int amount = 200_000, int maxTime = 100, int minTime = 16)
        {
            _random = new Random(seed);

            var startTime = DateTime.Now;

            _timeStamps = new DateTime[amount];
            _values = new double[amount];
            for (int i = 0; i < amount; i++)
            {
                // Chatgpt code
                var delta = _random.Next(_minDelta, _maxDelta);
                var noise = _random.Next(_minNoise, _maxNoise);
                _currentValue += delta + noise;
                if (_currentValue > _maxValue)
                    _currentValue = _maxValue;
                else if (_currentValue < _minValue) _currentValue = _minValue;
                // Chatgpt code

                _values[i] = _currentValue;

                if (_timeStamps[0] == default(DateTime))
                    _timeStamps[i] = startTime;
                else
                    _timeStamps[i] = _timeStamps[i - 1] + TimeSpan.FromMilliseconds(_random.Next(minTime, maxTime));
            }
        }

        public Task<double> GetNextValue(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<DataEntry<double>> GetNextDataEntryAsync(CancellationToken cancellationToken = default)
        {
            if (_index == _timeStamps.Length - 1)
            {
                Debug.WriteLine($"[{nameof(GetNextDataEntryAsync)}] Resetting index");
                _index = 0;
            }

            await Task.Delay(_random.Next(16, 110), CancellationToken.None);
            var returnValue = new DataEntry<double>(_timeStamps[_index], _values[_index]);
            _index++;
            return returnValue;
        }
    }

    public sealed class RandomValuesSourceMocked : ISignalSourceService
    {
        private readonly List<IAnalogueSignalSource> _availableAnalogueSources = new();
        private readonly List<IDigitalSignalSource> _availableDigitalSources = new();

        public RandomValuesSourceMocked(int analogueSources, int digitalSources)
        {
            for (int i = 0; i < analogueSources; i++)
                _availableAnalogueSources.Add(new AnalogueSignalSourceMocked(
                    new PreGeneratedValuesGenerator(seed: i * 10),
                    $"#{i}D"));

            // _availableAnalogueSources.Add(new AnalogueSignalSourceMocked(new DoubleValueGenerator(),$"#{i}D"));

            //for (int i = 0; i < digitalSources; i++) _availableSources.Add(new DigitalSensorLine());
        }

        public async Task<IReadOnlyList<IBaseSensorLine>> ListAvailableSourcesAsync(
            CancellationToken cancellationToken = default)
        {
            await Task.Delay(300, cancellationToken);
            List<IBaseSensorLine> sources = new();
            sources.AddRange(_availableAnalogueSources);
            sources.AddRange(_availableDigitalSources);
            return sources.AsReadOnly();
        }

        public Task<IDigitalSignalSource> GetDigitalSourceAsync(IBaseSensorLine sourSensorLine,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IAnalogueSignalSource> GetAnalogueSourceAsync(IBaseSensorLine sourSensorLine,
            CancellationToken cancellationToken = default)
        {
            await Task.Delay(10, cancellationToken);
            return _availableAnalogueSources.FirstOrDefault(availableAnalogueSource =>
                availableAnalogueSource.Id == sourSensorLine.Id);
        }
    }


    public sealed class AnalogueSignalSourceMocked : BaseSensorLine, IAnalogueSignalSource
    {
        private readonly IValueGenerator _doubleValueGenerator;
        private readonly AnalogueSensorLine _liveSensorLine;
        public IReadOnlyList<IDataEntry<double>> CurrentLiveSensorLines => _liveSensorLine.DataEntries;

        internal AnalogueSignalSourceMocked(IValueGenerator doubleValueGenerator, string appendedString)
        {
            _doubleValueGenerator = doubleValueGenerator;
            DisplayName = "DisplayName" + appendedString;
            SignalName = "SignalName" + appendedString;
            Source = "Source" + appendedString;
            Unit = "Unit" + appendedString;
            SignalType = SignalType.Analog;
            _liveSensorLine = new AnalogueSensorLine(DisplayName, SignalName, Source, Unit, SignalType);
        }

        public void SetId(IBaseSensorLine sensorLine)
            => Id = sensorLine.Id;


        public new Guid Id { get; private set; } = Guid.NewGuid();

        public async Task<bool> AvailableAsync()
        {
            await Task.Delay(50);
            return true;
        }


        public async Task<DataEntry<double>> GetNextValueAsync(CancellationToken cancellationToken = default)
        {
            var dateTime = DateTime.Now;
            var data = await _doubleValueGenerator.GetNextValue(cancellationToken);
            var dataEntry = new DataEntry<double>(dateTime, data);
            _liveSensorLine.AddEntry(dataEntry);

            return dataEntry;
        }

        public async IAsyncEnumerable<DataEntry<double>> GetValuesAsyncEnumerable(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
                yield return await _doubleValueGenerator.GetNextDataEntryAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            Debug.WriteLine($"Disposing source: {SignalName}");
            return ValueTask.CompletedTask;
        }
    }

    public sealed class DigitalSignalSourceMocked : BaseSensorLine, IDigitalSignalSource
    {
        public async ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public void SetId(IBaseSensorLine sensorLine)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AvailableAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> GetNextValueAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<DataEntry<bool>> GetValuesAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}