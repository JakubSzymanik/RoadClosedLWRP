 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

using System.IO;

public class MenuInputManager : MonoBehaviour
{
    [SerializeField] private float slideTreshold;
    public System.IObservable<bool> SlideStream { get { return slideSubject; } }
    private Subject<bool> slideSubject = new Subject<bool>();

    private void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            StartCoroutine(ProcessSlide());
        }

        #if UNITY_EDITOR
        KeyboardInput();
        #endif
    }

    IEnumerator ProcessSlide()
    {
        //Vector2 startPos = Input.GetTouch(0).position;
        Vector2 deltaPos = Vector2.zero;
        //Vector2 relativeDeltaPos = Vector2.zero;
        while(Input.GetTouch(0).phase != TouchPhase.Ended)
        {
            deltaPos += Input.GetTouch(0).deltaPosition;
            //relativeDeltaPos.x = deltaPos.x / Screen.width;
            //relativeDeltaPos.y = deltaPos.y / Screen.height;
            if(deltaPos.magnitude >= slideTreshold)
            {
                float angle = Vector2.SignedAngle(Vector2.right, deltaPos);
                if(angle < 30 && angle > -30)
                {
                    slideSubject.OnNext(true);
                } else if(angle < -150 || angle > 150)
                {
                    slideSubject.OnNext(false);
                }
                break;
            }
            yield return null;
        }
    }

#if UNITY_EDITOR
    private void KeyboardInput()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            slideSubject.OnNext(true);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            slideSubject.OnNext(false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            File.Delete(Application.persistentDataPath + "/save1.rcs");
        }
    }
#endif
}
