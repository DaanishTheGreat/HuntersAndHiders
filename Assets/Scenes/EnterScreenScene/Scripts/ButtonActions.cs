using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonActions : MonoBehaviour
{
   public Button CreateGame;
   //hellox2 - tahir

	void Start () {
        CreateGame.GetComponent<Button>().onClick.AddListener(CreateGameMoveToMainGame);
	}

	void CreateGameMoveToMainGame()
	{
		Debug.Log("Create Game Button Pressed");
		SceneManager.LoadScene("CreateGameLobby");
	}
}
