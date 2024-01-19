# Plugin.Maui.ScreenRecording

`Plugin.Maui.ScreenRecording` provides the ability to record the screen from within your .NET MAUI app.

## Getting Started

* Available on NuGet: <http://www.nuget.org/packages/Plugin.Maui.ScreenRecording> [![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.ScreenRecording.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.ScreenRecording/)

## API Usage

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

### Android

For Android you will need to setup a few things in your `AndroidManifest.xml` file. See an example below.\
You should already have a `AndroidManifest.xml` file in your project, make sure to compare it with the example below and add the missing things.

```xml
<application>
	<service android:name="Plugin.Maui.ScreenRecording.ScreenRecordingImplementation.ScreenRecordingService" android:exported="false" android:foregroundServiceType="mediaProjection" />
</application>

<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />

<!-- This one is only needed when targeting API 34 and up -->
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_MEDIA_PROJECTION" />
```

### iOS / macOS

If you want to save recordings to the Photos app, you will need to declare the `NSPhotoLibraryAddUsageDescription` permission in your `info.plist` file.

For example:

```xml
<key>NSPhotoLibraryAddUsageDescription</key>
<string>We'd like to add the screen recordings to your Photos app!</string>
```

The permission will automatically be requested by the library when needed.

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

## Acknowledgements

This project could not have came to be without these projects and people, thank you! <3

A big thank you to [@rdurish](https://github.com/rdurish) who provided the initial implementation for Android. Amazing work!