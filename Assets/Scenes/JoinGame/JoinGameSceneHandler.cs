using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class JoinGameSceneHandler : MonoBehaviour //MonoBehaviour
{
    private string JoinCode;
    private string PlayerName = "empty";
    //public GameObject PlayerClientPrefab;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.StartClient();
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
        GameObject EnterPlayerName = GameObject.Find("PlayerNameInputField");
        GameObject Player = GameObject.Find("PlayerClient(Clone)");

        GameObject[] PlayerClients = GameObject.FindGameObjectsWithTag("PlayerClientPrefab");

        foreach(GameObject val in PlayerClients)
        {
            NetworkObject IsOwnerOrNot = val.GetComponent<NetworkObject>();
            if(IsOwnerOrNot.IsOwner)
            {
                Player = val;
            }
        }

        PlayerInstanceScript PlayerInstance = Player.GetComponent<PlayerInstanceScript>();
        TMP_InputField EnterPlayerNameTextMesh = EnterPlayerName.GetComponent<TMP_InputField>();
        PlayerInstance.SendPlayerNameToServerRpc(EnterPlayerNameTextMesh.text);
        Debug.Log(EnterPlayerNameTextMesh.text);
        //PlayerInstance.UpdatePlayerName(PlayerName);
    }
}
