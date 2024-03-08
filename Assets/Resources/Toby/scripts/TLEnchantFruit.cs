using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TLEnchantFruit : apt283Rock {
    

    protected apt283PulseEffect _pulseEffect;
    public float normalPulsePeriod = 1f;
    public float heldPulsePeriod = 0.5f;
    public bool startsInAir = false;

    public Color newColor = new Color(0.5f, 0, 0.5f, 1f); // Set the desired color in the Inspector
    private Color originalColor;
    private TileTags originalTagsWeChase;
    
    
    
    void Start()
    {
        _pulseEffect = GetComponentInChildren<apt283PulseEffect>();
        if (startsInAir)
        {
            _isInAir = true;
            _afterThrowCounter = afterThrowTime;
        }
    }

    public override void useAsItem(Tile tileUsingUs) {
		if (_tileHoldingUs != tileUsingUs) {
			return;
		}
		if (onTransitionArea()) {
			return; // Don't allow us to be thrown while we're on a transition area.
		}
		AudioManager.playAudio(throwSound);

		_sprite.transform.localPosition = Vector3.zero;

		_tileThatThrewUs = tileUsingUs;
		_isInAir = true;

		// We use IgnoreCollision to turn off collisions with the tile that just threw us.
		if (_tileThatThrewUs.GetComponent<Collider2D>() != null) {
			Physics2D.IgnoreCollision(_tileThatThrewUs.GetComponent<Collider2D>(), _collider, true);
		}

		// We're thrown in the aim direction specified by the object throwing us.
		Vector2 throwDir = _tileThatThrewUs.aimDirection.normalized;

		// Have to do some book keeping similar to when we're dropped.
		_body.bodyType = RigidbodyType2D.Dynamic;
		transform.parent = tileUsingUs.transform.parent;
		_tileHoldingUs.tileWereHolding = null;
		_tileHoldingUs = null;

		_collider.isTrigger = false;

		// Since we're thrown so fast, we switch to continuous collision detection to avoid tunnelling
		// through walls.
		_body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

		// Finally, here's where we get the throw force.
		_body.AddForce(throwDir*throwForce);

		_afterThrowCounter = afterThrowTime;
	}
    public override void OnCollisionEnter2D(Collision2D collision) {
        GameObject other = collision.gameObject;
        Tile otherTile = collision.gameObject.GetComponent<Tile>();
        BasicAICreature creature = collision.gameObject.GetComponent<BasicAICreature>();
        apt283FollowEnemy enemy = collision.gameObject.GetComponent<apt283FollowEnemy>();
        apt283AStarEnemy aStarEnemy = collision.gameObject.GetComponent<apt283AStarEnemy>();
        if (creature != null)
        {
                /*if (aStarEnemy == null && enemy != null)
                {
                    if (other.GetComponentInChildren<TileDetector>() != null)
                    {
                        other.GetComponentInChildren<TileDetector>().tagsToDetect = TileTags.Water;
                    }

                }*/
                if (creature.GetComponent<HealthMeterTint>() != null)
                {
                    creature.GetComponent<HealthMeterTint>().enabled = false;
                }
                creature.sprite.color = newColor;
                if(enemy != null)
                {
                    enemy.tileWereChasing = null;
                    enemy.moveSpeed = 2;
                    enemy.damageAmount = 0;
                    enemy.damageForce = 0;
                }
                //creature.tagsWeChase = TileTags.Water;
                creature.tags = TileTags.Friend;
            Debug.Log(creature.tagsWeChase);
        }
        if (collision.relativeVelocity.magnitude > damageThreshold) {
            base.die();
        }
    }


    protected override void Update() {
        base.Update();
        if (_pulseEffect != null) {
            if (_tileHoldingUs != null) {
                _pulseEffect.pulsePeriod = heldPulsePeriod;		
            }
            else {
                _pulseEffect.pulsePeriod = normalPulsePeriod;
            }
        }
    }
    
   

    

}