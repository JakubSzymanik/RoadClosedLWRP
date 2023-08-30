using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class WorldPanel : MonoBehaviour
{
    [Header("Values:")]
    [SerializeField] private float moveSpeed;
    [Header("Prefab references:")]
    [SerializeField] private List<LevelButton> lvlBtns;
    [SerializeField] private WorldLockPanel lockPanel;
    [SerializeField] private GameObject thePanel; //child panel with all ui elements

    public System.IObservable<int> LvlBtnClickStream { get { return lvlBtnClickSubject; } }
    public System.IObservable<Unit> UnlockClickStream { get { return unlockClickSubject; } }
    private Subject<int> lvlBtnClickSubject = new Subject<int>();
    private Subject<Unit> unlockClickSubject = new Subject<Unit>();

    private bool isOnLeft;
    public static int MovingPanels { get; private set; } = 0;

    private RectTransform rectTransform;
    private static Vector2 leftMax, leftMin, rightMax, rightMin; //Max: right, top   Min: left, bottom

    //Mono Behaviour
    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        leftMin = new Vector2(-Screen.width, 0) * 1080 / Screen.width;
        leftMax = new Vector2(-Screen.width, 0) * 1080 / Screen.width;
        rightMin = new Vector2(Screen.width, 0) * 1080 / Screen.width;
        rightMax = new Vector2(Screen.width, 0) * 1080 / Screen.width;
        moveSpeed *= Screen.width;
    }

    private void OnEnable()
    {
        lockPanel.UnlockClick.AddListener(Unlock);
    }

    private void OnDisable()
    {
        lockPanel.UnlockClick.RemoveAllListeners();
    }

    //SetUp
    public void SetUp(ref List<LevelData> levelDatas, WorldData worldData, bool isActive, bool atLeft, bool isUnlocked, int currentGlobalPoints, int lastGlobalPoints, List<LevelSaveData> currentLevelSaveData, List<LevelSaveData> previousLevelSaveData)
    {
        if (!isActive)
        {
            rectTransform.offsetMax = atLeft ? leftMax : rightMax;
            rectTransform.offsetMin = atLeft ? leftMin : rightMin;
            thePanel.gameObject.SetActive(false);
        }

        for(int i = 0; i < levelDatas.Count; i++)
        {
            int currentLvlPoints = 0, previousLvlPoints = 0;
            if(currentLevelSaveData.Exists(v => v.levelID == i + 1))
                currentLvlPoints = currentLevelSaveData.Find(v => v.levelID == i + 1).points;
            if (previousLevelSaveData.Exists(v => v.levelID == i + 1))
                previousLvlPoints = previousLevelSaveData.Find(v => v.levelID == i + 1).points;
            else
                previousLvlPoints = currentLvlPoints;

            lvlBtns[i].SetUp(levelDatas[i], currentLvlPoints, previousLvlPoints, currentGlobalPoints, lastGlobalPoints, isActive);

            int sceneID = levelDatas[i].SceneID;
            lvlBtns[i].OnClick.AddListener(() => lvlBtnClickSubject.OnNext(sceneID));
        }

        if (!isUnlocked)
        {
            lockPanel.SetUp(worldData.ScoreToUnlock, currentGlobalPoints);
            lockPanel.gameObject.SetActive(true);
        }
    }

    //Activation / Unlocking
    public void Activate(bool isActive, bool left)
    {
        StopAllCoroutines();
        if(isActive)
        {
            thePanel.gameObject.SetActive(true);
            StartCoroutine(Activate());
        }
        else
        {
            StartCoroutine(Deactivate(left));
        }
    }

    private void Unlock()
    {
        unlockClickSubject.OnNext(new Unit());
        Destroy(lockPanel.gameObject);
    }

    private IEnumerator Activate()
    {
        MovingPanels++;
        while(rectTransform.offsetMax.x != 0)
        {
            rectTransform.offsetMax = Vector2.MoveTowards(rectTransform.offsetMax, Vector2.zero, moveSpeed * Time.smoothDeltaTime);
            rectTransform.offsetMin = Vector2.MoveTowards(rectTransform.offsetMin, Vector2.zero, moveSpeed * Time.smoothDeltaTime);
            yield return null;
        }
        MovingPanels--;
    }

    private IEnumerator Deactivate(bool left)
    {
        MovingPanels++;
        if (left)
        {
            while (rectTransform.offsetMax.x != leftMax.x)
            {
                rectTransform.offsetMax = Vector2.MoveTowards(rectTransform.offsetMax, leftMax, moveSpeed * Time.smoothDeltaTime);
                rectTransform.offsetMin = Vector2.MoveTowards(rectTransform.offsetMin, leftMin, moveSpeed * Time.smoothDeltaTime);
                yield return null;
            }
        }
        else
        {
            while (rectTransform.offsetMax.x != rightMax.x)
            {
                rectTransform.offsetMax = Vector2.MoveTowards(rectTransform.offsetMax, rightMax, moveSpeed * Time.smoothDeltaTime);
                rectTransform.offsetMin = Vector2.MoveTowards(rectTransform.offsetMin, rightMin, moveSpeed * Time.smoothDeltaTime);
                yield return null;
            }
        }
        MovingPanels--;
        thePanel.gameObject.SetActive(false);
    }
}
