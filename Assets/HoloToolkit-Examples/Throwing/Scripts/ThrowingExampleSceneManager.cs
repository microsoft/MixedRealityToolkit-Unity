using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class ThrowingExampleSceneManager : MonoBehaviour
{
    public GameObject[] ThrowingAssets;
    public InteractionSourceNode ControllerPose = InteractionSourceNode.Grip;
    public Transform RealWorldRoot;

    private readonly Dictionary<uint, Transform> devices = new Dictionary<uint, Transform>();
    private readonly Dictionary<uint, int> modelIndecies = new Dictionary<uint, int>();
    private readonly Dictionary<uint, bool> isDetatched = new Dictionary<uint, bool>();

    // Use this for initialization
    void Start ()
    {
		if (ThrowingAssets.Length == 0)
        {
            throw new System.Exception("ThrowingSceneManager needs to have some throwable objects!");
        }   

        InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
        Application.onBeforeRender += Application_onBeforeRender;
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
    {
        uint id = args.state.source.id;
        if (args.pressType == InteractionSourcePressType.Menu || args.pressType == InteractionSourcePressType.Touchpad)
        {
            if (devices.ContainsKey(id))
            {
                int modelIndex = this.modelIndecies[id];
                RemoveDevice(id);
                AddDevice(id, ++modelIndex % ThrowingAssets.Length);
            }
            else if (modelIndecies.ContainsKey(id))
            {
                this.modelIndecies[id] = ++this.modelIndecies[id] % ThrowingAssets.Length;
            }
        }
        else if (args.pressType == InteractionSourcePressType.Grasp || args.pressType == InteractionSourcePressType.Select)
        {
            if (isDetatched.ContainsKey(id))    
            {
                isDetatched[id] = false;
            }
        }
    }

    private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
    {
        if (args.pressType == InteractionSourcePressType.Grasp || args.pressType == InteractionSourcePressType.Select)
        {
            uint id = args.state.source.id;
            if (devices.ContainsKey(id))
            {
                var go = devices[id];
                var rigidbody = go.GetComponent<Rigidbody>();
                if (rigidbody == null)
                {
                    rigidbody = go.GetComponentInChildren<Rigidbody>();
                }
                if (rigidbody.TryThrow(args.state.sourcePose))
                {
                    DetatchDevice(id);
                }
                else
                {
                    throw new System.Exception("Throw failed!!!");
                }
            }
        }
    }

    /// <summary>
    /// Unity will have an updated predicted rotation here, use this function to tweak
    /// the rendered model last minute to have the smoothest visual experience.
    /// </summary>
    private void Application_onBeforeRender()
    {
        foreach (var sourceState in InteractionManager.GetCurrentReading())
        {
            uint id = sourceState.source.id;
            var handedness = sourceState.source.handedness;
            var sourcePose = sourceState.sourcePose;
            Vector3 position;
            Quaternion rotation;
            if (devices.ContainsKey(id))
            {
                if (sourcePose.TryGetPosition(out position, this.ControllerPose) &&
                    sourcePose.TryGetRotation(out rotation, this.ControllerPose)) // defaults to grip
                {
                    SetTransform(devices[id], position, rotation);
                }
            }
            else if (sourceState.source.supportsPointing)
            {
                if (this.modelIndecies.ContainsKey(id))
                {
                    this.AddDevice(id, this.modelIndecies[id]);
                }
                else
                {
                    this.AddDevice(id);
                }

                if (!isDetatched.ContainsKey(id) || !isDetatched[id])
                {
                    if (sourcePose.TryGetPosition(out position, this.ControllerPose) &&
                    sourcePose.TryGetRotation(out rotation, this.ControllerPose)) // defaults to grip
                    {
                        SetTransform(devices[id], position, rotation);
                    }
                }
            }
        }
    }

    private void AddDevice(uint id, int index = 0)
    {
        if (!devices.ContainsKey(id) && (!isDetatched.ContainsKey(id) || !isDetatched[id]))
        {
            GameObject go = Instantiate(this.ThrowingAssets[index], this.RealWorldRoot);
            go.name = "Controller " + id;
            devices[id] = go.transform;
            modelIndecies[id] = index;
            isDetatched[id] = false;
        }
    }

    private void RemoveDevice(uint id)
    {
        if (devices.ContainsKey(id))
        {
            Destroy(devices[id].gameObject);
            devices.Remove(id);
        }
    }

    private void DetatchDevice(uint id)
    { 
        if (devices.ContainsKey(id))
        {
            devices[id].SetParent(null);
            isDetatched[id] = true;
            devices.Remove(id);
        }
    }

    private void SetTransform(Transform t, Vector3 position, Quaternion rotation)
    {
        // This check shouldn't be necessary
        if (!(float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z) ||
            float.IsNaN(rotation.w) || float.IsNaN(rotation.x) || float.IsNaN(rotation.y) || float.IsNaN(rotation.z)))
        {
            t.localPosition = position;
            t.localRotation = rotation;
        }
    }

    void OnDestroy()
    {
        InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
        Application.onBeforeRender -= Application_onBeforeRender;
    }
}
