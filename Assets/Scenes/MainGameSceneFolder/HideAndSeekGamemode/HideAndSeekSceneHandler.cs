using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAndSeekSceneHandler : MonoBehaviour
{
    public GameObject MapImageGameObject;
    public GameObject AllRequiredScriptsGameObject;
    public GameObject SpriteImageGameObject;

    //Handler Variables
    private int UserLocationDateIsPrimed = 0;
    private int Event_HasObtainedMapImage = 0;
    private int Event_HasObtainedBoundingBox = 0;
    private int ObtainedMapImageAPICalled = 0;

    private PlayerLocationService_CommonComponent PlayerLocationServiceObject;
    private PlayerSpriteUpdateLocation_CommonComponent PlayerSpriteUpdateLocationObject;

    // Start is called before the first frame update
    void Start()
    {
        PlayerLocationServiceObject = AllRequiredScriptsGameObject.GetComponent<PlayerLocationService_CommonComponent>();
        StartCoroutine(PlayerLocationServiceObject.GetUserLocationData());
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
            UpdatePlayerLocation();
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
}
