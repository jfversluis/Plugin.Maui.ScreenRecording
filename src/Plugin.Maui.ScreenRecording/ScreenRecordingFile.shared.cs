namespace Plugin.Maui.ScreenRecording;

/// <summary>
/// Represents a screen recording file that results from a screen recording.
/// </summary>
public class ScreenRecordingFile(string fullPath) : FileResult(fullPath)
{
}
