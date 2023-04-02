using System;
using Microsoft.Maui.Storage;

namespace Plugin.Maui.ScreenRecording
{
	/// <summary>
	/// Represents a screen recording file that results from a screen recording.
	/// </summary>
	public class ScreenRecordingFile : FileResult
	{
		public ScreenRecordingFile(string fullPath)
			: base(fullPath)
		{

		}
	}
}
