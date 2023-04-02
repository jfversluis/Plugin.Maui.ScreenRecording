using Plugin.Maui.ScreenRecording;

namespace ScreenRecordingSample;

public partial class MainPage : ContentPage
{
	readonly IScreenRecording screenRecording;

	public MainPage(IScreenRecording screenRecording)
	{
		InitializeComponent();
		this.screenRecording = screenRecording;
	}

	async void StartRecordingClicked(object sender, EventArgs e)
	{
		if (!screenRecording.IsSupported)
		{
			await DisplayAlert("Not Supported", "Screen recording is not supported", "OK");
			return;
		}

		await screenRecording.StartRecording(recordMicrophone.IsToggled);
	}

	async void StopRecordingClicked(object sender, EventArgs e)
	{
		var result = await screenRecording.StopRecording();
	}
}
