using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuniksOuija : apt283Gun{
    public float ghostLifetime = 1f;
    public float ghostSpeed = 1f;
    private float distractionTimer = 0f;
    private bool distraction = false;


    protected override void Update(){
        base.Update();

        distractionTimer+=Time.deltaTime;
        if(distraction && distractionTimer>ghostLifetime){
            EndDistraction();
        }
    }

    public override void useAsItem(Tile tileUsingUs){
        if (_cooldownTimer > 0) {
			return;
		}

		// First, make sure we're aimed properly (to avoid shooting ourselves by accident)
		aim();


		// Check to see if the muzzle is overlapping anything. 
		int numBlockers = Physics2D.OverlapPointNonAlloc(muzzleFlashObj.transform.position, _maybeColliderResults);
		for (int i = 0; i < numBlockers && i < _maybeColliderResults.Length; i++) {
			if (!_maybeColliderResults[i].isTrigger && _maybeColliderResults[i] != mainCollider) {
				ObjShake maybeSpriteShake = _sprite.GetComponent<ObjShake>();
				if (maybeSpriteShake != null) {
					maybeSpriteShake.shake();
				}

				return;
			}
		}

		// Let's spawn the bullet. The bullet will probably need to be a child of the room. 
		GameObject newBullet = Instantiate(bulletPrefab);
		newBullet.transform.parent = tileUsingUs.transform.parent;
		newBullet.transform.position = muzzleFlashObj.transform.position;
		// newBullet.transform.rotation = transform.rotation;

		newBullet.GetComponent<tuniksGhost>().init();
        newBullet.GetComponent<tuniksGhost>().Initialize(ghostLifetime, ghostSpeed, tileUsingUs.aimDirection.normalized);
		newBullet.GetComponent<tuniksGhost>().addForce(tileUsingUs.aimDirection.normalized*shootForce);

		_cooldownTimer = cooldownTime;
        StartDistraction();
    }

    protected void StartDistraction(){
        distraction = true;
        distractionTimer = 0;
        _tileHoldingUs.removeTag(TileTags.Player);
        _tileHoldingUs.removeTag(TileTags.Friendly);
        _tileHoldingUs.removeTag(TileTags.Creature);
    }

    protected void EndDistraction(){
        distraction = false;
        _tileHoldingUs.addTag(TileTags.Player);
        _tileHoldingUs.addTag(TileTags.Friendly);
        _tileHoldingUs.addTag(TileTags.Creature);
    }
}
