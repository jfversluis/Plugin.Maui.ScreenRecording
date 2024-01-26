namespace Plugin.Maui.ScreenRecording;

public class ScreenRecordingOptions
{
	internal const string defaultAndroidNotificationTitle = "Screen recording in progress...";
	internal const string defaultAndroidNotificationText = "A screen recording is currently in progress, be careful with any sensitive information.";
	
	/// <summary>
	/// Gets or sets the path to save the recording to.
	/// The default is the device's temporary folder with a timestamped file,
	/// e.g. screenrecording_20230101_133700.mp4.
	/// </summary>
	public string SavePath { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets whether to make this recording available
	/// in the device's gallery app(s). Default value is <see langword="false"/>.
	/// </summary>
	/// <remarks>Currently only implemented for iOS.</remarks>
	public bool SaveToGallery { get; set; }

	/// <summary>
	/// Gets or sets whether to also record microphone input.
	/// Default value is <see langword="false"/>.
	/// </summary>
	public bool EnableMicrophone { get; set; }

	/// <summary>
	/// Gets or sets the notification content title.
	/// Default value is "Screen recording in progress...".
	/// </summary>
	/// <remarks>This property only has effect on Android. On other platforms no notification is shown when a screen recording is being made.</remarks>
	public string NotificationContentTitle { get; set; } = defaultAndroidNotificationTitle;

	/// <summary>
	/// Gets or sets the notification content text.
	/// Default value is "A screen recording is currently in progress, be careful with any sensitive information.".
	/// </summary>
	/// <remarks>This property only has effect on Android. On other platforms no notification is shown when a screen recording is being made.</remarks>
	public string NotificationContentText { get; set; } = defaultAndroidNotificationText;
}
