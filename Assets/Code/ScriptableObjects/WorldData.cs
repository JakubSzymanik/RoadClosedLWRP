using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New World", menuName = "Levels/World Data")]
public class WorldData : ScriptableObject
{
    public int ID;
    public string Name;
    public int ScoreToUnlock;
    public List<LevelData> levels;
}
