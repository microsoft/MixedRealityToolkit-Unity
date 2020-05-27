
# Migration window

As the MRTK undergoes changes, some components might get deprecated and replacements will get introduced.
The migration window is a tool that helps users to automatically migrate a subset of those deprecated components to the new replacements.

![Migration window](../Images/MigrationWindow/MRTK_Migration_Window.png)

## Usage

To open the window, select *Mixed Reality Toolkit* > *Utilities* > *Migration Window*. Once the migration window is open, the selection mode navigation tabs can be enabled by choosing the component specific implementation of the migration handler.  

![Migration selection modes](../Images/MigrationWindow/MRTK_Migration_Modes.png)

### Object mode

Selecting the objects tab enables the object Field to where the user can drag and drop any Game objects from the currently open scene or prefabs from the project folder to be migrated.
Pressing the remove *(-)* button displayed at the right side of the listed object removes the object from the selection list.

Once all the desired objects are in the list, pressing the *Migrate* button will apply the changes required by the chosen migration handler implementation to all components in the selection that match the implementation.

![Selection migration](../Images/MigrationWindow/MRTK_Object_Migration.png)

### Scene mode

Allows user to drag and drop scene assets containing objects to be migrated.

![Selecting scenes for migration](../Images/MigrationWindow/MRTK_Scene_Selection.png)

### Project mode

Pressing the *Migrate* button will update the component targeted by the migration handler implementation for all prefabs and scenes in the project.

![Migrating a complete project](../Images/MigrationWindow/MRTK_Project_Migration.png)

## See also

- [Updating from earlier versions](../Updating.md)
- [Microsoft Mixed Reality Toolkit releases](../ReleaseNotes.md)
- [MRTK roadmap](../Contributing/Roadmap.md)
