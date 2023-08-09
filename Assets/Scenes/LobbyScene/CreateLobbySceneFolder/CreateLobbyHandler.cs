using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using TMPro;

public class CreateLobbyHandler : MonoBehaviour
{
    //Start UGS
    public GameObject PlayerPrefab; 

    private bool IsConnected = false;
    private bool PlayerNameUpdatedToServer = false;
    async void Start()
	{
		try
		{
			await UnityServices.InitializeAsync();
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
            Debug.Log("Some Error");
		}
        
        GameObject PlayerTextListGameObject = GameObject.Find("Players");
        TextMeshProUGUI PlayerTextList = PlayerTextListGameObject.GetComponent<TextMeshProUGUI>();
        PlayerTextList.text = CreateGameLobbyHandler.PlayerName_InputField;
	}

    void Update() //Change to on client connect server updates, sending RPC every frame is extremely inefficient
    {
        if(IsConnected == true)
        {
            GameObject PlayerTextListGameObject = GameObject.Find("Players");
            TextMeshProUGUI PlayerTextList = PlayerTextListGameObject.GetComponent<TextMeshProUGUI>();

            string PlayerListToString = "";
            foreach(string val in PlayerInstanceScript.PlayerNames)
            {
                PlayerListToString = PlayerListToString + " " + val;
            }
            PlayerTextList.text = PlayerListToString;
        }
    }

    public void UpdateList()
    {
        GameObject PlayerEnt = GetHostPlayer();
        PlayerInstanceScript PlayerEntityInstanceScript = PlayerEnt.GetComponent<PlayerInstanceScript>();
        PlayerEntityInstanceScript.SendPlayerNameToServerRpc(CreateGameLobbyHandler.PlayerName_InputField);

        Debug.Log("Player Connected: " + PlayerInstanceScript.PlayerNames.ToString());

        GameObject PlayerTextListGameObject = GameObject.Find("Players");
        TextMeshProUGUI PlayerTextList = PlayerTextListGameObject.GetComponent<TextMeshProUGUI>();

        string PlayerListToString = "";
        foreach(string val in PlayerInstanceScript.PlayerNames)
        {
            PlayerListToString = PlayerListToString + " " + val;
        }
        PlayerTextList.text = PlayerListToString;
    }

    private GameObject GetHostPlayer()
    {
        GameObject PlayerHost = null;
        GameObject[] PlayerClients = GameObject.FindGameObjectsWithTag("PlayerClientPrefab");

        foreach(GameObject val in PlayerClients)
        {
            NetworkObject IsOwnerOrNot = val.GetComponent<NetworkObject>();
            if(IsOwnerOrNot.IsOwner)
            {
                PlayerHost = val;
            }
        }
        return PlayerHost;
    }

    public void OnClientConnected(ulong ClientId)
    {
        if(PlayerNameUpdatedToServer == false)
        {
            GameObject PlayerHost = GetHostPlayer();
            PlayerInstanceScript PlayerHostInstanceScript = PlayerHost.GetComponent<PlayerInstanceScript>();
            PlayerHostInstanceScript.SendPlayerNameToServerRpc(CreateGameLobbyHandler.PlayerName_InputField);
            PlayerNameUpdatedToServer = true;
        }
        GameObject PlayerTextListGameObject = GameObject.Find("Players");
        TextMeshProUGUI PlayerTextList = PlayerTextListGameObject.GetComponent<TextMeshProUGUI>();

        string PlayerListToString = "";
        foreach(string val in PlayerInstanceScript.PlayerNames)
        {
            PlayerListToString = PlayerListToString + " " + val;
        }
        PlayerTextList.text = PlayerListToString;

        IsConnected = true;
    }

}
