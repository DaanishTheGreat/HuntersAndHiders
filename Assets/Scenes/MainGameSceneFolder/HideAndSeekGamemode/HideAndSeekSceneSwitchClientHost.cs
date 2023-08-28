using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAndSeekSceneSwitchClientHost : MonoBehaviour
{
    public GameObject CanvasGameObject;

    // Start is called before the first frame update
    void Start()
    {
        if(JoinGameSceneHandler.JoinGameSceneCalled == 0)
        {
            CanvasGameObject.GetComponent<HideAndSeekSceneHandlerHost>().enabled = true;
            CanvasGameObject.GetComponent<HideAndSeekSceneSwitchClientHost>().enabled = false;
        }
        else
        {
            CanvasGameObject.GetComponent<HideAndSeekHandlerClient>().enabled = true;
            CanvasGameObject.GetComponent<HideAndSeekSceneSwitchClientHost>().enabled = false;
        }
    }
}
