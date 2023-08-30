using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New World List", menuName = "Levels/World List")]
public class WorldList : ScriptableObject
{
    public List<WorldData> worlds;
}
