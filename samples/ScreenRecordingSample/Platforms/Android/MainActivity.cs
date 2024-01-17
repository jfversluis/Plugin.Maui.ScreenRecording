using Android.App;
using Android.Content.PM;
using Android.OS;
using Autofac;
using Plugin.Maui.ScreenRecording;

namespace ScreenRecordingSample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	ScreenRecordingImplementation screenRecordingImplementation;
	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		// Initialize other components...

		// Initialize ScreenRecordingImplementation
		screenRecordingImplementation = (ScreenRecordingImplementation)App.Container.Resolve<IScreenRecording>();
		screenRecordingImplementation.Setup();
	}

	protected override async void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent? data)
	{
		base.OnActivityResult(requestCode, resultCode, data);

		screenRecordingImplementation.OnScreenCapturePermissionGranted((int)resultCode, data);
	}

}