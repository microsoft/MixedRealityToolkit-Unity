# Readme: AppState Sharing and Networking Demos
This app goes through a number of demos using our AppState Sharing system.

The components in these demos use Photon networking, but any networking package could in theory be used.

To run an example make sure both instances of the application are on the same demo. Press the "Connect as server" button to create the server; one instance must be designated as the server before you can have two instances interact.

Also ensure that both the server and the client are "Connected and in room" before starting. 

NextDemo button loads the next demo in the list. 
Reload, loads the current demo again. 

## Demo: BasicAppState
Demonstrates raw app state data.

States can be added by manually designating the stateID or by having a stateID randomly generated. The value of a state can also be set.

Flush propagates local changes to other users.

## Demo: BasicHologramStates
Demonstrates app data being used to control the appearance of in-game objects.

Hologram's ColorR, ColorG, ColorB, are properties of the hologram's object state.

Colors can be changed on the server/client and have their states passed to other connected instances when the "Flush" button is pressed.

## Demo: HologramStateObjects
Demonstrates the StateObjects component, a manager which automatically spawns / syncs prefabs to states in the AppState.

StateObject scripts make it easy for objects to recieve changed states as well as modify their own state.

The objects in this demo choose new target positions for themselves at random intervals.

Flushing of new states is handled automatically.

Servers or clients can add objects to the scene at any time.

## Demo: SessionsDemo
Sessions lets you chop up your app experience into stages that can be stepped through in a sequence.

Stages can be used for tutorials, automated scene switching, and experience progression management. 

In this example the progression of stages is controlled by the server. The server initalizes, starts, can pause, and can forceably switch to the session's next stage.

Sessions can have any number of stages. In this example we've included 5:
* InitializeStage - Adds 5 holograms, then waits for user to proceed
* StageA - Randomly changes hologram colors between red / blue
* StageB - Randomly changes hologram colors between cyan / magenta
* StageC - Randomly changes hologram colors between yellow / green
* FinalStage - Sets all hologram colors to white

The SessionState has these properties:
* ItemID
* SessionID
* LayoutSceneName - scene to be loaded on initialization
* State - State of Stage
    * WaitingForApp
    * Initializing
    * ReadyToStart
    * InProgress
    * Completed
* SessionStartTime - Time that a stage starts
* CurrentStageNum - Stage number
* CurrentStageState - State of the current stage:
    * "Not Started"
    * "Started"
    * "Running"
    * "Stopped"
* CurrentStageType: 
    * Automatic
    * Manual
* CurrentStageStartTime - Start time of the current stage

## Demo: TimeSyncDemo
Demonstrates a utility to sync time across all users. Time synchronization is an important feature of broadcast experiences.

This functionality is available in most networking packages, but the behavior from package to package is often inconsistent.

When the client joins NetworkTime.Time will be at zero and will sync to the servers time rapidly.

## Demo: DevicesAndUsersDemos
Demonstrates using the app state to manage users and devices.

The scene is split between a management view, at the top of the screen, and a scene view, occupying the bottom of the scene. 

When the server starts, you will see a number of User Definition slots. These slots designate devices with specific roles: Participant, Spectator, or Observer. 

Once a client is connected the Devices column will fill with a Device node. Click the Node to assign it to a slot (this can happen on both the client or server).

** Make sure that the window for viewing the demo is large enough to see the entire management slate and scene view. **

## Demo: StateLoggingAndPlayback

Demonstrates two components, AppStateLogger and AppStatePlayback. These let you record and then play back experiences.

AppStateLogger stores a log of time-stamped changes made to the AppState.

AppStatePlayback can read that log and re-play the changes.

Objects don't know the difference between 'real' AppState data and pre-recorded data.
