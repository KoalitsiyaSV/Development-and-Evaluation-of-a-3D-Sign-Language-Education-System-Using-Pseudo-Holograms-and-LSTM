using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraDataSender : MonoBehaviour
{
    public Camera targetCamera;
    private float captureInterval = 0.1f;

    private void Start()
    {
        StartCoroutine(SaveImageRepeatedly());
    }

    IEnumerator SaveImageRepeatedly()
    {
        while (true)
        {
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            targetCamera.targetTexture = rt;
            Texture2D cameraTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            targetCamera.Render();
            RenderTexture.active = rt;
            cameraTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            targetCamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            byte[] bytes = cameraTexture.EncodeToJPG();

            string filePath = "C:/Proejct/Unity/LeapMotionTest_Image/capturedImage.jpg";

            File.WriteAllBytes (filePath, bytes);

            yield return new WaitForSeconds(captureInterval);
        }
    }
}
