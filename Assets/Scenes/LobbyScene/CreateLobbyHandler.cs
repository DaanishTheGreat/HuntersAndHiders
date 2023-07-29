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
            var playerID = AuthenticationService.Instance.PlayerId;
        }
        catch(Exception e)
        {
            Debug.Log("Failed To Authenticate Player, Exception: " + e.Message);
        }
        Debug.Log("Host Authentication Made");
    }

    public static async Task<RelayServerData> AllocateRelayServerAndFetchJoinCode(int MaxConnections = 16, string region = null)
    {
        Allocation allocate = null; 
        string JoinCode;
        try
        {
            allocate = await RelayService.Instance.CreateAllocationAsync(MaxConnections, region);
        }
        catch(Exception e)
        {
            Debug.Log("Failed To Create Allocation(Maybe Unity Relays is Down?) Exception: " + e.Message);
        }

        Debug.Log("Allocation Made");

        try
        {
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocate.AllocationId);
        }
        catch(Exception e)
        {
            Debug.LogError("Relay create join code request failed, Exception: " + e.Message);
            throw;
        }

        return new RelayServerData(allocate, "dtls");
    }
}
