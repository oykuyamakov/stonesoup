using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLBuffedUp : Tile
{
    public float damageThreshold = 1f;

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>() != null)
        {
            float impact = collisionImpactLevel(collision);
            if (impact < damageThreshold)
            {
                return;
            }

            Tile otherTile = collision.gameObject.GetComponent<Tile>();
            otherTile.takeDamage(this, 1);
        }
    }
}
