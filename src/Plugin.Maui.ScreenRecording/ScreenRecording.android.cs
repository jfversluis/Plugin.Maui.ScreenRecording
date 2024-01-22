using Android.Content;
using Android.Media;
using Android.Media.Projection;
using Android.Hardware.Display;
using Android.Views;
using Application = Android.App.Application;

namespace Plugin.Maui.ScreenRecording;

public partial class ScreenRecordingImplementation : MediaProjection.Callback, IScreenRecording
{
	const int requestMediaProjectionCode = 1;
	string? filePath;
	bool enableMicrophone;

	string NotificationContentTitle { get; set; }
	string NotificationContentText { get; set; }

	MediaProjectionManager? ProjectionManager { get; set; }
	MediaProjection? MediaProjection { get; set; }
	VirtualDisplay? VirtualDisplay { get; set; }
	MediaRecorder? MediaRecorder { get; set; }

	public bool IsRecording { get; private set; }

	public bool IsSupported => ProjectionManager is not null;

	public ScreenRecordingImplementation()
	{
		ProjectionManager = (MediaProjectionManager?)Platform.AppContext.GetSystemService(Context.MediaProjectionService);
	}

	public void StartRecording(ScreenRecordingOptions? options = null)
	{
		if (IsSupported)
		{
			enableMicrophone = options?.EnableMicrophone ?? false;

			NotificationContentTitle = options?.NotificationContentTitle ?? "Screen Recording";
			NotificationContentText = options?.NotificationContentText ?? "Recording screen...";

			var saveOptions = options ?? new();
			var savePath = saveOptions.SavePath;

			if (string.IsNullOrWhiteSpace(savePath))
			{
				savePath = Path.Combine(Path.GetTempPath(),
					$"screenrecording_{DateTime.Now:ddMMyyyy_HHmmss}.mp4");
			}

			filePath = savePath;

			Setup();
		}
		else
		{
			throw new NotSupportedException("Screen recording not supported on this device.");
		}
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
		Intent intent = new(Application.Context, typeof(ScreenRecordingService));
		intent.PutExtra("ContentTitle", NotificationContentTitle);
		intent.PutExtra("ContentText", NotificationContentText);

		// Android O
		if (OperatingSystem.IsAndroidVersionAtLeast(26))
		{
			Application.Context.StartForegroundService(intent);
		}
		else
		{
			Application.Context.StartService(intent);
		}

		// Wait for the foreground service to be started
		await Task.Delay(1000); // TODO can we do this better?

		MediaProjection = ProjectionManager?.GetMediaProjection(resultCode, data!);
		MediaProjection?.RegisterCallback(this, null);

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
		if (MediaRecorder is null)
		{
			throw new Exception("MediaRecorder has not been created.");
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

		int width = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
		int height = (int)DeviceDisplay.Current.MainDisplayInfo.Height;
		int density = (int)DeviceDisplay.Current.MainDisplayInfo.Density;

		MediaRecorder.SetVideoSize(width, height);
		MediaRecorder.SetVideoFrameRate(30);
		MediaRecorder.SetOutputFile(filePath);

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
			MediaRecorder?.Release();
		}
	}

	public void Setup()
	{
		if (ProjectionManager is not null)
		{
			Intent captureIntent = ProjectionManager.CreateScreenCaptureIntent();
			Platform.CurrentActivity?.StartActivityForResult(captureIntent, requestMediaProjectionCode);
		}
	}

	public override void OnStop()
	{
		base.OnStop();

		VirtualDisplay?.Release();
		MediaRecorder?.Release();
	}
}

class MyVirtualDisplayCallback : VirtualDisplay.Callback
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