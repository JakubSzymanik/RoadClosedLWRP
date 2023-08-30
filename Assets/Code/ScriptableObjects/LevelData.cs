using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels/Level Data")]
[System.Serializable]
public class LevelData : ScriptableObject
{
    public int ID;
    public int SceneID;
    public int pointsToUnlock;
    public int movesTo3;
    public int movesTo2;
}