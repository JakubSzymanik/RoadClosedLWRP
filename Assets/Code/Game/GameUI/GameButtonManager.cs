using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameButtonManager : MonoBehaviour
{
    public System.IObservable<ButtonType> buttonStream { get { return buttonSubject; } }
    private Subject<ButtonType> buttonSubject = new Subject<ButtonType>(); 

    [SerializeField] private Button restartBtn;
    [SerializeField] private Button backBtn;

    private void OnEnable()
    {
        restartBtn.onClick.AddListener(() => buttonSubject.OnNext(ButtonType.Restart));
        backBtn.onClick.AddListener(() => buttonSubject.OnNext(ButtonType.Menu));
    }

    private void OnDisable()
    {
        restartBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
    }
}
