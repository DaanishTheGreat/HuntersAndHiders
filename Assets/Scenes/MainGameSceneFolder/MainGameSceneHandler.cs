using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainGameSceneHandler : MonoBehaviour
{
    public GameObject LocationServiceStatusTextGameObject;
    public GameObject LatitudeTextGameObject;
    public GameObject LongitudeTextGameObject;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetUserLocationData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetUserLocationData()
    {
        TMP_Text LocationServiceStatusTextMeshPro = LocationServiceStatusTextGameObject.GetComponent<TMP_Text>();
        if(!Input.location.isEnabledByUser)
        {
            LocationServiceStatusTextMeshPro.text = "Location Services Status Access Not Given";
            LocationServiceStatusTextMeshPro.color = Color.red;
            Debug.Log("App does not have access to Phone's Location Service Data");
            yield break;
        }
        Input.location.Start();
        
        int WaitForSeconds = 10;
        while(Input.location.status == LocationServiceStatus.Initializing && WaitForSeconds > 0)
        {
            yield return new WaitForSeconds(1);
            WaitForSeconds--;
        }
        if(WaitForSeconds < 1 || Input.location.status == LocationServiceStatus.Failed)
        {
            LocationServiceStatusTextMeshPro.text = "Location Services Status: Failed to start after " + WaitForSeconds + " seconds";
            LocationServiceStatusTextMeshPro.color = Color.red;
            Debug.Log("Location Services Failed to Start after " + WaitForSeconds + " Seconds");
        }
        else
        {
            //Location Services Running Fine
            LocationServiceStatusTextMeshPro.text = "Location Service Status: Running"; 
            LocationServiceStatusTextMeshPro.color = Color.green;
            InvokeRepeating("UpdateGPSData", 0.5f, 1f);
        }
        //Not Finished Watch Tutorial: https://www.youtube.com/watch?v=JWccDbm69Cg&ab_channel=Code3Interactive
    }

    private void UpdateGPSData()
    {
        if(Input.location.status == LocationServiceStatus.Running)
        {
            TMP_Text LatitudeTextMeshPro = LatitudeTextGameObject.GetComponent<TMP_Text>();
            TMP_Text LongitudeTextMeshPro = LongitudeTextGameObject.GetComponent<TMP_Text>();

            LatitudeTextMeshPro.text = "Latitude: " + Input.location.lastData.latitude.ToString();
            LongitudeTextMeshPro.text = "Longitude: " + Input.location.lastData.longitude.ToString();

            LatitudeTextMeshPro.color = Color.green;
            LongitudeTextMeshPro.color = Color.green;
        }
        else
        {
            TMP_Text LocationServiceStatusTextMeshPro = LocationServiceStatusTextGameObject.GetComponent<TMP_Text>();
            LocationServiceStatusTextMeshPro.text = "Location Service Stopped";
            LocationServiceStatusTextMeshPro.color = Color.red;
            Debug.Log("Location Service Stopped");
        }
    }
}
// New Idea: Stay in place hide and seek gamemode