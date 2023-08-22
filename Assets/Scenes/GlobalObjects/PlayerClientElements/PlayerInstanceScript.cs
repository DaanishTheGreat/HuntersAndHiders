using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerInstanceScript : NetworkBehaviour
{
    
    public static List<string> PlayerNames = new List<string>();

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
}
