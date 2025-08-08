using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Leap;
using Leap.Unity;

public class LeapLatencyLogger : MonoBehaviour
{
    public int measurementCount = 50;
    public float interval = 0.5f;

    private Controller leapController;
    private List<float> latencies = new List<float>();

    private float leapTimeOffset = 0f;
    private bool offsetInitialized = false;

    float totalOffset = 0f;
    int offsetSamples = 10;


    void Start()
    {
        leapController = new Controller();
        StartCoroutine(AutoMeasureLatency());
    }

    IEnumerator AutoMeasureLatency()
    {
        // offset 초기화를 여러 프레임에 걸쳐서 평균으로 구하기
        for (int i = 0; i < offsetSamples; i++)
        {
            Frame frame = leapController.Frame();
            float unityNow = Time.realtimeSinceStartup;
            float leapTimestampSec = frame.Timestamp / 1_000_000f;
            totalOffset += unityNow - leapTimestampSec;
            yield return null; // 다음 프레임까지 대기
        }
        leapTimeOffset = totalOffset / offsetSamples;
        offsetInitialized = true;
        Debug.Log($"Calculated LeapTimeOffset: {leapTimeOffset:F3}");

        // 이후 측정 시작
        int count = 0;
        while (count < measurementCount)
        {
            Frame frame = leapController.Frame();
            float leapTimestampSec = frame.Timestamp / 1_000_000f;
            float inputTime = leapTimestampSec + leapTimeOffset;

            yield return new WaitForEndOfFrame();
            float renderTime = Time.realtimeSinceStartup;

            float latency = (renderTime - inputTime) * 1000f;
            latencies.Add(latency);

            Debug.Log($"[{count + 1}] Latency: {latency:F2} ms");

            count++;
            yield return new WaitForSeconds(interval);
        }

        SaveCSV();
    }

    void SaveCSV()
    {
        string path = Application.dataPath + "/LeapLatencyData.csv";
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("Measurement,Latency(ms)");
            for (int i = 0; i < latencies.Count; i++)
            {
                writer.WriteLine($"{i + 1},{latencies[i]:F2}");
            }
        }
        Debug.Log("Leap Motion latency data saved to: " + path);
    }
}