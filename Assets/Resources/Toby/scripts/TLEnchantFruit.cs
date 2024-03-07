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

    public override void OnCollisionEnter2D(Collision2D collision) {
        GameObject other = collision.gameObject;
        Tile otherTile = collision.gameObject.GetComponent<Tile>();
        BasicAICreature creature = collision.gameObject.GetComponent<BasicAICreature>();
        apt283FollowEnemy enemy = collision.gameObject.GetComponent<apt283FollowEnemy>();
        apt283AStarEnemy aStarEnemy = collision.gameObject.GetComponent<apt283AStarEnemy>();
        if (creature != null)
        {
            if (aStarEnemy == null && enemy != null)
            {
                if (other.GetComponentInChildren<TileDetector>() != null)
                {
                    other.GetComponentInChildren<TileDetector>().tagsToDetect = TileTags.Enemy;
                }
            }
            if (creature.GetComponent<HealthMeterTint>() != null)
            {
                creature.GetComponent<HealthMeterTint>().enabled = false;
            }
            creature.sprite.color = newColor;
            if(enemy != null)
            {
                enemy.tileWereChasing = null;
            }
            creature.tagsWeChase = TileTags.Enemy;
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