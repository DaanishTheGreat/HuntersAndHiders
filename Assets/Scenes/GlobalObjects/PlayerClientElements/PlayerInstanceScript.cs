using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class PlayerClientData : INetworkSerializable
{
    private double Latitude_Normalized;
    private double Longitude_Normalized;
    private string PlayerClientName;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Latitude_Normalized);
        serializer.SerializeValue(ref Longitude_Normalized);
        serializer.SerializeValue(ref PlayerClientName);
    }

    public PlayerClientData(string PlayerClientNameInput, double LatitudeInput, double LongitudeInput)
    {
        PlayerClientName = PlayerClientNameInput;
        Latitude_Normalized = LatitudeInput;
        Longitude_Normalized = LongitudeInput;
    }

    public List<double> GetCoordinates()
    {
        List<double> CoordinateList = new List<double>
        {
            Latitude_Normalized,
            Longitude_Normalized
        };

        return CoordinateList;
    }

    public void UpdateCoordinates(double LatitudeInput, double LongitudeInput)
    {
        Latitude_Normalized = LatitudeInput;
        Longitude_Normalized = LongitudeInput;
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

    [ServerRpc]
    public void SendPlayerLocationsToServerRpc(string PlayerName, double NormalizedLatitude, double NormalizedLongitude)
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
            PlayerClientDataAtIndex.UpdateCoordinates(NormalizedLatitude, NormalizedLongitude);
            PlayerClientDataList[PlayerClientDataIndex] = PlayerClientDataAtIndex;
        }
        else
        {
            PlayerClientData NewPlayerClientDataObject = new PlayerClientData(PlayerName, NormalizedLatitude, NormalizedLongitude);
            PlayerClientDataList.Add(NewPlayerClientDataObject);
        }
    }

    // Start of Server Client Get Request RPCs
    [ServerRpc]
    public void RequestAllPlayerCoordinatesServerRpc()
    {   
        int Index = 0;
        foreach(PlayerClientData PlayerClientDataObj in PlayerClientDataList)
        {
            SendAllPlayerCoordinatestoClientRpc(PlayerClientDataObj, Index);
            Index++;
        }
    }

    [ClientRpc]
    public void SendAllPlayerCoordinatestoClientRpc(PlayerClientData PlayerClientDataInput, int Index)
    {
        PlayerClientDataList[Index] = PlayerClientDataInput;
    }
    // End of Server Client Get Request RPCs

    // End of Main Game Scenes Location Sync
}
