using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateGameLobbyHandler : MonoBehaviour
{
    public GameObject ProvideNameErrorText;
    public static string PlayerName_InputField = "";
    public static int NumberOfPlayers = 0; 

    //Gamemode Key: Hide and Seek(0), Hot and Cold(1), Capture the Flag(2), [Zombies(3), Hot and Cold(4)]
    public static int PlayerChosenGameMode = 0;

    void Start()
    {
        ProvideNameErrorText.SetActive(false); 
    }

    public void UpdateNumberOfPlayers(string NumberOfPlayersString)
    {
        try
        {
            NumberOfPlayers = Convert.ToInt32(NumberOfPlayersString);
        }
        catch
        {
            //Need NumberOfPlayerInputfield specific error, this is just a placeholder to show not valid input value
            //Need Int
            ProvideNameErrorText.SetActive(true);
        }
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
