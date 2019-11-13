# Hand Physics Extension Service
The Hand Physics service enables rigid body collision events and interactions with articulated hands.

## Enabling the extension
To enable the extension, open your RegisteredServiceProvider profile. Click Register a new Service Provider to add a new configuration. In the Component Type field, select HandPhysicsService. In the Configuration Profile field, select the default hand physics profile included with the extension.

## Profile Options
### Hand Physics Layer
Controls the layer the instantiated hand joints will go to.

While the service defaults to the "default" layer (0), it is recommended to use a separate layer for Hand Physics objects. Otherwise there may be unwanted collisions and/or inaccurate raycasts.

### Finger Tip Kinematic Body Prefab
Controls which prefab is instantiated on fingertips. In order for the service to work as expected, the prefab requires:
- A rigidbody component, with isKinematic enabled
- A collider
- `JointKinematicBody` component

### Use Palm Kinematic Body
Controls whether the service will attempt to instantiate a prefab on the palm joint.

### Palm Kinematic Body Prefab
When UsePalmKinematicBody is enabled, this is the prefab it will instantiate. Just like `FingerTipKinematicBodyPrefab`, this prefab requires:
- A rigidbody component, with isKinematic enabled
- A collider
- `JointKinematicBody` component

## How to use the service
Once enabled, use any collider's `IsTrigger` property to receive collision events from all 10 digits (and palms if they're enabled).