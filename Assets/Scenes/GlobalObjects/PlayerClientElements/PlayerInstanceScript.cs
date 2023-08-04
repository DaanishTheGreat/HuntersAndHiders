using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerInstanceScript : NetworkBehaviour
{
    
    public static List<string> PlayerNames = new List<string>();

    public string InstancePlayerName = "Null Manually Instantiated";

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc]
    public void SendPlayerNameToServerRpc(string PlayerName = "Developer Instantiated Null")
    {
        PlayerNames.Add(PlayerName);
        InstancePlayerName = PlayerName;
        Debug.Log(PlayerName);
    }

    public void LocalSendPlayerName(string PlayerName = "Empty")
    {
        PlayerNames.Add(PlayerName);
        InstancePlayerName = PlayerName;
    }

}
