using ReplayKit;

namespace Plugin.Maui.ScreenRecording;

partial class ScreenRecordingImplementation : IScreenRecording
{
	public bool IsRecording => RPScreenRecorder.SharedRecorder.Recording;

	public bool IsSupported => RPScreenRecorder.SharedRecorder.Available;

	public async Task StartRecording(bool enableMicrophone)
	{
		// TODO: else throw? Give option to throw?
		if (IsSupported)
		{
			await RPScreenRecorder.SharedRecorder.StartRecordingAsync(enableMicrophone);
		}
	}

	public async Task StopRecording()
	{
		if (RPScreenRecorder.SharedRecorder.Recording)
		{
			await RPScreenRecorder.SharedRecorder.StopRecordingAsync();

			// TODO save to disk
			// TODO save to gallery?
			// TODO show preview view controller?
		}
	}
}