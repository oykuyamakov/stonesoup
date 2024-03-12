using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example of a basic enemy tile.
// Enemies have the following behavior:
// Every once in a while, they'll try to move to a neighboring empty spot.
// If they are empty handed and find a weapon, they pick up the weapon.
// Every once in a while, they scan for friendly tiles.
// If they find a friendly tile and they're holding a weapon, they aim the weapon at the friendly tile
// and try to use it.
public class TLGhostlyEnemy : BasicAICreature{

	// How much force we inflict if something collides with us.
	public float damageForce = 1000;
	public int damageAmount = 1;
	public GameObject teleportEffect;
	public float teleportEffectDuration = 0.5f;

	// We use counters to determine when to next try to move.
	protected float _nextMoveCounter;
	public float timeBetweenMovesMin = 1.5f;
	public float timeBetweenMovesMax = 3f;

	// Occasionally we'll start with a weapon pre-spawned on top of us. 
	public GameObject[] maybeWeaponsToStartWith;
	public TileText m_Text;

	public override void init() {
		base.init();

		// Here's where we spawn a random weapon for us. 
		if (maybeWeaponsToStartWith != null && maybeWeaponsToStartWith.Length > 0) {
			GameObject maybeWeaponPrefab = GlobalFuncs.randElem(maybeWeaponsToStartWith);
			if (maybeWeaponPrefab != null) {
				Vector2 ourGridPos = toGridCoord(localX, localY);
				Tile.spawnTile(maybeWeaponPrefab, transform.parent, (int)ourGridPos.x, (int)ourGridPos.y);
			}
		}
	}



	public override void Start() {
		_targetGridPos = Tile.toGridCoord(globalX, globalY);
		_nextMoveCounter = Random.Range(timeBetweenMovesMin, timeBetweenMovesMax);
		m_Text.DisplayText("Where is my body?",100,true);
	}

	void Update() {
		// Update our counters.
		if (_nextMoveCounter > 0) {
			_nextMoveCounter -= Time.deltaTime;
		}

		// When it's time to try a new move.
		if (_nextMoveCounter <= 0) {
			takeStep();
		}

		updateSpriteSorting();
	}	

	protected override void takeStep() {
		// Specify the range to examine around the current target position
		int range = 3;

		// Clear the list of neighboring positions
		_neighborPositions.Clear();

		// Iterate over the area of tiles
		for (int x = (int)_targetGridPos.x - range; x <= (int)_targetGridPos.x + range; x++) {
			for (int y = (int)_targetGridPos.y - range; y <= (int)_targetGridPos.y + range; y++) {
				Vector2 gridPos = new Vector2(x, y);

				// Exclude the current position
				if (gridPos != _targetGridPos ){
					_neighborPositions.Add(gridPos);
				}
			}
		}

		// If there's an empty neighbor, choose one randomly.
		if (_neighborPositions.Count > 0) {
			StartCoroutine(TeleportEffect());
			_targetGridPos = GlobalFuncs.randElem(_neighborPositions);
			transform.position = Tile.toWorldCoord(_targetGridPos.x, _targetGridPos.y);
			_nextMoveCounter = Random.Range(timeBetweenMovesMin, timeBetweenMovesMax);
			StartCoroutine(PostTeleportEffect());
		}
	}

	IEnumerator TeleportEffect() {
		// Play teleportation effect here
		Instantiate(teleportEffect, transform.position, Quaternion.identity);
		// You can customize the duration or other aspects of the effect here
		yield return new WaitForSeconds(teleportEffectDuration);
	}

	IEnumerator PostTeleportEffect() {
		// Play additional effect after teleportation here
		// You can customize the duration or other aspects of the effect here
		Instantiate(teleportEffect, transform.position, Quaternion.identity);
		yield return null;  // Add your logic or wait duration here
	}
	



	public override void tileDetected(Tile otherTile) {
		if (otherTile == this) {
			return;
		}
		// If we're holding a weapon and we detect something we'd like to attack, FIRE!
		if (tileWereHolding != null && otherTile.hasTag(tagsWeChase)) {
			aimDirection = ((Vector2)otherTile.transform.position-(Vector2)transform.position).normalized;
			tileWereHolding.useAsItem(this);
		}
		// If we're not holding a weapon and we detect a nearby weapon, PICK IT UP!
		if (tileWereHolding == null && otherTile.hasTag(TileTags.Weapon) && otherTile.hasTag(TileTags.CanBeHeld)) {
			otherTile.pickUp(this);
		}
	}


	// Colliding with somethign we want to attack should hurt it.
	void OnCollisionEnter2D(Collision2D collision) {
		Tile otherTile = collision.gameObject.GetComponent<Tile>();
		if (otherTile != null && otherTile.hasTag(tagsWeChase)) {
			otherTile.takeDamage(this, damageAmount);
			Vector2 toOtherTile = (Vector2)otherTile.transform.position - (Vector2)transform.position;
			toOtherTile.Normalize();
			otherTile.addForce(damageForce*toOtherTile);
		}
	}

	// Check for potential weapons the moment we overlap them (we also poll for them). 
	void OnTriggerEnter2D(Collider2D other) {
		Tile otherTile = other.GetComponent<Tile>();
		if (otherTile != null && tileWereHolding == null && otherTile.hasTag(TileTags.CanBeHeld) && otherTile.hasTag(TileTags.Weapon)) {
			otherTile.pickUp(this);
		}
	}

}
