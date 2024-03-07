using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuniksKorby : Tile {
	public Sprite suckingSprite;
    public Sprite idleSprite; 

	public GameObject tileDetectorObj;

    public float suckForce = 10f;

	public float suckTimer = 2f; 
    public float idleTimer = 1f;
    protected bool isSucking = false;
    protected float currentTimer = 0;

    // CHOMPING HIPPO?

	void Update() {
		if (isSucking) {
			currentTimer += Time.deltaTime;
            if(currentTimer > suckTimer){
                currentTimer = 0;
                isSucking = false;
                _sprite.sprite = idleSprite;
            }
		} else {
            currentTimer += Time.deltaTime;
            if(currentTimer > idleTimer){
                currentTimer = 0;
                isSucking = true;
                _sprite.sprite = suckingSprite;
            }
        }
	}

	public override void tileDetected(Tile otherTile) {
		if (!isSucking) {
			return;
		}

        Vector2 direction = (transform.position - otherTile.transform.position).normalized;
        otherTile.addForce(direction * suckForce);
	}	
}
