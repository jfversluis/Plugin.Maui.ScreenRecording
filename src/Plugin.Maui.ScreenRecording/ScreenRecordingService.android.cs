using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;

namespace Plugin.Maui.ScreenRecording;

[Service(ForegroundServiceType = ForegroundService.TypeMediaProjection)]
class ScreenRecordingService : Service
{
    public const int NotificationId = 1337;
    public const string ChannelId = "ScreenRecordingService";
    public const string ExtraExternalMessenger = "ExternalMessenger";
    public const string ExtraContentTitle = "ContentTitle";
    public const string ExtraContentText = "ContentText";
    public const string ExtraCommandNotificationSetup = "SetupNotification";
    public const string ExtraCommandBeginRecording = "BeginRecording";
    
    public const int MsgServiceStarted = 1;

    public override IBinder? OnBind(Intent? intent)
    {
        return null;
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        NotifyHandler(intent);

        if (intent?.GetBooleanExtra(ExtraCommandNotificationSetup, false) == true)
        {
            SetupForegroundNotification(intent);
        }

        if (intent?.GetBooleanExtra(ExtraCommandBeginRecording, false) == true)
        {
            BeginRecording();
        }

        return StartCommandResult.Sticky;
    }

    public void BeginRecording()
    {
        try
        {
            // TODO ugly fix
            var instance = (ScreenRecordingImplementation)ScreenRecording.Default;
            instance.BeginRecording();
            Log.Debug(ChannelId, "Recording started.");
        }
        catch (Exception ex)
        {
            Log.Error(ChannelId, $"Failed to start recording: {ex}");
            StopSelf();
        }
    }

    private static void NotifyHandler(Intent? intent)
    {
        // Notify the external observer that the service has started.
        if (GetParcelableExtra<Messenger>(intent, ExtraExternalMessenger) is Messenger messenger)
        {
            try
            {
                var msg = Message.Obtain(null, MsgServiceStarted);
                messenger.Send(msg);
            }
            catch (RemoteException ex)
            {
                Log.Error(ChannelId, "Failed to send message to activity: " + ex);
            }
        }
    }

    private void SetupForegroundNotification(Intent? intent)
    {
        // Android O
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            CreateNotificationChannel();

            var contentTitle = intent?.GetStringExtra(ExtraContentTitle) ?? ScreenRecordingOptions.defaultAndroidNotificationTitle;
            var contentText = intent?.GetStringExtra(ExtraContentText) ?? ScreenRecordingOptions.defaultAndroidNotificationText;

            var notification = new Notification.Builder(this, ChannelId)
                .SetContentTitle(contentTitle)
                .SetContentText(contentText)
                .SetSmallIcon(global::Android.Resource.Drawable.PresenceVideoOnline)
                .Build();

            // Android Q
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                StartForeground(NotificationId, notification, ForegroundService.TypeMediaProjection);
            }
            else
            {
                StartForeground(NotificationId, notification);
            }
        }
        else
        {
            // Pre-Android O
            StartForeground(NotificationId, CreateFallbackNotification());
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "The callee bears responsibility for ensuring an OS version check before invoking this method.")]
    private void CreateNotificationChannel()
    {
        var channel = new NotificationChannel(ChannelId, "Screen Recording Service", NotificationImportance.Default)
        {
            Description = "Notification Channel for Screen Recording Service"
        };

        var notificationManager = (NotificationManager?)GetSystemService(NotificationService);
        notificationManager?.CreateNotificationChannel(channel);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1422:Validate platform compatibility", Justification = "The callee bears responsibility for ensuring an OS version check before invoking this method.")]
    private Notification CreateFallbackNotification()
    {
        return new Notification.Builder(this)
            .SetContentTitle("Screen Recording")
            .SetContentText("Screen recording is running.")
            .SetSmallIcon(global::Android.Resource.Drawable.PresenceVideoOnline)
            .Build();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1422:Validate platform compatibility", Justification = "Compiler noob")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Compiler noob")]
    private static T? GetParcelableExtra<T>(Intent? intent, string name) where T : Java.Lang.Object =>
        OperatingSystem.IsAndroidVersionAtLeast(33)
            ? intent?.GetParcelableExtra(name) as T
            : intent?.GetParcelableExtra(name, Java.Lang.Class.FromType(typeof(T))) as T;
}
