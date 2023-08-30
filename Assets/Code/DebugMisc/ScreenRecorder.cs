using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRecorder : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ScreenCapture.CaptureScreenshot("D:\\Unity Projects\\Road Work Ahead.png", 50);
        }
    }
}
