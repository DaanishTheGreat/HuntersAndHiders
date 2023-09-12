using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.SceneManagement;
using System;


public class PlayerInstanceScript : NetworkBehaviour
{
    
    public static List<string> PlayerNames = new List<string>();

    public static List<PlayerClientData> PlayerClientDataList = new List<PlayerClientData>();

    public string InstancePlayerName = "Null Manually Instantiated";

    public int CurrentSortingProperty = 0;



    // Start Hide and Seek Variables

    // ((Reference HideAndSeekGamemodeGameMechanicsInfo.txt) Pregame=0: Game Setup and Timer for Hiders to disperse
    // ActualGame = 1: Seeker Goes and Find Hiders
    // EndGame = 2: Transfer all players to GameSummaryScene
    private int GameState_HideAndSeekGamemode = 0; //(HOST/SERVER Only)

    public static List<BoundariesData_HideAndSeek> BoundariesDataList_HideAndSeek = new List<BoundariesData_HideAndSeek>();

    // End of Hide and Seek Variables 


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
            PlayerClientData NewPlayerClientDataObject = new PlayerClientData();
            NewPlayerClientDataObject.InitializePlayerClientData(PlayerName, NormalizedLatitude, NormalizedLongitude, CurrentSortingProperty);
            CurrentSortingProperty++;
            PlayerClientDataList.Add(NewPlayerClientDataObject);
        }
    }

    // Start of Server Client Get Request RPCs
    [ServerRpc]
    public void RequestAllPlayerCoordinatesServerRpc()
    {   
        int Index = 0;

        List<PlayerClientData> PlayerClientDataListLocal = PlayerClientDataList;

        foreach(PlayerClientData PlayerClientDataObj in PlayerClientDataListLocal.ToList())
        {
            SendAllPlayerCoordinatestoClientRpc(PlayerClientDataObj, Index);
            Index++;
        }
    }

    [ClientRpc]
    public void SendAllPlayerCoordinatestoClientRpc(PlayerClientData PlayerClientDataInput, int Index)
    {
        if(Index == 0)
        {
            PlayerClientDataList.Clear();
        }

        PlayerClientDataList.Add(PlayerClientDataInput);
        
        Debug.Log("Sending Data To Client: " + PlayerClientDataInput.GetPlayerClientName());
    }
    // End of Server Client Get Request RPCs

    // End of Main Game Scenes Location Sync


    // Start of Hide and Seek Specific RPCs (Not Implemented Yet)
    
    [ServerRpc]
    public void HideAndSeekGamemode_UpdatePlayerClientHiderOrSeekerStatusServerRpc()
    {
        GameMechanicsHideAndSeek GameMechanicsHideAndSeek_Object = new GameMechanicsHideAndSeek();

        PlayerClientDataList = GameMechanicsHideAndSeek_Object.CheckDistanceOfPlayerClientsAndFlagAsSeekerIfCaught(PlayerClientDataList);
    }

    [ClientRpc]
    public void HideAndSeekGamemode_AnnounceEventClientRpc()
    {

    }

    [ServerRpc]
    public void HideAndSeekGamemode_AddBoundaryServerRpc()
    {
        
    }

    // End of Hide and Seek Specific RPCs
    
}
