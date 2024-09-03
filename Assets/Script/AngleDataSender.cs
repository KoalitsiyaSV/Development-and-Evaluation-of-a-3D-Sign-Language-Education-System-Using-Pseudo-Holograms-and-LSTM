using Leap;
using Leap.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
//using static UnityEditor.PlayerSettings;

//[System.Serializable]
//public class FingerAngleData 
//{
//    public string fingerJoint;
//    public Quaternion jointEulerAngles;
//}

[System.Serializable]
public class FingerAngleData
{
    public string[] fingerJoint;
    public Quaternion[] jointAngles;
}

public class AngleDataSender : MonoBehaviour
{
    LeapServiceProvider leapProvider;
    
    

    FingerAngleData handData;

    Dictionary<string, Quaternion> angles;
    //List<FingerAngleData> fingerDataList = new List<FingerAngleData>();

    //FingerAngleData[] handData = new FingerAngleData[20];

    //List<List<FingerAngleData>> groupedDataList = new List<List<FingerAngleData>>();

    private bool isAction;
    //public float settedTime;
    //public string fileName;
    //public int index = 0;
    private bool isDetect = false;
    string jsonData;
    string folderPath;

    public TextMeshProUGUI time;
    public TextMeshProUGUI fileName;
    public TextMeshProUGUI filenum;

    public GameObject startBtn;
    public String setTime;
    public String setFileName;
    public String setFileNum;
    
    [Header("Data Sender")]
    public DataSender dataSender;
    public string url;

    void Start()
    {
        url = "http://117.16.153.64:5000/";

        angles = new Dictionary<string, Quaternion>();

        handData = new FingerAngleData();
        handData.fingerJoint = new string[44];
        handData.jointAngles = new Quaternion[44];

        folderPath = Path.Combine(Application.streamingAssetsPath, "Data");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // LeapServiceProvider 찾기
        leapProvider = FindObjectOfType<LeapServiceProvider>();
        
        // LeapServiceProvider 이벤트 핸들러 등록

        leapProvider.OnUpdateFrame += OnLeapFrame;

        //if (IsValidURL(url))
        //{
        //    Debug.Log("Valid URL: " + url);
        //    StartCoroutine(CheckServerStatus(url));
        //}
        //else
        //{
        //    Debug.LogError("Malformed URL: " + url);
        //}
    }

    //bool IsValidURL(string url)
    //{
    //    Uri uriResult;
    //    bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
    //                  && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    //    return result;
    //}

    //IEnumerator CheckServerStatus(string url)
    //{
    //    using (UnityWebRequest request = UnityWebRequest.Get(url))
    //    {
    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
    //        {
    //            Debug.LogError("Server connection failed: " + request.error);
    //        }
    //        else
    //        {
    //            Debug.Log("Server connection succeeded: " + url);
    //        }
    //    }
    //}

    private void Update()
    {

    }

