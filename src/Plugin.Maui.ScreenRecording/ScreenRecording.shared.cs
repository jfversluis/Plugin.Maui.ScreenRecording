namespace Plugin.Maui.ScreenRecording;

public static class ScreenRecording
{
	static IScreenRecording? defaultImplementation;

	/// <summary>
	/// Provides the default implementation for static usage of this API.
	/// </summary>
	public static IScreenRecording Default =>
		defaultImplementation ??= new ScreenRecordingImplementation();

	internal static void SetDefault(IScreenRecording? implementation) =>
		defaultImplementation = implementation;
}
