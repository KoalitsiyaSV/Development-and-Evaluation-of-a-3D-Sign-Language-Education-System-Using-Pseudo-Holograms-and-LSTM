using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class DataSender : MonoBehaviour
{
    private string[] stringArray = new string[5];
    private int count;

    public void DataSend(string url)
    {
        for(int i = 0; i < 5; i++)
        {
            SendJsonDataFromResource(stringArray[i], url, "test");
        }
    }

    public void SendJsonDataFromResource(string fileName, string url, string jsonFileName)
    {
        string jsonContent = "";
        string filePath = Path.Combine(Application.streamingAssetsPath, "data", fileName + ".json");

        // 파일 읽기
        if (File.Exists(filePath))
        {
            jsonContent = File.ReadAllText(filePath);
            // 여기서 jsonContent를 처리합니다.
        }
        else
        {
            Debug.LogError("파일이 존재하지 않습니다: " + filePath);
        }

        //// Resources 폴더에서 JSON 파일 읽기
        //TextAsset jsonFile = Resources.Load<TextAsset>("data");

        //if (jsonFile == null)
        //{
        //    Debug.LogError("JSON file not found: " + jsonFileName);
        //    return;
        //}

        Debug.Log("test2");

        //string jsonData = jsonFile.text;

        Debug.Log(jsonContent);

        StartCoroutine(SendData(url, jsonContent));
    }

    public void SendDataImmediately(string url, string data)
    {
        if (data == "")
        {
            Debug.LogError("JSON Data is NULL: ");
            return;
        }

        StartCoroutine(SendData(url, data));
    }

    IEnumerator SendData(string url, string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // HTTP 헤더 설정 (옵션)
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            //Debug.Log(jsonData);
            //Debug.Log("test3");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request error: " + request.error);
            }
            else
            {
                Debug.Log("Data sent successfully");
                Debug.Log("test4");
            }
        }

        StartCoroutine(GetData(url));
    }

    IEnumerator GetData(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Data requesting error: " + request.error);
            }
            else
            {
                string receivedData = request.downloadHandler.text;
                stringArray[count] = receivedData;
                count++;
                Debug.Log("Received data: " + receivedData);
            }
        }
    }

    // 호출 예시
    void Start()
    {
        //StartCoroutine(SendData(url, jsonData));
        stringArray[0] = "";
        stringArray[1] = "";
        stringArray[2] = "";
        stringArray[3] = "";
        stringArray[4] = "";

        count = 0;
    }

    private void Update()
    {
        string serverIp = "http://203.232.193.173:5002/";
        //string jsonData = "{\"data\": \"your_data_here\"}"; // 예시 데이터

        //string jsonFileName = "data";
        //string url = "YourURLHere";

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    Debug.Log("test");

        //    GetComponent<DataSender>().SendJsonDataFromResource(serverIp, jsonFileName);
        //    //StartCoroutine(SendData(serverIp, jsonData));
        //}

        if (Input.GetKeyUp(KeyCode.B))
        {
            StartCoroutine(GetData(serverIp));
        }
    }
}