using System.Collections;
using UnityEngine;

public class TLCryingAngel : Tile
{
    public float damageThreshold = 0f;
    private bool isScriptDisabled = false;
    public EdgeCollider2D EdgeCollider;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Tile>() != null)
        {
            Tile otherTile = other.gameObject.GetComponent<Tile>();
            otherTile.takeDamage(this, 1);
            
            StartCoroutine(DisableScriptForSeconds(otherTile, 2f));
        }
    }

    private IEnumerator DisableScriptForSeconds(Tile tile, float seconds)
    {
        if (tile != null)
        {
            EdgeCollider.enabled = true;
            SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.black;

            yield return new WaitForSeconds(seconds);

            // Enable the script after the delay
            EdgeCollider.enabled = false;
            spriteRenderer.color = Color.white;

            // Perform any other actions to re-enable the script
            // For example, you might want to enable specific behaviors or components
        }
    }
}