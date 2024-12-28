using Android.Content;
using Android.Hardware.Display;
using Android.Media;
using Android.Media.Projection;
using Android.OS;
using Android.Views;
using Application = Android.App.Application;

namespace Plugin.Maui.ScreenRecording;

public partial class ScreenRecordingImplementation : MediaProjection.Callback, IScreenRecording
{
	private const int RequestMediaProjectionCode = 1;
	private string? filePath;
	private bool enableMicrophone;
    private TaskCompletionSource<bool>? serviceStartAwaiter;

    string NotificationContentTitle { get; set; } =
		ScreenRecordingOptions.defaultAndroidNotificationTitle;

	string NotificationContentText { get; set; } =
		ScreenRecordingOptions.defaultAndroidNotificationText;

	MediaProjectionManager? ProjectionManager { get; set; }
	MediaProjection? MediaProjection { get; set; }
	VirtualDisplay? VirtualDisplay { get; set; }
	MediaRecorder? MediaRecorder { get; set; }

	public bool IsRecording { get; private set; }

	public bool IsSupported => ProjectionManager is not null;

	bool isSavingToGallery;

	public ScreenRecordingImplementation()
	{
		ProjectionManager = (MediaProjectionManager?)Platform.AppContext.GetSystemService(Context.MediaProjectionService);
	}

	public void StartRecording(ScreenRecordingOptions? options = null)
	{
		if (!IsSupported)
		{
			throw new NotSupportedException("Screen recording not supported on this device.");
		}
		
		enableMicrophone = options?.EnableMicrophone ?? false;

        if (!string.IsNullOrWhiteSpace(options?.NotificationContentTitle))
		{
			NotificationContentTitle = options.NotificationContentTitle;
		}

		if (!string.IsNullOrWhiteSpace(options?.NotificationContentText))
		{
			NotificationContentText = options.NotificationContentText;
		}

		var saveOptions = options ?? new();
		var savePath = saveOptions.SavePath;

		if (string.IsNullOrWhiteSpace(savePath))
		{
			savePath = Path.Combine(Path.GetTempPath(),
				$"screenrecording_{DateTime.Now:ddMMyyyy_HHmmss}.mp4");
		}

		if (saveOptions.SaveToGallery)
		{
			isSavingToGallery = saveOptions.SaveToGallery;
			Java.IO.File picturesDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures)!;
			string fileName = Path.GetFileName(savePath);
			Java.IO.File destinationFile = new(picturesDirectory, fileName);
			filePath = destinationFile.AbsolutePath;
		}
		else
		{
			filePath = savePath;
		}

