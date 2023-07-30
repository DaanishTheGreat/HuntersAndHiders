using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using System;

public class ButtonActions : MonoBehaviour
{
	public void OnClickCreateGameMoveToMainGame()
	{
		SceneManager.LoadScene("CreateGameLobby");
	}
	
	public void OnClickJoinGameScene()
	{
		SceneManager.LoadScene("JoinGameScene");
	}
}
