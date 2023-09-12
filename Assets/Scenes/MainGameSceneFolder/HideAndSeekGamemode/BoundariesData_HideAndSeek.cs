using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundariesData_HideAndSeek : MonoBehaviour
{
    private double CenterLatitude_Normalized;
    private double CenterLongitude_Normalized;
    private double RadiusSize;

    private string BoundaryName;

    public void InitializeBoundariesData(double LatitudeCenterNormalized_Input, double LongitudeCenterNormalized_Input, double RadiusSize_Input, string BoundaryName_Input)
    {
        CenterLatitude_Normalized = LatitudeCenterNormalized_Input;
        CenterLongitude_Normalized = LongitudeCenterNormalized_Input;
        RadiusSize = RadiusSize_Input;
        BoundaryName = BoundaryName_Input;
    }
}
