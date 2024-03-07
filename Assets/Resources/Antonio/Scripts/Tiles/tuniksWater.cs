using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuniksWater : Tile {
	public float onGroundThreshold = 1f;
	public float force = 1f;

	protected Vector2 dir = Vector2.zero;
	protected float _destroyTimer = 0.5f;
	protected ContactPoint2D[] _contacts = null;

	void Start() {
		_contacts = new ContactPoint2D[10];
		if (GetComponent<TrailRenderer>() != null) {
			GetComponent<TrailRenderer>().Clear();
		}
	}

	void Update() {
		// If we're moving kinda slow now we can just delete ourselves.
		if (_body.velocity.magnitude <= onGroundThreshold) {
			_destroyTimer -= Time.deltaTime;
			if (_destroyTimer <= 0) {
				die();
			}
		}
	}

	public virtual void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.GetComponent<Tile>() != null) {
			Tile otherTile = collision.gameObject.GetComponent<Tile>();
			otherTile.addForce(dir * force);
			dir = Vector2.zero;
		}
	}

	public void SetDirection(Vector2 _dir){
		dir = _dir;
	}
}
