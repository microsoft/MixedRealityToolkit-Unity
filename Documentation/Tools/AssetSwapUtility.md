# Asset swap utility

Find and replace is ubiquitous when working in text and content creation tools. When you need to swap many assets within a Unity scene this is where the `AssetSwapUtility` (xref:Microsoft.MixedReality.Toolkit.Utilities.Editor.AssetSwapUtility) [ScriptableObject](https://docs.unity3d.com/Manual/class-ScriptableObject.html) and editor can lend a hand. The utility is included with the `Microsoft.MixedReality.Toolkit.Unity.Tools` package.


The `AssetSwapUtility` saves all find and replace actions as a ScriptableObject so that it is trival to swap back and forth or save swap "themes" for future use.

## Swapping assets

Swapping assets is easy once you have created a `AssetSwapCollection`. Let's demonstrate use by swapping two red cubes with two blue spheres in a scene. First add two red cubes to your scene that use the default Unity cube and the `MRTK_Standard_Red` material.

To create an `AssetSwapCollection`, navigate to *Mixed Reality Toolkit > Utilities > Create Asset Swap Collection* . With the `AssetSwapCollection` selected fill out the properties as seen in the below image:

![Asset swap utility example](../../Documentation/Images/AssetSwapUtility/MRTK_AssetSwapUtility_Example_AssetSwapCollection.png)

Next select "Blue Spheres" from the "Selected Theme" dropdown and hit "Apply." All red cubes within your scene should be replaced with blue spheres.

![Asset swap utility apply](../../Documentation/Images/AssetSwapUtility/MRTK_AssetSwapUtility_Apply_AssetSwapCollection.png)

In this example we performed a full scene replacement but you may replace portions of your scene by changing the "Selection Mode." You may also swap back to red cubes by selecting "Red Cubes" from the "Selected Theme" dropdown and hitting "Apply" again.

Note, it's possible to swap any asset type such as audio files, fonts, prefabs, etc. The `AssetSwapUtility` will perform a few sanity checks to ensure you are swapping to similar types.
