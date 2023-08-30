using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GameInputManager : MonoBehaviour
{
    [SerializeField] private float swipeTreshold;

    public System.IObservable<Vector2Int> SwipeStream { get { return swipeSubject as System.IObservable<Vector2Int>; } }
    private Subject<Vector2Int> swipeSubject = new Subject<Vector2Int>();

    void Update()
    {
        KeyboardControls();
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            StartCoroutine(ProcessSwipe());
        }
    }

    private IEnumerator ProcessSwipe()
    {
        Vector2 swipeStart = Input.GetTouch(0).position;
        Vector2 swipe = Vector2.zero;
        while(Input.GetTouch(0).phase != TouchPhase.Ended)
        {
            swipe = Input.GetTouch(0).position - swipeStart;
            if(swipe.magnitude >= swipeTreshold)
            {
                float angle = Vector2.Angle(Vector2.up, swipe);
                if (angle < 45)
                    swipeSubject.OnNext(Vector2Int.up);
                else if (angle > 135)
                    swipeSubject.OnNext(Vector2Int.down);
                else if (swipe.x > 0)
                    swipeSubject.OnNext(Vector2Int.right);
                else if (swipe.x < 0)
                    swipeSubject.OnNext(Vector2Int.left);
            }
            yield return null;
        }
    }

    private void KeyboardControls()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            swipeSubject.OnNext(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            swipeSubject.OnNext(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            swipeSubject.OnNext(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            swipeSubject.OnNext(Vector2Int.right);
        }
    }
}
