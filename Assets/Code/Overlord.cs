using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Overlord
{
    public static GameSaveData saveData;
    public static MenuStateChangeData menuChangeData;
    public static int lastSceneIndex = 0;
    public static int currentSceneIndex = 0;

    public static void Save()
    {
        if (saveData == null)
            saveData = new GameSaveData();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/save1.rcs", FileMode.Create);
        bf.Serialize(stream, saveData);
        stream.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/save1.rcs"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/save1.rcs", FileMode.Open);

            saveData = bf.Deserialize(stream) as GameSaveData;
            stream.Close();
        }
        else
        {
            saveData = new GameSaveData();
        }
    }
}
