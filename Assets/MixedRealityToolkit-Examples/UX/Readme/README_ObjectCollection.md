# Object Collection
![Object Collection](/External/ReadMeImages/MRTK_ObjectCollection.jpg)

Object collection is a script which helps you lay out an array of objects in predefined three-dimensional shapes. It supports four different surface styles - plane, cylinder, sphere and scatter. You can adjust the radius, size and the space between the items. Since it supports any object in Unity, you can use it to layout both 2D and 3D objects.

## Object collection examples ##
Periodic Table of the Elements is an example app that demonstrates how Object collection works. It uses Object collection to layout the 3D element boxes in different shapes.

<img src="/External/ReadMeImages/MRTK_ObjectCollection_Types.jpg" alt="ObjectCollection">

### 3D Objects ###

You can use Object collection to layout imported 3D objects. The example below shows the plane and cylindrical layouts of 3D chair model objects using Object collection.

<img src="/External/ReadMeImages/MRTK_ObjectCollection_3DObjects.jpg" alt="ObjectCollection">

### 2D Objects ###

You can also use 2D images with Object collection. For example, you can easily display multiple images in grid style using Object collection.


<img src="/External/ReadMeImages/MRTK_ObjectCollection_Layout_3DObjects_3.jpg" alt="ObjectCollection">

<img src="/External/ReadMeImages/MRTK_ObjectCollection_Layout_2DImages.jpg" alt="ObjectCollection">

## Ways to use Object collection ##
You can find the examples in the scene **ObjectCollection_Examples.unity**. In this scene, you can find the **ObjectCollection.cs** script under **Assets/MixedRealityToolkit/UX/Scripts/Collections**

1. To create a collection, simply create an empty GameObject and assign the ObjectCollection.cs script to it. 
2. Then you can add any object(s) as a child of the GameObject. 
3. Once you finished adding a child object, click the **Update Collection** button in the Inspector Panel. 
4. You will then see the object(s) laid out in selected Surface Type. 


<img src="/External/ReadMeImages/MRTK_ObjectCollection_Unity.jpg" alt="ObjectCollection in Unity">

<img src="/External/ReadMeImages/MRTK_ObjectCollection_ExampleScene1.jpg" alt="ObjectCollection in Unity">

<img src="/External/ReadMeImages/MRTK_ObjectCollection_ExampleScene2.jpg" alt="ObjectCollection in Unity">
