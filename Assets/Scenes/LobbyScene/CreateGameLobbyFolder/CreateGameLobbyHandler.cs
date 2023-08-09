using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateGameLobbyHandler : MonoBehaviour
{
    public GameObject ProvideNameErrorText;
    public static string PlayerName_InputField = "";

    //Gamemode Key: Hide and Seek(0), Hot and Cold(1), Capture the Flag(2)
    private int PlayerChosenGameMode = 0;

    void Start()
    {
        ProvideNameErrorText.SetActive(false); 
    }

    public void UpdatePlayerName(string PlayerName)
    {
        PlayerName_InputField = PlayerName;
    }

    public void UpdateGameMode(int GameMode)
    {
        PlayerChosenGameMode = GameMode;
    }

    public void CreateGameOnClick()
    {
        if(PlayerName_InputField.Equals(""))
        {
            ProvideNameErrorText.SetActive(true);
        }
        else
        {
            SceneManager.LoadSceneAsync("CreateLobbyScene");
        }
    }
}
