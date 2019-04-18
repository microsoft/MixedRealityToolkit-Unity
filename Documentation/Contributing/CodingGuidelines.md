# Coding Guidelines

This document outlines the recommended coding guidelines for the Mixed Reality Toolkit.  The majority of these suggestions follow the [recommended standards from MSDN](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

---

## Script license information headers

All scripts posted to the MRTK should have the standard License header attached, exactly as shown below:

```c#
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
```

Any script files submitted without the license header will be rejected

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

The Mixed Reality Toolkit uses a feature based namespace model, where all foundational namespaces begin with "Microsoft.MixedReality.Toolkit". In general, you need not specify the toolkit layer (ex: Core, Providers, Services) in your namespaces.

The currently defined namespaces are:

- Microsoft.MixedReality.Toolkit
- Microsoft.MixedReality.Toolkit.Boundary
- Microsoft.MixedReality.Toolkit.Diagnostics
- Microsoft.MixedReality.Toolkit.Editor
- Microsoft.MixedReality.Toolkit.Input
- Microsoft.MixedReality.Toolkit.SpatialAwareness
- Microsoft.MixedReality.Toolkit.Teleport
- Microsoft.MixedReality.Toolkit.Utilities

For namespaces with a large amount of types, it is acceptable to create a limited number of sub-namespaces to aid in scoping usage.

### Do:

```c#
namespace Microsoft.MixedReality.Toolkit.Boundary
{
    // Interface, class or data type definition.
}
```

Omitting the namespace for an interface, class or data type will cause your change to be blocked.

## Spaces vs Tabs
Please be sure to use 4 spaces instead of tabs when contributing to this project.

Additionally, ensure that spaces are added for conditional / loop functions like if / while / for

### Don't:

```c#
private Foo () // < - space between Foo and ()
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

## Spacing

Do not to add additional spaces between square brackets and parenthesis:

### Don't:

```c#
private Foo()
{
    int[ ] var = new int [ 9 ];
    Vector2 vector = new Vector2 ( 0f, 10f );
}

```

### Do:

```c#
private Foo()
{
    int[] var = new int[9];
    Vector2 vector = new Vector2(0f, 10f);
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

>Fields should always be `private`, with `public` or `protected` property accessors.

>Use [expression-bodied members](https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6#expression-bodied-function-members) and [auto properties](https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6#auto-property-enhancements) where possible

### Don't:

```c#
// protected field should be private
protected int myVariable = 0;

// property should have protected setter
public int MyVariable { get { return myVariable; } }

// No public / private access modifiers
void Foo() { }
void Bar() { }
```

### Do:

 ```c#
public int MyVariable { get; protected set; } = 0;

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
    if (Bar==null) DoThing(); <- if action on same line with no surrounding brackets 
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

## Public classes, structs, and enums should all go in their own files.

If the class, struct, or enum can be made private then it's okay to be included in the same file.  This avoids compilations issues with Unity and ensure that proper code abstraction occurs, it also reduces conflicts and breaking changes when code needs to change.

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

 MyStruct.cs
 ```c#
 // Public Struct / Enum definitions for use in your class.  Try to make them generic for reuse.
public struct MyStruct
{
    public string Var1;
    public string Var2;
}
```

MyEnumType.cs
```c#
public enum MuEnumType
{
    Value1,
    Value2 // <- note, no "," on last value to denote end of list.
}
```

MyClass.cs
```c#
public class MyClass
{
    private MyStruct myStructreference;
    private MyEnumType myEnumReference;
}
 ```

## Initialize Enums.

To ensure all Enum's are initialized correctly starting at 0, .NET gives you a tidy shortcut to automatically initialize the enum by just adding the first (starter) value.

> E.G. Value 1 = 0  (Remaining values are not required)

### Don't:

```c#
public enum Value
{
    Value1, <- no initializer
    Value2,
    Value3
}
```

### Do:

 ```c#
public enum ValueType
{
    Value1 = 0,
    Value2,
    Value3
}
 ```

## Order Enums for appropriate extension.

It is critical that if an Enum is likely to be extended in the future, to order defaults at the top of the Enum, this ensures Enum indexes are not affected with new additions.

### Don't:

```c#
public enum SDKType
{
    WindowsMR,
    OpenVR,
    OpenXR,
    None, <- default value not at start
    Other <- anonymous value left to end of enum
}
```

### Do:

 ```c#
    /// <summary>
    /// The SDKType lists the VR SDK's that are supported by the MRTK
    /// Initially, this lists proposed SDK's, not all may be implemented at this time (please see ReleaseNotes for more details)
    /// </summary>
    public enum SDKType
    {
        /// <summary>
        /// No specified type or Standalone / non-VR type
        /// </summary>
        None = 0,
        /// <summary>
        /// Undefined SDK.
        /// </summary>
        Other,
        /// <summary>
        /// The Windows 10 Mixed reality SDK provided by the Universal Windows Platform (UWP), for Immersive MR headsets and HoloLens. 
        /// </summary>
        WindowsMR,
        /// <summary>
        /// The OpenVR platform provided by Unity (does not support the downloadable SteamVR SDK).
        /// </summary>
        OpenVR,
        /// <summary>
        /// The OpenXR platform. SDK to be determined once released.
        /// </summary>
        OpenXR
    }
```

## End Enum names with "Type"
Enum names should clearly indicate their nature by using the Type suffix.
### Don't:
```c#
public enum Ordering
{
    First,
    Second,
    Third
}
```
```c#
public enum OrderingEnum
{
    First,
    Second,
    Third
}
```
### Do:
```c#
public enum OrderingType
{
    First = 0,
    Second,
    Third
}
```


## Review Enum use for Bitfields.

If there is a possibility for an enum to require multiple states as a value, e.g. Handedness = Left & Right. Then the Enum needs to be decorated correctly with BitFlags to enable it to be used correctly

> The Handedness.cs file has a concrete implementation for this

### Don't:

```c#
public enum Handedness
{
    None,
    Left,
    Right
}
```

### Do:

 ```c#
 [flags]
public enum HandednessType
{
    None = 0 << 0,
    Left = 1 << 0,
    Right = 1 << 1,
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
 // private field, only accessible within script (field is not serialized in Unity)
 private float myValue;
  ```

### Do:

 ```c#
 // Enable private field to be configurable only in editor (field is correctly serialized in Unity)
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
 // Enable field to be configurable in the editor and available externally to other scripts (field is correctly serialized in Unity)
 [SerializeField]
 [ToolTip("If using a tooltip, the text should match the public property's summary documentation, if appropriate.")]
 private float myValue; // <- Notice we co-located the backing field above our corresponding property.

 /// <summary>
 /// If using a tooltip, the text should match the public property's summary documentation, if appropriate.
 /// </summary>
 public float MyValue
 {
     get{ return myValue; }
     set{ myValue = value }
 }
 ```

## Use `for` instead of `foreach` when possible.

In some cases a foreach is required, e.g. when looping over an IEnumerable.  But for performance benefit, avoid foreach when you can.

### Don't:

```c#
foreach(var item in items)
```

### Do:

 ```c#
int length = items.length; // cache reference to list/array length
for(int i=0; i < length; i++)
 ```

## Cache values and serialize them in the scene/prefab whenever possible.

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

## Cache references to materials, do not call the ".material" each time.

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

## Prefer DateTime.UtcNow over DateTime.Now

DateTime.UtcNow is faster than DateTime.Now. In previous performance investigations we've found that using DateTime.Now adds significant overhead especially when used in the Update() loop. [Others have hit the same issue](https://stackoverflow.com/questions/1561791/optimizing-alternatives-to-datetime-now).

Prefer using DateTime.UtcNow unless you actually need the localized times (a legitmate reason may be you wanting to show the current time in the user's time zone). If you are dealing with relative times (i.e. the delta between some last update and now), it's best to use DateTime.UtcNow to avoid the overhead of doing timezone conversions.