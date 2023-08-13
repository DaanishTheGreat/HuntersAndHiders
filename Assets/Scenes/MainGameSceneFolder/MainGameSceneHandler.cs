using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainGameSceneHandler : MonoBehaviour
{
    public GameObject MapImageGameObject; 
    public Sprite SpriteImage; 

    // Start is called before the first frame update
    void Start()
    {
        ObtainImageFromPlayerLocation();
        //https://dev.virtualearth.net/REST/v1/Imagery/Map/Road/38.9688721098223,%20-77.51960143650247/15?&mapSize=2000,2000&key=AqD8HayAQ00gtXGOd6w9ZDycXXDWz_MbAPxRnYmW1JpLMsstsWIHpgN8dCqZxyN0
        // ^^ API with center coordinates, zoom set to 15, and map size set to 2000x2000
        StartCoroutine(DownloadImageFromAPI("https://dev.virtualearth.net/REST/v1/Imagery/Map/Road/38.9688721098223,%20-77.51960143650247/15?&mapSize=2000,2000&key=AqD8HayAQ00gtXGOd6w9ZDycXXDWz_MbAPxRnYmW1JpLMsstsWIHpgN8dCqZxyN0"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ObtainImageFromPlayerLocation()
    {
        RawImage MapImageUiElement = MapImageGameObject.GetComponent<RawImage>();
        MapImageUiElement.texture = SpriteImage.texture;
    }

    IEnumerator DownloadImageFromAPI(string ImageUrl)
    {
       UnityWebRequest GetImageRequest = UnityWebRequestTexture.GetTexture(ImageUrl);
       yield return GetImageRequest.SendWebRequest();
       
       if(UnityWebRequest.Result.ConnectionError.Equals(true))
       {
          Debug.Log("Error getting Image: " + GetImageRequest.error);
       }
       else
       {
         RawImage MapImageUiElement = MapImageGameObject.GetComponent<RawImage>();
         MapImageUiElement.texture = ((DownloadHandlerTexture) GetImageRequest.downloadHandler).texture;
       }
    }
}


/*
Components to Implement: 

PlayerLocation Services(and Handling Exceptions): PlayerLocation.cs

DownloadMapImage Function family: Found in MainGameSceneHandler.cs with methods ObtainImageFromPlayerLocation() --> DownloadImageFromAPI()

Display and Google Mapsify Images: MapTestSceneHandler.cs 
*/