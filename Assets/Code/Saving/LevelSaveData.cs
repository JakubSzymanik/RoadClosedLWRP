[System.Serializable]
public class LevelSaveData
{
    public int worldID;
    public int levelID;
    public int points;

    public LevelSaveData(int worldID, int levelID, int points)
    {
        this.worldID = worldID;
        this.levelID = levelID;
        this.points = points;
    }
}
