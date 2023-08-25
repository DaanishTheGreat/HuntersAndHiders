using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class PlayerClientData
{
    private double Latitude;
    private double Longitude;
    private string PlayerClientName;

    public PlayerClientData(string PlayerClientNameInput, double LatitudeInput, double LongitudeInput)
    {
        PlayerClientName = PlayerClientNameInput;
        Latitude = LatitudeInput;
        Longitude = LongitudeInput;
    }

    public List<double> GetCoordinates()
    {
        List<double> CoordinateList = new List<double>
        {
            Latitude,
            Longitude
        };

        return CoordinateList;
    }

    public void UpdateCoordinates(double LatitudeInput, double LongitudeInput)
    {
        Latitude = LatitudeInput;
        Longitude = LongitudeInput;
    }

    public string GetPlayerClientName()
    {
        return PlayerClientName;
    }
}

public class PlayerInstanceScript : NetworkBehaviour
{
    
    public static List<string> PlayerNames = new List<string>();

    public static List<PlayerClientData> PlayerClientDataList = new List<PlayerClientData>();

    public string InstancePlayerName = "Null Manually Instantiated";

    [ServerRpc]
    public void SendPlayerNameToServerRpc(string PlayerName = "Developer Instantiated Null")
    {
        PlayerNames.Add(PlayerName);
        InstancePlayerName = PlayerName;
        Debug.Log(PlayerName);
    }


    //Start of Client Requested Player Name Data Rpc Bundle
    [ServerRpc]
    public void RequestPlayerNamesToServerRpc()
    {
        string PlayerNamesToString = "";
        foreach(string PlayerName in PlayerNames)
        {
            PlayerNamesToString = PlayerNamesToString + ":" + PlayerName;
        }
        RespondWithPlayerNamesToClientRpc(PlayerNamesToString);
    }

    [ClientRpc]
    public void RespondWithPlayerNamesToClientRpc(string PlayerNames_Input)
    {
        PlayerNames = PlayerNames_Input.Split(":").ToList();
        PlayerNames.RemoveAll(Element => Element == "");
    }
    //End of Client Requested Player Name Data Rpc Bundle

    [ServerRpc]
    public void ChangeHostSceneServerRpc(string SceneName)
    {
        NetworkManager.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }

/*
    public override void OnNetworkSpawn()
    {
        Debug.Log("Is Host: " + IsHost);
        if(IsHost == false)
        {
            //Cant work because only server can start Network Scene Event
            //NetworkManager.SceneManager.LoadScene("ClientConnectedScene", LoadSceneMode.Single);

            SceneManager.LoadScene("ClientConnectedScene", LoadSceneMode.Additive);
        }
    }
    */

    // Start of Main Game Scenes Location Sync

    [ServerRpc]
    public void SendPlayerLocationsToServerRpc(string PlayerName, double Latitude, double Longitude)
    {
        int PlayerClientDataIndex = 0;
        bool PlayerClientObjectWithPlayerNameExistsInList = false;
        foreach(PlayerClientData EachPlayerClientData in PlayerClientDataList)
        {
            if(EachPlayerClientData.GetPlayerClientName().Equals(PlayerName))
            {
                PlayerClientObjectWithPlayerNameExistsInList = true;
                break;
            }
            PlayerClientDataIndex++;
        }

        if(PlayerClientObjectWithPlayerNameExistsInList == true)
        {
            PlayerClientData PlayerClientDataAtIndex = PlayerClientDataList[PlayerClientDataIndex];
            PlayerClientDataAtIndex.UpdateCoordinates(Latitude, Longitude);
            PlayerClientDataList[PlayerClientDataIndex] = PlayerClientDataAtIndex;
        }
        else
        {
            PlayerClientData NewPlayerClientDataObject = new PlayerClientData(PlayerName, Latitude, Longitude);
            PlayerClientDataList.Add(NewPlayerClientDataObject);
        }
    }

    // Start of Server Client Get Request RPCs
    [ServerRpc]
    public void RequestAllPlayerCoordinatesServerRpc()
    {
        // DO THIS NEXT
    }

    [ClientRpc]
    public void SendAllPlayerCoordinatestoClientRpc()
    {

    }
    // End of Server Client Get Request RPCs

    // End of Main Game Scenes Location Sync
}
