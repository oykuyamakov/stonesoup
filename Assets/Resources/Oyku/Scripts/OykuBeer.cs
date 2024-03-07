using UnityEngine;
public class OykuBeer : Tile
{
    public AudioClip pickupSound, drinkSound;

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
        AudioManager.playAudio(drinkSound);

        AudioManager.playAudio(drinkSound);

        tileUsingUs.restoreAllHealth();

        if (tileUsingUs.TryGetComponent<Player>(out var pl))
        {
            pl.moveSpeed += 2;
        }

        if (tileUsingUs.TryGetComponent<OykuDrunkNpc>(out var mNpc))
        {
            mNpc.moveSpeed += 0.5f;
        }

        die();
    }
}