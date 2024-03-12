using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendCountText : MonoBehaviour{
    protected Text _text;

    void Start(){
        _text = GetComponent<Text>();
    }

    void Update() {
        if (Player.instance != null) {
			_text.text = string.Format("Friends: {0}", GameManager.friendCount);
		}
    }
}
