using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UINetworkingManager : MonoBehaviour
{
    public static UINetworkingManager instance;

    public TMP_InputField ipInput;
    public TMP_InputField nameInput;

    public GameObject joinPanel;
    public GameObject lobbyPanel;

    public GameObject playerList;
    public GameObject playerLabelPrefab;

    [SerializeField]
    public UnityEngine.UI.Button startGameButton;

    public string lobby = "Lobby:";

    //stupid hack because prevents calling unity-based functions from threads
    bool wantsToUpdateLobby = false;
    public void Update()
    {
        if(wantsToUpdateLobby)
        {
            updateLobby();
            wantsToUpdateLobby = false;
        }
    }

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
        lobbyPanel.SetActive(false);
    }

    public void connectToServer()
    {
        string ip = ipInput.text;
        string clientName = nameInput.text;

        GameClient.instance.setIP(ip);
        GameClient.instance.setName(clientName);
        GameClient.instance.ConnectToServer();
        goToLobby();

       
    }

    public void hostServer()
    {
        GameServer.instance.startServer();
        string clientName = nameInput.text;
        GameClient.instance.setName(clientName);
        GameClient.instance.ConnectToServer();
        goToLobby();
    }
  
    public void setNewLobbyString(string _lobby)
    {
        lobby = _lobby;
        wantsToUpdateLobby = true;
    }
    public void updateLobby()
    {

        foreach(Transform child in playerList.transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

     

        string[] names = lobby.Split(" ");
        names = names.Skip(1).ToArray();
        names = names.SkipLast(1).ToArray();

        foreach(var name in names)
        {
           

                GameObject newInstance = Instantiate(playerLabelPrefab, playerList.transform);
                TMP_Text textObject = newInstance.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
                textObject.text = name;
  
        }

    }
    public void goToLobby()
    {
        joinPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        if(!GameServer.instance.isRunning)
        {
            //we are not the host so we disable the start game button
            startGameButton.interactable = false;
        }

        updateLobby();
    
    }

    public void startGame()
    {
        GameServer.instance.writeStartGameToAllPlayers();
        UDPServer.StartServer(); //imo powinno byæ w Start() w tej klasie, by siê aktywowa³ serwer przy za³adowaniu sceny
    }
}
