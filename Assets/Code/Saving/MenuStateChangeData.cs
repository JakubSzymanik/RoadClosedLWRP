using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStateChangeData
{
    public int lastScore;
    public List<LevelSaveData> levelSaveDatas;

    public MenuStateChangeData(int lastScore)
    {
        this.lastScore = lastScore;
        levelSaveDatas = new List<LevelSaveData>();
    }
}
