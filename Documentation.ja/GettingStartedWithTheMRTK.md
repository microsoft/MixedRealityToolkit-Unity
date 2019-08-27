# MRTKを始める

![MRTK Logo](../Documentation/Images/MRTK_Logo_Rev.png)

Mixed Reality Toolkit (MRTK) は、Virtual Reality (VR) 及び Augmented Reality (AR) の Mixed Reality エクスペリエンス を構築するためのクロスプラットフォームツールキットです。

## 前提条件

Mixed Reality Toolkit を始めるには、次のものが必要です。

* [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)
* [Unity 2018.4.x, 2019.1.x or 2019.2.x](https://unity3d.com/get-unity/download/archive)

  MRTKは、 Unity 2018 で IL2CPP と .NET scripting backends の両方をサポートします。

* [最新の MRTK release](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases)
* [Windows SDK 18362+](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk).

  これは WMR、HoloLens 1、または HoloLens 2 向けのUWPアプリを構築する場合に必要です。OpenVR向けに構築する場合は不要です。

## チュートリアルを始める

MRTK、またはMR開発が初めての場合は、MRTKv2を使った [チュートリアルを始める](https://docs.microsoft.com/en-us/windows/mixed-reality/mrlearning-base)をチェックすることをお勧めします。

## MRTK を Unity Projectに追加する

### 最新の MRTK Unity パッケージを取得する

1. [MRTK のリリースページ](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases) を開きます。
2. Assets の下から以下をダウンロードします。
    - `Microsoft.MixedRealityToolkit.Unity.Foundation.unitypackage`
    - `Microsoft.MixedRealityToolkit.Unity.Extensions.unitypackage`
    - `Microsoft.MixedRealityToolkit.Unity.Tools.unitypackage`
    - `Microsoft.MixedRealityToolkit.Unity.Examples.unitypackage`

より詳細な配布の仕組みは、[MRTKをダウンロードする](DownloadingTheMRTK.md)を参照して下さい。

### Unity プロジェクトをターゲットプラットフォームに切り替える

次のステップで、**MRTK のパッケージ を Unity プロジェクトへインポート** すると、インポートした時点でプロジェクトで選択されているプラットフォームに応じた変更がプロジェクトに適応されます。

次のステップへ進む前に、正しいプラットフォームを選択していることを確認してください。

例えば、HoloLensアプリケーションを作成する場合は、Universal Windows Platform に切り替えます。

- File > Build Settings からメニューを開きます。
- **Platform** の一覧から、**Universal Windows Platform** を選択します。
- **Switch Platform** ボタンを押します。

### MRTK のパッケージを Unity プロジェクトにインポートする

1. 新しい Unity プロジェクトを作成するか、既存のプロジェクトを開きます。新しいプロジェクトを作成する場合は、テンプレートタイプに "3D" が選択されていることを確認してください。
1. ダウンロードした `Microsoft.MixedRealityToolkit.Unity.Foundation.unitypackage` をインポートします。「Asset -> Import Package -> Custom Package」から, .unitypackage ファイルを選択し, インポートする全ての項目がチェックされていることを確認してから、「Import」を選択します.
1. `Microsoft.MixedRealityToolkit.Unity.Examples.unitypackage` も上記と同様の手順でインポートします。examples のパッケージは、オプションであり、現在の MRTK の機能の有用なデモシーンが含まれています。
1. `Microsoft.MixedRealityToolkit.Unity.Tools.unitypackage` も Foundaiton パッケージと同様にインポートします。tools のパッケージは、オプションで、MRTK 開発者のエクスペリエンスを向上させる、ExtensionServiceCreator などの便利なツールが含まれています。
1. Import the `Microsoft.MixedRealityToolkit.Unity.Extensions.unitypackage` も Foundaiton パッケージと同様にインポートします。 extensions パッケージは、オプションで、 MRTK の便利なオプションコンポーネントのセットを提供します。

Foundation パッケージをインポートすると、次のようなセットアッププロンプトが表示される場合があります。

![UnitySetupPrompt](../Documentation/Images/MRTK_UnitySetupPrompt.png)

MRTK は、以下を実行することで Mixed Reality ソリューションを構築するためのプロジェクトをセットアップします。

 * 現在のプラットフォームで XR Settings を有効にします (XR チェックボックスを有効にします)。
 * テキストのシリアライズを強制 / メタファイルの可視化 （ソース管理をする　Unity プロジェクトに推奨）。

これらのオプションを適応するかは選択的ですが、推奨されています。

一部のプレハブ、及びアセットには、TextMesh Pro が必要です。つまり、TextMesh Pro のパッケージをインストールし、アセットがプロジェクト内にあることが必要です。（Window -> TextMeshPro -> Import TMP Essential Resources）
 **TMP Essentials Resources をインポートした後、変更を確認するには Unity を再起動する必要があります**。

### HandInteractionExamples のシーンを Editor で開いて実行する

[![HandInteractionExample scene](../Documentation/Images/MRTK_Examples.png)](README_HandInteractionExamples.md)

[Hand Interaction のサンプルシーン](README_HandInteractionExamples.md) は、MRTK の様々なUXコントロールとインタラクションを公開しているため、MRTK を始めるには最適な場所です。はじめに、MRTK をインポートし、サンプルシーンを開いて、Editor でシーンを探索します。

1. 新しい Unity プロジェクトを作成し、[上記の手順](#import-mrtk-packages-into-your-unity-project) に従って、**Foundation** と **Examples** の Unity パッケージの両方をインポートします。
2. `Assets\MixedRealityToolkit.Examples\Demos\HandTracking\Scenes\HandInteractionExamples` の下の HandInteractionExamples　のシーンを開きます。

3. 「TMP Essentials」をインポートするかを尋ねるプロンプトが表示されます。

![TMP Essentials](../Documentation/Images/getting_started/MRTK_GettingStarted_TMPro.png)

このようなプロンプトが表示された場合は、「Import TMP essentials」 ボタンを選択します。「TMP Essentials」とは、Text Mesh プライグインを指し、MRTK のサンプルの一部はテキストレンダリングを改善するために使用しています。(詳細については、[Unity のテキスト](https://docs.microsoft.com/en-us/windows/mixed-reality/text-in-unity)を参照してください。)

4. Close the TMP dialog. After this you need to reload the scene. You can do this by double clicking the scene in the project tab.
4. TMP ダイアログを閉じます。この後、シーンをリロードする必要があります。これを行うには、プロジェクトタブでシーンをダブルクリックします。

5. Playボタンを押します。

## Using the In-Editor Hand Input Simulation to test a scene

The in-editor input simulation allows you to test virtual object behavior given a specific type of input such as [hands](InputSimulation/InputSimulationService.md#hand-simulation) or [eyes](EyeTracking/EyeTracking_BasicSetup.md#simulating-eye-tracking-in-the-unity-editor).

How to move around in the scene: 
- Use W/A/S/D keys to move the camera forward/left/back/right.
- Press and hold the right mouse to rotate the camera.

How to simulate hand input:
- Press and hold the space bar to enable the right hand. 
- While holding the space bar, move your mouse to move the hand.
- Use the middle mouse scroll to adjust the depth of the hand.
- Click the left mouse to switch gestures.

Have fun exploring the scene! You can learn more about the UI controls [in the hand interaction examples guide](README_HandInteractionExamples.md). Also, read through [input simulation docs](InputSimulation/InputSimulationService.md) to learn more about in-editor hand input simulation in MRTK.

Congratulations, you just used your first MRTK scene. Now onto creating your own experiences...

### Add MRTK to a new scene or new project

1. Create a new Unity project, or start a new scene in your current project. 

2. Make sure you have imported the MRTK packages (we recommend both Foundation and Examples, though Examples is not required) following [the steps above](#import-mrtk-packages-into-your-unity-project).

3. From the menu bar, select Mixed Reality Toolkit -> Add to Scene and Configure

![Configure to scene](../Documentation/Images/MRTK_ConfigureScene.png)

4. You will see a prompt like this:

![MRTK Configure Dialog](../Documentation/Images/MRTK_ConfigureDialog.png)

Click "OK".

5. You will then be prompted to choose an MRTK Configuration profile. Double click "DefaultMixedRealityToolkitConfigurationProfile".

![MRTK Select Configure Dialog](../Documentation/Images/MRTK_SelectConfigurationDialog.png)

> **NOTE**: Note that if you are getting started on the HoloLens 2, you should choose the "DefaultHoloLens2ConfigurationProfile" instead.
> See the [profiles](Profiles/Profiles.md#hololens-2-profile) for more information on the differences between 
> DefaultMixedRealityToolkitConfigurationProfile and DefaultHoloLens2ConfigurationProfile.

You will then see the following in your Scene hierarchy:

![MRTK Scene Setup](../Documentation/Images/MRTK_SceneSetup.png)

Which contains the following:

* Mixed Reality Toolkit - The toolkit itself, providing the central configuration entry point for the entire framework.
* MixedRealityPlayspace - The parent object for the headset, which ensures the headset / controllers and other required systems are managed correctly in the scene.
* The Main Camera is moved as a child to the Playspace - Which allows the playspace to manage the camera in conjunction with the SDKs

**Note** While working in your scene, **DO NOT move the Main Camera** (or the playspace) from the scene origin (0,0,0).  This is controlled by the MRTK and the active SDK.
If you need to move the players start point, then **move the scene content and NOT the camera**!

6. Hit play and test out hand simulation by pressing spacebar.

You are now ready to build and deploy to device! Follow the steps instructions at [Build and Deploy MRTK](BuildAndDeploy.md).

## Next steps

Here are some suggested next steps:

* Add a [PressableButton](README_Button.md) to your scene (we recommend using the `PressableButtonPlated` prefab to start)).
* Add a cube to your scene, then make it movable using the [ManipulationHandler](README_ManipulationHandler.md) component.
* Learn about the UX controls available in MRTK in [building blocks for UI and interactions](#building-blocks-for-ui-and-interactions).
* Read through [input simulation guide](InputSimulation/InputSimulationService.md) to learn how to simulate hand input in editor.
* Learn how to work with the MRTK Configuration profile in the [mixed reality configuration guide](MixedRealityConfigurationGuide.md).

## Building blocks for UI and interactions

|  [![Button](Images/Button/MRTK_Button_Main.png)](README_Button.md) [Button](README_Button.md) | [![Bounding Box](Images/BoundingBox/MRTK_BoundingBox_Main.png)](README_BoundingBox.md) [Bounding Box](README_BoundingBox.md) | [![Manipulation Handler](Images/ManipulationHandler/MRTK_Manipulation_Main.png)](README_ManipulationHandler.md) [Manipulation Handler](README_ManipulationHandler.md) |
|:--- | :--- | :--- |
| A button control which supports various input methods including HoloLens 2's articulated hand | Standard UI for manipulating objects in 3D space | Script for manipulating objects with one or two hands |
|  [![Slate](Images/Slate/MRTK_Slate_Main.png)](README_Slate.md) [Slate](README_Slate.md) | [![System Keyboard](Images/SystemKeyboard/MRTK_SystemKeyboard_Main.png)](README_SystemKeyboard.md) [System Keyboard](README_SystemKeyboard.md) | [![Interactable](Images/Interactable/InteractableExamples.png)](README_Interactable.md) [Interactable](README_Interactable.md) |
| 2D style plane which supports scrolling with articulated hand input | Example script of using the system keyboard in Unity  | A script for making objects interactable with visual states and theme support |
|  [![Solver](Images/Solver/MRTK_Solver_Main.png)](README_Solver.md) [Solver](README_Solver.md) | [![Object Collection](Images/ObjectCollection/MRTK_ObjectCollection_Main.png)](README_ObjectCollection.md) [Object Collection](README_ObjectCollection.md) | [![Tooltip](Images/Tooltip/MRTK_Tooltip_Main.png)](README_Tooltip.md) [Tooltip](README_Tooltip.md) |
| Various object positioning behaviors such as tag-along, body-lock, constant view size and surface magnetism | Script for lay out an array of objects in a three-dimensional shape | Annotation UI with flexible anchor/pivot system which can be used for labeling motion controllers and object. |
|  [![App Bar](Images/AppBar/MRTK_AppBar_Main.png)](README_AppBar.md) [App Bar](README_AppBar.md) | [![Pointers](Images/Pointers/MRTK_Pointer_Main.png)](/Input/Pointers.md) [Pointers](/Input/Pointers.md) | [![Fingertip Visualization](Images/Fingertip/MRTK_FingertipVisualization_Main.png)](README_FingertipVisualization.md) [Fingertip Visualization](README_FingertipVisualization.md) |
| UI for Bounding Box's manual activation | Learn about various types of pointers | Visual affordance on the fingertip which improves the confidence for the direct interaction |
|  [![Slider](Images/Slider/MRTK_UX_Slider_Main.jpg)](README_Sliders.md) [Slider](README_Sliders.md) | [![MRTK Standard Shader](Images/MRTKStandardShader/MRTK_StandardShader.jpg)](README_MRTKStandardShader.md) [MRTK Standard Shader](README_MRTKStandardShader.md) | [![Hand Joint Chaser](Images/HandJointChaser/MRTK_HandJointChaser_Main.jpg)](README_HandJointChaser.md) [Hand Joint Chaser](README_HandJointChaser.md) |
| Slider UI for adjusting values supporting direct hand tracking interaction | MRTK's standard shader supports various fluent design elements with performance | Demonstrates how to use solver to attach objects to the hand joints |
|  [![Eye Tracking: Target Selection](Images/EyeTracking/mrtk_et_targetselect.png)](EyeTracking/EyeTracking_TargetSelection.md) [Eye Tracking: Target Selection](EyeTracking/EyeTracking_TargetSelection.md) | [![Eye Tracking: Navigation](Images/EyeTracking/mrtk_et_navigation.png)](EyeTracking/EyeTracking_Navigation.md) [Eye Tracking: Navigation](EyeTracking/EyeTracking_Navigation.md) | [![Eye Tracking: Heat Map](Images/EyeTracking/mrtk_et_heatmaps.png)](EyeTracking/EyeTracking_ExamplesOverview.md#visualization-of-visual-attention) [Eye Tracking: Heat Map](EyeTracking/EyeTracking_ExamplesOverview.md#visualization-of-visual-attention) |
| Combine eyes, voice and hand input to quickly and effortlessly select holograms across your scene | Learn how to auto scroll text or fluently zoom into focused content based on what you are looking at| Examples for logging, loading and visualizing what users have been looking at in your app |


## Tools
|  [![Optimize Window](Images/MRTK_Icon_OptimizeWindow.png)](Tools/OptimizeWindow.md) [Optimize Window](Tools/OptimizeWindow.md) | [![Dependency Window](Images/MRTK_Icon_DependencyWindow.png)](Tools/DependencyWindow.md) [Dependency Window](Tools/DependencyWindow.md) | ![Build Window](Images/MRTK_Icon_BuildWindow.png) Build Window | [![Input recording](Images/MRTK_Icon_InputRecording.png)](InputSimulation/InputAnimationRecording.md) [Input recording](InputSimulation/InputAnimationRecording.md) |

| :--- | :--- | :--- | :--- |
| Automate configuration of Mixed Reality projects for performance optimizations | Analyze dependencies between assets and identify unused assets |  Configure and execute end-to-end build process for Mixed Reality applications | Record and playback head movement and hand tracking data in-editor |

## Upgrading from the HoloToolkit (HTK/MRTK v1)

There is not a direct upgrade path from the HoloToolkit to Mixed Reality Toolkit v2 due to the rebuilt framework. However, it is possible to import the MRTK into your HoloToolkit project and migrate your implementation. For more information please see the [HoloToolkit to Mixed Reality Toolkit Porting Guide](HTKToMRTKPortingGuide.md)