		Setup();
	}

	public Task<ScreenRecordingFile?> StopRecording()
	{
		if (IsRecording)
		{
			IsRecording = false;

			try
			{
				MediaRecorder?.Stop();
				MediaRecorder?.Release();
				MediaRecorder = null;
				VirtualDisplay?.Release();
			}
			catch (Exception ex)
			{
				Console.WriteLine("STACK TRACE:");
				Console.WriteLine(ex.StackTrace);

				Console.WriteLine("Exception Message:");
				Console.WriteLine(ex.Message);

				if (ex.InnerException != null)
				{
					Console.WriteLine("INNER Exception Message:");
					Console.WriteLine(ex.InnerException.Message);
				}
			}
			finally
			{
				MediaProjection?.Stop();
			}

			var context = Application.Context;
            context.StopService(new Intent(context, typeof(ScreenRecordingService)));

            return Task.FromResult<ScreenRecordingFile?>(new ScreenRecordingFile(filePath ?? string.Empty));
		}

		return Task.FromResult<ScreenRecordingFile?>(null);
	}

	internal async void OnScreenCapturePermissionGranted(int resultCode, Intent? data)
    {
		serviceStartAwaiter = new TaskCompletionSource<bool>();

        var context = Application.Context;
        var messenger = new Messenger(new ExternalHandler(serviceStartAwaiter));

        Intent notificationSetup = new(context, typeof(ScreenRecordingService));
        notificationSetup.PutExtra(ScreenRecordingService.ExtraCommandNotificationSetup, true);
        notificationSetup.PutExtra(ScreenRecordingService.ExtraExternalMessenger, messenger);
        notificationSetup.PutExtra(ScreenRecordingService.ExtraContentTitle, NotificationContentTitle);
        notificationSetup.PutExtra(ScreenRecordingService.ExtraContentText, NotificationContentText);

		// Android O
		if (OperatingSystem.IsAndroidVersionAtLeast(26))
		{
            context.StartForegroundService(notificationSetup);
		}
		else
		{
			context.StartService(notificationSetup);
        }

		await serviceStartAwaiter.Task;

        // Prepare MediaProjection which will be later be used by the ScreenRecordingService
        // and call the BeginRecording()
        MediaProjection = ProjectionManager?.GetMediaProjection(resultCode, data!);
        MediaProjection?.RegisterCallback(this, null);

		Intent beginRecording = new(context, typeof(ScreenRecordingService));
        beginRecording.PutExtra(ScreenRecordingService.ExtraCommandBeginRecording, true);

        // Android O
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            context.StartForegroundService(beginRecording);
        }
        else
        {
            context.StartService(beginRecording);
        }
    }

	public void BeginRecording()
	{
        try
        {
            MediaRecorder = SetUpMediaRecorder(enableMicrophone);
            MediaRecorder.Start();
            IsRecording = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new NotSupportedException("Screen recording did not start.");
        }
    }

	private MediaRecorder SetUpMediaRecorder(bool enableMicrophone)
	{
        if (MediaRecorder is not null)
        {
            MediaRecorder.Reset();
        }
        else
        {
            // Android S
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                MediaRecorder = new(Application.Context);
            }
            else
            {
                MediaRecorder = new();
            }
        }

		MediaRecorder.SetVideoSource(VideoSource.Surface);

		if (enableMicrophone)
		{
			MediaRecorder.SetAudioSource(AudioSource.Mic);
		}

		MediaRecorder.SetOutputFormat(OutputFormat.Mpeg4);
		MediaRecorder.SetVideoEncoder(VideoEncoder.H264);

        if (enableMicrophone)
		{
			MediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
		}

		var (width, height, density, frameRate, bitRate) = GetDefaultSettings();

		MediaRecorder.SetVideoSize(width, height);
		MediaRecorder.SetVideoFrameRate(frameRate);
        MediaRecorder.SetVideoEncodingBitRate(bitRate);
        MediaRecorder.SetOutputFile(filePath);

		if (isSavingToGallery)
		{
			//This provides a way for applications to pass a newly created or downloaded media file to the media scanner service.
			//The media scanner service will read metadata from the file and add the file to the media content provider.
			//Source:https://developer.android.com/reference/android/media/MediaScannerConnection
			MediaScannerConnection.ScanFile(Application.Context,
									[filePath!],
									["video/mp4"],
									null);
		}

		try
		{
			MediaRecorder.Prepare();

			MyVirtualDisplayCallback mVirtualDisplayCallback = new();

			VirtualDisplay = MediaProjection?.CreateVirtualDisplay("ScreenCapture",
				width, height, density, DisplayFlags.Presentation,
				MediaRecorder.Surface, mVirtualDisplayCallback, null);
		}
		catch (Java.IO.IOException ex)
		{
			Console.WriteLine($"MediaRecorder preparation failed: {ex.Message}");
            // Handle preparation failure
            VirtualDisplay?.Release();
            MediaRecorder?.Release();
			throw;
		}

		return MediaRecorder;
	}

	public void Setup()
	{
		if (ProjectionManager is not null)
		{
			Intent captureIntent = ProjectionManager.CreateScreenCaptureIntent();
			Platform.CurrentActivity?.StartActivityForResult(captureIntent, RequestMediaProjectionCode);
		}
	}

	public override void OnStop()
	{
		base.OnStop();

		VirtualDisplay?.Release();
		MediaRecorder?.Release();
	}

    private static (int Width, int Height, int Density, int FrameRate, int BitRate) GetDefaultSettings()
    {
        int width = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
        int height = (int)DeviceDisplay.Current.MainDisplayInfo.Height;
        int density = (int)DeviceDisplay.Current.MainDisplayInfo.Density;

        // A higher frame rate (e.g., 60 FPS) results in smoother video but increases file size and processing requirements.
        // A lower frame rate (e.g., 15 FPS) reduces file size but may result in choppy video, especially for high-motion content.
        int frameRate = 30;

        // A higher value results in better quality but larger file sizes, while a lower value reduces quality.
        int bitRate = 3;

        return (width, height, density, frameRate, bitRate);
    }
}

internal class MyVirtualDisplayCallback : VirtualDisplay.Callback
{
	public override void OnPaused()
	{
		base.OnPaused();
	}

	public override void OnResumed()
	{
		base.OnResumed();
	}

	public override void OnStopped()
	{
		base.OnStopped();
	}
}

internal class ExternalHandler(TaskCompletionSource<bool> tcs) : Handler
{
    private readonly TaskCompletionSource<bool> _tcs = tcs;

    public override void HandleMessage(Message msg)
    {
        if (msg.What == ScreenRecordingService.MsgServiceStarted)
        {
            _tcs.TrySetResult(true);
        }
    }
}
