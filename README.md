# Plugin.Maui.ScreenRecording

`Plugin.Maui.ScreenRecording` provides the ability to get or set the screen brightness inside a .NET MAUI application.

## Getting Started

* Available on NuGet: <http://www.nuget.org/packages/Plugin.Maui.ScreenRecording> [![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.ScreenRecording.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.ScreenRecording/)

## API Usage

`Plugin.Maui.ScreenRecording` provides the `ScreenRecording` class that has a single property `Brightness` that you can get or set.

You can either use it as a static class, e.g.: `ScreenRecording.Default.Brightness = 1` or with dependency injection: `builder.Services.AddSingleton<IScreenRecording>(ScreenRecording.Default);`