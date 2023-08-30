using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoadCompletionTester
{
    public bool CheckIfCompleted(ref Block[,] blocks, ref MapRoad[] mapRoads)
    {
        foreach(MapRoad mapRoad in mapRoads)
        {
            Vector2Int target = mapRoad.MapPosition + new Vector2Int(mapRoad.End.x, mapRoad.End.y);
            if (!IsCompleted(ref blocks, ref mapRoads, target, mapRoad.End))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsCompleted(ref Block[,] blocks, ref MapRoad[] mapRoads, Vector2Int targetPos, Vector3Int fromRoad)
    {
        if (targetPos.x < 0 || targetPos.x >= blocks.GetLength(0) || targetPos.y < 0 || targetPos.y >= blocks.GetLength(1))
            return false;
        
        Block block = blocks[targetPos.x, targetPos.y];
        if (block == null)
            return false;

        Road road = block.HasRoadFrom(fromRoad);
        if (road == null)
            return false;

        List<Vector3Int> ends = road.GetEndsExcept(fromRoad);
        foreach (Vector3Int end in ends)
        {
            Vector2Int target = targetPos + new Vector2Int(end.x, end.y);

            if (mapRoads.Select(v => v.MapPosition).ToList().Contains(target))
                return true;

            if(!IsCompleted(ref blocks, ref mapRoads, target, end))
            {
                return false;
            }
        }
        return true;
    }
}
