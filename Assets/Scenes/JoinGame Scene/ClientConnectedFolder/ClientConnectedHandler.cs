using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ClientConnectedHandler : MonoBehaviour
{

    public GameObject ClientInstructionsGameObject;
    public GameObject ConnectionStatusGameObject; 
    public GameObject ConnectedPlayersGameObject;

    private bool IsConnected = false; 
    private bool PlayerNameUpdatedToServer = false;

    // Start is called before the first frame update
    void Start()
    {
        ClientInstructionsGameObject.SetActive(false); 
        ConnectedPlayersGameObject.SetActive(false);
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    // Update is called once per frame
    void Update() //Change to on client connect server updates, sending RPC every frame is extremely inefficient
    {
        if(IsConnected == true)
        {
            GameObject PlayerHost = GetHostPlayer(); 
            PlayerInstanceScript PlayerHostInstanceScript = PlayerHost.GetComponent<PlayerInstanceScript>();
            PlayerHostInstanceScript.RequestPlayerNamesToServerRpc();
            string PlayerNames = "";
            foreach(string PlayerName in PlayerInstanceScript.PlayerNames)
            {
                PlayerNames = PlayerNames + " " + PlayerName;
            }
            TMP_Text ConnectedPlayerTextMeshPro = ConnectedPlayersGameObject.GetComponent<TMP_Text>();
            ConnectedPlayerTextMeshPro.text = "Connected Players: " + PlayerNames; 
        }

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

    private void OnClientConnected(ulong ClientId)
    {
        if(PlayerNameUpdatedToServer == false)
        {
            GameObject PlayerHost = GetHostPlayer();
            PlayerInstanceScript PlayerHostInstanceScript = PlayerHost.GetComponent<PlayerInstanceScript>();
            PlayerHostInstanceScript.SendPlayerNameToServerRpc(JoinGameSceneHandler.PlayerName);
            PlayerNameUpdatedToServer = true;
        }
        ClientInstructionsGameObject.SetActive(true);
        ConnectedPlayersGameObject.SetActive(true);
        TMP_Text ConnectionStatusTextMeshPro = ConnectionStatusGameObject.GetComponent<TMP_Text>();
        ConnectionStatusTextMeshPro.text = "Connected!";
        ConnectionStatusTextMeshPro.color = Color.green;
        IsConnected = true;
    }
}
