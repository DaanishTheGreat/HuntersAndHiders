using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;
using System.Security.Cryptography.X509Certificates;

public class MapCenter
{
    public List<string> coordinates { get; set; }
}

public class Resource
{
    public string __type { get; set; }
    public List<double> bbox { get; set; }
    public string imageHeight { get; set; }
    public string imageWidth { get; set; }
    public MapCenter mapCenter { get; set; }
    public List<object> pushpins { get; set; }
    public string zoom { get; set; }
}

public class ResourceSet
{
    public int estimatedTotal { get; set; }
    public List<Resource> resources { get; set; }
}

public class Root
{
    public string authenticationResultCode { get; set; }
    public string brandLogoUri { get; set; }
    public string copyright { get; set; }
    public List<ResourceSet> resourceSets { get; set; }
    public int statusCode { get; set; }
    public string statusDescription { get; set; }
    public string traceId { get; set; }
}


public class PlayerSpriteUpdateLocation : MonoBehaviour
{
    public GameObject ImageMapGameObject;
    //public GameObject PlayerSpriteGameObject; 

    //Only valid when ObtainedJsonString variable is 1
    public static List<double> BoundingBox;

    //// ObtainedJsonString Key --> (0): Obtaining Json String in progress, (1): Successfully Obtained Json String, (-1): Failed to Obtain Json String
    public static int ObtainedJsonString = 0;

    private string BingMapsApiUrlPart1 = "https://dev.virtualearth.net/REST/v1/Imagery/Map/Road/";
    //In between here add Latitude and Longitude Coordinates
    private string BingMapsAPIUrlPart2 = "/15?&mapSize=2000,2000&mapMetadata=1&o=json&key=";
    //SECRET KEY *****************************************************************************************************************
    private string BingMapsAPIUrlPart3Key = "AqD8HayAQ00gtXGOd6w9ZDycXXDWz_MbAPxRnYmW1JpLMsstsWIHpgN8dCqZxyN0"; //SECRET KEY

    public IEnumerator UpdateLocationOnMap(double Latitude, double Longitude)
    {
        string BingMapsApiUrl = BingMapsApiUrlPart1 + Latitude + "," + Longitude + BingMapsAPIUrlPart2 + BingMapsAPIUrlPart3Key;
        
        UnityWebRequest WorldWideWeb = UnityWebRequest.Get(BingMapsApiUrl);
        yield return WorldWideWeb.SendWebRequest();

        if(WorldWideWeb.result != UnityWebRequest.Result.Success)
        {
            ObtainedJsonString = -1;
            Debug.Log("Failed to Obtain Json String, Error: " + WorldWideWeb.error);
            yield break;
        }
        else
        {
            string JsonString = WorldWideWeb.downloadHandler.text;
            
            Root JsonMetaDataObject = JsonConvert.DeserializeObject<Root>(JsonString);
            List<double> CoordinateBounds = new List<double>();

            foreach (var resourceSet in JsonMetaDataObject.resourceSets)
            {

                foreach (var resource in resourceSet.resources)
                {
                    foreach(double Coordinate in resource.bbox)
                    {
                        CoordinateBounds.Add(Coordinate);
                    }
                }
            }

            BoundingBox = CoordinateBounds;

            ObtainedJsonString = 1; 
            yield break;
        }
    }

    public List<double> NormalizationOfLocationCoordinates(double XCoordinate, double YCoordinate)
    {
        if(ObtainedJsonString != 1)
        {
            Debug.Log("ObtainedJsonString was not called or was not successful, Normalization function cannot run");
            return null;
        }

        UnityEngine.UI.Image ImageMap = ImageMapGameObject.GetComponent<UnityEngine.UI.Image>();
        List<double> NormalizedCoordinates = new List<double>();


        double SouthLatitude = BoundingBox[0];
        double WestLongitude = BoundingBox[1];

        double NorthLatitude = BoundingBox[2];
        double EastLongitude = BoundingBox[3];


        double XLatitudeRange = Math.Abs(SouthLatitude - NorthLatitude);
        double YLongitudeRange = Math.Abs(WestLongitude - EastLongitude); 

        //Hardcoded Range Targets, Change to height and 
        double XRangeTarget = ImageMap.rectTransform.sizeDelta.x;
        double YRangeTarget = ImageMap.rectTransform.sizeDelta.y;

        double XNormalized = (XCoordinate - SouthLatitude) / XLatitudeRange;
        double YNormalized = (YCoordinate - WestLongitude) / YLongitudeRange;

        //1250 hardcoded Change it to dynamic variable
        double FinalXCoordinate = (XNormalized * XRangeTarget) - 1250;
        double FinalYCoordinate = (XNormalized * XRangeTarget) - 1250;

        NormalizedCoordinates.Add(FinalXCoordinate);
        NormalizedCoordinates.Add(FinalYCoordinate);

        return NormalizedCoordinates;
    }
}