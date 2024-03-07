using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuniksYoyo : Tile{
	protected enum Phase{
		Idle,
		Spin,
		Attack,
		Retreat
	};
	
	// We use a pivot object to swing around whatever's holding us
	// (since we can't rotate whatever's holding us).
	// When we're not swinging, the pivot hangs around as our child.
	// When we're swinging, we swap places with the pivot so it becomes our parent.
	public Transform swingPivot;

	// We behave differently when we're swinging vs. when we're not.
	protected Phase phase = Phase.Idle;
	public float damageForce = 500;

	// Spin variables
	public float spinReleaseTimer = .25f;
	protected float currentSpinTimer = 0;

	public float maxSpinSpeed = 3000f;
	public float spinAcceleration = 200f;
	protected float currentSpinSpeed = 0f;

	protected float _swingAngle;

	// Attack variables
	public float yoyoLength = 7.5f;
	public float maxYoyoSpeed = 20f;
	public float minYoyoSpeed = 5f;
	public float maxAttackTimer = 2f;
	protected float currentAttackTimer = 0f;

	// Retreat variables
	public float retreatSpeed = 20f;


	// We use the aim direction to determine where to start our swing, so we need to keep track of the start angle
	// to tell when we've hit 360 degrees.
	protected float _pivotStartAngle;


	// We don't take damage if we're spinning or being held by an object.
	public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType) {
		if (phase != Phase.Idle || _tileHoldingUs != null) {
			return;
		}
		base.takeDamage(tileDamagingUs, amount, damageType);
	}


	public override void useAsItem(Tile tileUsingUs) {
		if (phase == Phase.Attack || phase == Phase.Retreat || _tileHoldingUs != tileUsingUs) {
			return;
		}

		if(phase != Phase.Spin){
			phase = Phase.Spin;
			currentSpinSpeed = 0;

			// We use Atan2 to find the pivot angle given the aim direciton.
			_pivotStartAngle = Mathf.Rad2Deg*Mathf.Atan2(tileUsingUs.aimDirection.y, tileUsingUs.aimDirection.x);

			// Here's where we pull the switcheroo where we become the child of our pivot.
			swingPivot.transform.parent = tileUsingUs.transform;
			swingPivot.transform.localPosition = Vector3.zero;
			swingPivot.transform.localRotation = Quaternion.Euler(0, 0, _pivotStartAngle);
			transform.parent = swingPivot;
			
		}

		phase = Phase.Spin;
		currentSpinTimer = 0;
		currentSpinSpeed = Mathf.Clamp(currentSpinSpeed + spinAcceleration, 0, maxSpinSpeed);
	}

	// Can't drop us while we're spinning or attacking.
	public override void dropped(Tile tileDroppingUs) {
		if (phase != Phase.Idle) {
			return;
		}
		base.dropped(tileDroppingUs);
	}

	void Update() {
		if (phase == Phase.Spin){
			currentSpinTimer += Time.deltaTime;
			_swingAngle += currentSpinSpeed*Time.deltaTime;
			swingPivot.transform.localRotation = Quaternion.Euler(0, 0, _pivotStartAngle+_swingAngle);
			
			if(currentSpinTimer > spinReleaseTimer){
				transform.parent = _tileHoldingUs.transform;
				transform.localPosition = new Vector3(heldOffset.x, heldOffset.y, -0.1f);
				transform.localRotation = Quaternion.Euler(0, 0, heldAngle);
				swingPivot.transform.parent = transform;
				currentSpinTimer = 0;
				phase = Phase.Attack;
			}
		}

		if(phase == Phase.Attack){
			Vector2 target = _tileHoldingUs.transform.position;
			target += _tileHoldingUs.aimDirection.normalized * yoyoLength;

			transform.position = Vector2.MoveTowards(transform.position, target, GetYoyoSpeed() * Time.deltaTime);

			currentAttackTimer += Time.deltaTime;
			if(currentAttackTimer >= maxAttackTimer){
				currentAttackTimer = 0;
				phase = Phase.Retreat;
			}
		}

		if(phase == Phase.Retreat){
			transform.localPosition = Vector2.MoveTowards(transform.localPosition, heldOffset, retreatSpeed * Time.deltaTime);
			if((Vector2)transform.localPosition == heldOffset){
				phase = Phase.Idle;
			}
		}
	}

	// Finally, try to hurt any tile we hit while we're swinging. 
	void OnTriggerEnter2D(Collider2D other) {
		if ((phase == Phase.Attack || phase == Phase.Retreat) && other.gameObject.GetComponent<Tile>() != null) {
			Tile otherTile = other.gameObject.GetComponent<Tile>();
			if (otherTile != _tileHoldingUs && !otherTile.hasTag(TileTags.CanBeHeld)) {
				otherTile.takeDamage(this, GetDamage());
				otherTile.addForce((other.transform.position-_tileHoldingUs.transform.position).normalized*damageForce);
			}
		}
	}

	protected float GetYoyoSpeed(){
		float ratio = currentSpinSpeed/maxSpinSpeed;
		return Mathf.Clamp(ratio * maxYoyoSpeed, minYoyoSpeed, maxYoyoSpeed);
	}

	protected int GetDamage(){
		float slice = maxSpinSpeed/3f;
		if(currentSpinSpeed < slice) return 1;
		if(currentSpinSpeed < 2 * slice) return 2;
		return 3;
	}
}
