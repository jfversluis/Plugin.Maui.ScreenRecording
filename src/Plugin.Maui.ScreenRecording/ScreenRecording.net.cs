namespace Plugin.Maui.ScreenRecording;

public partial class ScreenRecordingImplementation : IScreenRecording
{
	public bool IsRecording => throw new PlatformNotSupportedException();

	public bool IsSupported => throw new PlatformNotSupportedException();

	public void Setup()
	{
		throw new PlatformNotSupportedException();
	}
	public Task StartRecording(bool enableMicrophone)
	{
		throw new PlatformNotSupportedException();
	}

	public Task<ScreenRecordingFile?> StopRecording(ScreenRecordingOptions? options)
	{
		throw new PlatformNotSupportedException();
	}
}