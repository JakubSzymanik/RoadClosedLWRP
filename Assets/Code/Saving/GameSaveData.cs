using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public int score;
    public List<LevelSaveData> levelSaveDatas;
    public List<int> unlockedWorlds;
    public bool firstPlay;

    public GameSaveData()
    {
        score = 0;
        levelSaveDatas = new List<LevelSaveData>();
        unlockedWorlds = new List<int>() { 0 };
        firstPlay = true;
    }
}
