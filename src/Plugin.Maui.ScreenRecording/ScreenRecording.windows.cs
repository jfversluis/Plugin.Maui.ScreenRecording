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
    private bool _IsRecording { get; set; } = false;
    public bool IsSupported => GraphicsCaptureSession.IsSupported();
    private Recorder? rec;

    public async Task<bool> StartRecording(ScreenRecordingOptions? options)
    {
        if (!IsSupported)
            throw new Exception("Screen recording not supported on this device.");
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
            AudioOptions = new AudioOptions { IsInputDeviceEnabled = enableMicrophone, IsOutputDeviceEnabled = true, IsAudioEnabled = true }

        };
        rec = Recorder.CreateRecorder(opts);
        rec.OnRecordingComplete += (s, e) =>
        {
            path = e.FilePath;
            Debug.WriteLine("Recording completed");
            _IsRecording = false;
        };
        rec.Record(path = savePath);
        return true;
    }
    private string path { get; set; }
    public async Task<ScreenRecordingFile?> StopRecording()
    {
        rec?.Stop();
        rec?.Dispose();
        rec = null;
        return new(path);
    }
}