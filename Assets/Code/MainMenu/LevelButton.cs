using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Text levelNoTxt;
    [SerializeField] private List<Image> pointImgs;
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private Text scoreToUnlockTxt;

    public LevelData LevelData { get; private set; }

    public UnityEvent OnClick { get { return this.GetComponent<Button>().onClick; } }

    public void SetUp(LevelData levelData, int currentLvlPoints, int lastLvlPoints, int currentGlobalPoints, int lastGlobalPoints,  bool playChangeEffects)
    {
        //set up for current data
            //set level number
        levelNoTxt.text = levelData.ID.ToString();
            //set gained score
        for(int i = currentLvlPoints; i < 3; i++)
        {
            Destroy(pointImgs[i].gameObject);
        }
            //set locked/unlocked
        if(currentGlobalPoints >= levelData.pointsToUnlock)
        {
            Destroy(lockedPanel);
        }
        else
        {
            lockedPanel.SetActive(true);
            this.GetComponent<Button>().interactable = false;
            scoreToUnlockTxt.text = levelData.pointsToUnlock.ToString();
        }

        if (!playChangeEffects)
            return;

        //set state change effects
            //point gain effects
        for(int i = lastLvlPoints; i < currentLvlPoints; i++)
        {
            GainPoint(pointImgs[i]);
        }
        //unlock effects
        if (lastGlobalPoints < levelData.pointsToUnlock && currentGlobalPoints >= levelData.pointsToUnlock)
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        
    }

    private void GainPoint(Image pointImg)
    {
        //MenuParticleManager.instance.RequestPointGainedParticles(pointImg.rectTransform.rect.center);
    }
}
