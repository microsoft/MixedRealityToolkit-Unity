# Coding Guidelines

This document outlines the recommended coding guidelines for the Mixed Reality Toolkit.  The majority of these suggestions follow the [recommended standards from MSDN](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

---

## Spaces vs Tabs
Please be sure to use 4 spaces instead of tabs when contributing to this project.

Additionally, ensure that spaces are added for conditional / loop functions like if / while / for

### Don't:

```
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

 ```
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

---

## Naming Conventions

Always use `PascalCase` for public / protected / virtual properties, and `camelCase` for private / internal properties and fields.
> The only exception to this is for data structures that require the fields to be serialized by the `JsonUtility`.

### Don't:

```
public string myProperty; // <- Starts with a lower case letter
private string MyProperty; // <- Starts with an uppercase case letter
```

### Do:

 ```
public string MyProperty;
protected string MyProperty;
private string myProperty;
internal string myProperty;
 ```
---

## Access Modifiers


Always declare an access modifier for all fields, properties and methods.

>All Unity API Methods should be `private` by default, unless you need to override them in a derived class. In this case `protected` should be used.

### Don't:

```
// No public / private / internal access modifiers
void Foo(){ }
void Bar(){ }
```

### Do:

 ```
private void foo(){ }
public void Bar(){ }
 ```

---

## Use Braces

Always use braces to surround the action of an IF.

### Don't:

```
private Foo()
{
    if (Bar==null) // <- missing braces surrounding if action
        DoThing();
    else
        DoTheOtherThing();
}
```

### Don't:

```
private Foo() { // <- Open bracket on same line
    if (Bar==null) DoThing(); <- if action on same line with no surrounding brackets 
    else DoTheOtherThing();
}
```

### Do:

```
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

## Encapsulation
---

Always use private fields and public properties if access to the field is needed from outside the class or struct.

If you need to have the ability to edit your field in the inspector, it's best practice to follow the rules for Encapsulation and serialize your backing field.

>The only exception to this is for data structures that require the fields to be serialized by the `JsonUtility`, where a data class is required to have all public fields for the serialization to work.

### Don't:

```
public float MyValue;
```

### Do:

 ```
 // private field, only accessible within script (field is not serialized in Unity)
 private float myValue;
  ```

### Do:

 ```
 // Enable private field to be configurable only in editor (field is correctly serialized in Unity)
 [SerializeField] 
 private float myValue;
  ```

 ### Do:

 ```
 // Enable field to be configurable in the editor and available externally to other scripts (field is correctly serialized in Unity)
 [SerializeField] 
 private float myValue;
 public float MyValue
 {
     get{ return myValue; }
     set{ myValue = value }
 }
 ```

Public classes, structs, and enums should all go in their own files.
---

>If the class, struct, or enum can be made private then it's okay to be included in the same file.  This avoid compilations issues with Unity and ensure that proper code abstraction occurs, it also reduces conflicts and breaking changes when code needs to change.

### Don't:

```
public class MyClass
{
    public struct MyStruct() { }
    public enum MyEnumType() { }
    public class MyNestedClass() { }
}
```

### Do:

 ```
 // Private references for use inside the class only
public class MyClass
{
    private struct MyStruct() { }
    private enum MyEnumType() { }
    private class MyNestedClass() { }
}
 ```

 ### Do:

 ```
 // Public Struct / Enum definitions for use in your class.  Try to make them generic for reuse.
MyStruct.cs
public struct MyStruct
{
    public string Var1;
    public string Var2;
}

MyEnumType.cs
public enum MuEnumType
{
    Value1,
    Value2 // <- note, no "," on last value to denote end of list.
}

MyClass.cs
public class MyClass
{
    private MyStruct myStructreference;
    private MyEnumType myEnumReference;
}
 ```
---

## Best Practices, including Unity recommendations
Some of the target platforms of this project require us to take performance into consideration.  What this in mind we should always be careful of allocating memory in frequently called code in tight update loops or algorithms.

---

Use `for` instead of `foreach` when possible.
---
> In some cases a foreach is required, e.g. when looping over an IEnumerable.  But for performance benefit, avoid foreach when you can.

### Don't:

```
foreach(var item in items)
```

### Do:

 ```
int length = items.length; // cache reference to list/array length
for(int i=0; i < length; i++)
 ```

---

Cache values and serialize them in the scene/prefab whenever possible.
---

### Don't:

```
void Update()
{
    gameObject.GetComponent<Renderer>().Foo(Bar);
}
```

### Do:

 ```
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

---

Cache references to materials, do not call the ".material" each time.
---

Unity will create a new material each time you use ".material", which will cause a memory leak if not cleaned up properly.

### Don't:

```
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

 ```
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
}
 ```

> Alternatively, use Unity's "SharedMaterial" property which does not create a new material each time it is referenced.