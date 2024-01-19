using Plugin.Maui.ScreenRecording;

namespace ScreenRecordingSample;

public partial class MainPage : ContentPage
{
	readonly IScreenRecording screenRecording;

	public MainPage(IScreenRecording screenRecording)
	{
		InitializeComponent();
		this.screenRecording = screenRecording;

		btnStart.IsEnabled = true;
		btnStop.IsEnabled = false;
	}

	async void StartRecordingClicked(object sender, EventArgs e)
	{
		if (!screenRecording.IsSupported)
		{
			await DisplayAlert("Not Supported", "Screen recording is not supported", "OK");
			return;
		}

		btnStart.IsEnabled = false;
		btnStop.IsEnabled = true;
		screenRecording.StartRecording(new() { EnableMicrophone = recordMicrophone.IsToggled });
	}

	async void StopRecordingClicked(object sender, EventArgs e)
	{
		ScreenRecordingFile screenResult = await screenRecording.StopRecording();

		if (screenResult != null)
		{
			FileInfo f = new(screenResult.FullPath);
			await Shell.Current.DisplayAlert("File Created", $"Path: {screenResult.FullPath} Size: {f.Length.ToString("N0")} bytes", "OK");

			mediaElement.Source = screenResult.FullPath;
		}
		else
		{
			await Shell.Current.DisplayAlert("No Screen Recording", "NADA", "OK");
		}

		btnStart.IsEnabled = true;
		btnStop.IsEnabled = false;
	}
}
