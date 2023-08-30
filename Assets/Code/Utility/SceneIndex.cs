using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneIndex
{
    public static int Menu = 0;
    public static int GetLevelIndex(int level)
    {
        return level + 1; // +1: nubmer of scenes before levels
    }
}
