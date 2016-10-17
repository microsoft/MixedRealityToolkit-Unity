using UnityEngine;

/// <summary>
/// any geometry class inherit this interface should be closeable
/// </summary>
public interface IPolygonClosable
{
    //finish special ploygon
    void ClosePloygon(GameObject LinePrefab, GameObject TextPrefab);
}
