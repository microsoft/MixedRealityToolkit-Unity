using Pixie.Core;
using Pixie.Initialization;
using Pixie.StateControl;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.DeviceControl.Users
{
    public class UserTransforms : MonoBehaviour, IUserTransforms
    {
        private IUserObject userObject;
        private IAppStateReadWrite appState;

        private Dictionary<TransformTypeEnum, Transform> transformLookup = new Dictionary<TransformTypeEnum, Transform>();

        public bool GetTransform(TransformTypeEnum type, out Transform transform)
        {
            return transformLookup.TryGetValue(type, out transform);
        }

        private void OnEnable()
        {
            userObject = (IUserObject)gameObject.GetComponent(typeof(IUserObject));

            ComponentFinder.FindInScenes<IAppStateReadWrite>(out appState);
        }

        private void Update()
        {
            if (!userObject.IsAssigned)
                return;

            // Make sure we've created all the transforms that we need for our state
            foreach (LocalTransformState transformState in appState.GetStates<LocalTransformState>())
            {
                // Skip any states that don't belong to us
                if (transformState.UserID != userObject.UserID)
                    continue;

                // Skip any that aren't assigned
                if (transformState.Type == TransformTypeEnum.None)
                    continue;

                Transform localTransform = GetOrCreateTransform(transformState.Type);

                if (userObject.IsLocalUser)
                {
                    // If we're the local object, we want to copy our local transform's info into this state, then update it
                    // TODO assign velocities
                    LocalTransformState newTransformState = transformState;
                    newTransformState.LocalPosition = localTransform.localPosition;
                    newTransformState.LocalEulerAngles = localTransform.localEulerAngles;
                    appState.SetState<LocalTransformState>(newTransformState);
                }
                else
                {
                    // If we're the remote object, we want to copy this state into our local transform
                    localTransform.localPosition = transformState.LocalPosition;
                    localTransform.localEulerAngles = transformState.LocalEulerAngles;
                }
            }
        }

        private Transform GetOrCreateTransform(TransformTypeEnum type)
        {
            Transform localTransform = null;
            if (!transformLookup.TryGetValue(type, out localTransform))
            {
                localTransform = new GameObject(type.ToString()).transform;
                localTransform.parent = userObject.SceneAlignment;
                transformLookup.Add(type, localTransform);
            }
            return localTransform;
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (!userObject.IsAssigned)
                return;

            foreach (KeyValuePair<TransformTypeEnum,Transform> localTransform in transformLookup)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.TransformPoint(localTransform.Value.position), Vector3.one * 0.2f);
            }
        }
    }
}