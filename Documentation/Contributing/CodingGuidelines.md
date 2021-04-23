# Coding guidelines

This document outlines coding principles and conventions to follow when contributing to MRTK.

---

## Philosophy

### Be concise and strive for simplicity

The simplest solution is often the best. This is an overriding aim of these guidelines and should be the goal of all coding activity. Part of being simple is being concise, and consistent with existing code. Try to keep your code simple.

Readers should only encounter artifacts that provide useful information. For example, comments that restate what is obvious provide no extra information and increase the noise to signal ratio.

Keep code logic simple. Note that this is not a statement about using the fewest number of lines, minimizing the size of identifier names or brace style, but about reducing the number of concepts and maximizing the visibility of those through familiar patterns.

### Produce consistent, readable code

Code readability is correlated with low defect rates. Strive to create code that is easy to read. Strive to create code that has simple logic and re-uses existing components as it will also help ensure correctness.

All details of the code you produce matter, from the most basic detail of correctness to consistent style and formatting. Keep your coding style consistent with what already exists, even if it is not matching your preference. This increases the readability of the overall codebase.

### Support configuring components both in editor and at run-time

MRTK supports a diverse set of users – people who prefer to configure components in the Unity editor and load prefabs, and people who need to instantiate and configure objects at run-time.

All your code should work by BOTH adding a component to a GameObject in a saved scene, and by instantiating that component in code. Tests should include a test case both for instantiating prefabs and instantiating, configuring the component at runtime.

### Play-in-editor is your first and primary target platform

Play-In-Editor is the fastest way to iterate in Unity. Providing ways for our customers to iterate quickly allows them to both develop solutions more quickly and try out more ideas. In other words, maximizing the speed of iteration empowers our customers to achieve more.

Make everything work in editor, then make it work on any other platform. Keep it working in the editor. It is easy to add a new platform to Play-In-Editor. It is very difficult to get Play-In-Editor working if your app only works on a device.

### Add new public fields, properties, methods and serialized private fields with care

Every time you add a public method, field, property, it becomes part of MRTK’s public API surface. Private fields marked with `[SerializeField]` also expose fields to the editor and are part of the public API surface. Other people might use that public method, configure custom prefabs with your public field, and take a dependency on it.

New public members should be carefully examined. Any public field will need to be maintained in the future. Remember that if the type of a public field (or serialized private field) changes or gets removed from a MonoBehaviour, that could break other people. The field will need to first be deprecated for a release, and code to migrate changes for people that have taken dependencies would need to be provided.

### Prioritize writing tests

