using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinGameSceneHandler : MonoBehaviour //MonoBehaviour
{
    public static string JoinCode = "empty";
    public static string PlayerName = "empty";

    public static int JoinGameSceneCalled = 0;


    //public GameObject PlayerClientPrefab;
    void Start()
    {
        JoinGameSceneCalled = 1;
    }

    void Update()
    {

    }

    public void UpdateJoinCode(string Join_Code)
    {
        JoinCode = Join_Code;
    }

    public void UpdatePlayerName(string name)
    {
        PlayerName = name;
    }

    public void JoinGameOnClick()
    {
        InitializeNetworking();
    }

    private async void InitializeNetworking()
    {
        // NOTE: Load scene: "LoadingScene" to improve UI and fluidness
        await UnityServices.InitializeAsync();
        JoinRelay();
    }

    private async void JoinRelay()
    {
        Debug.Log("Join Code: " + JoinCode);
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            JoinAllocation JoinAllocationObject = await RelayService.Instance.JoinAllocationAsync(JoinCode);

            RelayServerData RelayServerDataObject = new RelayServerData(JoinAllocationObject, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(RelayServerDataObject);
            NetworkManager.Singleton.StartClient();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedClient;
        }
        catch (RelayServiceException e)
        {
            Debug.Log("Unable to Connect to Relay, maybe Invalid JoinCode? Error: " + e);
        }
    }

    public void OnClientConnectedClient(ulong ClientId)
    {
        GameObject PlayerHost = GetHostPlayer();
        PlayerInstanceScript PlayerHostInstanceScript = PlayerHost.GetComponent<PlayerInstanceScript>();
        PlayerHostInstanceScript.SendPlayerNameToServerRpc(PlayerName);

        PlayerInstanceScript PlayerHostInstanceScriptTwo = PlayerHost.GetComponent<PlayerInstanceScript>();
        PlayerHostInstanceScriptTwo.RequestPlayerNamesToServerRpc();
    }

    private GameObject GetHostPlayer()
    {
        GameObject PlayerHost = null;
        GameObject[] PlayerClients = GameObject.FindGameObjectsWithTag("PlayerClientPrefab");

        foreach (GameObject val in PlayerClients)
        {
            NetworkObject IsOwnerOrNot = val.GetComponent<NetworkObject>();
            if (IsOwnerOrNot.IsOwner)
            {
                PlayerHost = val;
            }
        }
        return PlayerHost;
    }
}
