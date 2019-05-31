using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

// Example script that spawns a prefab at the pointer hit location.
public class SpawnOnPointerEvent : MonoBehaviour
{
    public GameObject PrefabToSpawn;

    public void Spawn(MixedRealityPointerEventData eventData)
    {
        if (PrefabToSpawn != null)
        {
            var result = eventData.Pointer.Result;
            Instantiate(PrefabToSpawn, result.Details.Point, Quaternion.LookRotation(result.Details.Normal));
        }
    }
}
