![](nuget.png)

# Plugin.Maui.ScreenRecording

`Plugin.Maui.ScreenRecording` provides the ability to record the screen from within your .NET MAUI app.

I have also recorded a video on how to get started with this plugin. [Watch it here!](https://www.youtube.com/watch?v=M9lDKEYzwn0&list=PLfbOp004UaYVgzmTBNVI0ql2qF0LhSEU1&index=35)

## Install Plugin

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.ScreenRecording.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.ScreenRecording/)

Available on [NuGet](http://www.nuget.org/packages/Plugin.Maui.ScreenRecording).

Install with the dotnet CLI: `dotnet add package Plugin.Maui.ScreenRecording`, or through the NuGet Package Manager in Visual Studio.

### Supported Platforms

| Platform | Supported | What is recorded |
|----------|-----------|-----------------|
| Android | ✅ | **Full device screen** (including other apps, notifications, etc.) |
| iOS | ✅ | **App screen only** (not the full device) |
| macOS (Catalyst) | ✅ | **App screen only** (not the full device) |
| Windows | ✅ | Full monitor |

> [!IMPORTANT]
> **iOS Simulator**: Screen recording is **not supported** on the iOS Simulator. You must use a physical device to test screen recording on iOS.

> [!NOTE]
> On Android, the recording captures the full device screen including content from other apps if the user navigates away. On iOS and macOS, only the app's own content is captured via ReplayKit.

## Setup

In `MauiProgram.cs` add the reference to the screen recording plugin:

```csharp
using Plugin.Maui.ScreenRecording;
```

Then add a call to `.UseScreenRecording()` on your `MauiAppBuilder`. For example:

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseScreenRecording()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    });
```

### Android Setup

For Android you need to register the foreground service and permissions in your `AndroidManifest.xml` file. Make sure to compare it with the example below and add the missing entries:

```xml
<application>
    <service android:name="Plugin.Maui.ScreenRecording.ScreenRecordingImplementation.ScreenRecordingService" android:exported="false" android:foregroundServiceType="mediaProjection" />
</application>

<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE_MEDIA_PROJECTION" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
```

> [!NOTE]
> `FOREGROUND_SERVICE_MEDIA_PROJECTION` is required on Android 14 (API 34) and above. If you target API 34+, this must be declared.
> `RECORD_AUDIO` is only required if you set `EnableMicrophone = true` in your recording options.

If you want to save recordings to the gallery, also add:
```xml
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
```

### iOS / macOS Setup

If you want to save recordings to the Photos app, you need to declare the `NSPhotoLibraryAddUsageDescription` permission in your `Info.plist` file:

```xml
<key>NSPhotoLibraryAddUsageDescription</key>
<string>We'd like to add the screen recordings to your Photos app!</string>
```

The permission will automatically be requested by the library when needed.

## Usage

### Dependency Injection (recommended)

Register the service via `UseScreenRecording()` (shown above), then inject it:

```csharp
public partial class RecordingPage : ContentPage
{
    readonly IScreenRecording screenRecording;

    public RecordingPage(IScreenRecording screenRecording)
    {
        InitializeComponent();
        this.screenRecording = screenRecording;
    }
}
```

### Static Access

Alternatively, use the static accessor directly:

```csharp
readonly IScreenRecording screenRecording = ScreenRecording.Default;
```

### Check Support

To check if the device is capable of making screen recordings:

```csharp
if (!screenRecording.IsSupported)
{
    // Screen recording not available on this device
    return;
}
```

### Start Recording

```csharp
bool started = await screenRecording.StartRecording();
```

Optionally provide `ScreenRecordingOptions` to customize behavior:

```csharp
var options = new ScreenRecordingOptions
{
    EnableMicrophone = true,
    SaveToGallery = true,
    SavePath = Path.Combine(Path.GetTempPath(), "myRecording.mp4"),
    // Android-only: customize the foreground service notification
    NotificationContentTitle = "Recording in progress",
    NotificationContentText = "Your screen is being recorded."
};

bool started = await screenRecording.StartRecording(options);
```

On Android, `StartRecording` returns `false` if the user denies the screen capture permission. On iOS, it returns `false` if ReplayKit encounters an error.

### Stop Recording

```csharp
ScreenRecordingFile? result = await screenRecording.StopRecording();

if (result is not null)
{
    // result.FullPath contains the path to the recorded video file
    Console.WriteLine($"Recording saved to: {result.FullPath}");
}
```

### Check Recording State

```csharp
if (screenRecording.IsRecording)
{
    // A recording is currently in progress
}
```

## API Reference

### `IScreenRecording`

| Member | Type | Description |
|--------|------|-------------|
| `IsRecording` | `bool` | Whether a screen recording is currently in progress |
| `IsSupported` | `bool` | Whether the device supports screen recording |
| `StartRecording(options?)` | `Task<bool>` | Starts recording. Returns `true` on success. |
| `StopRecording()` | `Task<ScreenRecordingFile?>` | Stops recording and returns the file. Returns `null` if not recording. |

### `ScreenRecordingOptions`

| Property | Default | Description |
|----------|---------|-------------|
| `SavePath` | Temp folder | Path where the recording will be saved |
| `SaveToGallery` | `false` | Whether to make the recording available in the device gallery |
| `EnableMicrophone` | `false` | Whether to also record microphone input |
| `NotificationContentTitle` | "Screen recording in progress..." | Android only: foreground service notification title |
| `NotificationContentText` | "A screen recording is currently in progress..." | Android only: foreground service notification text |

## Acknowledgements

This project could not have came to be without these projects and people, thank you! <3

A big thank you to [@rdurish](https://github.com/rdurish) who provided the initial implementation for Android. Amazing work!
