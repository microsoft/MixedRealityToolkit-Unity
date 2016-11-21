## [UI]()

Useful common UI controls that you can leverage in your application.



### [Materials](Materials)

Materials used in prefabs

#### 3DTextSegoeUI.mat

Material for 3DTextPrefab with occlusion support. Requires 3DTextShader.shader

### [Prefabs](Prefabs)

Common useful UI prefabs 

**IMPORTANT**: To use Text prefabs, please make sure to import Segoe UI.TTF font file from your system font folder (C:\Windows\Fonts\)  

1. Assign font texture to 3DTextSegoeUI.mat material. 
2. On 3DTextSegoeUI.mat material, select the shader Custom/3DTextShader.shader This material will make text object occluded by the object in front of them.
3. Assign Segoe UI font and 3DTextSegoeUI material to the text components in the prefabs.
4. Follow the same steps for the sample text layout prefabs in HoloToolkit-Examples\Text




#### 3DTextPrefab.prefab

3D Text Mesh prefab with optimized scaling factor at 2-meter distance.



#### UITextPrefab.prefab

UI Text Mesh prefab with optimized scaling factor at 2-meter distance.




### [Scripts](Scripts)



### [Shaders](Shaders)

Materials used in prefabs

#### 3DTextShader.shader

Shader for 3DTextPrefab with occlusion support. Assign this to 3DTextSegoeUI.mat

---
##### [Go back up to the table of contents.](../../../README.md)
---