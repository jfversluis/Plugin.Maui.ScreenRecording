# Plugin.Maui.ScreenRecording

`Plugin.Maui.ScreenRecording` provides the ability to record the screen from within your .NET MAUI app.

This is a work in progress, feel free to help out!

<!--## Getting Started

* Available on NuGet: <http://www.nuget.org/packages/Plugin.Maui.ScreenRecording> [![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.ScreenRecording.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.ScreenRecording/)

TODO-->

## Maui Installation Instructions:

1. Install the AutoFac Nuget plugin (version 7.1.0 known working) https://www.nuget.org/packages/autofac/

2. In Main App.xaml.cs add the reference to AutoFac and the screen recording plugin:
```
    using Autofac;
    using Plugin.Maui.ScreenRecording;
```

3. Add variables inside your partial app class:
```
    public static Autofac.IContainer Container;
    static readonly Autofac.ContainerBuilder builder = new Autofac.ContainerBuilder();
```

4. In Main App.xaml.cs add the following inside App Constructor:
```
    builder.RegisterType<ScreenRecordingImplementation>().As<IScreenRecording>().SingleInstance();
    Container = (Autofac.IContainer)builder.Build();
```

## Android Instructions:

1. In AndroidManifest.xml inside the manifest node add:
```
<application android:allowBackup="true" android:supportsRtl="true" android:requestLegacyExternalStorage="true" android:preserveLegacyExternalStorage="true">
	<service android:name="Plugin.Maui.ScreenRecording.ScreenRecordingImplementation.ScreenRecordingService" android:exported="false" android:foregroundServiceType="mediaProjection" />
</application>
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
```

3. Reference AutoFac in MainActivity.cs

```using Autofac;```


4. In MainActivity.cs add the following just inside your public class:

```ScreenRecordingImplementation screenRecordingImplementation;```

5. In MainActivity.cs add the OnCreate method:
	
```
    protected override void OnCreate(Bundle savedInstanceState)
    	{
    		base.OnCreate(savedInstanceState);
    
    		// Initialize other components...
    
    		// Initialize ScreenRecordingImplementation
    		screenRecordingImplementation = (ScreenRecordingImplementation)App.Container.Resolve<IScreenRecording>();
    		screenRecordingImplementation.Setup();
    	}
```


6. In MainActivity.cs add the OnActivityResult method:
```
    protected override async void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent? data)
    {
    	base.OnActivityResult(requestCode, resultCode, data);
    
    	screenRecordingImplementation.OnScreenCapturePermissionGranted((int)resultCode, data);
    }
```



## IOS / Mac Instructions:

No Setup Needed.

## Windows Instructions

Not Implemented yet.


# Usage:

On the page you want the screen recorder, create a read only variable and set it.
```
    readonly IScreenRecording screenRecording;
    
    this.screenRecording = App.Container.Resolve<IScreenRecording>();
```

To Check if device is capable:


To Start Recording:

    ```await screenRecording.StartRecording(recordMicrophone True/False);```

To Stop Recording:

    ```ScreenRecordingFile screenResult = await screenRecording.StopRecording();```

