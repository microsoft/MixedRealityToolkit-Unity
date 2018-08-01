# Coding Guidelines

This document outlines the recommended coding guidelines for the Mixed Reality Toolkit.  The majority of these suggestions follow the [recommended standards from MSDN](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

---

## Script license information headers

All scripts posted to the MRTK should have the standard License header attached, exactly as shown below:

```c#
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
```

Any script files submitted without the license header will be rejected.

## Function / Method summary headers

All public classes, structs, enums, functions, properties, fields posted to the MRTK should be described as to it's purpose and use, exactly as shown below:

```c#
    /// <summary>
    /// The Controller definition defines the Controller as defined by the SDK / Unity.
    /// </summary>
    public struct Controller
    {
        /// <summary>
        /// The ID assigned to the Controller
        /// </summary>
        public string ID;
    }
```

This ensures documentation is properly generated and disseminated for all all classes, methods, and properties.

>Any script files submitted without proper summary tags will be rejected.

## MRTK namespace rules

The vNext structure adheres to a strict namespace culture of mapping the namespace 1-1 with the folder structure of the project.  This ensures that classes are easy to discover and maintain.  It also ensures the dependencies of any class are laid out in the beginning usings of the file.

### Do:

```c#
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputSystem
{
    /// <summary>
    /// The ButtonAction defines the set of actions exposed by a controller.
    /// Denoting the available buttons / interactions that a controller supports.
    /// </summary>
    public enum ButtonAction
    {
    }
}
```

Absolutely no class / struct / enum or other definition should be entered in to the project without the appropriate namespace definition.

## Spaces vs Tabs

Please be sure to use 4 spaces instead of tabs when contributing to this project.

Additionally, ensure that spaces are added for conditional / loop functions like if / while / for

### Don't:

```c#
private Foo()
{
    if(Bar==null) // <- no space between if and ()
    {
        DoThing();
    }

    while(true) // <- no space between while and ()
    {
        Do();
    }
}
```

### Do:

```c#
private Foo()
{
    if (Bar==null)
    {
        DoThing();
    }

    while (true)
    {
        Do();
    }
}
```

## Naming Conventions

Always use `PascalCase` for public / protected / virtual properties, and `camelCase` for private properties and fields.
>The only exception to this is for data structures that require the fields to be serialized by the `JsonUtility`.

### Don't:

```c#
public string myProperty; // <- Starts with a lower case letter
private string MyProperty; // <- Starts with an uppercase case letter
```

### Do:

```c#
public string MyProperty;
protected string MyProperty;
private string myProperty;
```

## Access Modifiers

Always declare an access modifier for all fields, properties and methods.

>All Unity API Methods should be `private` by default, unless you need to override them in a derived class. In this case `protected` should be used.

### Don't:

```c#
// No public / private access modifiers
void Foo() { }
void Bar() { }
```

### Do:

```c#
private void Foo() { }
public void Bar() { }
protected virtual void FooBar() { }
```

## Use Braces

Always use braces after each statement block, and place them on the next line.

### Don't:

```c#
private Foo()
{
    if (Bar==null) // <- missing braces surrounding if action
        DoThing();
    else
        DoTheOtherThing();
}
```

### Don't:

```c#
private Foo() { // <- Open bracket on same line
    if (Bar==null) DoThing(); // <- if action on same line with no surrounding brackets
    else DoTheOtherThing();
}
```

### Do:

```c#
private Foo()
{
    if (Bar==true)
    {
        DoThing();
    }
    else
    {
        DoTheOtherThing();
    }
}
```

## Public classes, structs, and enums should all go in their own files

If the class, struct, or enum can be made private then it's okay to be included in the same file.  This avoid compilations issues with Unity and ensure that proper code abstraction occurs, it also reduces conflicts and breaking changes when code needs to change.

### Don't:

```c#
public class MyClass
{
    public struct MyStruct() { }
    public enum MyEnumType() { }
    public class MyNestedClass() { }
}
```

### Do:

```c#
// Private references for use inside the class only
public class MyClass
{
    private struct MyStruct() { }
    private enum MyEnumType() { }
    private class MyNestedClass() { }
}
```

### Do:

```c#
// Public Struct / Enum definitions for use in your class.  Try to make them generic for reuse.
MyStruct.cs
public struct MyStruct
{
    public string Var1;
    public string Var2;
}
```

```c#
MyEnumType.cs
public enum MuEnumType
{
    Value1,
    Value2 // <- note, no "," on last value to denote end of list.
}
```

```c#
MyClass.cs
public class MyClass
{
    private MyStruct myStructReference;
    private MyEnumType myEnumReference;
}
```

## Initialize Enums

To ensure all Enum's are initialized correctly starting at 0, .NET gives you a tidy shortcut to automatically initialize the enum by just adding the first (starter) value.

> E.G. Value 1 = 0  (Remaining values are not required)

### Don't:

```c#
public enum MyEnum
{
    Value1, // <- no initializer
    Value2,
    Value3
}
```

### Do:

```c#
public enum MyEnum
{
    Value1 = 0,
    Value2,
    Value3
}
```

## Order Enums for appropriate extension

It is critical that if an Enum is likely to be extended in the future, to order defaults at the top of the Enum, this ensures Enum indexes are not affected with new additions.

### Don't:

```c#
public enum AudioFormat
{
    PCM,
    MP3,
    Ogg,
    None, // <- default value not at start
    Other // <- anonymous value left to end of enum
}
```

### Do:

```c#
/// <summary>
/// AudioFormat lists the supported / known formats of audio data
/// </summary>
public enum AudioFormat
{
    /// <summary>
    /// No specified format
    /// </summary>
    None = 0,
    /// <summary>
    /// Undefined format.
    /// </summary>
    Other,
    /// <summary>
    /// PCM
    /// </summary>
    PCM,
    /// <summary>
    /// MP3.
    /// </summary>
    MP3,
    /// <summary>
    /// Ogg Vorbis
    /// </summary>
    Ogg
}
```

## Review Enum use for Bitfields

If there is a possibility for an enum to require multiple states as a value, e.g. Handedness = Left & Right. Then the Enum needs to be decorated correctly with BitFlags to enable it to be used correctly

> The Handedness.cs file has a concrete implementation for this

### Don't:

```c#
public enum MyEnum
{
    None,
    Left,
    Right
}
```

### Do:

```c#
[Flags]
public enum MyEnum
{
    None = 1  << 0,
    Left = 1  << 1,
    Right = 1 << 2,
    Both = Left | Right
}
```

## Best Practices, including Unity recommendations

Some of the target platforms of this project require us to take performance into consideration.  With this in mind we should always be careful of allocating memory in frequently called code in tight update loops or algorithms.

## Encapsulation

Always use private fields and public properties if access to the field is needed from outside the class or struct.  Be sure to co-locate the private field and the public property. This makes it easier to see, at a glance, what backs the property and that the field is modifiable by script.

If you need to have the ability to edit your field in the inspector, it's best practice to follow the rules for Encapsulation and serialize your backing field.

>The only exception to this is for data structures that require the fields to be serialized by the `JsonUtility`, where a data class is required to have all public fields for the serialization to work.

### Don't:

```c#
public float MyValue;
```

### Do:

```c#
/// <summary>
/// A value that is accessible within script only (it is not serialized in Unity for visibility in the Inspector)
/// </summary>
private float myValue;
```

### Do:

```c#
/// <summary>
/// A value that is serialized in Unity for setting via the Inspector (not modifiable from other scripts)
/// </summary>
[Tooltip("A value that is serialized in Unity for setting via the Inspector (not modifiable from other scripts)")]
[SerializeField]
private float myValue;
```

---

### Don't:

```c#
private float myValue1;
private float myValue2;

public float MyValue1
{
    get{ return myValue1; }
    set{ myValue1 = value }
}

public float MyValue2
{
    get{ return myValue2; }
    set{ myValue2 = value }
}
```

### Do:

```c#
/// <summary>
/// A value that is serialized in Unity for setting via the Inspector and is modifiable from other scripts
/// </summary>
[Tooltip("A value that is serialized in Unity for setting via the Inspector and is modifiable from other scripts")]
[SerializeField]
private float myValue; // <- Notice we co-located the backing field above our corresponding property.
public float MyValue
{
    get{ return myValue; }
    set{ myValue = value }
}
```

## Use `for` instead of `foreach` when possible

In some cases a foreach is required, e.g. when looping over an IEnumerable.  But for performance benefit, avoid foreach when you can.

### Don't:

```c#
foreach(var item in items)
```

### Do:

```c#
int length = items.length; // cache reference to list/array length
for (int i=0; i < length; i++)
```

## Cache values and serialize them in the scene/prefab whenever possible

With the HoloLens in mind, it's best to optimize for performance and cache references in the scene or prefab to limit runtime memory allocations.

### Don't:

```c#
void Update()
{
    gameObject.GetComponent<Renderer>().Foo(Bar);
}
```

### Do:

```c#
[SerializeField] // To enable setting the reference in the inspector.
private Renderer myRenderer;

private void Awake()
{
    // If you didn't set it in the inspector, then we cache it on awake.
    if (myRenderer == null)
    {
        myRenderer = gameObject.GetComponent<Renderer>();
    }
}

private void Update()
{
    myRenderer.Foo(Bar);
}
```

## Cache references to materials, do not call the ".material" each time

Unity will create a new material each time you use ".material", which will cause a memory leak if not cleaned up properly.

### Don't:

```c#
public class MyClass
{
    void Update()
    {
        Material myMaterial = GetComponent<Renderer>().material;
        myMaterial.SetColor("_Color", Color.White);
    }
}
```

### Do:

```c#
// Private references for use inside the class only
public class MyClass
{
    private Material cachedMaterial;

    private void Awake()
    {
        cachedMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        cachedMaterial.SetColor("_Color", Color.White);
    }

    private void OnDestroy()
    {
        Destroy(cachedMaterial);
    }
}
```

>Alternatively, use Unity's "SharedMaterial" property which does not create a new material each time it is referenced.

## Use [platform dependent compilation](https://docs.unity3d.com/Manual/PlatformDependentCompilation.html) to ensure the Toolkit won't break the build on another platform

* Use `WINDOWS_UWP` in order to use UWP-specific, non-Unity APIs. This will prevent them from trying to run in the Editor or on unsupported platforms. This is equivalent to `UNITY_WSA && !UNITY_EDITOR` and should be used in favor of.
* Use `UNITY_WSA` to use UWP-specific Unity APIs, such as the `UnityEngine.XR.WSA` namespace. This will run in the Editor when the platform is set to UWP, as well as in built UWP apps.

This chart can help you decide which `#if` to use, depending on your use cases and the build settings you expect.

| | UWP IL2CPP | UWP .NET | Editor |
| --- | --- | --- | --- |
| `UNITY_EDITOR` | False | False | True |
| `UNITY_WSA` | True | True | True |
| `WINDOWS_UWP` | True | True | False |
| `UNITY_WSA && !UNITY_EDITOR` | True | True | False |
| `ENABLE_WINMD_SUPPORT` | True | True | False |
| `NETFX_CORE` | False | True | False |