    void OnLeapFrame(Frame frame)
    {
        if (isDetect)
        {
            Hand hand = new Hand();
            
            if (leapProvider.CurrentFrame.Hands.Count == 0)
                return;

            int index = 0;

            for(int i = 0; i < leapProvider.CurrentFrame.Hands.Count; i++)
            {
                //Debug.Log(leapProvider.CurrentFrame.Hands.Count);

                hand = leapProvider.CurrentFrame.Hands[i];

                foreach (Finger fingerModel in hand.Fingers)
                {
                    string fingerName = "";

                    if (hand.IsLeft)
                        fingerName = "왼손-";
                    else
                        fingerName = "오른손-";

                    switch (fingerModel.Id % 5)
                    {
                        case 0:
                            fingerName += "엄지";
                            break;
                        case 1:
                            fingerName += "검지";
                            break;
                        case 2:
                            fingerName += "중지";
                            break;
                        case 3:
                            fingerName += "약지";
                            break;
                        case 4:
                            fingerName += "소지";
                            break;
                        default:
                            break;
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        Bone bone = fingerModel.bones[j];
                        // Vector3 pos = bone.Center;
                        Quaternion jointRotation = bone.Rotation;
                        jointRotation.x = Mathf.Round(jointRotation.x * 1000f) / 1000f;
                        jointRotation.y = Mathf.Round(jointRotation.y * 1000f) / 1000f;
                        jointRotation.z = Mathf.Round(jointRotation.z * 1000f) / 1000f;
                        jointRotation.w = Mathf.Round(jointRotation.w * 1000f) / 1000f;
                        handData.fingerJoint[index] = fingerName + "-" + j;
                        handData.jointAngles[index] = jointRotation;

                        //Debug.Log(handData.fingerJoint[index] + handData.jointAngles[index] + index);
                        index++;
                    }
                }

                string armName = "";

                if (hand.IsLeft)
                    armName = "왼팔";
                else
                    armName = "오른팔";

                handData.fingerJoint[index] = armName;
                handData.jointAngles[index++] = leapProvider.CurrentFrame.Hands[i].Arm.Rotation;

                Vector3 pos = leapProvider.CurrentFrame.Hands[i].Arm.Center;

                handData.fingerJoint[index] = armName + " 위치 값";
                handData.jointAngles[index++] = new Quaternion(pos.x, pos.y, pos.z, -2f);
            }

            if (leapProvider.CurrentFrame.Hands.Count == 1)
            {
                hand = leapProvider.CurrentFrame.Hands[0];

                for (int i = 0; i < 5; i++)
                {
                    string fingerName = "";

                    if (hand.IsLeft)
                        fingerName = "오른손-";
                    else
                        fingerName = "왼손-";

                    switch (i % 5)
                    {
                        case 0:
                            fingerName += "엄지";
                            break;
                        case 1:
                            fingerName += "검지";
                            break;
                        case 2:
                            fingerName += "중지";
                            break;
                        case 3:
                            fingerName += "약지";
                            break;
                        case 4:
                            fingerName += "소지";
                            break;
                        default:
                            break;
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        Quaternion jointRotation = new Quaternion();
                        jointRotation.x = -2;
                        jointRotation.y = -2;
                        jointRotation.z = -2;
                        jointRotation.w = -2;

                        handData.fingerJoint[index] = fingerName + "-" + j;
                        handData.jointAngles[index] = jointRotation;

                        //Debug.Log(handData.fingerJoint[index] + handData.jointAngles[index] + index);
                        index++;
                    }
                }
                string armName = "";

                if (hand.IsLeft)
                    armName = "오른팔";
                else
                    armName = "왼팔";

                handData.fingerJoint[index] = armName;
                handData.jointAngles[index++] = new Quaternion(-2, -2, -2, -2);

                handData.fingerJoint[index] = armName + " 위치 값";
                handData.jointAngles[index++] = new Quaternion(-2f, -2f, -2f, -2f);
            }

            

            if(setTime == "" && setFileName == "" && setFileNum == "")
            {
                ConvertToFingerAngleJsonTest(handData);
            }
            else
            {
                ConvertToFingerAngleJson(handData);
            }
        }
    }

    public void ReverseTrigger()
    {
        Debug.Log("Here1");

        if (!isDetect)
        {
            string filePath = Path.Combine(folderPath, setFileName + ".json");
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.Write("[\n");
            }

            Debug.Log("Here3");
        }

        isDetect = true;

        //startBtn.SetActive(false);

