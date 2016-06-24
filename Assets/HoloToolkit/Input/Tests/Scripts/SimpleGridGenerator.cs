using System.Collections.Generic;
using UnityEngine;

public class SimpleGridGenerator : MonoBehaviour
{
    private const int DefaultRows = 3;
    private const int DefaultColumns = 3;

    [Tooltip("Number of rows in the grid.")]
    public int Rows = DefaultRows;

    [Tooltip("Number of columns in the grid.")]
    public int Columns = DefaultColumns;

    [Tooltip("Distance between objects in the grid.")]
    public float ObjectSpacing = 1.0f;

    [Tooltip("Array of object prefabs to instantiate on the grid.")]
    public List<GameObject> ObjectPrefabs;

    [Tooltip("Indicates whether to generate grid on component start.")]
    public bool GenerateOnStart = true;

    private void Start()
    {
        if (GenerateOnStart)
        {
            GenerateGrid();
        }
    }

    public void GenerateGrid()
    {
        float startX = -0.5f * ObjectSpacing * (Rows - 1);
        float startY = -0.5f * ObjectSpacing * (Columns - 1);
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject prefab = GetRandomPrefab();
                Vector3 pos = new Vector3(startX + i * ObjectSpacing, startY + j * ObjectSpacing, 0.0f);
                GameObject go = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
                go.transform.SetParent(transform, false);
            }
        }
    }

    private GameObject GetRandomPrefab()
    {
        if (ObjectPrefabs.Count > 0)
        {
            int index = Random.Range(0, ObjectPrefabs.Count);
            return ObjectPrefabs[index];
        }
        return null;
    }
}