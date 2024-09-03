using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        // WebSocket 서버 주소 설정
        string url = "ws://203.232.193.173:5000/";

        // WebSocket 객체 생성
        ws = new WebSocket(url);

        // 웹소켓 이벤트 핸들러 설정
        ws.OnOpen += OnOpen;
        ws.OnClose += OnClose;
        ws.OnError += OnError;

        // 서버 연결 시도
        ws.ConnectAsync();
    }

    // 연결이 성공했을 때 호출되는 메서드
    private void OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connected");

        // 연결이 성공하면 데이터를 보냄
        string dataToSend = "Hello, Flask!";
        ws.Send(dataToSend);
    }

    // 연결이 종료되었을 때 호출되는 메서드
    private void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket closed: " + e.Reason);
    }

    // 에러가 발생했을 때 호출되는 메서드
    private void OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("WebSocket error: " + e.Message);
    }

    // 객체가 파괴될 때 WebSocket 연결 해제
    private void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
            ws.Close();
    }
}