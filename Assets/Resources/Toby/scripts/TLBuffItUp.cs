using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLBuffItUp : Tile
{
    public GameObject[] itemsToSpawn;
    public GameObject muzzleFlashObj;
    public float recoilForce = 100;
    public float shootForce = 1000f;
    public float buffDuration = 5f; // Duration of the buff in seconds

    protected float _cooldownTimer;

    // Store original color, size, and tag
    protected Color originalColor;
    protected Vector3 originalSize;
    protected TileTags originalTag;

    protected void aim()
    {
        _sprite.transform.localPosition = new Vector3(1f, 0, 0);
        float aimAngle = Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, aimAngle);
        if (_tileHoldingUs.aimDirection.x < 0)
        {
            _sprite.flipY = true;
            muzzleFlashObj.transform.localPosition = new Vector3(muzzleFlashObj.transform.localPosition.x, -Mathf.Abs(muzzleFlashObj.transform.localPosition.y), muzzleFlashObj.transform.localPosition.z);
        }
        else
        {
            _sprite.flipY = false;
            muzzleFlashObj.transform.localPosition = new Vector3(muzzleFlashObj.transform.localPosition.x, Mathf.Abs(muzzleFlashObj.transform.localPosition.y), muzzleFlashObj.transform.localPosition.z);
        }
    }

    protected virtual void Update()
    {
        if (_cooldownTimer > 0)
        {
            _cooldownTimer -= Time.deltaTime;
        }

        if (_tileHoldingUs != null)
        {
            // If we're held, rotate and aim the gun.
            aim();
        }
        else
        {
            // Otherwise, move the gun back to the normal position. 
            _sprite.transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
        updateSpriteSorting();
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        if (_cooldownTimer > 0)
        {
            return;
        }

        // First, make sure we're aimed properly (to avoid shooting ourselves by accident)
        aim();

        // Store original color, size, and tag
        originalColor = tileUsingUs.sprite.color;
        originalSize = tileUsingUs.transform.localScale;
        originalTag = tileUsingUs.tags;

        tileUsingUs.sprite.color = new Color(0.5f, 0f, 0f, 1f);
        tileUsingUs.transform.localScale *= 1.2f;
        tileUsingUs.tags = TileTags.Wall;

        // Add the TLBuffedUp component
        TLBuffedUp buffedUpComponent = tileUsingUs.gameObject.AddComponent<TLBuffedUp>();

        // Start the coroutine to remove the buff after a duration
        tileUsingUs.StartCoroutine(RemoveBuffAfterDuration(tileUsingUs, buffedUpComponent));

        Destroy(this.gameObject);
    }

    private IEnumerator RemoveBuffAfterDuration(Tile tileUsingUs, TLBuffedUp buffedUpComponent)
    {
        yield return new WaitForSeconds(buffDuration);

        // Restore original color, size, and tag
        tileUsingUs.sprite.color = originalColor;
        tileUsingUs.transform.localScale = originalSize;
        tileUsingUs.tags = originalTag;

        // Remove the TLBuffedUp component after the buff duration
        if (buffedUpComponent != null)
        {
            Destroy(buffedUpComponent);
        }
    }

    public void deactivateFlash()
    {
        muzzleFlashObj.SetActive(false);
    }
}
