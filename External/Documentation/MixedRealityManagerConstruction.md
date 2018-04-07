<img src="External/ReadMeImages/MRTK_Logo_Rev.png">

# What makes a MixedRealityManager

To avoid the performance overheads of the MonoBehaviour class, all Managers (components that require independent operation in a Mixed Reality Solution, e.g. InputSystem, Boundary, SpatialUnderstanding) are required to be discrete classes which are registered with the MixedRealityManager.

The MixedRealityManager then coordinates all referencing between components and ensures that all managers receive the appropriate events (E.g. Awake/initialize, Update, Destroy) as well as being the central component where managers request references to other managers.

Additionally, the Mixed Reality manager also maintains the active VR/XR/AR SDK in use in the running project, to initialize the active device based on attached hardware and instigate proper operation.

# A Manager

An individual manager can be any component that needs to be active and live in the project.  If it needs to be alive in a scene and act on features, then it can be maintained by the Mixed Reality Manager.  This brings several benefits over the traditional MonoBehaviour method, namely:

* Performance - without the overhead of a MonoBehaviour, scripts are updates approximately 80% faster and don't need a gameobject to live.
* Referenceability - Managers can be discovered from the manager a lot faster and easier that searching objects in a scene
* No Type dependency - Though a method similar to Dependency Injection, managers can be decoupled from their type, this means the actual manager can be swapped out at any time without affecting code that consumes it.  E.G. Replacing the InputSystem with another of your choosing.

If a manager does need a game object in a scene (for example the Focus manager which needs a point in space to "look" at), it can simply reference / create that gameobject rather than be bound to it, which makes it a lot easier to manage.

# Manager Example

```
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.<Feature>
{
    /// <summary>
    /// An example manager
    /// </summary>
    public class MyManager : BaseManager
    {
        /// <summary>
        /// MyManager constructor
        /// </summary>
        public MyManager()
        {
            //Attach to Manager events
            MixedRealityManager.Instance.InitializeEvent += InitializeInternal;
            MixedRealityManager.Instance.UpdateEvent += Update;
            MixedRealityManager.Instance.DestroyEvent += Destroy;

            //Get relevant properties from the active profile of the Manager 
            MyManagerProperty1 = MixedRealityManager.Instance.ActiveProfile.ManagerProperty1;
            MyManagerProperty2 = MixedRealityManager.Instance.ActiveProfile.ManagerProperty2;
        }

        /// <summary>
        /// IMixedRealityManager Initialize function, called once the Mixed Reality Manager has finished registering all managers
        /// Subscription required to the "InitializeEvent" event of the MixedRealityManager
        /// </summary>
        void InitializeInternal()
        {
            //Initialize stuff 
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// Subscription required to the "UpdateEvent" event of the MixedRealityManager
        /// </summary>
        void Update()
        {
            if (Enabled)
            {
                //Update stuff 

            }
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// Subscription required to the "ProfileUpdateEvent" event of the MixedRealityManager
        /// </summary>
        private void ProfileUpdate()
        {
            //React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed
        /// Subscription required to the "DestroyEvent" event of the MixedRealityManager
        /// </summary>
        void Destroy()
        {
            //Destroy stuff 
        }
    }
```