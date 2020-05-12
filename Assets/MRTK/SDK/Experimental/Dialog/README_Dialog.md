# Dialog
![Dialog](../../../../../Documentation/Images/Dialog/MRTK_UX_Dialog_Main.png)

Dialog controls are UI overlays that provide contextual app information. They often request some kind of action from the user. Use dialogs to notify users of important information or to request confirmation or additional info before an action can be completed. 

## Example scene
You can find examples in the **DialogExample** scene under:
[MRTK/Examples/Experimental/Dialog](https://github.com/microsoft/MixedRealityToolkit-Unity/tree/mrtk_development/Assets/MRTK/SDK/Experimental/Dialog)

## How to use Dialog control
MRTK provides three Dialog prefabs: 
- DialogSmall_192x96.prefab
- DialogMedium_192x128.prefab
- DialogLarge_192x192.prefab

Use Dialog.Open() to open a new dialog. Specify the dialog prefab, number of buttons, title text, message text, placement distance(near or far), additional variables). Dialog provides 'Confirmation(single button)' and 'Choice(two-buttons)' dialog options. 

```c#
public static Dialog Open(GameObject dialogPrefab, DialogButtonType buttons, string title, string message, bool placeForNearInteraction, System.Object variable = null)
```

### Example of opening Large dialog with single 'OK' button, placed at far interaction range (gaze, hand ray, motion controller):
```c#
Dialog.Open(DialogPrefabLarge, DialogButtonType.OK, "Confirmation Dialog, Large, Far", "This is an example of a large dialog with only one button, placed at far interaction range", false);
```

### Example of opening Small dialog with single 'OK' button, placed at near interaction range (direct hand interaction):
```c#
Dialog.Open(DialogPrefabSmall, DialogButtonType.Yes | DialogButtonType.No, "Confirmation Dialog, Small, Far", "This is an example of a small dialog with a choice message, placed at near interaction range", true);
```

For more details, please see `DialogExampleController.cs` in DialogExample.unity scene.