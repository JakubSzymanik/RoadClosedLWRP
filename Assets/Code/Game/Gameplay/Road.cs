using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Road
{
    [SerializeField] private List<Vector3Int> ends;
    public List<Vector3Int> Ends { get { return ends; } }

    public void Rotate(Vector3 eulerAngle)
    {

    }

    public List<Vector3Int> GetEndsExcept(Vector3Int exceptEnd)
    {
        List<Vector3Int> temp = new List<Vector3Int>();
        foreach(Vector3Int end in ends)
        {
            if (end != new Vector3Int(-exceptEnd.x, -exceptEnd.y, exceptEnd.z))
                temp.Add(end);
        }
        return temp;
    }
}
