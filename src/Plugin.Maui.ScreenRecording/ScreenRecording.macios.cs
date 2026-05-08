using Foundation;
using Photos;
using ReplayKit;

namespace Plugin.Maui.ScreenRecording;

public partial class ScreenRecordingImplementation : IScreenRecording
{
	readonly ScreenRecordingOptions screenRecordingOptions = new();

	public bool IsRecording => RPScreenRecorder.SharedRecorder.Recording;

	public bool IsSupported => RPScreenRecorder.SharedRecorder.Available;

    public Task<bool> StartRecording(ScreenRecordingOptions? options)
	{
		if (options is not null)
		{
			screenRecordingOptions.SavePath = Path.Combine(Path.GetTempPath(),
				$"screenrecording_{DateTime.Now:ddMMyyyy_HHmmss}.mp4");

			if (!string.IsNullOrWhiteSpace(options.SavePath))
			{
				screenRecordingOptions.SavePath = options.SavePath;
			}

			screenRecordingOptions.EnableMicrophone = options.EnableMicrophone;
			screenRecordingOptions.SaveToGallery = options.SaveToGallery;
		}

		RPScreenRecorder.SharedRecorder.MicrophoneEnabled =
			screenRecordingOptions.EnableMicrophone;

		var tcs = new TaskCompletionSource<bool>();

        RPScreenRecorder.SharedRecorder.StartRecording(error =>
		{
			if (error is not null)
			{
				System.Diagnostics.Debug.WriteLine($"[ScreenRecording] Failed to start recording: {error.LocalizedDescription}");
			}

			tcs.TrySetResult(error == null);
		});

		return tcs.Task;
	}

	public async Task<ScreenRecordingFile?> StopRecording()
	{
		if (RPScreenRecorder.SharedRecorder.Recording)
		{
			var savePath = NSUrl.FromFilename(screenRecordingOptions.SavePath);

			await RPScreenRecorder.SharedRecorder.StopRecordingAsync(savePath);

			if (screenRecordingOptions.SaveToGallery)
			{
				var permissionResult =
					await Permissions.RequestAsync<Permissions.PhotosAddOnly>();

				if (permissionResult != PermissionStatus.Granted)
				{
					throw new ScreenRecordingException(
						"Photo library permission was not granted. The recording was saved to the temporary path but could not be added to the gallery.");
				}

				PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
				{
					PHAssetChangeRequest.FromVideo(savePath);
				}, (success, error) =>
				{
					if (!success && error is not null)
					{
						System.Diagnostics.Debug.WriteLine($"[ScreenRecording] Failed to save to photo library: {error.LocalizedDescription}");
					}
				});
			}

			return new ScreenRecordingFile(savePath.Path ?? string.Empty);
		}

		return null;
	}
}