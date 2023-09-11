using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HideAndSeekHandlerClient : MonoBehaviour
{
    public GameObject MapImageGameObject;
    public GameObject AllRequiredScriptsGameObject;
    public GameObject SpriteImageGameObject;
    public GameObject SpriteImageGameObjectPrefab;


    private int UserLocationDateIsPrimed = 0;
    private int Event_HasObtainedMapImage = 0;
    private int Event_HasObtainedBoundingBox = 0;
    private int ObtainedMapImageAPICalled = 0;

    private int GetAndUpdatePlayerLocationsCoroutineCalled = 0;

    private PlayerLocationService_CommonComponent PlayerLocationServiceObject;
    private PlayerSpriteUpdateLocation_CommonComponent PlayerSpriteUpdateLocationObject;
    
    void Start()
    {
        PlayerLocationServiceObject = AllRequiredScriptsGameObject.GetComponent<PlayerLocationService_CommonComponent>();
        StartCoroutine(PlayerLocationServiceObject.GetUserLocationData());

        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectedHandler;
    }

    // Update is called once per frame
    void Update()
    {
        UserLocationDateIsPrimed = PlayerLocationService_CommonComponent.LocationServicesPrimed;
        Event_HasObtainedBoundingBox = PlayerSpriteUpdateLocation_CommonComponent.ObtainedJsonString;

        if(UserLocationDateIsPrimed == 1 && Event_HasObtainedMapImage == 0)
        {
            List<double> PlayerLocationCoordinates = PlayerLocationServiceObject.UpdateGPSData();
            if(ObtainedMapImageAPICalled == 0)
            {
                ObtainMapImage(PlayerLocationCoordinates[0], PlayerLocationCoordinates[1]);
                ObtainedMapImageAPICalled = 1;
            }
            Event_HasObtainedMapImage = 1;
        }
        else if(UserLocationDateIsPrimed == 1 && Event_HasObtainedMapImage == 1 && Event_HasObtainedBoundingBox == 0)
        {
            ObtainBoundingBox();
        }
        else if(UserLocationDateIsPrimed == 1 && Event_HasObtainedMapImage == 1 && Event_HasObtainedBoundingBox == 1)
        {
            //UpdatePlayerLocation();

            if(GetAndUpdatePlayerLocationsCoroutineCalled == 0)
            {
                StartCoroutine(SendAndRecievePlayerData());
                GetAndUpdatePlayerLocationsCoroutineCalled = 1;
            }
        }
    }

    private void ObtainMapImage(double Latitude, double Longitude)
    {
        ObtainMapImage_CommonComponent ObtainMapImageObject = AllRequiredScriptsGameObject.GetComponent<ObtainMapImage_CommonComponent>();
        StartCoroutine(ObtainMapImageObject.DownloadMapImage(Latitude, Longitude));
    }

    private void ObtainBoundingBox()
    {
        PlayerSpriteUpdateLocationObject = AllRequiredScriptsGameObject.GetComponent<PlayerSpriteUpdateLocation_CommonComponent>();
        List<double> PlayerLocationCoordinates = PlayerLocationServiceObject.UpdateGPSData(); 
        StartCoroutine(PlayerSpriteUpdateLocationObject.UpdateLocationOnMap(PlayerLocationCoordinates[0], PlayerLocationCoordinates[1]));
    }

    private void UpdatePlayerLocation()
    {
        List<double> PlayerLocationCoordinates = PlayerLocationServiceObject.UpdateGPSData();
        List<double> Normalized_PlayerLocationCoordinates = PlayerSpriteUpdateLocationObject.ScaleLocationUsingLerp(PlayerLocationCoordinates[0], PlayerLocationCoordinates[1]);

        SpriteOnMap_CommonComponent InstanceOfOneSpriteOnMapPlayer = SpriteImageGameObject.GetComponent<SpriteOnMap_CommonComponent>();

        //SpriteOnMap Already Instantiated, if multiple are instantiated then adapt this to include multiple
        InstanceOfOneSpriteOnMapPlayer.UpdateSpriteLocationInGame(Normalized_PlayerLocationCoordinates[0], Normalized_PlayerLocationCoordinates[1]);
    }

    // RPC functions beyond this point

    private List<PlayerClientData> UpdatePlayerLocationsFromServer()
    {
        GameObject PlayerHostGameObject = GetHostPlayer();
        PlayerInstanceScript PlayerHostInstanceScript = PlayerHostGameObject.GetComponent<PlayerInstanceScript>();
        PlayerHostInstanceScript.RequestAllPlayerCoordinatesServerRpc();

        return PlayerInstanceScript.PlayerClientDataList;
    }

    private void SendPlayerLocationToServer()
    {
        GameObject[] PlayerClientPrefabList = GameObject.FindGameObjectsWithTag("PlayerClientPrefab");
        GameObject PlayerClientOwnedByClientGameObject = null;

        foreach(GameObject PlayerClientLocal in PlayerClientPrefabList)
        {
            NetworkObject PlayerClientNetworkObject = PlayerClientLocal.GetComponent<NetworkObject>();
            if(PlayerClientNetworkObject.IsOwner == true)
            {
                PlayerClientOwnedByClientGameObject = PlayerClientLocal;
            }
        }

        PlayerInstanceScript PlayerLocalPlayerInstanceScript = PlayerClientOwnedByClientGameObject.GetComponent<PlayerInstanceScript>();
        PlayerLocalPlayerInstanceScript.InstancePlayerName = JoinGameSceneHandler.PlayerName;

        List<double> PlayerLocationCoordinates = PlayerLocationServiceObject.UpdateGPSData();
        List<double> Normalized_PlayerLocationCoordinates = PlayerSpriteUpdateLocationObject.ScaleLocationUsingLerp(PlayerLocationCoordinates[0], PlayerLocationCoordinates[1]);


        PlayerLocalPlayerInstanceScript.SendPlayerLocationsToServerRpc(PlayerLocalPlayerInstanceScript.InstancePlayerName, Normalized_PlayerLocationCoordinates[0], Normalized_PlayerLocationCoordinates[1]);
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

    private IEnumerator SendAndRecievePlayerData()
    {
        int HasInitializedPlayerClientsOnMap = 0;
        // Updates PlayerClient Locations on Map Every 3 Seconds
        while(true)
        {
            SendPlayerLocationToServer();
            yield return new WaitForSeconds(1f);
            UpdatePlayerLocationsFromServer();
            yield return new WaitForSeconds(1f);
            if(HasInitializedPlayerClientsOnMap == 0)
            {
                InitializePlayerClientsOnMap();
                HasInitializedPlayerClientsOnMap = 1;
            }
            yield return new WaitForSeconds(1f);
            DisplayPlayerClientsOnMap();

        }
    }

    // InitializeAndDisplayPlayerClientsOnMap Start Here

    private void InitializePlayerClientsOnMap()
    {
        foreach(PlayerClientData PlayerClientDataInstance in PlayerInstanceScript.PlayerClientDataList)
        {
            GameObject InstantiatedSpriteImageGameObject = Instantiate(SpriteImageGameObjectPrefab);
            InstantiatedSpriteImageGameObject.transform.SetParent(MapImageGameObject.transform);
        }
        
        /* Look at HideAndSeekSceneHandlerHost script for details as to why this isnt needed

        GameObject[] ListOfInstantiatedPlayerClientPrefab = GameObject.FindGameObjectsWithTag("PlayerClientPrefab");
        
        foreach(GameObject InstantiatedPlayerClient in ListOfInstantiatedPlayerClientPrefab)
        {
            InstantiatedPlayerClient.AddComponent<SpriteOnMap_CommonComponent>();
        }

        */
    }

    private void DisplayPlayerClientsOnMap()
    {
        GameObject[] ListOfInstantiatedPlayerClientPrefab = GameObject.FindGameObjectsWithTag("SpriteImagePrefab");

        int Index = 0;
        foreach(PlayerClientData PlayerClientDataInstance in PlayerInstanceScript.PlayerClientDataList)
        {
            List<double> NormalizedCoordinates = PlayerClientDataInstance.GetCoordinates();

            GameObject PlayerClientPrefabInstance = ListOfInstantiatedPlayerClientPrefab[Index];

            SpriteOnMap_CommonComponent PlayerClientSpriteOnMapComponent = PlayerClientPrefabInstance.GetComponent<SpriteOnMap_CommonComponent>();
            PlayerClientSpriteOnMapComponent.UpdateSpriteLocationInGame(NormalizedCoordinates[0], NormalizedCoordinates[1]);

            Index++;
        }
    }

    // InitializeAndDisplayPlayerClientsOnMap End Here

    // Client Reconnection Handler

    public void ClientDisconnectedHandler(ulong ClientId)
    {
        NetworkManager NetworkManagerObject = GetComponent<NetworkManager>();

        if(NetworkManagerObject.ShutdownInProgress)
        {
            return;
        }

        StartCoroutine(Reconnect());
    }

    public IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(3);

        NetworkManager NetworkManagerObject = GetComponent<NetworkManager>();
        if(NetworkManagerObject.IsConnectedClient)
        {
            StopCoroutine("Reconnect");
        }

        NetworkManagerObject.StartClient();


    }

    // End of Client Recconection Manager
}
