using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerClientData : INetworkSerializable
{
    private double Latitude_Normalized;
    private double Longitude_Normalized;
    private string PlayerClientName;

    // PlayerInstanceScript Server keeps track of clients by the list's sorting order determined in lobby, therefore SortingProperty must be followed
    // in the list to ensure proper PlayerClientData list syncing
    private int SortingProperty;
    
    // Hide and Seek Game Specific Variables
    private int IsSeeker = 0; // if IsSeeker is 0 then Player is Hider

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Latitude_Normalized);
        serializer.SerializeValue(ref Longitude_Normalized);
        serializer.SerializeValue(ref PlayerClientName);
        serializer.SerializeValue(ref SortingProperty);

        serializer.SerializeValue(ref IsSeeker);
    }

    public void InitializePlayerClientData(string PlayerClientNameInput, double LatitudeInput, double LongitudeInput, int CurrentSortingProperty)
    {
        PlayerClientName = PlayerClientNameInput;
        Latitude_Normalized = LatitudeInput;
        Longitude_Normalized = LongitudeInput;
        SortingProperty = CurrentSortingProperty;
    }

    public List<double> GetCoordinates()
    {
        List<double> CoordinateList = new List<double>
        {
            Latitude_Normalized,
            Longitude_Normalized
        };

        return CoordinateList;
    }

    public void UpdateCoordinates(double LatitudeInput, double LongitudeInput)
    {
        Latitude_Normalized = LatitudeInput;
        Longitude_Normalized = LongitudeInput;
    }

    public string GetPlayerClientName()
    {
        return PlayerClientName;
    }

    public int GetSortingProperty()
    {
        return SortingProperty;
    }

    public int GetSeekerStatus_HideAndSeek()
    {
        return IsSeeker;
    }


    // _HideAndSeek methods are game specific to [Hide and Seek]
    public void SetSeekerStatus_HideAndSeek(bool IsSeeker_Input)
    {
        if(IsSeeker_Input == true)
        {
            IsSeeker = 1;
        }
        else
        {
            IsSeeker = 0;
        }
    }
}
