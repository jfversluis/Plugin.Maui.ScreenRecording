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

	public async Task<ScreenRecordingFile?> StopRecording(ScreenRecordingOptions? options)
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

			if (saveOptions.SaveToGallery)
			{
				var permissionResult =
					await Permissions.CheckStatusAsync<Permissions.PhotosAddOnly>();

				if (permissionResult != PermissionStatus.Granted)
				{
					// TODO what do we do here?
					return null;
				}

				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
				{
					PHAssetChangeRequest.FromVideo(savePath);
				}, (success, error) =>
				{
					// TODO what to do here?
				});
			}

			return new ScreenRecordingFile(savePath.Path ?? string.Empty);
			// TODO show preview view controller?
			// Show preview after recording, only on iOS/macOS
			//var cv = await RPScreenRecorder.SharedRecorder.StopRecordingAsync();

			//Platform.GetCurrentUIViewController().ShowViewController(cv, cv);
		}

		return null;
	}
}