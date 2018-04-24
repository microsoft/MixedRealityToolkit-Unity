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
    public class MyManager : BaseManager, IManagerIdentifierInterface
    {
        /// <summary>
        /// MyManager constructor
        /// </summary>
        public MyManager()
        {
            //Get relevant properties from the active profile of the Manager 
            MyManagerProperty1 = MixedRealityManager.Instance.ActiveProfile.ManagerProperty1;
            MyManagerProperty2 = MixedRealityManager.Instance.ActiveProfile.ManagerProperty2;
        }

        /// <summary>
        /// IMixedRealityManager Initialize function, called once the Mixed Reality Manager has finished registering all managers
        /// </summary>
        void Initialize()
        {
            //Initialize stuff
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
        private void Reset()
        {
            //React to profile change
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// </summary>
        void Update()
        {
            if (Enabled)
            {
                //Update stuff

            }
        }

        /// <summary>
        /// Optional Disable function to pause the manager.
        /// </summary>
        void Disable()
        {
            //Destroy stuff
        }

        /// <summary>
        /// Optional Enable function to enable / re-enable the manager.
        /// </summary>
        void Enable()
        {
            //Destroy stuff
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed
        /// </summary>
        void Destroy()
        {
            //Destroy stuff
        }
    }
```

# MixedReality Runtime Managers (Components)
The Mixed Reality Manager is also extensible enough that is can support runtime as well as design time managers, the key differences being:

## Mixed Reality Design Time Managers 

* Can only have a single instance (e.g. InputSystem, there can only be one)

Register
---
```
MixedRealityManager.Instance.AddManager(typeof(Internal.Interfaces.IMixedRealityInputSystem), new InputSystem.MixedRealityInputManager());
```

Retrieve
---
```
var inputSystem = MixedRealityManager.Instance.GetManager<Internal.Interfaces.IMixedRealityInputSystem>();
```

## Mixed Reality Runtime Managers

* Supports as many instances as you like, stored by interface type
* Also supports being named, in case you need an individual manager of a type that has a specific name

Register
---
```
//Add test component 1
MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager1), new TestComponentManager1());

//Add test component 2
MixedRealityManager.Instance.AddManager(typeof(ITestComponentManager2), new TestComponentManager2() { Name = "Tester2" });
```

Retrieve
---
```
//Retrieve Component1
var component1 = MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager1));

//Retrieve component named Tester2
TestComponentManager2 component2 = (TestComponentManager2)MixedRealityManager.Instance.GetManager(typeof(ITestComponentManager2), "Tester2");

//Get all components of Type 2
var type2Components = MixedRealityManager.Instance.GetManagers(typeof(ITestComponentManager2));
```

To keep things simple, the Mixed Reality Manager can already distinguish between each type of Manager and will manage them accordingly, so it is the same code for registering and retrieving managers.

# Manager Interfaces

The Manager container uses a predefined Interface type for storage and retrieval of any Manager, this ensures there are no hard dependencies within the Mixed Reality Toolkit, so that each subsystem can easily be swapped out with another (so long as it confirms to the interface).

Current System interfaces provided by the Mixed Reality Toolkit include:

* IMixedRealityInputSystem
* IMixedRealityBoundarySystem

When creating your own bespoke versions of these systems, you much ensure they comply with the interfaces provided by the Mixed Reality Toolkit (e.g. if you replace the InputSystem with another of your own design).

Additionally, if you create your own Runtime managers, these must also have their own Interface which derives from *IMixedRealityManager* to ensure it is supported by the Mixed Reality Manager.

> All Managers must also inherit from the **BaseManager** class, to implement the functions required by the Mixed Reality Manager for individual manager operation.  E.G. Initialize, Update, Destroy.