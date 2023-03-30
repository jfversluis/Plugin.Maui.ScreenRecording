using Windows.Graphics.Display;

namespace Plugin.Maui.ScreenRecording;

// TODO Not working, throwing exception???
partial class ScreenRecordingImplementation : IScreenRecording
{
	public float Brightness 
	{
		get
		{
			if (!BrightnessOverride.GetDefaultForSystem().IsSupported)
			{
				return 0;
			}

			return (float)BrightnessOverride.GetDefaultForSystem().BrightnessLevel;
		}
		
		set
		{
			BrightnessOverride.GetDefaultForSystem().StartOverride();
			BrightnessOverride.GetDefaultForSystem().SetBrightnessLevel(value, DisplayBrightnessOverrideOptions.UseDimmedPolicyWhenBatteryIsLow);
		}
	}
}