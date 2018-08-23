using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Physics;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableFinger : MonoBehaviour, IMixedRealityPointer
{

    public int Id;
    public MixedRealityInputAction InputAction;
    public bool Press;
    
    private Interactable[] interactables;
    private Interactable currentInteractable;
    private Collider myCollider;
    private EventSystem eventSystem;
    private IMixedRealityPointer pointer;
    private IMixedRealityController controller;
    private IMixedRealityInputSource source;
    private RayStep[] rays;

    public IMixedRealityInputSystem InputSystem
    {
        get
        {
            return InputSystem;
        }
    }

    public IMixedRealityController Controller { get { return controller; }  set { controller = value; } }

    public uint PointerId => (uint)Id;

    public string PointerName { get { return "Finger"; } set { } }

    public IMixedRealityInputSource InputSourceParent => source;

    public IMixedRealityCursor BaseCursor { get { return null; } set { } }
    public ICursorModifier CursorModifier { get { return null; } set { } }
    public IMixedRealityTeleportHotSpot TeleportHotSpot { get { return null; } set { } }

    public bool IsInteractionEnabled => true;

    public bool IsFocusLocked { get { return false; } set { } }
    public float PointerExtent { get { return 1; } set { } }

    public RayStep[] Rays => rays;

    public LayerMask[] PrioritizedLayerMasksOverride { get { return null; } set { } }
    public IMixedRealityFocusHandler FocusTarget { get { return (IMixedRealityFocusHandler)currentInteractable; } set { currentInteractable = (Interactable)value; } }
    public IPointerResult Result { get { return null; } set { } }
    public IBaseRayStabilizer RayStabilizer { get { return null; } set { } }
    public RaycastModeType RaycastMode { get { return RaycastModeType.Simple; } set { } }
    public float SphereCastRadius { get { return 0.01f; } set { } }

    public float PointerOrientation => 0;

    public new bool Equals(object x, object y)
    {
        return x == y;
    }

    public int GetHashCode(object obj)
    {
        return obj.GetHashCode();
    }

    public void OnPostRaycast()
    {
        
    }

    public void OnPreRaycast()
    {
        
    }

    public bool TryGetPointerPosition(out Vector3 position)
    {
        position = Vector3.zero;
        return false;
    }

    public bool TryGetPointerRotation(out Quaternion rotation)
    {
        rotation = Quaternion.identity;
        return false;
    }

    public bool TryGetPointingRay(out Ray pointingRay)
    {
        pointingRay = new Ray();
        return false;
    }

    private void OnEnable()
    {
        Transform parent = transform.parent;
        interactables = parent.gameObject.GetComponentsInChildren<Interactable>();
        myCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        for (int i = 0; i < interactables.Length; i++)
        {
            Collider otherCollider = interactables[i].GetComponentInChildren<Collider>();

            if (otherCollider != null && otherCollider.bounds.Intersects(myCollider.bounds))
            {
                Interactable interactable = interactables[i];
            }
        }
    }



}
