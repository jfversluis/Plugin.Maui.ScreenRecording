namespace Plugin.Maui.ScreenRecording;

/// <summary>
/// Exception thrown when a screen recording operation fails.
/// </summary>
public class ScreenRecordingException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ScreenRecordingException"/> class.
	/// </summary>
	public ScreenRecordingException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ScreenRecordingException"/> class with a specified error message.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	public ScreenRecordingException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ScreenRecordingException"/> class with a specified error message
	/// and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception.</param>
	public ScreenRecordingException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
