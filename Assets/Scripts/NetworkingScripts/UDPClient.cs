using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    static string serverIp = "127.0.0.1";
    static int port = 2137;
    static UdpClient client;
    //private int delay = 10, counter = 0;
    public static void SetupClient()
    {
        client = new UdpClient();
        string welcomeString = $"MyID: {GameClient.id}";
        Send(welcomeString);
    }

    public void FixedUpdate()
    {
        //counter++;
        //if (counter % delay == 0) //doda³em tymczasowo na czas debugowania, ale nawet siê nie przyda³o...
        //{
            //counter = 0;
            if (!UDPServer.isStarted)
            {
                return;
            }

            SendPlayerInfo();
            ReceivePlayersInfo();
        //}
    }

    public static void SendPlayerInfo()
    {
        // todo: change for real clientInfo object (no need to create, would just change position info)
        UDPClientInfo clientInfo = new UDPClientInfo(2137, new IPEndPoint(2137, 2137));
        string message = "Data:" + JsonUtility.ToJson(clientInfo);
        Send(message);
    }

    public static void ReceivePlayersInfo()
    {
        string otherPlayersJsonInfo = Receive();
        UDPClientInfoArray playersInfo = JsonUtility.FromJson<UDPClientInfoArray>(otherPlayersJsonInfo);
        Debug.Log($"[{GameClient.id}] Received: {playersInfo}");
        // todo: update other players
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
