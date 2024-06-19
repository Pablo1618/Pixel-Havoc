using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor;

public class UDPClient : MonoBehaviour
{
    static string serverIp = "127.0.0.1";
    static int port = 5000;
    static UdpClient client;
    static UDPClient instance;
    private static UDPClientInfoArray playerPositions;
    [SerializeField]
    public GameObject enemy;
    public static GameObject[] enemies;

    public static void SetupClient()
    {
        client = new UdpClient();
        enemies = new GameObject[0];
        string welcomeString = $"MyID: {GameClient.id}";
        Send(welcomeString);
        Debug.Log($"[Client {GameClient.id}]: Sent welcome!");
        Thread readThread = new Thread(() =>
        {
            ReceivePlayersInfo();
        });
        readThread.Start();
    }

    public void Awake()
    {
        instance = this;
    }

    public void FixedUpdate()
    {
        SendPlayerInfo();
        UpdatePlayersInfo(playerPositions);
    }

    public static void SendPlayerInfo()
    {
        string message = "Data:" + JsonUtility.ToJson(PlayerController.clientInfo);
        Debug.Log($"[Client {GameClient.id}]: sent: {message}");
        Send(message);
    }

    public static void SendKillMessage(int whoGotRekt)
    {
        string message = $"Kill:{GameClient.id}:{whoGotRekt}";
        Debug.Log($"{GameClient.id} GOT A NICE HEADSHOT!");
        Send(message);
    }

    public static void UpdatePlayersInfo(UDPClientInfoArray players) 
    {
        Debug.Log("UPDATING PLAYER POSITIONS!");
        if(players.clientsInfo.Length > enemies.Length)
        {
            GameObject[] newEnemies = new GameObject[players.clientsInfo.Length];

            for (int i = 0; i < players.clientsInfo.Length; i++)
            {
                Vector3 position = new Vector3(players.clientsInfo[i].playerInfo.x, players.clientsInfo[i].playerInfo.y, 0); //enemy spawn coordinates
                Quaternion spawnRotation = Quaternion.Euler(0, 0, players.clientsInfo[i].playerInfo.rotation);//enemy rotation
                GameObject newEnemy = GameObject.Instantiate(instance.enemy, position, spawnRotation);//spawn enemy
                newEnemy.GetComponent<EnemyInfo>().EnemyID = players.clientsInfo[i].id;
                if (i == GameClient.id)
                    newEnemy.SetActive(false);
                newEnemies[i] = newEnemy;
            }
            enemies = newEnemies;
        }
        else
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].transform.position = new Vector3(players.clientsInfo[i].playerInfo.x, players.clientsInfo[i].playerInfo.y, 0);//move player
                enemies[i].transform.rotation = Quaternion.Euler(0, 0, players.clientsInfo[i].playerInfo.rotation);//rotate player
            }
        }
    }

    public static void ReceivePlayersInfo()
    {
        while (true)
        {
            string message = Receive();
            Debug.Log($"{GameClient.id} RECEIVED: {message}");
            if (message == "Respawn!")
            {
                PlayerController.instance.shouldRespawn = true;
            }
            else
            {
                UDPClientInfoArray playersInfo = JsonUtility.FromJson<UDPClientInfoArray>(message);
                Debug.Log($"[Client {GameClient.id}]: Received: {message}");
                playerPositions = playersInfo;
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
