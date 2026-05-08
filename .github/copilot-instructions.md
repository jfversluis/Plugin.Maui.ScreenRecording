# Plugin.Maui.ScreenRecording - Copilot Instructions

## Project Overview

This is a .NET MAUI plugin that provides the ability to record the device screen from within your app. It targets Android, iOS, macOS (Catalyst), and Windows.

## Architecture

Core interface: `IScreenRecording` with `StartRecording`/`StopRecording`.

Key components: `ScreenRecordingOptions`, `ScreenRecordingFile`, `AppBuilderExtensions`.

Platform specifics:
- Android: MediaProjection + foreground service (`ScreenRecordingService.android.cs`)
- iOS/macOS: ReplayKit
- Windows: `Windows.Graphics.Capture`

Note: `UseScreenRecording()` builder extension is required for Android.

## Code Conventions

### Namespace
All code uses: `Plugin.Maui.ScreenRecording`

### File Naming
- `*.shared.cs` - Cross-platform code
- `*.android.cs` - Android-specific code
- `*.macios.cs` - iOS/macOS-specific code
- `*.windows.cs` - Windows-specific code
- `*.net.cs` - Generic .NET fallback

### Standards
- File-scoped namespaces
- `camelCase` for private fields, `PascalCase` for public
- XML docs required on all public APIs
- Null-conditional operators for platform interop

## Building

```bash
dotnet build src/Plugin.Maui.ScreenRecording/Plugin.Maui.ScreenRecording.csproj -c Release
```

## When Making Changes
1. Ensure the plugin builds on all target platforms
2. If adding public API, update the interface
3. Implement on all supported platforms
4. Update sample app and README
