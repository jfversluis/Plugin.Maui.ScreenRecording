![](nuget.png)

# Plugin.Maui.ScreenRecording

`Plugin.Maui.ScreenRecording` provides the ability to record the screen from within your .NET MAUI app.

I have also recorded a video on how to get started with this plugin. [Watch it here!](https://www.youtube.com/watch?v=M9lDKEYzwn0&list=PLfbOp004UaYVgzmTBNVI0ql2qF0LhSEU1&index=35)

## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.ScreenRecording.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.ScreenRecording/)

Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.ScreenRecording).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.ScreenRecording`, or through the NuGet Package Manager in Visual Studio.

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

On the page you want the screen recorder, create a variable and retrieve the static instance of the `ScreenRecording` object.

```csharp
    readonly IScreenRecording screenRecording;
    
    this.screenRecording = ScreenRecording.Default;
```

<!-- TODO add instructions for constructor injection -->

To check if device is capable of making screen recordings:

`screenRecording.IsSupported;`

To start recording:

`screenRecording.StartRecording();`

Additionally you can provide `ScreenRecordingOptions` to influence the behavior:

```csharp
ScreenRecordingOptions options = new()
{
	EnableMicrophone = true,
	SaveToGallery = true,
	SavePath = Path.Combine(Path.GetTempPath(), "myRecording.mp4"),
};

screenRecording.StartRecording(options);
```

To stop recording:

`ScreenRecordingFile screenResult = await screenRecording.StopRecording();`

## Acknowledgements

This project could not have came to be without these projects and people, thank you! <3

A big thank you to [@rdurish](https://github.com/rdurish) who provided the initial implementation for Android. Amazing work!
