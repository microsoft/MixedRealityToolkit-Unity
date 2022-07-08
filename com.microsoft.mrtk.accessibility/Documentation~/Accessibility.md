# Microsoft Mixed Reality Toolkit Accessibility

The accessibility package contains features that enable developers to create mixed reality experiences for everyone.

> [!NOTE]
> The accessibility package is in early preview and does not contain all of the planned features. The existing features
> may undergo major, breaking architectural changes before release.

## Features

### DescribableObject

> [!NOTE]
> DescribableObject is in the early stages of development and is provided to allow review and to gather feedback.

`DescribableObject` is a script that can be added to user interface elements and scene objects to enable a future assistive reader
to understand the role of the object as well as to provide details for the user.

For example, a person with low or no vision encounters a dialog in a mixed reality experience. The assistive reader recognizes that
the dialog is describable and indicates to the user, via a sound, that there is an object of interest nearby. The user expresses interest, the reader describes the dialog, its purpose and the contents to enable the user to interact as appropriate.

### Invert text color

For a person with low vision and/or issues with color perception, text can be difficult to read if the color does not properly contrast with the background.

The accessibility subsystem provides an option to automatically invert the color of TextMesh Pro text using the `TextAccessibility` script. To enable a TextMesh Pro object to support inversion, simply attach this script to the object.

> [!NOTE]
> The invert text color feature supports both backed and non-backed TextMesh Pro objects. In Augmented Reality experiences, non-backed text will not invert as effectively as backed text.
