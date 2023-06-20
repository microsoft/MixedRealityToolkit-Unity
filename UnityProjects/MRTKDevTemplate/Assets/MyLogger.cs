using UnityEngine;
using UnityEngine.InputSystem;

public class MyLogger
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        foreach (var device in InputSystem.devices)
        {
            Debug.Log($"Device {device} was added before launch");
        }

        InputSystem.onDeviceChange +=
              (device, change) =>
              {
                  switch (change)
                  {
                      case InputDeviceChange.Added:
                          Debug.Log($"Device {device} was added!!!");
                          break;
                      case InputDeviceChange.Removed:
                          Debug.Log($"Device {device} was removed!!!");
                          break;
                      default:
                          Debug.Log($"Device {device} was changed!!!");
                          break;
                  }
              };

    }
}
