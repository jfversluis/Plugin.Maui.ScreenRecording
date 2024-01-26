using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Plugin.Maui.ScreenRecording;
[Service(ForegroundServiceType = ForegroundService.TypeMediaProjection)]
class ScreenRecordingService : Service
{
	public override IBinder? OnBind(Intent? intent)
	{
		return null;
	}

	public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
	{
		string CHANNEL_ID = "ScreenRecordingService";
		int NOTIFICATION_ID = 1337;
		Notification? foregroundNotification = null;

		// Android O
		if (OperatingSystem.IsAndroidVersionAtLeast(26))
		{
			var channelName = "Screen Recording Service";
			var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Default)
			{
				Description = "Notification Channel for Screen Recording Service"
			};

			var notificationManager = (NotificationManager?)GetSystemService(NotificationService)
				?? throw new Exception($"{nameof(NotificationManager)} system service could not be retrieved.");

			notificationManager.CreateNotificationChannel(channel);

			string contentTitle = intent?.GetStringExtra("ContentTitle") ??
				ScreenRecordingOptions.defaultAndroidNotificationTitle;

			string contentText = intent?.GetStringExtra("ContentText") ??
				ScreenRecordingOptions.defaultAndroidNotificationText;

			// Create a notification for the foreground service
			var notificationBuilder = new Notification.Builder(this, CHANNEL_ID)
				.SetContentTitle(contentTitle)
				.SetContentText(contentText)
				.SetSmallIcon(Resource.Drawable.notification_template_icon_low_bg); // Ensure you have 'ic_notification' in Resources/drawable

			foregroundNotification = notificationBuilder.Build();

			// Start the service in the foreground
			// Android Q
			if (OperatingSystem.IsAndroidVersionAtLeast(29))
			{
				StartForeground(NOTIFICATION_ID, foregroundNotification, ForegroundService.TypeMediaProjection);
			}
			// Android O
			else if (OperatingSystem.IsAndroidVersionAtLeast(26))
			{
				StartForeground(NOTIFICATION_ID, foregroundNotification);
			}
		}
		// Any Android before O
		else
		{
			StartService(intent);
		}

		return StartCommandResult.Sticky;
	}
}
