#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using Microsoft.UI.Xaml;
using SignalPlot.Data.Models;

// ReSharper disable once CheckNamespace
namespace SignalPlot.ViewModels;

public partial class SignalMainViewModel
{
    private IFilePickerWrapper? _filePicker;

    /// <summary>
    /// This is method is used to replace the _filePicker with a mocked version for testing.
    /// </summary>
    /// <param name="filePickerWrapper"></param>
    internal void SetFilePickerWrapper(IFilePickerWrapper filePickerWrapper)
        => _filePicker = filePickerWrapper;

    private bool CanReadFile()
        => !StartReadingCommand.IsRunning;
    
    [RelayCommand(CanExecute = nameof(CanReadFile))]
    private async Task ReadFileAsync(CancellationToken cancellationToken = default)
    {
        _filePicker ??= new FilePickerWrapper(_window);

        var pickedFile = await _filePicker.PickSingleFileAsync();
        if (pickedFile == null) return;
        IList<string> lines;
        try
        {
            lines = await _filePicker.ReadLinesAsync(pickedFile, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        try
        {
            _csvSignalManager.ReadSignalsFromLines(lines);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error reading: {pickedFile.Name}\n" +
                            $"\tException: {e}");
            return;
        }

        _signalOverviewVm.ClearSignals();
        _signalPlotManager.ClearSignals();


        var signalOverviews = new List<OverviewSensorLine>();

        foreach (var analogueSensorLine in _csvSignalManager.GetAnalogueSensorLines())
        {
            if (analogueSensorLine.DataEntries.Count == 0) continue;

            var scatterSignal = _signalPlotManager.AddSignal(analogueSensorLine);
            var sColor = scatterSignal.LineStyle.Color;
            signalOverviews.Add(new(analogueSensorLine,
                Color.FromArgb(sColor.Alpha, sColor.Red, sColor.Green, sColor.Blue)));
        }


        foreach (var boolSensorLine in _csvSignalManager.GetBoolSensorLines())
        {
            if (boolSensorLine.DataEntries.Count == 0) continue;
            var barsSignal = _signalPlotManager.AddSignal(boolSensorLine);
            var sColor = barsSignal.Series.FirstOrDefault()!.Color;

            signalOverviews.Add(new(boolSensorLine,
                Color.FromArgb(sColor.Alpha, sColor.Red, sColor.Green, sColor.Blue)));
        }


        signalOverviews.ForEach(s => _signalOverviewVm.AddSensorLine(s));

        _signalPlotManager.FrameAllSignals();
    }

    internal interface IFilePickerWrapper
    {
        /// <inheritdoc cref="FileOpenPicker.PickSingleFileAsync(string)"/>
        Task<StorageFile?> PickSingleFileAsync();

        /// <summary>
        /// <inheritdoc cref="FileIO.ReadLinesAsync(Windows.Storage.IStorageFile)"/>
        /// </summary>
        /// <param name="pickedFile"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException"></exception>
        Task<IList<string>> ReadLinesAsync(StorageFile pickedFile, CancellationToken cancellationToken = default);
    }

    internal class FilePickerWrapper : IFilePickerWrapper
    {
        private FileOpenPicker? _filePicker = null;
        private readonly Window _window;

        public FilePickerWrapper(Window window)
        {
            _window = window;
        }

        public async Task<StorageFile?> PickSingleFileAsync()
        {
            _filePicker ??= InitFilePicker();
            return await _filePicker.PickSingleFileAsync();
        }

        public async Task<IList<string>> ReadLinesAsync(StorageFile pickedFile,
            CancellationToken cancellationToken = default)
            => await Task.Run(async () => await FileIO.ReadLinesAsync(pickedFile), cancellationToken);

        private FileOpenPicker InitFilePicker()
        {
            var filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            filePicker.FileTypeFilter.Add(".csv");
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_window);
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);
            return filePicker;
        }
    }
}