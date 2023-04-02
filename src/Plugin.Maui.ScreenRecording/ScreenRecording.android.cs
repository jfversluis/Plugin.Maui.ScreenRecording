namespace Plugin.Maui.ScreenRecording;

//TODO
partial class ScreenRecordingImplementation : IScreenRecording
{
	public bool IsRecording => throw new NotImplementedException();

	public bool IsSupported => throw new NotImplementedException();

	public Task StartRecording(bool enableMicrophone)
	{
		throw new NotImplementedException();
	}

	public Task<ScreenRecordingFile?> StopRecording(ScreenRecordingOptions? options)
	{
		throw new NotImplementedException();
	}
}