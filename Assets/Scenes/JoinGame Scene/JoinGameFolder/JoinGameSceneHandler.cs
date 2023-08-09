using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinGameSceneHandler : MonoBehaviour //MonoBehaviour
{
    public static string JoinCode = "empty";
    public static string PlayerName = "empty";
    //public GameObject PlayerClientPrefab;
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
    }

    public void UpdatePlayerName(string name)
    {
        PlayerName = name;
    }

    public void JoinGameOnClick()
    {
        SceneManager.LoadScene("ClientConnectedScene");

    }
}
