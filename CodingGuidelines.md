# Coding Guidelines

This document outlines the recommended coding guidelines for the Mixed Reality Toolkit.  The majority of these suggestions follow the [recommended standards from MSDN](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

## Spaces vs Tabs
---
Please be sure to use 4 spaces when contributing to this project.

## Naming Conventions
---

Always use `PascalCase` for public properties, and `camelCase` for private properties and fields.
> The only exception to this is for data structures that require the fields to be serialized by the `JsonUtility`.

## Access Modifiers
---

Always declare an access modifier for all fields, properties and methods.

>All Unity API Methods should be `private` by default, unless you need to override them in a derived class. In this case `protected` should be used.

### <font color="red">Don't:</font>

```
void Foo(){ }
void Bar(){ }
```

### <font color="green">Do:</font>

 ```
private void Foo(){ }
public void Bar(){ }
 ```

## Encapsulation
---
Always use private fields and public properties if access to the field is needed from outside the class or struct.

If you need to have the ability to edit your field in the inspector, it's best practice to follow the rules for Encapsulation and serialize your backing field.

>The only exception to this is for data structures that require the fields to be serialized by the `JsonUtility`.

### <font color="red">Don't:</font>

```
public float MyValue;
```

### <font color="green">Do:</font>

 ```
 [SerializeField] // Only use this attribute to view/edit in inspector.
 private float myValue;
 public float MyValue
 {
     get{ return myValue; }
     set{ myValue = value }
 }
 ```

## Code for Performance in mind
---
Some of the target platforms of this project require us to take performance into consideration.  What this in mind we should always be careful of allocating memory in frequently called code in tight update loops or algorithms.

> Use `for` instead of `foreach` when possible.

### <font color="red">Don't:</font>

```
foreach(var item in items)
```

### <font color="green">Do:</font>

 ```
for(int i=0; i < items.length; i++)
 ```

>Cache values and serialize them in the scene/prefab whenever possible.

### <font color="red">Don't:</font>

```
void Update()
{
    gameObject.GetComponent<Renderer>().Foo(Bar);
}
```

### <font color="green">Do:</font>

 ```
 [SerializeField] // You could set the reference in the inspector.
 private Renderer myRenderer;

private void Awake()
{
    // If you didn't set it in the inspector, then we cache it on awake.
    if(myRenderer == null)
    {
        myRenderer = gameObject.GetComponent<Renderer>();
    }
}

private void Update()
{
    myRenderer.Foo(Bar);
}

 ```
