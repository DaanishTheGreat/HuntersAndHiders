using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerClientData : INetworkSerializable
{
    private double Latitude_Normalized;
    private double Longitude_Normalized;
    private string PlayerClientName;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Latitude_Normalized);
        serializer.SerializeValue(ref Longitude_Normalized);
        serializer.SerializeValue(ref PlayerClientName);
    }

    public void InitializePlayerClientData(string PlayerClientNameInput, double LatitudeInput, double LongitudeInput)
    {
        PlayerClientName = PlayerClientNameInput;
        Latitude_Normalized = LatitudeInput;
        Longitude_Normalized = LongitudeInput;
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
}
