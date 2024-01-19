namespace Plugin.Maui.ScreenRecording;

/// <summary>
/// Provides the ability to record the screen within your app.
/// </summary>
public interface IScreenRecording
{
	/// <summary>
	/// Gets whether or not screen recording is currently happening.
	/// </summary>
	bool IsRecording { get; }

	/// <summary>
	/// Gets whether or not screen recording is supported on this device.
	/// </summary>
	bool IsSupported { get; }

	/// <summary>
	/// Starts the screen recording.
	/// </summary>
	/// <param name="options">The options to use for this screen recording.</param>
	/// <remarks>
	/// A permission to access the microphone might be needed, this is not requested by this method.
	/// Make sure to add an entry in the metadata of your app and request the runtime permission
	/// before calling this method.
	/// </remarks>
	void StartRecording(ScreenRecordingOptions? options = null);

	/// <summary>
	/// Stops the recording and saves the video file to the device's gallery.
	/// </summary>
	/// <returns>A <see cref="Task"/> object with information about this operation.</returns>
	Task <ScreenRecordingFile?> StopRecording();
}
