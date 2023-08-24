using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Notes: NumberOfPlayerInputField change to drop down or scroll dont leave as input box

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
            //ANS:  Try doing-> catch (FormatException) -> the use of this is when the format of an argument is invalid or when a composite format string is not well formed
            //  https://learn.microsoft.com/en-us/dotnet/api/system.formatexception?view=net-7.0
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
        /*We should have a rule where the length of the characters should be 2 characters to 8 characters
        if(PlayerName_InputField.Equals("") || PlayerName_InputField.Length < 3 || PlayerName_InputField > 8)
        */

        {
            ProvideNameErrorText.SetActive(true);
        }
        else
        {
            SceneManager.LoadSceneAsync("CreateLobbyScene");
        }
    }
}
