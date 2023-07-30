using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JoinGameSceneHandler : MonoBehaviour
{
    private string JoinCode;
    private string PlayerName = "empty";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateJoinCode(string Join_Code)
    {
        JoinCode = Join_Code;
        Debug.Log(JoinCode);
    }

    public void UpdatePlayerName(string name)
    {
        PlayerName = name;
    }

    public void JoinGameOnClick()
    {
        Debug.Log("Joining Game");
        GameObject Player = GameObject.Find("PlayerClient");
        PlayerInstanceScript PlayerInstance = Player.GetComponent<PlayerInstanceScript>();
        PlayerInstance.UpdatePlayerName(PlayerName);
        NetworkManager.Singleton.StartClient();
    }
}
