# QR Tracking

QrTracking is implemented in the Windows mixed reality driver. The example scene shows how to use this preview feature. The feature expects that the windows Oct 2018 update, and latest mixed reality driver.

# How to Use?
There is an example scene “QRCodesSample” this scene shows how to use the QRScanner prefab scripts to talk to the windows mixed reality driver. Whenever a QR code is detected it is uniquely identified and the events are fired which the application can listen to. Added, Updated, and Removed are the events which the apps can expect. The sample scene demonstrates how these events are used, the scene uses the information about the QR code found and displays it in the world. The Id of the QR code is used with the new platform API to get the spatial coordinate system as shown by SpatialGraphCoordinateSystem script. 
	A second scene “AttachToQRCode” shows how the QR code can be used to attach holograms. As an example, whenever the application sees a QR code with text "pink chair" it places a hologram of the chair near it. The coordinates of the chair relative to the QR code should be adjusted accordingly.

# Enabling/Disabling the feature
The feature can be turned on/off in the driver using a reg key.
reg add "HKLM\SOFTWARE\Microsoft\HoloLensSensors" /v EnableQRTrackerDefault /t REG_DWORD /d 1 /F

When not using this feature it should be turned off if performace is of concern
reg add "HKLM\SOFTWARE\Microsoft\HoloLensSensors" /v EnableQRTrackerDefault /t REG_DWORD /d 0 /F

# Requirements and Notes
Note: You are unable to test this feature in the Unity editor. You must built to a UWP application and test using a Windows Mixed Reality VR headset. It is not yet compatible with Microsoft HoloLens.

Note: You need to have Windows 10 SDK 17763 installed, otherwise your Unity project will throw build errors regarding windows spatial perception preview namespaces.
