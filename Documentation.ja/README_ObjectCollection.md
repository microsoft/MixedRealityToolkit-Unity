# Object collection #

![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollection_Main.jpg)

Object collection is a script to help lay out an array of objects in predefined three-dimensional shapes. It supports various surface styles including plane, cylinder, sphere, and radial. Radius, size, and the space between the items can be adjusted. Since it supports any object in Unity, it can be used to layout both 2D and 3D objects.

# Object collection scripts #
- [`GridObjectCollection.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/GridObjectCollection.cs) supports Cylinder, Plane, Sphere, Radial surface types
- [`ScatterObjectCollection.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/ScatterObjectCollection.cs) supports scattered style collection  
- [`TileGridObjectCollection.cs`](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.SDK/Features/UX/Scripts/Collections/TileGridObjectCollection.cs) provides some additional options to GridObjectCollection.

|![Grid Object Collection - Cylinder](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionCylinder.png) Grid Object Collection - Cylinder | ![Grid Object Collection - Sphere](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionSphere.png) Grid Object Collection - Sphere |
|:--- | :--- |
|![Grid Object Collection - Radial](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionRadial.png) Grid Object Collection - Radial | ![Grid Object Collection - Plane](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionPlane.png) Grid Object Collection - Plane |
|![Scattered Object Collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionScattered.png) Scattered Object Collection | ![Tile Grid Object Collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionTileGrid.png) Tile Grid Object Collection |


## How to use an object collection ##

To create a collection, create an empty GameObject and assign one of the Object Collection scripts to it. Any object(s) can be added as a child of the GameObject. Once finished adding child objects, click the *Update Collection* button in the inspector panel to generate the object collection. The objects will be laid out in the scene according to the selected surface type. Update Collection can be accessed thorugh the code too.




![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollectionScript.png)

## Object collection examples ##

The [ObjectCollectionExamples.unity](https://github.com/Microsoft/MixedRealityToolkit-Unity/blob/mrtk_release/Assets/MixedRealityToolkit.Examples/Demos/UX/Collections/Scenes/ObjectCollectionExamples.unity) example scene contains various examples of object collection types.

[Periodic table of the elements](https://github.com/Microsoft/MRDesignLabs_Unity_PeriodicTable) is an example app that demonstrates how object collections work. It uses object collection to layout the 3D element boxes in different shapes.

## Object collection types ##

**3D objects**
An object collection can be used to layout imported 3D objects. The example below shows the plane and cylindrical layouts of 3D chair model objects using a collection.

![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollection_3DObjects.jpg)

**2D Objects**

An object collection can also be crated from 2D images. For example, multiple images can be placed in a grid style.

![Object collection](../Documentation/Images/ObjectCollection/MRTK_ObjectCollection_Layout_2DImages.jpg)
