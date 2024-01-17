# Plugin.Maui.ScreenRecording

`Plugin.Maui.ScreenRecording` provides the ability to record the screen from within your .NET MAUI app.

This is a work in progress, feel free to help out!

<!--## Getting Started

* Available on NuGet: <http://www.nuget.org/packages/Plugin.Maui.ScreenRecording> [![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.ScreenRecording.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.ScreenRecording/)

TODO-->

## Maui Installation Instructions:


In `MauiProgram.cs` add the reference to the screen recording plugin:
```
    using Plugin.Maui.ScreenRecording;
```

Then add a call to `.UseScreenRecording()` on your `MauiAppBuilder`. For example:

```
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseScreenRecording() // This line was added
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    });
```

## Android Instructions:

In `AndroidManifest.xml` inside the manifest node add:
```
<application android:allowBackup="true" android:supportsRtl="true" android:requestLegacyExternalStorage="true" android:preserveLegacyExternalStorage="true">
	<service android:name="Plugin.Maui.ScreenRecording.ScreenRecordingImplementation.ScreenRecordingService" android:exported="false" android:foregroundServiceType="mediaProjection" />
</application>
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
```

## IOS / Mac Instructions:

No additional setup is needed.

## Windows Instructions

Not supported (yet).

# Usage:

On the page you want the screen recorder, create a read only variable and set it.
```
    readonly IScreenRecording screenRecording;
    
    this.screenRecording = ScreenRecording.Default;
```

<!-- TODO add instructions for constructor injection -->

To Check if device is capable:

    ```screenRecording.IsSupported;```

To Start Recording:

    ```await screenRecording.StartRecording(recordMicrophone True/False);```

To Stop Recording:

    ```ScreenRecordingFile screenResult = await screenRecording.StopRecording();```

