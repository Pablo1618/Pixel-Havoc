using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class GameServer : MonoBehaviour
{
    public static GameServer instance;

    public static int port = 2137;

    private static TcpListener tcpListener;

    public static Dictionary<int, ClientData> clients = new Dictionary<int, ClientData>();

    public static int newClientID = 0;

    public bool isRunning = false;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

 

    public void startServer()
    {
        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(connectClientTCP), null);
        
        Debug.Log($"Server started on port {port}");
        isRunning = true;
    }

    private static void connectClientTCP(IAsyncResult result)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(connectClientTCP), null);

        Debug.Log("Connection from client incoming!");

        clients.Add(newClientID, new ClientData(newClientID, instance));
        clients[newClientID].tcp.connect(client);

    

        newClientID++;

    }

    public Dictionary<int, ClientData> getClientDatas()
    {
        return clients;
    }


    public void writeLobbyDataToAllPlayers()
    {
        foreach(KeyValuePair<int, ClientData> client in clients)
        {
            client.Value.tcp.writeLobbyDataToPlayers();
        }
    }

    public void writeStartGameToAllPlayers()
    {
        foreach (KeyValuePair<int, ClientData> client in clients)
        {
            client.Value.tcp.writeStart();
        }
    }

    public void OnApplicationQuit()
    {
        if(tcpListener != null)
            tcpListener.Stop();
    }
}
