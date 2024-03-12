using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FriendEndingScene : MonoBehaviour {

	public AudioClip clickSound;

	public Text endingText;
    public int goodEndingFriendCount = 5;
    public int currentEnding = 0;

	void Start() {
        if(GameManager.friendCount >= goodEndingFriendCount){
            endingText.text = string.Format("Congratulations, you made {0} friends! These bonds give you strength, confidence and a sense of fulfillment. You are part of a community. You give and you take. Together with your friends you lead a happy and edifying life outside of this dungeon.", GameManager.friendCount);
        } else if (GameManager.friendCount > 0){
            endingText.text = string.Format("You only made {0} friends. Although you escaped the dungeon, the few connections you made through life weren't as long-lasting as you thought they would be. You die, old and lonely, regretting being the person you turned out to be.", GameManager.friendCount);
        } else {
            endingText.text = "You made 0 friends. Simply pathetic. Were you even trying? Your antisocial behavior is a burden on humanity, and the world would be a better place if you had never escaped the dungeon.";
        }
	}

	public void playAnotherLevel() {
		AudioManager.playAudio(clickSound);
		SceneManager.LoadScene("PlayScene");
	}

	public void returnToMenu() {
		AudioManager.playAudio(clickSound);
		GameManager.levelNumber = 1;
		GameManager.friendCount = 0;
		SceneManager.LoadScene("MainMenuScene");
	}
}
