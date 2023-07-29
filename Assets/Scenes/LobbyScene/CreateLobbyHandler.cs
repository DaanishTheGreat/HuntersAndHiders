using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;

using System.Linq;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;

public class CreateLobbyHandler : MonoBehaviour
{
    // Connection Configuration
    const int MaxConnections = 12;
    public string AccessCodeFromRelay;

    async void AuthenticateHost()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerID;
        }
        catch(Exception e)
        {
            Debug.Log("Failed To Authenticate Player");
        }
        Debug.Log("Host Authentication Made");
    }

    public static async Task<RelayServerData> AllocateRelayServerAndFetchJoinCode(int MaxConnections = 16, string region = null)
    {
        Allocation allocate; 
        string JoinCode;
        try
        {
            allocate = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
        }
        catch(Exception e)
        {
            Debug.Log("Failed To Create Allocation(Maybe Unity Relays is Down?)");
        }

        Debug.Log("Allocation Made");

        try
        {
            createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        return new RelayServerData(allocate, "dtls");
    }
}
