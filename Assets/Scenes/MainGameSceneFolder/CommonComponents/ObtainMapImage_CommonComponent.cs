using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ObtainMapImage_CommonComponent : MonoBehaviour
{
   // ObtainedMapImage Key --> (0): Obtaining Image in progress, (1): Successfully Obtained Image, (-1): Failed to Obtain Image
   public static int ObtainedMapImage = 0;

    public GameObject MapImageGameObject;

    private string BingMapsApiUrlPart1 = "https://dev.virtualearth.net/REST/v1/Imagery/Map/Road/";
    //In between here add Latitude and Longitude Coordinates
    private string BingMapsAPIUrlPart2 = "/15?&mapSize=2000,2000&key=";
    //SECRET KEY *****************************************************************************************************************
    private string BingMapsAPIUrlPart3Key = "AqD8HayAQ00gtXGOd6w9ZDycXXDWz_MbAPxRnYmW1JpLMsstsWIHpgN8dCqZxyN0"; //SECRET KEY

/*
    public ObtainMapImage(GameObject GetMapImageGameObject)
    {
        MapImageGameObject = GetMapImageGameObject;
    }
*/
    //This is an IEnumerator function so make sure to call StartCoroutine(DownloadMapImage(x, y));
    public IEnumerator DownloadMapImage(double LatitudeCoordinate, double LongitudeCoordinate)
    {
       string BingMapsApiUrl = BingMapsApiUrlPart1 + LatitudeCoordinate + "," + LongitudeCoordinate + BingMapsAPIUrlPart2 + BingMapsAPIUrlPart3Key;

       UnityWebRequest GetImageRequest = UnityWebRequestTexture.GetTexture(BingMapsApiUrl);
       yield return GetImageRequest.SendWebRequest();
       
       if(UnityWebRequest.Result.ConnectionError.Equals(true))
       {
          Debug.Log("Error getting Image: " + GetImageRequest.error);

          ObtainedMapImage = -1;
       }
       else
       {
         Image MapImageUiElement = MapImageGameObject.GetComponent<Image>();
         MapImageUiElement.sprite = Sprite.Create(((DownloadHandlerTexture) GetImageRequest.downloadHandler).texture, new Rect(0, 0, 1500, 1500), new Vector2(0, 0));
         MapImageUiElement.rectTransform.sizeDelta = new Vector2(2500, 2500);

         ObtainedMapImage = 1;
        
         yield break;
       }
    }
    
}
