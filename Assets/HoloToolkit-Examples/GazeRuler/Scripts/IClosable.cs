using UnityEngine;

/// <summary>
/// any class inherit this interface should be closeable
/// </summary>
public interface IClosable  {
    //finish special ploygon
    void ClosePloygon(GameObject LinePrefab, GameObject TextPrefab);
}
