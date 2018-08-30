# UnityBarcodeScanner-iOS

This native plugin enables Unity to take advantage of specific features of Vision Framework on the iOS platform.
This plugin is able to work together with Unity's ARKit plugin or without it. When using ARKit, image analysis is performed on ARKit's CoreVideo pixel buffer. If this is not available, the plugin also accepts native pointers to Unity textures.

### Currently supported features:

* Barcode scanning

* Rectangle detection

## Installation

### Requirements:

The plugin was tested using Unity 2018.1.0f2, but it should work with Unity 2017 as well, however this was never confirmed.

### Follow the steps below to integrate the plugin to your Unity project:

1. Copy the contents of UnityBarcodeScanner-iOS/Assets/Plugins/iOS/Vision to YourProject/Assets/Plugins/iOS/Vision.
2. Set the following values in player settings:
    * Scripting backend: IL2CPP
    * Target minimum iOS Version: 11.0
    * Architecture: ARM64
