using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FrameDataLogger : MonoBehaviour
{
    private class FPSRecord
    {
        public float time;  // 측정 시작 후 누적 시간 (초)
        public float avgFPS;
    }

    private List<FPSRecord> records = new List<FPSRecord>();

    private float fpsSum = 0f;
    private int fpsCount = 0;
    private float timer = 0f;
    private float loggingDuration = 60f; // 총 측정 시간 (초)

    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "AverageFPSLog.csv");
        StartCoroutine(LogAverageFPSRoutine());
    }

    IEnumerator LogAverageFPSRoutine()
    {
        float startTime = Time.time;

        while (Time.time - startTime < loggingDuration)
        {
            // 프레임별 즉시 FPS 계산
            float currentFPS = 1f / Time.unscaledDeltaTime;
            fpsSum += currentFPS;
            fpsCount++;
            timer += Time.unscaledDeltaTime;

            if (timer >= 1f) // 1초 경과 시 평균 FPS 기록
            {
                float avgFPS = fpsSum / fpsCount;
                float elapsedTime = Time.time - startTime;
                records.Add(new FPSRecord() { time = elapsedTime, avgFPS = avgFPS });

                // 초기화
                fpsSum = 0f;
                fpsCount = 0;
                timer = 0f;
            }

            yield return null;
        }

        SaveToCSV();
    }

    private void SaveToCSV()
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Time(sec),AverageFPS");
            foreach (var rec in records)
            {
                writer.WriteLine($"{rec.time:F2},{rec.avgFPS:F2}");
            }
        }

        Debug.Log($"Average FPS data saved to: {filePath}");
    }
}
