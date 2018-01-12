#############################
# HoloToolkit-Examples\Dialogs
#############################

Fitbox
-------

    /// <summary>
    /// 
    /// Used to place the scene origin on startup
    /// 
    /// A Fitbox can be used to allow the user to move to a position and place content in that position.
    /// 
    /// A Fitbox covers a healthy portion of the screen and follows the users gaze(but doesnt screen lock).  It initially disables the content associated with it.
    /// It can be used to give important guidance to the user before allowing him/her to continue with the application.
    /// Once a user clicks on the Fitbox, it will dissapear and allow the user to view the scene.Additionally you can set a flag to reposition the scene object relative to where the Fitbox was dismissed.  Some have used it as a type of splash screen before showing a initial set of menus or content.
    /// 
    /// Fitbox has three properties that can be set in the editor.
    ///     bool MoveCollectionOnDismiss
    ///     GameObject StartupObject
    ///     float Distance 
    ///     
    /// StartupObject refers to The scene object to activate and possibly reposition
    /// MoveCollectionOnDismiss toggles wether you want to reposition the StartupObject to the place where the Fitbox was clicked on (defaults to false) 
    /// Distance refers to the distance at which you want the Fitbox, and subsequent content, to display in front of the user. This is fed to the interpolator. (defaults to 1.0f )
    /// 
    /// Fitbox requires an Interpolator, A Holotoolkit MonoBehaviour that interpolates a transform's position, rotation or scale.
    /// and helps the fitbox smoothly stay in front of the user wherever he/she moves
    /// 
    /// A FitBox also has a child object Quad whose material can be set to a logo or some image representing guidance to the user
    /// You could replace this with your own child objects.
    /// 
    /// If the user moves anywhere the fitbox will stay at the same distance set in front of the user and this is where the followup content may be moved to if MoveCollectionOnDismiss is set to true
    ///
    /// </summary>


HoloToolkit-Examples\Dialogs\Prefabs
------------------------------------

--Fitbox
is a prefab that has already assigned the HoloToolkit\Dialogs\Scripts\Fitbox.cs and required HoloToolkit\Utilities\Scripts\Interpolator.cs
it also has a quad child object with proper rotation and scale set for displaying a 1623x996 .png  you can edit/replace this "logo" to your liking or 
remove the quad and add your own child objects.

( the logo material and png is also located in this directory and named FBWWLogo )

( this is a copy of the same prefab found at HoloToolkit\Dialogs\Prefabs )


HoloToolkit-Examples\Dialogs\Scenes
------------------------------------

--FitboxTest.unity
is a scene that shows how to integrate a fitbox into your application.

This scene is set up with the basic Holotoolkit components such as 
MixedRealityCameraParent
DefaultCursor
Directional Light
InputManager


Of notable importance is the InputManager which uses the Holotoolkits input system to forward your clicks to the Fitbox.
This works because HoloToolkit\Dialogs\Scripts\Fitbox.cs implements the OnInputClicked(InputClickedEventData eventData)  function of 
HoloToolkit.Unity.InputModule.IInputClickHandler


In addition the scene has the HoloToolkit\Dialogs\Prefabs\Fitbox prefab set up with some screen content ready for test
just press play

the Hierarchy object ContentYouWantToShowAfterFitBoxClick will be shown after the user clicks on the Fitbox and moved to where the user moved the fitbox to if MoveCollectionOnDismiss was set to true

-enjoy




##########################
###### HoloToolkit\Dialogs
###########################

HoloToolkit\Dialogs\Scripts
----------------------------
Fitbox.cs

contains the script HoloToolkit.Unity.Dialogs.Fitbox which has the signature 
public class Fitbox : MonoBehaviour, IInputClickHandler
ships with the HoloToolkit


HoloToolkit\Dialogs\Prefabs
---------------------------
--Fitbox
a copy of the same Fitbox prefab explained above , 
ships with the HoloToolkit

