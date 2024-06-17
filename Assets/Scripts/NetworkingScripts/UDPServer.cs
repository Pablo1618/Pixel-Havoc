using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;

public class UDPServer : MonoBehaviour
{
    public static List<UDPClientInfo> clientsInfo = new List<UDPClientInfo>();
    public static UdpClient server;
    public static bool isStarted = false;
    private int delay = 100, counter = 0;

    public static void StartServer()
    {
        int port = 5000;
        server = new UdpClient(port);
        Debug.Log($"Server started on port: {port}");

        Thread readThread = new Thread(() =>
        {
            Listen();
        });
        readThread.Start();
        isStarted = true; 
    }

    public void FixedUpdate()
    {
        counter++;
        if (counter % delay == 0)
        {
            counter = 0;
            if (!isStarted)
            {
                return;
            }
            UpdateAllClients();
        }
    }

    public static void Listen()
    {
        while (true)
        {
            try
            {
                Debug.Log("Server reading...");
                IPEndPoint clientIP = null; 
                byte[] data = server.Receive(ref clientIP);
                string message = Encoding.ASCII.GetString(data);
                Debug.Log($"Server read: {message}");
                if(message.StartsWith("MyID:"))
                {
                    int clientID = int.Parse(message.Split(':')[1]);
                    clientsInfo.Add(new UDPClientInfo(clientID, clientIP));
                    Debug.Log("Added user!");
                }
                else if(message.StartsWith("Data:"))
                {
                    Debug.Log("Updating user...");
                    UDPClientInfo clientInfo = JsonUtility.FromJson<UDPClientInfo>(message.Substring("Data:".Length));
                    clientsInfo.Find(info => info.id == clientInfo.id).Update(clientInfo);
                    UDPClientInfo user = clientsInfo.Find(info => info.id == clientInfo.id);
                    Debug.Log($"Updated user:{user.id} X:{user.playerInfo.x} Y:{user.playerInfo.y}");
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server error: {0}", e.Message);
            }
        }
    }

    public static void UpdateAllClients()
    {
        UDPClientInfoArray tmp = new UDPClientInfoArray(clientsInfo); // workaround because unity does not support serializing arrays
        byte[] clientsInfoToBytes = Encoding.ASCII.GetBytes(JsonUtility.ToJson(tmp));
        foreach (var clientInfo in clientsInfo)
        {
            server.Send(clientsInfoToBytes, clientsInfoToBytes.Length, clientInfo.clientIP);
        }
    }

    public void OnApplicationQuit()
    {
        if(server!= null)
            server.Close();
    }
}
