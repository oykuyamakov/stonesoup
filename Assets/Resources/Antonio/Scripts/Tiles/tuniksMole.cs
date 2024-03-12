using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuniksMole : BasicAICreature{
	// We use counters to determine when to next try to move.
	protected float _nextMoveCounter;
	public float timeBetweenMovesMin = .15f;
	public float timeBetweenMovesMax = .4f;

    public int stepsBelow = 16;
    public int stepsAbove = 4;
    private int currentStep = 0;

	// Occasionally we'll start with a weapon pre-spawned on top of us. 
	public GameObject[] possibleLoot;

	private TileText tt;

	public override void Start() {
		_targetGridPos = Tile.toGridCoord(globalX, globalY);
		_nextMoveCounter = Random.Range(timeBetweenMovesMin, timeBetweenMovesMax);
		tt = GetComponent<TileText>();
	}

	void Update() {
		// Update our counters.
		if (_nextMoveCounter > 0) {
			_nextMoveCounter -= Time.deltaTime;
		}

		// When it's time to try a new move.
		if (_nextMoveCounter <= 0) {
			ResetTimer();
            if(currentStep < 2){
                PopUp();
            } else {
                DigDown();
                takeStep();
            }
            currentStep = (currentStep+1)%(stepsAbove+stepsBelow);
		}

		updateSpriteSorting();
	}	

	protected override void takeStep() {
		// Try to move to one of our neighboring positions if it is empty.
		_neighborPositions.Clear();

		// We test neighbor locations by casting in specific directions. 

        _collider.enabled = true;
		Vector2 upGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y+1);
		if (pathIsClear(toWorldCoord(upGridNeighbor))) {
			_neighborPositions.Add(upGridNeighbor);
		}
		Vector2 rightGridNeighbor = new Vector2(_targetGridPos.x+1, _targetGridPos.y);
		if (pathIsClear(toWorldCoord(rightGridNeighbor))) {
			_neighborPositions.Add(rightGridNeighbor);
		}
		Vector2 downGridNeighbor = new Vector2(_targetGridPos.x, _targetGridPos.y-1);
		if (pathIsClear(toWorldCoord(downGridNeighbor))) {
			_neighborPositions.Add(downGridNeighbor);
		}
		Vector2 leftGridNeighbor = new Vector2(_targetGridPos.x-1, _targetGridPos.y);
		if (pathIsClear(toWorldCoord(leftGridNeighbor))) {
			_neighborPositions.Add(leftGridNeighbor);
		}
        _collider.enabled = false;

		// If there's an empty neighbor, choose one randomly.
		if (_neighborPositions.Count > 0) {
			_targetGridPos = GlobalFuncs.randElem(_neighborPositions);
		}
	}

    private void ResetTimer(){
        _nextMoveCounter = Random.Range(timeBetweenMovesMin, timeBetweenMovesMax);
    }

    private void PopUp(){
        _sprite.enabled = true;
        _collider.enabled = true;
		tt.DisplayText("You can't catch me!", .4f);
    }

    private void DigDown(){
        _sprite.enabled = false;
        _collider.enabled = false;
    }

    protected override void die(){
        if (possibleLoot != null && possibleLoot.Length > 0) {
			GameObject maybeWeaponPrefab = GlobalFuncs.randElem(possibleLoot);
			if (maybeWeaponPrefab != null) {
				Vector2 ourGridPos = toGridCoord(localX, localY);
				Tile.spawnTile(maybeWeaponPrefab, transform.parent, (int)ourGridPos.x, (int)ourGridPos.y);
			}
		}

        base.die();
    }
}
