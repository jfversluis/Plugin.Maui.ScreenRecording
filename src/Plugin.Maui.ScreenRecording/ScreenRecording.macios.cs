using Microsoft.Maui.ApplicationModel;
using Photos;
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
			var tempPath = NSUrl.FromFilename(Path.Combine(Path.GetTempPath(),
				$"screenrecording-{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}.mp4"));

			await RPScreenRecorder.SharedRecorder.StopRecordingAsync(tempPath);

			var permissionResult = await Permissions.RequestAsync<Permissions.PhotosAddOnly>();

			if (permissionResult != PermissionStatus.Granted)
			{
				// TODO what do we do here?
				return;
			}

			PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
			{
				PHAssetChangeRequest.FromVideo(tempPath);
			}, (success, error) =>
			{
				// TODO what to do here?
			});

			// TODO show preview view controller?
			// Show preview after recording, only on iOS/macOS
			//var cv = await RPScreenRecorder.SharedRecorder.StopRecordingAsync();

			//Platform.GetCurrentUIViewController().ShowViewController(cv, cv);
		}
	}
}