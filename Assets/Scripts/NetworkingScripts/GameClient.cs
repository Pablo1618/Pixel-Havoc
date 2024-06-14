using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine.SceneManagement;

public class GameClient : MonoBehaviour
{
    public static GameClient instance;
    public static int bufferSize = 4096;

    public static string ip = "127.0.0.1";
    public static int port = 2137;
    public static int id = 0;
    public static  TCP tcp;

    public static string clientName = "Player";

    public static string lobby = "Lobby: placeholder";
    public static bool gameStarted = false;

    public string getLobby()
    {
        return lobby;
    }

    public class TCP
    {
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        public void connect()
        {
            socket = new TcpClient();
            socket.ReceiveBufferSize = bufferSize;
            socket.SendBufferSize = bufferSize;


            receiveBuffer = new byte[bufferSize];
            socket.BeginConnect(ip, port, HandleConnection, socket);
        }

        private void HandleConnection(IAsyncResult result)
        {
            socket.EndConnect(result);

            if(!socket.Connected)
            {
                return;
            }
            stream = socket.GetStream();

            Debug.Log("[GameClient] started read");
            Thread readThread = new Thread(() =>
            {
         
                stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveData, null);
        
                
            });
            readThread.Start();

        }

        private void ReceiveData(IAsyncResult result)
        {
            try
            {
                int resultLength = stream.EndRead(result);
                if (resultLength > 0)
                {
                    string dataReceived = Encoding.ASCII.GetString(receiveBuffer, 0, resultLength);
                    Debug.Log("[GameClient] Received: " + dataReceived);

                    switch(dataReceived)
                    {
                        case "clientDataReady":
                            StreamWriter writer = new StreamWriter(stream,
          Encoding.ASCII);
                            writer.AutoFlush = true;
                            writer.WriteAsync("Name: " + clientName);
                            // writer.Close();

                            Debug.Log("[GameClient] sent name");
                            break;

                        case "Start":
                            Debug.Log($"Start {id}");
                            gameStarted = true; // zmiana sceny po otrzymaniu Start
                            break;

                        default:
                            if(dataReceived.StartsWith("Lobby:"))
                            {

                                lobby = dataReceived;

                                UINetworkingManager.instance.setNewLobbyString(lobby);
                                
                            }

                            else if(dataReceived.StartsWith("ID:"))
                            {
                                id = int.Parse(dataReceived.Split(" ")[1]);
                                Debug.Log($"My ID is: {id}");
                            }

                            break;
                    }
                   
                }
               
            }
            catch (Exception exception)
            {
                Debug.Log("ERROR WHILE RECEIVING DATA: " + exception.GetBaseException()) ;
            }
            stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveData, null);
        }

    }


    public void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
        else if (instance != this)
        {
                Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
    }

    public void setIP(String _ip)
    {
        ip = _ip;
    }
    public void setName(String _name)
    {
        clientName = _name;
    }

    public void ConnectToServer()
    {
        Debug.Log("GameClient attempting connection.");
        tcp.connect();
    }

    public void FixedUpdate()
    {
        if(gameStarted)
            SceneManager.LoadScene("Game"); //g³upie obejœcie w¹tków w unity
    }
}
