using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        // WebSocket ���� �ּ� ����
        string url = "ws://203.232.193.173:5000/";

        // WebSocket ��ü ����
        ws = new WebSocket(url);

        // ������ �̺�Ʈ �ڵ鷯 ����
        ws.OnOpen += OnOpen;
        ws.OnClose += OnClose;
        ws.OnError += OnError;

        // ���� ���� �õ�
        ws.ConnectAsync();
    }

    // ������ �������� �� ȣ��Ǵ� �޼���
    private void OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connected");

        // ������ �����ϸ� �����͸� ����
        string dataToSend = "Hello, Flask!";
        ws.Send(dataToSend);
    }

    // ������ ����Ǿ��� �� ȣ��Ǵ� �޼���
    private void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket closed: " + e.Reason);
    }

    // ������ �߻����� �� ȣ��Ǵ� �޼���
    private void OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError("WebSocket error: " + e.Message);
    }

    // ��ü�� �ı��� �� WebSocket ���� ����
    private void OnDestroy()
    {
        if (ws != null && ws.IsAlive)
            ws.Close();
    }
}