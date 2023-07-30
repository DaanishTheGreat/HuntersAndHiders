using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstanceScript : MonoBehaviour
{
    
    public string PlayerName = "Empty";

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerName(string name)
    {
        PlayerName = name;
    }

}
