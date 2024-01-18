using Android.Content;
using Android.Media;
using Android.Media.Projection;
using Android.Hardware.Display;
using Android.Views;
using Android.Content.PM;
using Android.OS;
using Android.App;
using Application = Android.App.Application;

namespace Plugin.Maui.ScreenRecording;

public partial class ScreenRecordingImplementation : MediaProjection.Callback, IScreenRecording
{
	public MediaProjectionManager? ProjectionManager { get; set; }
	public MediaProjection? MediaProjection { get; set; }
	public VirtualDisplay? VirtualDisplay { get; set; }
	public MediaRecorder? MediaRecorder { get; set; }
	public string? FilePath;
	public const int REQUEST_MEDIA_PROJECTION = 1;

	public static EventHandler<ScreenRecordingEventArgs>? ScreenRecordingPermissionHandler;

	public bool IsRecording { get; private set; }

	public bool IsSupported { get; private set; } = true;

	bool enableMicrophone;

	public Task StartRecording(bool enableMicrophone)
	{
		if (IsSupported)
		{
			this.enableMicrophone = enableMicrophone;
			Setup();
		}
		else
		{
			throw new NotSupportedException("Screen recording not supported on this device.");
		}

		return Task.CompletedTask;
	}

	public Task<ScreenRecordingFile?> StopRecording(ScreenRecordingOptions? options)
	{
		if (IsRecording)
		{
			IsRecording = false;
			try
			{
				MediaRecorder?.Stop();
				MediaRecorder?.Release();
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

			return Task.FromResult<ScreenRecordingFile?>(new ScreenRecordingFile(FilePath));
		}

		return Task.FromResult<ScreenRecordingFile?>(null);
	}

	public async void OnScreenCapturePermissionGranted(int resultCode, Intent? data)
	{
		Intent intent = new(Application.Context, typeof(ScreenRecordingService));
		if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
		{
			Application.Context.StartForegroundService(intent);
		}
		else
		{
			Application.Context.StartService(intent);
		}
		// Additional setup or start recording
		await Task.Delay(1000);

		MediaProjection = ProjectionManager?.GetMediaProjection(resultCode, data);
		MediaProjection?.RegisterCallback(this, null);

		if (MediaRecorder != null)
		{
			MediaRecorder.Reset();
		}
		else
		{
			if (Build.VERSION.SdkInt > BuildVersionCodes.R)
			{
				MediaRecorder = new(Application.Context);
			}
			else
			{
				MediaRecorder = new();
			}
		}

		// TODO make this more unique?
		FilePath = Path.Combine(Application.Context.FilesDir.AbsolutePath, "Screen.mp4");

		try
		{
			SetUpMediaRecorder(enableMicrophone);
			MediaRecorder.Start();
			IsRecording = true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			throw new NotSupportedException("Screen recording did not start.");
		}
	}

	public void SetUpMediaRecorder(bool enableMicrophone)
	{
		MediaRecorder?.SetVideoSource(VideoSource.Surface);

		if (enableMicrophone)
		{
			MediaRecorder?.SetAudioSource(AudioSource.Mic);
		}

		MediaRecorder?.SetOutputFormat(OutputFormat.Mpeg4);
		MediaRecorder?.SetVideoEncoder(VideoEncoder.H264);

		if (enableMicrophone)
		{
			MediaRecorder?.SetAudioEncoder(AudioEncoder.AmrNb);
		}

		int width = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
		int height = (int)DeviceDisplay.Current.MainDisplayInfo.Height;
		int density = (int)DeviceDisplay.Current.MainDisplayInfo.Density;
		MediaRecorder?.SetVideoSize(width, height);
		MediaRecorder?.SetVideoFrameRate(30);
		MediaRecorder?.SetOutputFile(FilePath);

		try
		{
			MediaRecorder?.Prepare();

			MyVirtualDisplayCallback mVirtualDisplayCallback = new();

			// TODO Seems to never return from here???
			HandlerThread handlerThread = new("VirtualDisplayThread");
			handlerThread.Start();
			Handler handler = new(handlerThread.Looper);
			VirtualDisplay = MediaProjection?.CreateVirtualDisplay("ScreenCapture", width, height, density, DisplayFlags.Presentation, MediaRecorder.Surface, mVirtualDisplayCallback, handler);
		}
		catch (Java.IO.IOException ex)
		{
			Console.WriteLine($"MediaRecorder preparation failed: {ex.Message}");
			// Handle preparation failure
			MediaRecorder?.Release();
		}
	}

	public void Setup()
	{
		ProjectionManager = (MediaProjectionManager)Platform.AppContext.GetSystemService(Context.MediaProjectionService);

		if (ProjectionManager != null)
		{
			IsSupported = true;
		}
		else
		{
			IsSupported = false;
		}

		Intent captureIntent = ProjectionManager?.CreateScreenCaptureIntent();
		Platform.CurrentActivity?.StartActivityForResult(captureIntent, REQUEST_MEDIA_PROJECTION);
	}

	public override void OnStop()
	{
		base.OnStop();

		// TODO cleanup mediaprojection things
		VirtualDisplay?.Release();
		MediaRecorder?.Release();
	}
}

public class MyVirtualDisplayCallback : VirtualDisplay.Callback
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

public class ScreenRecordingEventArgs : EventArgs
{
	public Result ResultCode { get; set; }

	public Intent? Data { get; set; }
}

[Service(ForegroundServiceType = ForegroundService.TypeMediaProjection)]
public class ScreenRecordingService : Service
{
	public override IBinder? OnBind(Intent? intent)
	{
		return null;
	}

	public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
	{
		string CHANNEL_ID = "ScreenRecordingService";
		int NOTIFICATION_ID = 1337;

		if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
		{
			var channelName = "Screen Recording Service";
			var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
			{
				Description = "Notification Channel for Screen Recording Service"
			};

			var notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.CreateNotificationChannel(channel);
		}

		// Create a notification for the foreground service
		var notificationBuilder = new Notification.Builder(this, CHANNEL_ID)
			.SetContentTitle("Screen Recording")
			.SetContentText("Recording screen...")
			.SetSmallIcon(Resource.Drawable.notification_template_icon_low_bg); // Ensure you have 'ic_notification' in Resources/drawable

		var notification = notificationBuilder.Build();

		// Start the service in the foreground
		StartForeground(NOTIFICATION_ID, notification, ForegroundService.TypeMediaProjection);

		return StartCommandResult.Sticky;
	}
}