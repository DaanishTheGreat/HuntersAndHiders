using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;

public class CreateLobbyHandler : MonoBehaviour
{
    // Connection Configuration
    const int MaxConnections = 16;
    public string AccessCodeFromRelay;

    public void GetAccessCode()
    {

    }

    public static async Task<RelayServerData> AllocateRelayServerAndFetchJoinCode(int MaxConnections = 16, region)
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
    }
}
