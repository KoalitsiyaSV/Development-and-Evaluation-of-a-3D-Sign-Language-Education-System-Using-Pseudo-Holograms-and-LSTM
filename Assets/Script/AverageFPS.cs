using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AverageFPS : MonoBehaviour
{
    public int sampleCount = 300;  // 샘플링할 프레임 수
    private int frameCounter = 0;
    private float totalTime = 0f;
    private float avgFPS = 0f;

    void Update()
    {
        totalTime += Time.unscaledDeltaTime;
        frameCounter++;

        if (frameCounter >= sampleCount)
        {
            avgFPS = frameCounter / totalTime;
            Debug.Log($"Average FPS over {sampleCount} frames: {avgFPS:F2}");
            frameCounter = 0;
            totalTime = 0f;
        }
    }
}