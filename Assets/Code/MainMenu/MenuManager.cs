using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using System.IO;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuInputManager inputManager;
    [SerializeField] private MenuUIManager uiManager;
    [SerializeField] private WorldList worldList;
    [SerializeField] private List<WorldPanel> worldPanels;

    private int activeWorldPanelIndex;

    //Mono Behaviour
    private void Start()
    {
        activeWorldPanelIndex = PlayerPrefs.GetInt("ActiveWorldPanelIndex", 0);
        if (Overlord.saveData == null)
        {
            Overlord.Load();
        }

        SubscribeStreams();
        SetUpWorldPanels();
        SetUpUI();

        if(Overlord.saveData.firstPlay)
        {
            Overlord.saveData.firstPlay = false;
            Overlord.Save();
            ExitMenuInstant(1);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.SetInt("ActiveWorldPanelIndex", activeWorldPanelIndex);
            Application.Quit();
        }
    }

    //Set Up
    private void SubscribeStreams()
    {
        inputManager.SlideStream
        .Merge(uiManager.MenuWPBtnStream)
            .Where(_ => WorldPanel.MovingPanels < 2)
            .Subscribe(left =>
            {
                if(left && activeWorldPanelIndex > 0)
                {
                    SoundManager.SoundManagerInstance.RequestSound(SoundType.Slide);
                    worldPanels[activeWorldPanelIndex].Activate(false, false);
                    activeWorldPanelIndex--;
                    worldPanels[activeWorldPanelIndex].Activate(true, true);
                }
                else if(!left && activeWorldPanelIndex < worldList.worlds.Count - 1)
                {
                    SoundManager.SoundManagerInstance.RequestSound(SoundType.Slide);
                    worldPanels[activeWorldPanelIndex].Activate(false, true);
                    activeWorldPanelIndex++;
                    worldPanels[activeWorldPanelIndex].Activate(true, true);
                }
            });

        uiManager.MenuBtnStream
            .Subscribe(v =>
            {
                switch (v)
                {
                    case ButtonType.Back:
                        PlayerPrefs.SetInt("ActiveWorldPanelIndex", activeWorldPanelIndex);
                        Application.Quit();
                        break;
                    case ButtonType.Sound:
                        SoundManager.SoundManagerInstance.SwitchVolume();
                        break;
                    case ButtonType.Settings:
                        //File.Delete(Application.persistentDataPath + "/save1.rcs");
                        //EnableSettings(true);
                        break;
                }
                SoundManager.SoundManagerInstance.RequestSound(SoundType.ButtonClick);
            });

        for(int i = 0; i < worldPanels.Count; i++)
        {
            worldPanels[i].LvlBtnClickStream
                .Subscribe(v =>
                {
                    SoundManager.SoundManagerInstance.RequestSound(SoundType.ButtonClick);
                    ExitMenu(v);
                });

            int id = i;
            worldPanels[i].UnlockClickStream
                .Subscribe(_ =>
                {
                    SoundManager.SoundManagerInstance.RequestSound(SoundType.ButtonClick);
                    Overlord.saveData.unlockedWorlds.Add(id);
                    Overlord.Save();
                });
        }
    }

    private void SetUpWorldPanels()
    {
        for(int i = 0; i < worldPanels.Count; i++)
        {
            worldPanels[i].SetUp(
                ref worldList.worlds[i].levels,
                worldList.worlds[i],
                activeWorldPanelIndex == i,
                i < activeWorldPanelIndex,
                Overlord.saveData.unlockedWorlds.Contains(i),
                Overlord.saveData.score,
                Overlord.menuChangeData != null ? Overlord.menuChangeData.lastScore : Overlord.saveData.score,
                Overlord.saveData.levelSaveDatas.Where(v => v.worldID == i).ToList(),
                Overlord.menuChangeData != null ? Overlord.menuChangeData.levelSaveDatas.Where(v => v.worldID == i).ToList() : Overlord.saveData.levelSaveDatas.Where(v => v.worldID == i).ToList());
        }
        
    }

    private void SetUpUI()
    {
        uiManager.SetUp(Overlord.saveData.score, 216, Overlord.menuChangeData != null ? Overlord.menuChangeData.lastScore : Overlord.saveData.score);
        Overlord.menuChangeData = null;
    }

    private void EnableSettings(bool enabled)
    {

    }

    private void ExitMenu(int toScene)
    {
        PlayerPrefs.SetInt("ActiveWorldPanelIndex", activeWorldPanelIndex);
        SceneTransitor.instance.LoadScene(toScene);
    }

    private void ExitMenuInstant(int toScene)
    {
        PlayerPrefs.SetInt("ActiveWorldPanelIndex", activeWorldPanelIndex);
        SceneTransitor.instance.LoadSceneInstant(toScene);
    }
}
