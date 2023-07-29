using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinGameSceneHandler : MonoBehaviour
{
    private string JoinCode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateJoinCode(string Join_Code)
    {
        JoinCode = Join_Code;
        Debug.Log(JoinCode);
    }

    public void JoinGameOnClick()
    {
        Debug.Log("Joining Game");
    }
}
