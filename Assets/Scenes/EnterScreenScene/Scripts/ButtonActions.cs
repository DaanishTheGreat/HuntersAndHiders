using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using System;

public class ButtonActions : MonoBehaviour
{
   public Button CreateGame;
   //hellox2 - tahir

	void Start () {
        CreateGame.GetComponent<Button>().onClick.AddListener(CreateGameMoveToMainGame);
	}

	async void Awake()
	{
		try
		{
			await UnityServices.InitializeAsync();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}

	void CreateGameMoveToMainGame()
	{
		Debug.Log("Create Game Button Pressed");
		SceneManager.LoadScene("CreateGameLobby");
	}
}
