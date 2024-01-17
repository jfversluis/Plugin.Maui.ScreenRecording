using Autofac;
using CommunityToolkit.Maui.Views;
using Plugin.Maui.ScreenRecording;

namespace ScreenRecordingSample;

public partial class MainPage : ContentPage
{
	readonly IScreenRecording screenRecording;

	public MainPage()
	{
		InitializeComponent();
		this.screenRecording = App.Container.Resolve<IScreenRecording>();

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
		await screenRecording.StartRecording(recordMicrophone.IsToggled);
	}

	async void StopRecordingClicked(object sender, EventArgs e)
	{
		ScreenRecordingFile screenResult = await screenRecording.StopRecording();

		if (screenResult != null)
		{
			FileInfo f = new FileInfo(screenResult.FullPath);
			await Shell.Current.DisplayAlert("File Created", $"Path: {screenResult.FullPath} Size: {f.Length.ToString("N0")} bytes", "OK");

			mediaElement.Source = screenResult.FullPath;
			Console.WriteLine($"Path: {screenResult.FullPath} Size: {f.Length.ToString("N0")} bytes");
		}
		else
		{
			await Shell.Current.DisplayAlert("No Screen Recoring", "NADA", "OK");
			Console.WriteLine("No Screen Recoring");
		}

		btnStart.IsEnabled = true;
		btnStop.IsEnabled = false;
	}
}
