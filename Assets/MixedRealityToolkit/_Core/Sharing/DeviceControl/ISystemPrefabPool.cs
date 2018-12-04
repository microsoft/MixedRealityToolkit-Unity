using System;
using UnityEngine;

namespace Pixie.DeviceControl
{
    public interface ISystemPrefabPool
    {
        Action<GameObject> OnSystemObjectSpawned { get; set; }

        GameObject InstantiateUser();
        GameObject InstantiateDevice();
    }
}