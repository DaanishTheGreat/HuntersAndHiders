using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Host only class
public class GameMechanicsHideAndSeek : MonoBehaviour
{

    //Change this for hide and seek trigger distance (Exclusive)
    private int MinimumDistanceForFlagTriggerInPixels = 5;

    public List<PlayerClientData> CheckDistanceOfPlayerClientsAndFlagAsSeekerIfCaught(List<PlayerClientData> PlayerClientList)
    {
        List<PlayerClientData> Seekers = new List<PlayerClientData>();
        List<PlayerClientData> Hiders = new List<PlayerClientData>();

        foreach(PlayerClientData PlayerClient_Sort in PlayerClientList)
        {
            if(PlayerClient_Sort.GetSeekerStatus_HideAndSeek() == 1)
            {
                Seekers.Add(PlayerClient_Sort);
            }
            else
            {
                Hiders.Add(PlayerClient_Sort);
            }
        }

        foreach(PlayerClientData PlayerClientSeeker in Seekers)
        {
            List<double> PlayerClientSeeker_NormalizedCoordinates = PlayerClientSeeker.GetCoordinates();

            foreach(PlayerClientData PlayerClientHider in Hiders)
            {
                List<double> PlayerClientHider_NormalizedCoordinates = PlayerClientHider.GetCoordinates();
                
                double DistanceInPixels = Math.Sqrt(Math.Pow(PlayerClientSeeker_NormalizedCoordinates[0] - PlayerClientHider_NormalizedCoordinates[0], 2) + Math.Pow(PlayerClientSeeker_NormalizedCoordinates[1] - PlayerClientHider_NormalizedCoordinates[1], 2)); 

                if(DistanceInPixels < MinimumDistanceForFlagTriggerInPixels)
                {
                    PlayerClientHider.SetSeekerStatus_HideAndSeek(true); // Make sure to Rpc to server to update list
                }
            }
        }

        List<PlayerClientData> SortedPlayerClientData = new List<PlayerClientData>();
        
        foreach(PlayerClientData SeekerPlayerClientData in Seekers)
        {
            SortedPlayerClientData.Add(SeekerPlayerClientData);
        }
        foreach(PlayerClientData HiderPlayerClientData in Hiders)
        {
            SortedPlayerClientData.Add(HiderPlayerClientData);
        }

        SortedPlayerClientData = SortedPlayerClientData.OrderBy(x => x.GetSortingProperty()).ToList();

        // Delete Later, for Debugging, specifically to ensure proper sorting of PlayerClientData Objects in list
        foreach(PlayerClientData PlayerClientData_Object in SortedPlayerClientData)
        {
            Debug.Log(PlayerClientData_Object.GetSortingProperty());
        }

        return SortedPlayerClientData;

    }

}

/*
    Game Mechanics: 
    
    if Game Mechanics is called then do this: check distance of each player in playerclient list, if any playerclient distance is
    less than x meters from another playerclient, check if one of the playerclient is a seeker, if they are, then call another function
    to convert that hider playerclient to seeker or stop his game.

*/