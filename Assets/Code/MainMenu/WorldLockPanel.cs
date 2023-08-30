using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WorldLockPanel : MonoBehaviour
{
    [SerializeField] private Text scoreToUnlockTxt;
    [SerializeField] private Button unlockBtn;

    public UnityEvent UnlockClick { get { return unlockBtn.onClick; } }

    public void SetUp(int scoreToUnlock, int currentScore)
    {
        scoreToUnlockTxt.text = scoreToUnlock.ToString();
        unlockBtn.interactable = currentScore >= scoreToUnlock;
    }
}
