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

    public static void SetupClient()
    {
        client = new UdpClient();
        string welcomeString = $"MyID: {GameClient.id}";
        Send(welcomeString);
        Debug.Log($"[Client {GameClient.id}]: Sent welcome!");
        Thread readThread = new Thread(() =>
        {
            ReceivePlayersInfo();
        });
    }

    public void FixedUpdate()
    {
        SendPlayerInfo();
    }

    public static void SendPlayerInfo()
    {
        string message = "Data:" + JsonUtility.ToJson(PlayerController.clientInfo);
        Debug.Log($"[Client {GameClient.id}]: sent: {message}");
        Send(message);
    }

    public static void ReceivePlayersInfo()
    {
        while (true)
        {
            //if (shouldReceive)
            //{
            //try
            //{
            Debug.Log($"[Client {GameClient.id}]: Trying to receive.");
            string otherPlayersJsonInfo = Receive();
            UDPClientInfoArray playersInfo = JsonUtility.FromJson<UDPClientInfoArray>(otherPlayersJsonInfo);
            Debug.Log($"[Client {GameClient.id}]: Received: {otherPlayersJsonInfo}");
            //}
            //catch (Exception e)
            //{
            //    Debug.Log($"[Client {GameClient.id}]: error: {e.Message}");
            //}
                // todo: update other players
                //shouldReceive = false;
            //}
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
