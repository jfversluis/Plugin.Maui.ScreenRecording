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
			// TODO do something with error?
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