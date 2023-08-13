using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocationService : MonoBehaviour
{
    // LocationServicesPrimed Key --> (0): In Priming Progress, (1): Priming Success now able to run UpdateGPSData(), (-1): Priming Failed 
    public static int LocationServicesPrimed = 0;

    public IEnumerator GetUserLocationData()
    {
       
        if(!Input.location.isEnabledByUser)
        {
            Debug.Log("App does not have access to Phone's Location Service Data");
            LocationServicesPrimed = -1;
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
            Debug.Log("Location Services Failed to Start after " + WaitForSeconds + " Seconds");
            LocationServicesPrimed = -1;
        }
        else
        {
            LocationServicesPrimed = 1; 
            yield break;
        }
    }

    public List<Double> UpdateGPSData()
    {
        List<double> CoordinatesList = new List<double>(); 

        if(Input.location.status == LocationServiceStatus.Running)
        {
            CoordinatesList.Add((double)Input.location.lastData.latitude);
            CoordinatesList.Add((double)Input.location.lastData.longitude); 
        }
        else
        {
            LocationServicesPrimed = -1; 
            Debug.Log("Location Service Stopped");
            return null;
        }

        return CoordinatesList;
    }
}
