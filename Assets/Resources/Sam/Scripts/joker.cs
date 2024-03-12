using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class joker : Tile
{
    private Text _weaponName;
    private Tile _newItem;
    private bool _coroutineRunning = false;
    private Tile _handler;
    private SpriteRenderer _sr;
    
    // Start is called before the first frame update
    void Start()
    {
        _weaponName = transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        _sr = GetComponent<SpriteRenderer>();
        _weaponName.enabled = false;
    }

    private void Update()
    {
        if (_tileHoldingUs != null)
        {
            transform.position = Vector2.Lerp(
                transform.position,
                new Vector2(_tileHoldingUs.transform.position.x + Mathf.Sin(3 * Time.time), _tileHoldingUs.transform.position.y + Mathf.Cos(3 * Time.time)),
                .5f);
        }
    }

    public override void useAsItem(Tile tileUsingUs)
    {
        if (!_coroutineRunning)
        {
            _handler = _tileHoldingUs;
            playCard();
        }
    }

    void playCard()
    {
        string[] contributorFolder = ContributorList.instance.activeContributorIDs;
        List<GameObject> possibleItem = new();

        for (int i = 0; i < contributorFolder.Length; i++)
        {
            //Debug.Log(contributorFolder[i] + "/Prefabs");
            Object[] prefabs = Resources.LoadAll(contributorFolder[i] + "/Prefabs", typeof(GameObject));
            foreach (GameObject gameObject in prefabs)
            {
                if (gameObject != null)
                {
                    Tile tileType = gameObject.GetComponent<Tile>();
                    if (tileType != null && tileType.hasTag(TileTags.CanBeHeld) && tileType.tileName != "joker" && !tileType.hasTag(TileTags.Consumable))
                    {
                        possibleItem.Add(gameObject);
                    }
                }
            }
        }
        
        int choice = Random.Range(0, possibleItem.Count);
        
        // show text then fade out
        _weaponName.text = possibleItem[choice].GetComponent<Tile>().tileName;
        _weaponName.color = new Color(1, .917f, .667f, 1);
        _weaponName.transform.localPosition = new Vector2(0, 2);
        StartCoroutine(textRiseAndFade());
        
        Vector2 ourGridPos = toGridCoord(_tileHoldingUs.localX, _tileHoldingUs.localY);
        _newItem = spawnTile(possibleItem[choice], _tileHoldingUs.transform.parent, (int)ourGridPos.x, (int)ourGridPos.y);
        //_newItem = spawnTile(possibleItem[choice], _tileHoldingUs.transform, 0, 0);
        //_newItem.useAsItem(_tileHoldingUs);
        dropped(_handler);
        _sr.enabled = false;
        _newItem.pickUp(_handler);
        _newItem.useAsItem(_handler);
        StartCoroutine(killWeapon());

    }
    
    IEnumerator textRiseAndFade()
    {
        Color c = _weaponName.color;
        Vector2 myPos = _weaponName.transform.localPosition;
        for (float alpha = 1f; alpha >= -.1f; alpha -= 0.03f)
        {
            c.a = alpha;
            myPos.y += .1f;
            _weaponName.color = c;
            _weaponName.transform.localPosition = myPos;
            yield return null;
        }
    }

    IEnumerator killWeapon()
    {
        _coroutineRunning = true;
        bool x = false;
        if (!x)
        {
            x = true;
            yield return new WaitForSeconds(1);
        }
        _newItem.dropped(_handler);
        _newItem.takeDamage(_handler, 1);
        _newItem.takeDamage(_handler, 1, DamageType.Explosive);
        pickUp(_handler);
        _sr.enabled = true;
        _coroutineRunning = false;
        StopCoroutine(killWeapon());
    }
}
