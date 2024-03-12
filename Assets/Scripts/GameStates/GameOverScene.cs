using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScene : MonoBehaviour {

	public AudioClip clickSound;

	public Text numLevelsCompletedText;

	void Start() {
		numLevelsCompletedText.text = string.Format("Friends Made: {0}", GameManager.friendCount);
	}

	public void tryAgain() {
		AudioManager.playAudio(clickSound);
		GameManager.levelNumber = 0;
		SceneManager.LoadScene("PlayScene");
	}

	public void returnToMenu() {
		AudioManager.playAudio(clickSound);
		GameManager.levelNumber = 0;
		GameManager.friendCount = 0;
		SceneManager.LoadScene("MainMenuScene");
	}
}
