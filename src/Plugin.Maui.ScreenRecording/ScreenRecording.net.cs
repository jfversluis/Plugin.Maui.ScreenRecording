namespace Plugin.Maui.ScreenRecording;

public partial class ScreenRecordingImplementation : IScreenRecording
{
	public bool IsRecording => throw new PlatformNotSupportedException();

	public bool IsSupported => throw new PlatformNotSupportedException();

	public void StartRecording(ScreenRecordingOptions? options)
	{
		throw new PlatformNotSupportedException();
	}

	public Task<ScreenRecordingFile?> StopRecording()
	{
		throw new PlatformNotSupportedException();
	}
}