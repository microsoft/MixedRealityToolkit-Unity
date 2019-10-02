# Material Instance

The [`MaterialInstance.cs`](xref:Microsoft.MixedReality.Toolkit.Rendering.MaterialInstance) behavior aides in tracking instance material lifetime and automatically destroys instanced materials for the user. This utility component can be used as a replacement to [Renderer.material]("https://docs.unity3d.com/ScriptReference/Renderer-material.html") or 
[Renderer.materials]("https://docs.unity3d.com/ScriptReference/Renderer-materials.html"). Note, [MaterialPropertyBlocks](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) are preferred over material instancing but are not always available  in all scenarios.


## Usage

When invoking Unity's [Renderer.material]("https://docs.unity3d.com/ScriptReference/Renderer-material.html")(s), Unity automatically instantiates new materials. It is the caller's responsibility to destroy the materials when a material is no longer needed or the game object is destroyed. The [`MaterialInstance.cs`](xref:Microsoft.MixedReality.Toolkit.Rendering.MaterialInstance) behavior helps avoid material leaks and keeps material allocation paths consistent during edit and run time.

When a [MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) can not be used and a material must be instanced, [`MaterialInstance.cs`](xref:Microsoft.MixedReality.Toolkit.Rendering.MaterialInstance) can be used as follows:

```csharp
public class MyBehaviour : MonoBehaviour
{
    // Assigned via the inspector. 
    public Renderer targetRenderer;

    private void OnEnable()
    {
        Material material = targetRenderer.EnsureComponent<MaterialInstance>().Material;
        material.color = Color.red;
        ...
    }
}
```

If multiple objects need ownership of the material instance it's best to take explicit ownership for reference tracking. (An optional interface called [`IMaterialInstanceOwner.cs`](xref:Microsoft.MixedReality.Toolkit.Rendering.IMaterialInstanceOwner) exists to aide with ownership.) Below is example usage:

```csharp
public class MyBehaviour : MonoBehaviour,  IMaterialInstanceOwner
{
    // Assigned via the inspector. 
    public Renderer targetRenderer;

    private void OnEnable()
    {
        Material material = targetRenderer.EnsureComponent<MaterialInstance>().AcquireMaterial(this);
        material.color = Color.red;
        ...
    }

    private void OnDisable()
    {
        targetRenderer.GetComponent<MaterialInstance>()?.ReleaseMaterial(this)
    }

    public void OnMaterialChanged(MaterialInstance materialInstance)
    {
        // Optional method for when materials change outside of the MaterialInstance.
        ...
    }
}
```


For more information please see the example usage demonstrated within the [`ClippingPrimitive.cs`](xref:Microsoft.MixedReality.Toolkit.Utilities.ClippingPrimitive) behavior.
