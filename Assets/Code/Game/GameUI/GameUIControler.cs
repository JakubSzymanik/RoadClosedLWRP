using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameUIControler : MonoBehaviour
{
    [SerializeField] private EndMenu endMenu;
    [SerializeField] private GameButtonManager gameButtonManager;
    [SerializeField] private Text levelDataTxt;
    [SerializeField] private Text moveCounterTxt;
    [SerializeField] private List<Image> signImages;
    [SerializeField] private Animator uiAnimator;

    public System.IObservable<ButtonType> uiButtonStream
        { get { return endMenu.ButtonStream
                .Merge(gameButtonManager.buttonStream); } }


    public void SetUp(int levelID, string worldName)
    {
        levelDataTxt.text = "Level " + levelID.ToString();
    }

    public void ActivateEndMenu(int pointsGained, int score, bool nextLevelAvailable, int pointsGainedBefore)
    {
        endMenu.SetUp(pointsGained, score, nextLevelAvailable, pointsGainedBefore);
        endMenu.gameObject.SetActive(true);
    }

    public void UpdateMovesCounter(int moveCount, int nextTreshold, int currentPoints)
    {
        moveCounterTxt.text = "Moves " + moveCount.ToString() + "/" + ((nextTreshold != -1) ? nextTreshold.ToString()  : "\u221E");
        for(int i = currentPoints; i < 3; i++)
        {
            signImages[i].gameObject.SetActive(false);
        }
    }

    public void Disable()
    {
        uiAnimator.Play("GameUIOutro");
    }
}
