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
    async void Awake()
	{
		try
		{
			await UnityServices.InitializeAsync();
            NetworkManager.Singleton.StartHost();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}

        Debug.Log("UGS started");
	}

    public void UpdateList()
    {
        GameObject PlayerEnt = GameObject.Find("PlayerClient(Clone)");
        PlayerInstanceScript PlayerEntityInstanceScript = PlayerEnt.GetComponent<PlayerInstanceScript>();
        PlayerEntityInstanceScript.LocalSendPlayerName("Daaaanish");

        Debug.Log("Player Connected: " + PlayerInstanceScript.PlayerNames.ToString());

        GameObject PlayerTextListGameObject = GameObject.Find("Players");
        //GameObject ListOfPlayerClientClones = GameObject.FindObjectsOfTypeAll()
        TextMeshProUGUI PlayerTextList = PlayerTextListGameObject.GetComponent<TextMeshProUGUI>();

        string PlayerListToString = "";
        foreach(string val in PlayerInstanceScript.PlayerNames)
        {
            PlayerListToString = PlayerListToString + " " + val;
        }
        PlayerTextList.text = PlayerListToString;
    }

    public static void UpdatePlayerName()
    {

    }


}
