using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;

public class UDPClient : MonoBehaviour
{
    static string serverIp = "127.0.0.1";
    static int port = 5000;
    static UdpClient client;
    private int delay = 50, counter = 0;
    public static bool shouldReceive = false;

    public static void SetupClient()
    {
        client = new UdpClient();
        string welcomeString = $"MyID: {GameClient.id}";
        Send(welcomeString);
        Debug.Log("Sent welcome!");
        Thread readThread = new Thread(() =>
        {
            ReceivePlayersInfo();
        });
    }

    public void FixedUpdate()
    {
        counter++;
        if (counter % delay == 0)
        {
            counter = 0;
            if (!UDPServer.isStarted)
            {
                return;
            }
            shouldReceive = true;
            SendPlayerInfo();
            Debug.Log($"Client sent and received a message!");
        }
    }

    public static void SendPlayerInfo()
    {
        Debug.Log($"Client {GameClient.id} sending...");
        string message = "Data:" + JsonUtility.ToJson(PlayerController.clientInfo);
        Debug.Log($"Client sent: {message}");
        Send(message);
    }

    public static void ReceivePlayersInfo()
    {
        while (true)
        {
            if (shouldReceive)
            {
                string otherPlayersJsonInfo = Receive();
                UDPClientInfoArray playersInfo = JsonUtility.FromJson<UDPClientInfoArray>(otherPlayersJsonInfo);
                Debug.Log($"[{GameClient.id}] Received: {otherPlayersJsonInfo}");
                // todo: update other players
                shouldReceive = false;
            }
        }
    }

    public static void Send(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        client.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(serverIp), port));
    }

    public static string Receive()
    {
        IPEndPoint remoteIp = null;
        byte[] responseData = client.Receive(ref remoteIp);
        return Encoding.ASCII.GetString(responseData);
    }
}
