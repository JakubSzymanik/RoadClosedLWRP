using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoad : MonoBehaviour
{
    [SerializeField] private Vector2Int mapPosition;
    [SerializeField] private Vector3Int end;

    public Vector2Int MapPosition { get { return mapPosition; } }
    public Vector3Int End { get { return end; } }

    public bool IsConnectedWith(Vector3Int fromDir)
    {
        return end == new Vector3Int(-fromDir.x, -fromDir.y, fromDir.z);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        transform.localPosition = new Vector3(mapPosition.x + 0.5f, 0, mapPosition.y + 0.5f);
        transform.localRotation = end.y == 0 ? Quaternion.identity : Quaternion.Euler(0, 90, 0);
    }
#endif
}
