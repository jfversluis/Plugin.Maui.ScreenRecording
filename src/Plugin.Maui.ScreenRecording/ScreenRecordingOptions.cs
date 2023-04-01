using System;

namespace Plugin.Maui.ScreenRecording;

public class ScreenRecordingOptions
{
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
	public bool SaveToGallery { get; set; }
}
