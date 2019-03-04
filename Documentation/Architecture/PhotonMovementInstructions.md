These instructions allow developers to enable synchronization of GameObject movement accross the network using Photon without any coding.

Step 1: For the GameObject which the developer desires to synchronize movement, select the GameObject and click the "add component" button in the inspector
Step 2: Search for and add the PhotonTransforView.cs component. Other depedent compenents will automatically be added, such as the photonview.cs script.
Step 3: In the Photon Transform View component, choose the properties you wish to sync over the network. Choices include position, rotation, and scale.

That's it! Shared movements with no coding.

Note: By default - the first person who joins will "own" the object, and only that person can maniulate the object. Everyone else will see changes in the tansform in real time, but will not be able to move/rotate/scale the object

Sean Ong will soon implement another capability to pass ownership between different participants without needing to code anything.

There is an example cube in the OngSharingPrefab for syncing object movements accross the network.
