using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class EndMenu : MonoBehaviour
{
    [SerializeField] private float signMoveSpeed;
    [Header("Scene reference:")]
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button menuBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private RectTransform scoreImg;
    [SerializeField] private Text scoreTxt;
    [SerializeField] private List<Image> signs;

    public System.IObservable<ButtonType> ButtonStream { get { return ButtonSubject; } }
    private Subject<ButtonType> ButtonSubject = new Subject<ButtonType>();

    private int pointsGained;
    private int pointsGainedBefore;
    private int score;

    private void OnEnable()
    {
        restartBtn.onClick.AddListener(() => ButtonSubject.OnNext(ButtonType.Restart));
        menuBtn.onClick.AddListener(() => ButtonSubject.OnNext(ButtonType.Menu));
        nextBtn.onClick.AddListener(() => ButtonSubject.OnNext(ButtonType.NextLvl));
    }

    private void OnDisable()
    {
        menuBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.RemoveAllListeners();
    }

    public void SetUp(int pointsGained, int score, bool nextLevelAvailable, int pointsGainedBefore)
    {
        this.score = score;
        scoreTxt.text = this.score.ToString();
        this.pointsGained = pointsGained;
        this.pointsGainedBefore = pointsGainedBefore;

        for (int i = 0; i < pointsGainedBefore; i++)
        {
            signs[i].color = Color.gray;
        }
        for (int i = pointsGained; i < 3; i++)
        {
            signs[i].color = Color.black;
        }

        nextBtn.interactable = nextLevelAvailable;

        SoundManager.SoundManagerInstance.RequestSound(SoundType.Success);
    }

    //adding points animation
    public void AddPoints()
    {
        StartCoroutine(AddPointsCR());
    }

    private IEnumerator AddPointsCR()
    {
        for(int i = pointsGainedBefore; i < pointsGained; i++)
        {
            StartCoroutine( MoveSign( Instantiate(signs[i].gameObject, signs[i].transform.parent).GetComponent<RectTransform>() ) );
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator MoveSign(RectTransform rt)
    {
        SoundManager.SoundManagerInstance.RequestSound(SoundType.Slide);
        float initialDistance = (rt.position - scoreImg.position).magnitude;
        while( (rt.position - scoreImg.position).magnitude > 0.01f)
        {
            rt.position = Vector2.MoveTowards(rt.position, scoreImg.position, Screen.height * signMoveSpeed * Time.deltaTime);
            rt.localScale = Vector3.one * (rt.position - scoreImg.position).magnitude / initialDistance;
            yield return null;
        }
        SoundManager.SoundManagerInstance.RequestSound(SoundType.Ding);
        IncrementScoreCounter();
    }

    private void IncrementScoreCounter()
    {
        score += 1;
        scoreTxt.text = score.ToString(); 
    }
}
