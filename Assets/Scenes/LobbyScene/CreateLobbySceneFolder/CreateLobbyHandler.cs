using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class CreateLobbyHandler : MonoBehaviour
{
    //Start UGS
    public GameObject PlayerPrefab;
    public GameObject ServerNetworkManager;

    public GameObject NetworkManagerPrefab;

    private bool IsConnected = false;
    private bool PlayerNameUpdatedToServer = false;
    void Start()
    {
        SetSceneForClientOrHostInstance();

        GameObject PlayerTextListGameObject = GameObject.Find("Players");
        TextMeshProUGUI PlayerTextList = PlayerTextListGameObject.GetComponent<TextMeshProUGUI>();
        PlayerTextList.text = CreateGameLobbyHandler.PlayerName_InputField;
    }

    void Update()
    {
        if (IsConnected == true)
        {
            GameObject PlayerTextListGameObject = GameObject.Find("Players");
            TextMeshProUGUI PlayerTextList = PlayerTextListGameObject.GetComponent<TextMeshProUGUI>();

            string PlayerListToString = "";
            foreach (string val in PlayerInstanceScript.PlayerNames)
            {
                PlayerListToString = PlayerListToString + " " + "[" + val + "]";
            }
            PlayerTextList.text = PlayerListToString;
        }
    }

    public void StartGameButton()
    {
        // Hide and Seek gamemode
        if (CreateGameLobbyHandler.PlayerChosenGameMode == 0)
        {
            /*
            GameObject[] ListOfPlayerClientObjects = GameObject.FindGameObjectsWithTag("PlayerClientPrefab");
            StartCoroutine(LoadSceneAndTransferGameObjects("HideAndSeekScene", ServerNetworkManager, ListOfPlayerClientObjects)); 
            */

            GameObject PlayerHost = GetHostPlayer();
            PlayerInstanceScript PlayerHostInstanceScript = PlayerHost.GetComponent<PlayerInstanceScript>();
            PlayerHostInstanceScript.ChangeHostSceneServerRpc("HideAndSeekScene");
        }
        // Hot and Cold
        else if (CreateGameLobbyHandler.PlayerChosenGameMode == 1)
        {
            // Not Created
        }
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

    public void OnClientConnectedHost(ulong ClientId)
    {
        if (PlayerNameUpdatedToServer == false)
        {
            GameObject PlayerHost = GetHostPlayer();
            PlayerInstanceScript PlayerHostInstanceScript = PlayerHost.GetComponent<PlayerInstanceScript>();
            PlayerHostInstanceScript.SendPlayerNameToServerRpc(CreateGameLobbyHandler.PlayerName_InputField);
            PlayerNameUpdatedToServer = true;
        }

        GameObject PlayerHostTwo = GetHostPlayer();
        PlayerInstanceScript PlayerHostInstanceScriptTwo = PlayerHostTwo.GetComponent<PlayerInstanceScript>();
        PlayerHostInstanceScriptTwo.RequestPlayerNamesToServerRpc(); 

        IsConnected = true;
    }

    public void OnClientConnectedClient(ulong ClientId)
    {
        GameObject PlayerHostTwo = GetHostPlayer();
        PlayerInstanceScript PlayerHostInstanceScriptTwo = PlayerHostTwo.GetComponent<PlayerInstanceScript>();
        PlayerHostInstanceScriptTwo.RequestPlayerNamesToServerRpc();
    }

    private async void CreateRelay()
    {
        try
        {
            Allocation RelayAllocation = await RelayService.Instance.CreateAllocationAsync(CreateGameLobbyHandler.NumberOfPlayers - 1);
            string JoinCode = await RelayService.Instance.GetJoinCodeAsync(RelayAllocation.AllocationId);

            GameObject JoinCodeTextGameObject = GameObject.Find("JoinCode");
            TMP_Text JoinCodeTextMeshPro = JoinCodeTextGameObject.GetComponent<TMP_Text>();
            JoinCodeTextMeshPro.text = JoinCode;
            JoinCodeTextMeshPro.color = Color.green;

            RelayServerData RelayServerDataObject = new RelayServerData(RelayAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(RelayServerDataObject);
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log("Failed to Create Allocation, Error: " + e);
        }
    }

    private async void SetSceneForClientOrHostInstance()
    {
        if (JoinGameSceneHandler.JoinGameSceneCalled == 1)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedClient;

            GameObject JoinCodeTextGameObject = GameObject.Find("JoinCode");
            TMP_Text JoinCodeTextMeshPro = JoinCodeTextGameObject.GetComponent<TMP_Text>();
            JoinCodeTextMeshPro.text = JoinGameSceneHandler.JoinCode;
            JoinCodeTextMeshPro.color = Color.green;

            GameObject StartGameButtonGameObject = GameObject.Find("StartGameButton");
            StartGameButtonGameObject.SetActive(false);

            IsConnected = true;
            PlayerNameUpdatedToServer = true;
        }
        else
        {
            Instantiate(NetworkManagerPrefab);

            try
            {
                await UnityServices.InitializeAsync();
                AuthenticationService.Instance.SignedIn += () =>
                {
                    Debug.Log("Signed In: " + AuthenticationService.Instance.PlayerId);
                };
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                CreateRelay();
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedHost;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Some Error");
            }
        }
    }
}
