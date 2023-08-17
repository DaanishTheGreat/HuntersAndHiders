using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameBaseSceneHandler : MonoBehaviour
{
    public GameObject MapImageGameObject;
    public GameObject AllRequiredScriptsGameObject;

    //Handler Variables
    private int UserLocationDateIsPrimed = 0;
    private int Event_HasObtainedMapImage = 0;
    private int Event_HasObtainedBoundingBox = 0;

    private PlayerLocationService PlayerLocationServiceObject;
    private PlayerSpriteUpdateLocation PlayerSpriteUpdateLocationObject;

    // Start is called before the first frame update
    void Start()
    {
        PlayerLocationServiceObject = AllRequiredScriptsGameObject.GetComponent<PlayerLocationService>();
        StartCoroutine(PlayerLocationServiceObject.GetUserLocationData());
    }

    // Update is called once per frame
    void Update()
    {
        UserLocationDateIsPrimed = PlayerLocationService.LocationServicesPrimed;
        Event_HasObtainedBoundingBox = PlayerSpriteUpdateLocation.ObtainedJsonString;
        if(UserLocationDateIsPrimed == 1 && Event_HasObtainedMapImage == 0)
        {
            List<double> PlayerLocationCoordinates = PlayerLocationServiceObject.UpdateGPSData();
            ObtainMapImage(PlayerLocationCoordinates[0], PlayerLocationCoordinates[1]);
            Event_HasObtainedMapImage = 1;
        }
        else if(UserLocationDateIsPrimed == 1 && Event_HasObtainedMapImage == 1 && Event_HasObtainedBoundingBox == 0)
        {
            ObtainBoundingBox();
        }
        else if(UserLocationDateIsPrimed == 1 && Event_HasObtainedMapImage == 1 && Event_HasObtainedBoundingBox == 1)
        {
            UpdatePlayerLocation();
        }
    }

    private void ObtainMapImage(double Latitude, double Longitude)
    {
        ObtainMapImage ObtainMapImageObject = AllRequiredScriptsGameObject.GetComponent<ObtainMapImage>();
        StartCoroutine(ObtainMapImageObject.DownloadMapImage(Latitude, Longitude));
    }

    private void ObtainBoundingBox()
    {
        PlayerSpriteUpdateLocationObject = AllRequiredScriptsGameObject.GetComponent<PlayerSpriteUpdateLocation>();
        List<double> PlayerLocationCoordinates = PlayerLocationServiceObject.UpdateGPSData(); 
        StartCoroutine(PlayerSpriteUpdateLocationObject.UpdateLocationOnMap(PlayerLocationCoordinates[0], PlayerLocationCoordinates[1]));
    }

    private void UpdatePlayerLocation()
    {
        List<double> PlayerLocationCoordinates = PlayerLocationServiceObject.UpdateGPSData();
        List<double> Normalized_PlayerLocationCoordinates = PlayerSpriteUpdateLocationObject.ScaleLocationUsingLerp(PlayerLocationCoordinates[0], PlayerLocationCoordinates[1]);

        SpriteOnMap InstanceOfOneSpriteOnMapPlayer = GameObject.Find("SpriteImage").GetComponent<SpriteOnMap>();

        //SpriteOnMap Already Instantiated, if multiple are instantiated then adapt this to include multiple
        InstanceOfOneSpriteOnMapPlayer.UpdateSpriteLocationInGame(Normalized_PlayerLocationCoordinates[0], Normalized_PlayerLocationCoordinates[1]);
    }
}