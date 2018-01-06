HoloToolkit CrossPlatform                        {#mainpage}
============

## Description

A collection of classes and methods created to make it easier to support the same application running on Editor (Mono) and as a Windows Store App (WinRT)

## Components
This addon is divided on several components for various tasks. All of them have one goal in common: use the same interface in Mono and WinRT.

### Reflection

WinRT moved the majority of the reflection methods to a different place where they are on .NET. By creating extensions methods on the type class, we can make sure the .NET interface still works on WinRT.