using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;

public class ClientData
{
    public static int sizeOfDataBuffer = 4096;
    public int clientID;
    public TCP tcp;
    public static GameServer parentGameServer;
    public string clientName;

    public ClientData(int id, GameServer _parentGameServer)
    {
        clientID = id;
        tcp = new TCP(id, this);
        parentGameServer = _parentGameServer;

    }

    public string getClientName()
    {
        return clientName;
    }

    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private byte[] receiveBuffer;
        private ClientData clientData;
        public TCP(int _id, ClientData _clientData)
        {
            id = _id;
            clientData = _clientData;
        }

        public void connect(TcpClient _socket)
        {
           

            socket = _socket;
            socket.ReceiveBufferSize = sizeOfDataBuffer;
            socket.SendBufferSize = sizeOfDataBuffer;
            receiveBuffer = new byte[sizeOfDataBuffer];


            stream = socket.GetStream();

            Thread readThread = new Thread(() =>
            {
        
                stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, receiveData, null);
              
            });
            readThread.Start();


            StreamWriter writer = new StreamWriter(stream,
           Encoding.ASCII);
            writer.AutoFlush = true;
            writer.Write("clientDataReady");
           // writer.Close();
            Debug.Log("[ClientData] Sent ready");

           
        }

        public void writeLobbyDataToPlayers()
        {
            StreamWriter writer = new StreamWriter(socket.GetStream(),
           Encoding.ASCII);
            writer.AutoFlush = true;

            string lobbyPlayers = "Lobby: ";

            Debug.Log("Player number: " + parentGameServer.getClientDatas().Count);
            foreach (var client in parentGameServer.getClientDatas())
            {
                lobbyPlayers += client.Value.getClientName() + " ";
            }
            Debug.Log(lobbyPlayers);
            writer.WriteAsync(lobbyPlayers);
        }

        public void writeStart()
        {
            StreamWriter writer = new StreamWriter(socket.GetStream(),
           Encoding.ASCII);
            writer.AutoFlush = true;

            string message = "Start";

            writer.WriteAsync(message);
        }

        private void receiveData(IAsyncResult result)
        {
            try
            {
                int resultLength = stream.EndRead(result);
                if (resultLength > 0)
                {
                    string dataReceived = Encoding.ASCII.GetString(receiveBuffer, 0, resultLength);
                    Debug.Log("[ClientData] Received : " + dataReceived);

                    if(dataReceived.StartsWith("Name:"))
                    {
                        dataReceived = dataReceived.Split(" ")[1];
                        clientData.clientName = dataReceived;

                        parentGameServer.writeLobbyDataToAllPlayers();
                       // writer.Close();

                    }
                    
                }
    
            }
            catch (Exception exception)
            {
                Debug.Log("ERROR WHILE RECEIVING DATA" + exception.GetBaseException());
            }
            stream.BeginRead(receiveBuffer, 0, sizeOfDataBuffer, receiveData, null);
        }

    }
}
