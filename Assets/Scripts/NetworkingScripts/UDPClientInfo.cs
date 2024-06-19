using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

[Serializable]
public class UDPClientInfo
{
    public UDPClientInfo(int id, IPEndPoint clientIP)
    {
        this.id = id;
        this.clientIP = clientIP;
        playerInfo = new PlayerPos();
    }

    public UDPClientInfo(int id) 
    {
        this.id = id;
        playerInfo = new PlayerPos();
    }

    public IPEndPoint clientIP;
    [SerializeField]
    public int id;
    [SerializeField]
    public PlayerPos playerInfo;
    private int kills = 0;
    private int deaths = 0;
    public void addKill()
    {
        kills += 1;
    }

    public void addDeaths()
    {
        deaths += 1;
    }

    public void Update(UDPClientInfo newInfo)
    {
        id = newInfo.id;
        playerInfo = newInfo.playerInfo; 
    }
}

// HACK - unity does not support serializing arrays -_-
public class UDPClientInfoArray
{
    public UDPClientInfo[] clientsInfo;
    public UDPClientInfoArray(List<UDPClientInfo> clientsInfo)
    {
        this.clientsInfo = clientsInfo.ToArray();
    }
    public List<UDPClientInfo> ToList()
    {
        return clientsInfo.ToList();
    }
}