        StartCoroutine(OffTrigger());
    }

    void ConvertToFingerAngleJson(FingerAngleData data)
    {

        StringBuilder jsonString = new StringBuilder();
        //jsonString.Append("{\n");
        jsonString.Append("\t{\n");

        for (int i = 0; i < data.fingerJoint.Length; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                switch (j) 
                { 
                    case 0:
                        jsonString.Append($"\t\t\"{data.fingerJoint[i]}-x\":{data.jointAngles[i].x}");
                        break;
                    case 1:
                        jsonString.Append($",\n\t\t\"{data.fingerJoint[i]}-y\":{data.jointAngles[i].y}");
                        break;
                    case 2:
                        jsonString.Append($",\n\t\t\"{data.fingerJoint[i]}-z\":{data.jointAngles[i].z}");
                        break;
                    case 3:
                        jsonString.Append($",\n\t\t\"{data.fingerJoint[i]}-w\":{data.jointAngles[i].w}");
                        break;
                }
            }

            if (i < data.fingerJoint.Length - 1)
            {
                jsonString.Append(",\n");
            }
        }

        jsonString.Append("\n},");

        //Debug.Log(jsonString.ToString());

        string filePath = Path.Combine(folderPath, setFileName + ".json");

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.Write(jsonString);
        }
    }

    List<List<T>> GroupData<T>(List<T> dataList, int groupSize)
    {
        // 리스트를 지정된 크기로 나누는 메서드
        List<List<T>> groupedData = new List<List<T>>();
        for (int i = 0; i < dataList.Count; i += groupSize)
        {
            groupedData.Add(dataList.GetRange(i, Mathf.Min(groupSize, dataList.Count - i)));
        }
        return groupedData;
    }

    IEnumerator OffTrigger()
    {
        yield return new WaitForSeconds(5f);
        isDetect = false;

        string filePath = Path.Combine(folderPath, setFileName + ".json");

        // JSON 파일 읽기
        string jsonContent = File.ReadAllText(filePath);

        // 문자열의 마지막 문자 삭제
        if (!string.IsNullOrEmpty(jsonContent))
        {
            jsonContent = jsonContent.Substring(0, jsonContent.Length - 1);
        }

        jsonContent += "]";

        // 수정된 내용으로 JSON 파일 다시 쓰기
        File.WriteAllText(filePath, jsonContent);

        Debug.Log(jsonContent);

        dataSender.SendJsonDataFromResource(setFileName, url, "test");
        //dataSender.DataSend(url);
        //dataSender.SendDataImmediately(url, jsonContent);

        //startBtn.SetActive(true);

        yield break;
    }

    public void ReverseTriggerTest()
    {
        Debug.Log("Here1");

        if (!isDetect)
        {
            string filePath = Path.Combine(folderPath, fileName.text + filenum.text +".json");
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.Write("[\n");
            }

            startBtn.SetActive(false);

            Debug.Log("Here3");
        }

        isDetect = true;

        //startBtn.SetActive(false);

        StartCoroutine(OffTriggerTest());
    }

    void ConvertToFingerAngleJsonTest(FingerAngleData data)
    {

        StringBuilder jsonString = new StringBuilder();
        //jsonString.Append("{\n");
        jsonString.Append("\t{\n");

        for (int i = 0; i < data.fingerJoint.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                switch (j)
                {
                    case 0:
                        jsonString.Append($"\t\t\"{data.fingerJoint[i]}-x\":{data.jointAngles[i].x}");
                        break;
                    case 1:
                        jsonString.Append($",\n\t\t\"{data.fingerJoint[i]}-y\":{data.jointAngles[i].y}");
                        break;
                    case 2:
                        jsonString.Append($",\n\t\t\"{data.fingerJoint[i]}-z\":{data.jointAngles[i].z}");
                        break;
                    case 3:
                        jsonString.Append($",\n\t\t\"{data.fingerJoint[i]}-w\":{data.jointAngles[i].w}");
                        break;
                }
            }

            if (i < data.fingerJoint.Length - 1)
            {
                jsonString.Append(",\n");
            }
        }

        jsonString.Append("\n},");

        //Debug.Log(jsonString.ToString());

        string filePath = Path.Combine(folderPath, fileName.text + filenum.text + ".json");

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.Write(jsonString);
        }
    }

    IEnumerator OffTriggerTest()
    {
        // time.text 값을 로그로 출력하여 디버깅
        Debug.Log("Raw time.text: '" + time.text.Trim() + "'");

        if (float.TryParse(time.text.Substring(0, time.text.Length - 1), out float waitTime))
        {
            // waitTime 만큼 대기
            yield return new WaitForSeconds(waitTime);
            Debug.Log("Waited for: " + waitTime + " seconds");
        }
        else
        {
            Debug.LogError("Invalid input, unable to parse to float.");
        }

        isDetect = false;

        string filePath = Path.Combine(folderPath, fileName.text + filenum.text + ".json");

        // JSON 파일 읽기
        string jsonContent = File.ReadAllText(filePath);

        // 문자열의 마지막 문자 삭제
        if (!string.IsNullOrEmpty(jsonContent))
        {
            jsonContent = jsonContent.Substring(0, jsonContent.Length - 1);
        }

        jsonContent += "]";

        // 수정된 내용으로 JSON 파일 다시 쓰기
        File.WriteAllText(filePath, jsonContent);

        Debug.Log(jsonContent);

        startBtn.SetActive(true);

        //dataSender.SendJsonDataFromResource(setFileName, url, "test");
        //dataSender.DataSend(url);
        //dataSender.SendDataImmediately(url, jsonContent);

        //startBtn.SetActive(true);

        yield break;
    }
}