MRTK is a community project, modified by a diverse range of contributors. These contributors may not know the details of your bug fix / feature, and accidentally break your feature. [MRTK runs continuous integration tests](https://dev.azure.com/aipmr/MixedRealityToolkit-Unity-CI/_build?definitionId=16) before completing every pull request. Changes that break tests cannot be checked in. Therefore, tests are the best way to ensure that other people do not break your feature.

When you fix a bug, write a test to ensure it does not regress in the future. If adding a feature, write tests that verify your feature works. This is required for all UX features except experimental features.

## C# Coding conventions

### Script license information headers

All Microsoft employees contributing new files should add the following standard License header at the top of any new files, exactly as shown below:

```c#
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
```

### Function / method summary headers

All public classes, structs, enums, functions, properties, fields posted to the MRTK should be described as to its purpose and use, exactly as shown below:

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

Any script files submitted without proper summary tags will be rejected.

### MRTK namespace rules

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

Omitting the namespace for an interface, class or data type will cause your change to be blocked.

### Adding new MonoBehaviour scripts

When adding new MonoBehaviour scripts with a pull request, ensure the [`AddComponentMenu`](https://docs.unity3d.com/ScriptReference/AddComponentMenu.html) attribute is applied to all applicable files. This ensures the component is easily discoverable in the editor under the *Add Component* button. The attribute flag is not necessary if the component cannot show up in editor such as an abstract class.

In the example below, the *Package here* should be filled with the package location of the component. If placing an item in *MRTK/SDK* folder, then the package will be *SDK*.

```c#
[AddComponentMenu("Scripts/MRTK/{Package here}/MyNewComponent")]
public class MyNewComponent : MonoBehaviour
```

### Adding new Unity inspector scripts

In general, try to avoid creating custom inspector scripts for MRTK components. It adds additional overhead and management of the codebase that could be handled by the Unity engine.

If an inspector class is necessary, try to use Unity's [`DrawDefaultInspector()`](https://docs.unity3d.com/ScriptReference/Editor.DrawDefaultInspector.html). This again simplifies the inspector class and leaves much of the work to Unity.

```c#
public override void OnInspectorGUI()
{
    // Do some custom calculations or checks
    // ....
    DrawDefaultInspector();
}
```

If custom rendering is required in the inspector class, try to utilize [`SerializedProperty`](https://docs.unity3d.com/ScriptReference/SerializedProperty.html) and [`EditorGUILayout.PropertyField`](https://docs.unity3d.com/ScriptReference/EditorGUILayout.PropertyField.html). This will ensure Unity correctly handles rendering nested prefabs and modified values.

If [`EditorGUILayout.PropertyField`](https://docs.unity3d.com/ScriptReference/EditorGUILayout.PropertyField.html) cannot be used due to a requirement in custom logic, ensure all usage is wrapped around a [`EditorGUI.PropertyScope`](https://docs.unity3d.com/ScriptReference/EditorGUI.PropertyScope.html). This will ensure Unity renders the inspector correctly for nested prefabs and modified values with the given property.

Furthermore, try to decorate the custom inspector class with a [`CanEditMultipleObjects`](https://docs.unity3d.com/ScriptReference/CanEditMultipleObjects.html). This tag ensure multiple objects with this component in the scene can be selected and modified together. Any new inspector classes should test that their code works in this situation in the scene.

```c#
    // Example inspector class demonstrating usage of SerializedProperty & EditorGUILayout.PropertyField
    // as well as use of EditorGUI.PropertyScope for custom property logic
    [CustomEditor(typeof(MyComponent))]
    public class MyComponentInspector : UnityEditor.Editor
    {
        private SerializedProperty myProperty;
        private SerializedProperty handedness;

        protected virtual void OnEnable()
        {
            myProperty = serializedObject.FindProperty("myProperty");
            handedness = serializedObject.FindProperty("handedness");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(destroyOnSourceLost);

            Rect position = EditorGUILayout.GetControlRect();
            var label = new GUIContent(handedness.displayName);
            using (new EditorGUI.PropertyScope(position, label, handedness))
            {
                var currentHandedness = (Handedness)handedness.enumValueIndex;

                handedness.enumValueIndex = (int)(Handedness)EditorGUI.EnumPopup(
                    position,
                    label,
                    currentHandedness,
                    (value) => {
                        // This function is executed by Unity to determine if a possible enum value
                        // is valid for selection in the editor view
                        // In this case, only Handedness.Left and Handedness.Right can be selected
                        return (Handedness)value == Handedness.Left
                        || (Handedness)value == Handedness.Right;
                    });
            }
        }
    }
```

### Adding new ScriptableObjects

When adding new ScriptableObject scripts, ensure the [`CreateAssetMenu`](https://docs.unity3d.com/ScriptReference/CreateAssetMenu.html) attribute is applied to all applicable files. This ensures the component is easily discoverable in the editor via the asset creation menus. The attribute flag is not necessary if the component cannot show up in editor such as an abstract class.

In the example below, the *Subfolder* should be filled with the MRTK subfolder, if applicable. If placing an item in *MRTK/Providers* folder, then the package will be *Providers*. If placing an item in the *MRTK/Core* folder, set this to "Profiles".

In the example below, the *MyNewService | MyNewProvider* should be filled with the your new class' name, if applicable. If placing an item in the *MixedRealityToolkit* folder, leave this string out.

```c#
[CreateAssetMenu(fileName = "MyNewProfile", menuName = "Mixed Reality Toolkit/{Subfolder}/{MyNewService | MyNewProvider}/MyNewProfile")]
public class MyNewProfile : ScriptableObject
```

### Logging

When adding new features or updating existing features, consider adding DebugUtilities.LogVerbose
logs to interesting code that may be useful for future debugging. There's a tradeoff here between
adding logging and the added noise and not enough logging (which makes diagnosis difficult).

An interesting example where having logging is useful (along with interesting payload):

```c#
DebugUtilities.LogVerboseFormat("RaiseSourceDetected: Source ID: {0}, Source Type: {1}", source.SourceId, source.SourceType);
```

This type of logging can help catch issues like https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8016,
which were caused by mismatched source detected and source lost events.

Avoid adding logs for data and events that are occurring on every frame - ideally logging should
cover "interesting" events driven by distinct user inputs (i.e. a "click" by a user and the set of
changes and events that come from that are interesting to log). The ongoing state of "user is still
holding a gesture" logged every frame is not interesting and will overwhelm the logs.

Note that this verbose logging is not turned on by default (it must be enabled in the
[Diagnostic System settings](../Diagnostics/ConfiguringDiagnostics.md#enable-verbose-logging))

### Spaces vs tabs

Please be sure to use 4 spaces instead of tabs when contributing to this project.

### Spacing

Do not to add additional spaces between square brackets and parenthesis:

#### Don't

```c#
private Foo()
{
    int[ ] var = new int [ 9 ];
    Vector2 vector = new Vector2 ( 0f, 10f );
}

```

#### Do

```c#
private Foo()
{
    int[] var = new int[9];
    Vector2 vector = new Vector2(0f, 10f);
}
```

### Naming conventions

Always use `PascalCase` for properties. Use `camelCase` for most fields, except use `PascalCase` for `static readonly` and `const` fields. The only exception to this is for data structures that require the fields to be serialized by the `JsonUtility`.

#### Don't

```c#
public string myProperty; // <- Starts with a lowercase letter
private string MyField; // <- Starts with an uppercase letter
```

#### Do

```c#
public string MyProperty;
protected string MyProperty;
private static readonly string MyField;
private string myField;
```

### Access modifiers

Always declare an access modifier for all fields, properties and methods.

- All Unity API Methods should be `private` by default, unless you need to override them in a derived class. In this case `protected` should be used.

- Fields should always be `private`, with `public` or `protected` property accessors.

- Use [expression-bodied members](https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6#expression-bodied-function-members) and [auto properties](https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6#auto-property-enhancements) where possible

#### Don't

```c#
// protected field should be private
protected int myVariable = 0;

// property should have protected setter
public int MyVariable => myVariable;

// No public / private access modifiers
void Foo() { }
void Bar() { }
```

#### Do

```c#
public int MyVariable { get; protected set; } = 0;

private void Foo() { }
public void Bar() { }
protected virtual void FooBar() { }
```

### Use braces

Always use braces after each statement block, and place them on the next line.

#### Don't

```c#
private Foo()
{
    if (Bar==null) // <- missing braces surrounding if action
        DoThing();
    else
        DoTheOtherThing();
}
```

#### Don't

```c#
private Foo() { // <- Open bracket on same line
    if (Bar==null) DoThing(); <- if action on same line with no surrounding brackets
    else DoTheOtherThing();
}
```

#### Do

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

### Public classes, structs, and enums should all go in their own files

If the class, struct, or enum can be made private then it's okay to be included in the same file.  This avoids compilations issues with Unity and ensure that proper code abstraction occurs, it also reduces conflicts and breaking changes when code needs to change.

#### Don't

```c#
public class MyClass
{
    public struct MyStruct() { }
    public enum MyEnumType() { }
    public class MyNestedClass() { }
}
```

#### Do

```c#
 // Private references for use inside the class only
public class MyClass
{
    private struct MyStruct() { }
    private enum MyEnumType() { }
    private class MyNestedClass() { }
}
```

#### Do

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
    private MyStruct myStructReference;
    private MyEnumType myEnumReference;
}
```

### Initialize enums

To ensure all enums are initialized correctly starting at 0, .NET gives you a tidy shortcut to automatically initialize the enum by just adding the first (starter) value. (e.g Value 1 = 0 Remaining values are not required)

#### Don't

```c#
public enum Value
{
    Value1, <- no initializer
    Value2,
    Value3
}
```

#### Do

```c#
public enum ValueType
{
    Value1 = 0,
    Value2,
    Value3
}
```

### Order enums for appropriate extension

It is critical that if an Enum is likely to be extended in the future, to order defaults at the top of the Enum, this ensures Enum indexes are not affected with new additions.

#### Don't

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

#### Do

```c#
/// <summary>
/// The SDKType lists the VR SDKs that are supported by the MRTK
/// Initially, this lists proposed SDKs, not all may be implemented at this time (please see ReleaseNotes for more details)
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

### Review enum use for bitfields

If there is a possibility for an enum to require multiple states as a value, e.g. Handedness = Left & Right. Then the Enum needs to be decorated correctly with BitFlags to enable it to be used correctly

The Handedness.cs file has a concrete implementation for this

### Don't

```c#
public enum Handedness
{
    None,
    Left,
    Right
}
```

### Do

```c#
[Flags]
public enum Handedness
{
    None = 0 << 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Both = Left | Right
}
```

### Hard-coded file paths

When generating string file paths, and in particular writing hard-coded string paths, do the following:

1. Use C#'s [`Path` APIs](https://docs.microsoft.com/dotnet/api/system.io.path?view=netframework-4.8) whenever possible such as `Path.Combine` or `Path.GetFullPath`.
1. Use / or [`Path.DirectorySeparatorChar`](https://docs.microsoft.com/dotnet/api/system.io.path.directoryseparatorchar?view=netframework-4.8) instead of \ or \\\\.

These steps ensure that MRTK works on both Windows and Unix-based systems.

### Don't

```c#
private const string FilePath = "MyPath\\to\\a\\file.txt";
private const string OtherFilePath = "MyPath\to\a\file.txt";

string filePath = myVarRootPath + myRelativePath;
```

### Do

```c#
private const string FilePath = "MyPath/to/a/file.txt";
private const string OtherFilePath = "folder{Path.DirectorySeparatorChar}file.txt";

string filePath = Path.Combine(myVarRootPath,myRelativePath);

// Path.GetFullPath() will return the full length path of provided with correct system directory separators
string cleanedFilePath = Path.GetFullPath(unknownSourceFilePath);
```

## Best practices, including Unity recommendations

Some of the target platforms of this project require to take performance into consideration. With this in mind always be careful when allocating memory in frequently called code in tight update loops or algorithms.

### Encapsulation

Always use private fields and public properties if access to the field is needed from outside the class or struct.  Be sure to co-locate the private field and the public property. This makes it easier to see, at a glance, what backs the property and that the field is modifiable by script.

> [!NOTE]
> The only exception to this is for data structures that require the fields to be serialized by the `JsonUtility`, where a data class is required to have all public fields for the serialization to work.

#### Don't

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

#### Do

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
    get => myValue;
    set => myValue = value;
}

/// <summary>
/// Getter/Setters not wrapping a value directly should contain documentation comments just as public functions would
/// </summary>
public float AbsMyValue
{
    get
    {
        if (MyValue < 0)
        {
            return -MyValue;
        }

        return MyValue
    }
}
```

### Cache values and serialize them in the scene/prefab whenever possible

With the HoloLens in mind, it's best to optimize for performance and cache references in the scene or prefab to limit runtime memory allocations.

#### Don't

```c#
void Update()
{
    gameObject.GetComponent<Renderer>().Foo(Bar);
}
```

#### Do

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

### Cache references to materials, do not call the ".material" each time

Unity will create a new material each time you use ".material", which will cause a memory leak if not cleaned up properly.

#### Don't

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

#### Do

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

> [!NOTE]
> Alternatively, use Unity's "SharedMaterial" property which does not create a new material each time it is referenced.

### Use [platform dependent compilation](https://docs.unity3d.com/Manual/PlatformDependentCompilation.html) to ensure the Toolkit won't break the build on another platform

- Use `WINDOWS_UWP` in order to use UWP-specific, non-Unity APIs. This will prevent them from trying to run in the Editor or on unsupported platforms. This is equivalent to `UNITY_WSA && !UNITY_EDITOR` and should be used in favor of.
- Use `UNITY_WSA` to use UWP-specific Unity APIs, such as the `UnityEngine.XR.WSA` namespace. This will run in the Editor when the platform is set to UWP, as well as in built UWP apps.

This chart can help you decide which `#if` to use, depending on your use cases and the build settings you expect.

| | UWP IL2CPP | UWP .NET | Editor |
| --- | --- | --- | --- |
| `UNITY_EDITOR` | False | False | True |
| `UNITY_WSA` | True | True | True |
| `WINDOWS_UWP` | True | True | False |
| `UNITY_WSA && !UNITY_EDITOR` | True | True | False |
| `ENABLE_WINMD_SUPPORT` | True | True | False |
| `NETFX_CORE` | False | True | False |

### Prefer DateTime.UtcNow over DateTime.Now

DateTime.UtcNow is faster than DateTime.Now. In previous performance investigations we've found that using DateTime.Now adds significant overhead especially when used in the Update() loop. [Others have hit the same issue](https://stackoverflow.com/questions/1561791/optimizing-alternatives-to-datetime-now).

Prefer using DateTime.UtcNow unless you actually need the localized times (a legitimate reason may be you wanting to show the current time in the user's time zone). If you are dealing with relative times (i.e. the delta between some last update and now), it's best to use DateTime.UtcNow to avoid the overhead of doing timezone conversions.

## PowerShell coding conventions

A subset of the MRTK codebase uses PowerShell for pipeline infrastructure and various scripts and utilities. New PowerShell code should follow the [PoshCode style](https://poshcode.gitbooks.io/powershell-practice-and-style/).

## See also

 [C# coding conventions from MSDN](https://docs.microsoft.com/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
