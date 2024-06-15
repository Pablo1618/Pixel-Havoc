using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEditor.VersionControl;

public class UDPServer : MonoBehaviour
{
    public static List<UDPClientInfo> clientsInfo = new List<UDPClientInfo>();
    public static UdpClient server;
    public static bool isStarted = false;
    public static void StartServer()
    {
        int port = 5000;
        server = new UdpClient(port);
        Console.WriteLine("Server started on port: {0}", port);

        Thread readThread = new Thread(() =>
        {
            Listen();
        });
        readThread.Start();
        isStarted = true; 
    }

    public void FixedUpdate()
    {
        if(!isStarted)
        {
            return;
        }
        UpdateAllClients();
    }

    public static void Listen()
    {
        while (true)
        {
            try
            {
                IPEndPoint clientIP = null; 
                byte[] data = server.Receive(ref clientIP);
                string message = Encoding.ASCII.GetString(data);
                
                if(message.StartsWith("MyIP:"))
                {
                    int clientID = int.Parse(message.Split(':')[1]);
                    clientsInfo.Add(new UDPClientInfo(clientID, clientIP));
                }
                else if(message.StartsWith("Data:"))
                {
                    UDPClientInfo clientInfo = JsonUtility.FromJson<UDPClientInfo>(message.Split(':')[1]);
                    clientsInfo.Find(info => info.id == clientInfo.id).Update(clientInfo);
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
}
