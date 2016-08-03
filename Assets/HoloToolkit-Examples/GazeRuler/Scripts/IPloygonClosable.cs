using UnityEngine;

/// <summary>
/// any geometry class inherit this interface should be closeable
/// </summary>
public interface IPloygonClosable
{
    //finish special ploygon
    void ClosePloygon(GameObject LinePrefab, GameObject TextPrefab);
}
