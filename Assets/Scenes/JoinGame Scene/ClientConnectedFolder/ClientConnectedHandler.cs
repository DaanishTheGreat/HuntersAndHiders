using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
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
        var InitializeUnityServicesTask = UnityServices.InitializeAsync();
        ClientInstructionsGameObject.SetActive(false); 
        ConnectedPlayersGameObject.SetActive(false);
        JoinRelay();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void Update()
    {

        if(IsConnected == true)
        {
        string PlayerNames = "";
        foreach(string PlayerName in PlayerInstanceScript.PlayerNames)
        {
            PlayerNames = PlayerNames + " " + "[" + PlayerName + "]";
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

        GameObject PlayerHostTwo = GetHostPlayer();
        PlayerInstanceScript PlayerHostInstanceScriptTwo = PlayerHostTwo.GetComponent<PlayerInstanceScript>();
        PlayerHostInstanceScriptTwo.RequestPlayerNamesToServerRpc();
        
        ClientInstructionsGameObject.SetActive(true);
        ConnectedPlayersGameObject.SetActive(true);

        TMP_Text ConnectionStatusTextMeshPro = ConnectionStatusGameObject.GetComponent<TMP_Text>();
        ConnectionStatusTextMeshPro.text = "Connected!";
        ConnectionStatusTextMeshPro.color = Color.green;
        IsConnected = true;
    }

    private async void JoinRelay()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            JoinAllocation JoinAllocationObject = await RelayService.Instance.JoinAllocationAsync(JoinGameSceneHandler.JoinCode);

            RelayServerData RelayServerDataObject = new RelayServerData(JoinAllocationObject, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(RelayServerDataObject);
            NetworkManager.Singleton.StartClient();
        }
        catch(RelayServiceException e)
        {
            Debug.Log("Unable to Connect to Relay, maybe Invalid JoinCode? Error: " + e);
        }
    }
}
