
# Migration Window

When SDK components get obsolete and need upgrade, large projects might require an upgrade tool to allow user controlled Migration.
The [Migration Window](https://github.com/microsoft/MixedRealityToolkit-Unity/tree/mrtk_development/Assets/MixedRealityToolkit.SDK/Experimental/Features/Utilities/Migration/Tools/MigrationWindow) allows the user to select Scene Objects, entire Scenes or the Complete Project for Migration of specific obsolete Components. 

![Migration window](../../Documentation/Images/MigrationWindow/MRTK_Migration_Window.png)


## Usage
To open the window, select *Mixed Reality Toolkit->Utilities->Migration Window*. Once the Migration Window is open, the selection mode navigation tabs can be enabled by choosing the Component-specific implementation of the Migration Handler.  

![Migration selection modes](../../Documentation/Images/MigrationWindow/MRTK_Migration_Modes.png)


### Object Mode
Selecting the Objects tab, enables the Object Field to where the user can Drag and Drop any Game Objects from the currently open Scene or Prefabs from the Project folder to be Migrated.
Pressing the Remove *(-)* button displayed at the right side of the listed object removes the object from the Selection list.

Once all the desired objects are in the list, pressing the *Migrate* button will apply the changes required by the chosen Migration Handler implementation to all components in the selection that match the implementation.

![Selection Migration](../../Documentation/Images/MigrationWindow/MRTK_Object_Migration.png)


### Scene Mode
Allows user to Drag and Drop Scene Assets containing objects to be Migrated.

![Selecting Scenes for Migration](../../Documentation/Images/MigrationWindow/MRTK_Scene_Selection.png)


### Project Mode
Pressing the *Migrate* button will update the component targeted by the Migration Handler implementation for all Prefabs and Scenes in the Project.

![Migrating a complete Project](../../Documentation/Images/MigrationWindow/MRTK_Project_Migration.png)


> [!NOTE]
> Migration of custom Components require implementation of the IMigrationHandler interface specific for the Component.
