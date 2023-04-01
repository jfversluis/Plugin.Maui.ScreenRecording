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
		await RPScreenRecorder.SharedRecorder.StartRecordingAsync(enableMicrophone);
	}

	public async Task StopRecording(ScreenRecordingOptions? options)
	{
		if (RPScreenRecorder.SharedRecorder.Recording)
		{
			var saveOptions = options ?? new();

			var savePath = NSUrl.FromFilename(Path.Combine(Path.GetTempPath(),
				$"screenrecording_{DateTime.Now.ToString("ddMMyyyy_HHmmss")}.mp4"));

			if (!string.IsNullOrWhiteSpace(saveOptions.SavePath))
			{
				savePath = NSUrl.FromFilename(saveOptions.SavePath);
			}

			await RPScreenRecorder.SharedRecorder
				.StopRecordingAsync(savePath);

			var permissionResult =
				await Permissions.CheckStatusAsync<Permissions.PhotosAddOnly>();

			if (permissionResult != PermissionStatus.Granted)
			{
				// TODO what do we do here?
				return;
			}

			if (saveOptions.SaveToGallery)
			{
				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
				{
					PHAssetChangeRequest.FromVideo(savePath);
				}, (success, error) =>
				{
					// TODO what to do here?
				});
			}

			// TODO show preview view controller?
			// Show preview after recording, only on iOS/macOS
			//var cv = await RPScreenRecorder.SharedRecorder.StopRecordingAsync();

			//Platform.GetCurrentUIViewController().ShowViewController(cv, cv);
		}
	}
}