using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SignalPlot.Data.Models;
using SignalPlot.Data.Models.Interfaces;

namespace SignalPlot.Data.Infrastructure;

public interface ISignalSourceService
{
    /// <summary>
    /// Returns a list of all available sources
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns> Readonly list of <see cref="IBaseSensorLine"/> </returns>
    Task<IReadOnlyList<IBaseSensorLine>> ListAvailableSourcesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Request a handler to interact with this analogue source
    /// </summary>
    /// <param name="sourSensorLine"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IDigitalSignalSource> GetDigitalSourceAsync(IBaseSensorLine sourSensorLine,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Request a handler to interact with this digital source
    /// </summary>
    /// <param name="sourSensorLine"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IAnalogueSignalSource> GetAnalogueSourceAsync(IBaseSensorLine sourSensorLine,
        CancellationToken cancellationToken = default);
}

public interface IAnalogueSignalSource : IBaseSensorLine, IAsyncDisposable
{
    /// <summary>
    /// Manually assign an ID to this source sensor line. If not manually set a random Guid wil be created.
    /// </summary>
    /// <param name="sensorLine"></param>
    void SetId(IBaseSensorLine sensorLine);
    /// <summary>
    /// Ask the source if it is available
    /// </summary>
    /// <returns><c>true</c> if source is available, and <c>false</c> if source is not available</returns>
    Task<bool> AvailableAsync();

    /// <summary>
    /// Get next value
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataEntry<double>> GetNextValueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Continually get next value, the enumerable will run until canceled or the source stops
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<DataEntry<double>> GetValuesAsyncEnumerable(CancellationToken cancellationToken = default);
}

public interface IDigitalSignalSource : IBaseSensorLine, IAsyncDisposable
{
    /// <inheritdoc cref="IAnalogueSignalSource.SetId"/>
    void SetId(IBaseSensorLine sensorLine);

    /// <inheritdoc cref="IAnalogueSignalSource.AvailableAsync"/>
    Task<bool> AvailableAsync();

    /// <inheritdoc cref="IAnalogueSignalSource.GetNextValueAsync"/>
    Task<bool> GetNextValueAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc cref="IAnalogueSignalSource.GetValuesAsyncEnumerable"/>
    IAsyncEnumerable<DataEntry<bool>> GetValuesAsyncEnumerable(CancellationToken cancellationToken = default);
}