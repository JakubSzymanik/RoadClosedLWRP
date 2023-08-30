using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class GameControler : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private WorldData worldData;
    [SerializeField] private WorldList theList;
    [SerializeField] private Map map;
    [SerializeField] private GameInputManager gameInputManager;
    [SerializeField] private GameUIControler gameUIControler;
    [SerializeField] private EffectsManager effectsManager;

    private LevelData nextLevelData;
    bool lastLevel = false;
    bool nextWorldAlreadyUnlocked = false;
    bool lastWorld;
    private int worldID = -1;
    private int toUnlockNextWorld = -1;

    private int moveCount = 0;
    private bool levelCompleted = false;

    private void Start()
    {
        SubscribeStreams();


        lastLevel = levelData.ID >= 12;
        worldID = worldData.ID;
        map.worldID = worldID;
        if (!lastLevel) //12: levels per world
        {
            nextLevelData = worldData.levels[levelData.ID];
        }
        else
        {
            if(worldID + 1 < theList.worlds.Count)
                toUnlockNextWorld = theList.worlds[worldID + 1].ScoreToUnlock;
        }

        if(Overlord.saveData != null && !Overlord.saveData.unlockedWorlds.Contains(worldID))
        {
            Overlord.saveData.unlockedWorlds.Add(worldID);
            Overlord.Save();
        }

        theList = null; //drop reference

        //set next level data
        gameUIControler.SetUp(levelData.ID, worldData.Name);
        gameUIControler.UpdateMovesCounter(moveCount,
                    levelData.movesTo3 >= moveCount ? levelData.movesTo3 : levelData.movesTo2 >= moveCount ? levelData.movesTo2 : -1,
                    levelData.movesTo3 >= moveCount ? 3 : levelData.movesTo2 >= moveCount ? 2 : 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitToMenu();
        }
    }

    private void SubscribeStreams()
    {
        //subscribe streams
        gameInputManager.SwipeStream
            .Where(_ => !map.BlocksMoving && !levelCompleted)
            .Subscribe(swipeDir =>
            {
                map.Move(swipeDir);
            });

        gameUIControler.uiButtonStream
            .Subscribe(buttonType =>
            {
                SoundManager.SoundManagerInstance.RequestSound(SoundType.ButtonClick);
                switch (buttonType)
                {
                    case ButtonType.Restart:
                        RestartLevel();
                        break;
                    case ButtonType.Menu:
                        ExitToMenu();
                        break;
                    case ButtonType.NextLvl:
                        LoadNextLevel();
                        break;
                }
            });

        map.moveStartedStream
            .Subscribe(_ =>
            {
                SoundManager.SoundManagerInstance.RequestSound(SoundType.Slide); //sound
                moveCount++;
                gameUIControler.UpdateMovesCounter(
                    moveCount,
                    levelData.movesTo3 >= moveCount ? levelData.movesTo3 : levelData.movesTo2 >= moveCount ? levelData.movesTo2 : -1,
                    levelData.movesTo3 >= moveCount ? 3 : levelData.movesTo2 >= moveCount ? 2 : 1);
            });

        map.lvlCompletedStream
            .Subscribe(_ => LevelCompleted());

        //map.moveCompletedStream
        //    .Subscribe(_ => effectsManager.PlayMoveCompletedEffects());
    }

    private void RestartLevel()
    {
        SceneTransitor.instance.LoadScene(levelData.SceneID);
    }

    private void LevelCompleted()
    {
        //for editor testing
        if (Overlord.saveData == null)
        {
            print("No save data, level completed");
            levelCompleted = true;
            effectsManager.DestroyBarriers();
            int pointsGainedTemp = moveCount <= levelData.movesTo3 ? 3 : moveCount <= levelData.movesTo2 ? 2 : 1;
            StartCoroutine(DelayEndMenu(pointsGainedTemp, 0, 0));
            return;
        }
        // /for editor testing

        LevelSaveData levelSaveData = Overlord.saveData.levelSaveDatas.Find(v => v.worldID == worldID && v.levelID == levelData.ID);
        int pointsGained = moveCount <= levelData.movesTo3 ? 3 : moveCount <= levelData.movesTo2 ? 2 : 1;
        int pointsGainedBefore;

        if (Overlord.menuChangeData == null)
            Overlord.menuChangeData = new MenuStateChangeData(Overlord.saveData.score);

        if (levelSaveData != null)
        {
            pointsGainedBefore = levelSaveData.points;

            Overlord.saveData.levelSaveDatas.Find(v => v.worldID == worldID && v.levelID == levelData.ID).points =
                levelSaveData.points < pointsGained ? pointsGained : levelSaveData.points;
        }
        else
        {
            pointsGainedBefore = 0;

            levelSaveData = new LevelSaveData(worldID, levelData.ID, pointsGained);
            Overlord.saveData.levelSaveDatas.Add(levelSaveData);
        }

        Overlord.saveData.score += (pointsGained - pointsGainedBefore) > 0 ? pointsGained - pointsGainedBefore : 0;
        Overlord.Save();
        Overlord.menuChangeData.levelSaveDatas.Add(new LevelSaveData(worldID, levelData.ID, pointsGainedBefore));

        levelCompleted = true;
        effectsManager.DestroyBarriers();
        StartCoroutine(DelayEndMenu(pointsGained, pointsGainedBefore, Overlord.saveData.score - pointsGained + pointsGainedBefore));
    }

    private void LoadNextLevel()
    {
        if (SceneTransitor.instance == null)
            print("No scene transitor but level completed");

        int id = levelData.SceneID + 1;
        SceneTransitor.instance.LoadScene(id);
    }

    private void ExitToMenu()
    {
        SceneTransitor.instance.LoadScene(SceneIndex.Menu);
    }

    private IEnumerator DelayEndMenu(int pointsGained, int pointsGainedBefore, int score)
    {
        yield return new WaitForSeconds(2 + 0.1f * effectsManager.RoadBarrierCount);

        gameUIControler.Disable(); //disable game ui

        if (Overlord.saveData == null) //for editor testing
        {
            gameUIControler.ActivateEndMenu(pointsGained, score, false, pointsGainedBefore);
            yield break;
        }

        gameUIControler.ActivateEndMenu
            (pointsGained, 
            score, 
            nextWorldAlreadyUnlocked || toUnlockNextWorld <= score || (nextLevelData != null && nextLevelData.pointsToUnlock <= score), 
            pointsGainedBefore);
    }
}
