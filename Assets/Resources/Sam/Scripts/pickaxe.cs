using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickaxe : Tile
{
    // Sound effects to play when we're swung or picked up.
    public AudioClip swingSound, pickupSound;

    // We use a pivot object to swing around whatever's holding us
    // (since we can't rotate whatever's holding us).
    // When we're not swinging, the pivot hangs around as our child.
    // When we're swinging, we swap places with the pivot so it becomes our parent.
    public Transform swingPivot;

    // We behave differently when we're swinging vs. when we're not.
    protected bool _swinging = false;
    public float damageForce = 1000;

    // The speed we swing (in degrees/second)
    public float swingSpeed = 300f;
    // The current angle of our swing (we swing 70 degrees before we stop swinging)
    protected float _swingAngle;

    // We use the aim direction to determine where to start our swing, so we need to keep track of the start angle
    // to tell when we've hit 360 degrees.
    protected float _pivotStartAngle;

    bool _swingingLeft;
    float _swingEndAngle;

    Camera cam;
    // Always point toward the cursor

    private void Start()
    {
        cam = Camera.main;
    }

    // We don't take damage if we're swinging or being held by an object.
    public override void takeDamage(Tile tileDamagingUs, int amount, DamageType damageType)
    {
        if (_swinging || _tileHoldingUs != null)
        {
            return;
        }
        base.takeDamage(tileDamagingUs, amount, damageType);
    }

    // Pick up is the same except we play an extra sound.
    public override void pickUp(Tile tilePickingUsUp)
    {
        base.pickUp(tilePickingUsUp);
        if (_tileHoldingUs != null)
        {
            AudioManager.playAudio(pickupSound);
        }
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        // We can't swing if we're already swinging.
        if (_swinging || _tileHoldingUs != tileUsingUs)
        {
            return;
        }

        AudioManager.playAudio(swingSound);

        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < _tileHoldingUs.transform.position.x)
        {
            _swingingLeft = true;
            _swingEndAngle = _pivotStartAngle + 70;
        }
        else
        {
            _swingingLeft = false;
            _swingEndAngle = _pivotStartAngle - 70;
        }

        _swinging = true;
        _swingAngle = _pivotStartAngle;
    }

    // Can't drop us while we're swinging.
    public override void dropped(Tile tileDroppingUs)
    {
        if (_swinging)
        {
            return;
        }
        base.dropped(tileDroppingUs);
    }

    void Update()
    {
        if (_swinging)
        {
            if (_swingingLeft)
            {
                _swingAngle += swingSpeed * Time.deltaTime;
                if (_swingAngle >= _swingEndAngle)
                {
                    _swinging = false;
                }
            }
            else
            {
                _swingAngle -= swingSpeed * Time.deltaTime;
                if (_swingAngle <= _swingEndAngle)
                {
                    _swinging = false;
                }
            }
            /*if (_swingAngle >= 360)
            {
                _swingAngle -= 360;
            }*/

            transform.localRotation = Quaternion.Euler(0, 0, _swingAngle);
            //transform.position = Vector2.Lerp(transform.position, (Vector2)_tileHoldingUs.transform.position + _tileHoldingUs.aimDirection.normalized, .5f);


            //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, _swingAngle), .5f);

            // Use the angle to get the direction mid-swing, so that the hypotenuse is always 1.
            Vector2 swingDirection = new();
            swingDirection.x = Mathf.Cos(Mathf.Deg2Rad * _swingAngle);
            swingDirection.y = Mathf.Sin(Mathf.Deg2Rad * _swingAngle);
            transform.position = Vector2.Lerp(transform.position, (Vector2)_tileHoldingUs.transform.position + swingDirection.normalized, 1f);
            swingPivot.transform.localRotation = Quaternion.Euler(0, 0, _pivotStartAngle + _swingAngle);

        }

        if (_tileHoldingUs != null && !_swinging)
        {
            // We use Atan2 to find the pivot angle given the aim direction.
            _pivotStartAngle = Mathf.Rad2Deg * Mathf.Atan2(_tileHoldingUs.aimDirection.y, _tileHoldingUs.aimDirection.x);
            // We want the aim direction to be the center of the swing arc.
            if (cam.ScreenToWorldPoint(Input.mousePosition).x < _tileHoldingUs.transform.position.x)
            {
                _pivotStartAngle -= 35;
            }
            else
            {
                _pivotStartAngle += 35;
            }
            // By default, we want to hold the pickaxe at the start angle.
            if (!_swinging)
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, _pivotStartAngle), .5f);
                transform.position = Vector2.Lerp(transform.position, (Vector2)_tileHoldingUs.transform.position + _tileHoldingUs.aimDirection.normalized, .5f);
            }
        }
    }

    // Finally, try to deal explosive damage to any walls we hit. 
    void OnTriggerStay2D(Collider2D other)
    {
        if (_swinging && other.gameObject.GetComponent<Wall>() != null)
        {
            Tile otherTile = other.gameObject.GetComponent<Wall>();
            if (otherTile != _tileHoldingUs && !otherTile.hasTag(TileTags.CanBeHeld))
            {
                otherTile.takeDamage(this, 1, DamageType.Explosive);
                otherTile.addForce((other.transform.position - _tileHoldingUs.transform.position).normalized * damageForce);
            }
        }
    }
}
