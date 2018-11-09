
Photon Unity Networking (PUN)
	This package is a re-implementation of Unity 4's Networking, using Photon.
	Also included: A setup wizard, demo scenes, documentation and Editor extensions.


PUN & PUN+
	PUN is the free package of Photon Unity Networking. Export to iOS or Android from Unity 4 requires Unity Pro Licenses.
	PUN+ contains special native plugins that allow export to mobiles from Unity 4.x Free. You also get a Photon Cloud subscription upgrade. See below.
    Unity 5 does not restrict export to mobiles.


Android and iOS Exports
	See "PUN & PUN+"
    iOS:    Set the stripping level to "Strip Bytecode" and use ".Net 2.0" in the player settings. 
            More aggressive stripping will break the runtime and you can't connect anymore with PUN Free.


UnityScript / JavaScript
    PUN is written with C# in mind primarily.
    To use PUN from UnityScript, you need to move some folders in your project.
    Move both folders "PhotonNetwork" and "UtilityScripts" to the Assets\Plugins\ folder.
        from:   \Photon Unity Networking\Plugins\
        and:    \Photon Unity Networking\
        to:     \Plugins\
    Now PUN compiles before UnityScript and that makes it available from regular UnityScript code.


Help and Documentation
	Please read the included chm (or pdf).
    Online documentation:   https://doc.photonengine.com/en-us/pun
	Exit Games Forum:       https://forum.photonengine.com/categories/unity-networking-plugin-pun
    Unity Forum Thread:     https://forum.unity3d.com/threads/photon-unity-networking.101734/


Integration
	This package adds an Editor window "PUN Wizard" for connection setup:
		Menu -> Window -> Photon Unity Networking (shortcut: ALT+P)
	It also adds a commonly used component "PhotonView" to this menu:
		Menu -> Component -> Miscellaneous -> PhotonView (shortcut: ALT+V)
	When imported into a new, empty project, the "PunStartup" script opens the "demo hub" and setup scenes to build.


Clean PUN Import (no demos)
	To get a clean import of PUN and PUN+ into your project, just skip the folders "Demos" and "UtilityScripts".
    UtilityScripts can be useful for rapid prototyping but are optional to use.
    "Important Files" are listed below.


Server
	Exit Games Photon can be run on your servers or you can subscribe to the Photon Cloud for managed servers.
	
	The window "Photon Unity Networking" will help you setup a Photon Cloud account.
	This service is geared towards room-based games and the server cannot be modified.
	Read more about it: http://www.photonengine.com

	Alternatively, download the Server SDK and run your own Photon Server.
	The SDK has the binaries to run immediately but also includes the source code and projects
	for the game logic. You can use that as basis to modify and extend it.
	A 100 concurrent user license is free (also for commercial use) per game.
	Read more about it: http://www.photonengine.com/en/OnPremise


PUN+ and Networking Guide Subscriptions
    Follow these steps when you bought an asset that includes an upgrade for a Photon Cloud subscription:
        • Use an existing Photon Cloud Account or register.     https://www.photonengine.com/Account/SignUp
        • Sign in and open the Dashboard.                       https://dashboard.photonengine.com
        • Select the Subscription to upgrade and click "Apply Unity Purchase".
        • Enter your Unity Invoice Number and App ID.
        
        • You find the App ID on: https://dashboard.photonengine.com
        • You find your Unity Invoice Number in the Unity AssetStore: 
            https://www.assetstore.unity3d.com/en/#!/account/transactions
            Or while logged in to the Asset Store, click on your name on the top right. 
            From the drop-down select the payment method you used to obtain PUN+).
            Navigate to your PUN+ purchase and copy the number following the "#" symbol (excluding the "#" and spaces).


Important Files

	Documentation
		PhotonNetwork-Documentation.chm (a pdf is also included)
		changelog.txt

	Extensions & Source
		Photon Unity Networking\Editor\PhotonNetwork\*.*
		Photon Unity Networking\Plugins\PhotonNetwork\*.*
        Plugins\**\Photon*.*
        

	The server-setup will be saved as file (can be moved into any Resources folder and edited in inspector)
		Photon Unity Networking\Resources\PhotonServerSettings.asset

	Demos
		All demos are in separate folders in Photon Unity Networking\Demos\. Delete this folder in your projects.
		Each has a Demo<name>-Scene.
