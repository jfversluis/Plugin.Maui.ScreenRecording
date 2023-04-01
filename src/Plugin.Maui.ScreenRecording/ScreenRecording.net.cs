namespace Plugin.Maui.ScreenRecording;

partial class ScreenRecordingImplementation : IScreenRecording
{
	public bool IsRecording => throw new PlatformNotSupportedException();

	public bool IsSupported => throw new PlatformNotSupportedException();

	public Task StartRecording(bool enableMicrophone)
	{
		throw new PlatformNotSupportedException();
	}

	public Task StopRecording(ScreenRecordingOptions? options)
	{
		throw new PlatformNotSupportedException();
	}
}