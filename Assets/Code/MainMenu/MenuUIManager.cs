using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private float completionBarSpeed;
    [SerializeField] private Text completionTextWhite;
    [SerializeField] private Text completionTextBlack;
    [SerializeField] private Image completionFillImg;
    [SerializeField] private Text scoreTxt;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button soundBtn;
    [SerializeField] private Button leftBtn;
    [SerializeField] private Button rightBtn;
    [SerializeField] private Image soundBtnRenderer;
    [SerializeField] private Sprite soundBtnOn, soundBtnOff;

    public System.IObservable<ButtonType> MenuBtnStream { get { return menuBtnSubject; } }
    private Subject<ButtonType> menuBtnSubject = new Subject<ButtonType>();

    public System.IObservable<bool> MenuWPBtnStream { get { return menuWPBtnSubject; } }
    private Subject<bool> menuWPBtnSubject = new Subject<bool>();

    private void OnEnable()
    {
        settingsBtn.onClick.AddListener(() => menuBtnSubject.OnNext(ButtonType.Settings));
        exitBtn.onClick.AddListener(() => menuBtnSubject.OnNext(ButtonType.Back));
        soundBtn.onClick.AddListener(() =>
        {
            menuBtnSubject.OnNext(ButtonType.Sound);
            soundBtnRenderer.sprite = AudioListener.volume < 0.1f ? soundBtnOff : soundBtnOn;
        });

        leftBtn.onClick.AddListener(() => menuWPBtnSubject.OnNext(true));
        rightBtn.onClick.AddListener(()=> menuWPBtnSubject.OnNext(false));
    }

    private void OnDisable()
    {
        settingsBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
        soundBtn.onClick.RemoveAllListeners();
        leftBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.RemoveAllListeners();
    }

    public void SetUp(int score, int maxScore, int previousScore)
    {
        soundBtnRenderer.sprite = AudioListener.volume < 0.1f ? soundBtnOff : soundBtnOn;

        scoreTxt.text = score.ToString();
        if(previousScore == score)
        {
            float completionFraction = (float)score / (float)maxScore;
            completionTextBlack.text = completionTextWhite.text = Mathf.Floor(completionFraction*100).ToString() + "%";
            completionFillImg.fillAmount = completionFraction;
        }
        else
        {
            float completionFraction = (float)previousScore / (float)maxScore;
            completionTextBlack.text = completionTextWhite.text = Mathf.Floor(completionFraction * 100).ToString() + "%";
            completionFillImg.fillAmount = completionFraction;
            StartCoroutine(CompletionBarAnimation(completionFraction, (float)score / (float)maxScore));
        }
    }

    private IEnumerator CompletionBarAnimation(float previousCompletionFraction, float targetCompletionFraction)
    {
        yield return new WaitForSeconds(.5f);
        while(previousCompletionFraction < targetCompletionFraction)
        {
            previousCompletionFraction = Mathf.MoveTowards(previousCompletionFraction, targetCompletionFraction, completionBarSpeed * Time.deltaTime);
            completionFillImg.fillAmount = previousCompletionFraction;
            completionTextBlack.text = completionTextWhite.text = Mathf.Floor(previousCompletionFraction * 100).ToString() + "%";
            yield return null;
        }
    }
}
