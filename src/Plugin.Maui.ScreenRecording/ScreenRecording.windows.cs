using System.Diagnostics;
using ScreenRecorderLib;
using Windows.Graphics.Capture;

namespace Plugin.Maui.ScreenRecording;

public partial class ScreenRecordingImplementation : IScreenRecording
{
    public ScreenRecordingImplementation()
    {
    }

    public bool IsRecording => _IsRecording;
    private bool _IsRecording { get; set; }
    public bool IsSupported => GraphicsCaptureSession.IsSupported();
    private Recorder? rec;
    private string? filePath;

    public Task<bool> StartRecording(ScreenRecordingOptions? options)
    {
        if (!IsSupported)
        {
            throw new NotSupportedException("Screen recording is not supported on this device.");
        }

        var enableMicrophone = options?.EnableMicrophone ?? false;

        var saveOptions = options ?? new();
        var savePath = saveOptions.SavePath;
        var filename = $"screenrecording_{DateTime.Now:ddMMyyyy_HHmmss}.mp4";

        if (string.IsNullOrWhiteSpace(savePath))
        {
            savePath = Path.Combine(Path.GetTempPath(), filename);
        }

        if (saveOptions.SaveToGallery)
        {
            string myVideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            savePath = Path.Combine(myVideosPath, filename);
        }

        var source = DisplayRecordingSource.MainMonitor;
        source.RecorderApi = RecorderApi.WindowsGraphicsCapture;

        var opts = new RecorderOptions
        {
            SourceOptions = new SourceOptions
            {
                RecordingSources = { { source } },
            },
            AudioOptions = new AudioOptions
            {
                IsInputDeviceEnabled = enableMicrophone,
                IsOutputDeviceEnabled = true,
                IsAudioEnabled = true
            }
        };

        rec = Recorder.CreateRecorder(opts);
        rec.OnRecordingFailed += (s, e) =>
        {
            Debug.WriteLine($"[ScreenRecording] Recording failed: {e.Error}");
            _IsRecording = false;
        };
        rec.OnRecordingComplete += (s, e) =>
        {
            filePath = e.FilePath;
            Debug.WriteLine("[ScreenRecording] Recording completed");
            _IsRecording = false;
        };

        filePath = savePath;
        rec.Record(savePath);
        _IsRecording = true;

        return Task.FromResult(true);
    }

    public Task<ScreenRecordingFile?> StopRecording()
    {
        if (!IsRecording && rec is null)
        {
            return Task.FromResult<ScreenRecordingFile?>(null);
        }

        rec?.Stop();
        rec?.Dispose();
        rec = null;
        _IsRecording = false;

        return Task.FromResult(string.IsNullOrEmpty(filePath)
            ? null
            : new ScreenRecordingFile(filePath));
    }
}