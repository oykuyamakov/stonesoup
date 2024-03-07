using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class swapper : Tile
{
    GameObject speechBubble;
    public Sprite giveItem;
    public Sprite thanks;
    private bool itemGiven = false;

    // Start is called before the first frame update
    void Start()
    {
        speechBubble = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        speechBubble.transform.localPosition = new Vector2(0, 2 + 2 * Mathf.Sin(Time.deltaTime * .1f));



        // checks for a dropped item
        int numObjectsFound = _body.Cast(Vector2.zero, _maybeRaycastResults);
        for (int i = 0; i < numObjectsFound && i < _maybeRaycastResults.Length; i++)
        {
            RaycastHit2D result = _maybeRaycastResults[i];
            Tile tileHit = result.transform.GetComponent<Tile>();
            if (tileHit == null)
            {
                continue;
            }
            if (tileHit.hasTag(TileTags.CanBeHeld) && !itemGiven)
            {
                // apparently you're not supposed to use Tile.die() to delete things, so we're dealing damage instead
                // I'm resisting the temptation to Destroy(tileHit.gameObject) lol
                tileHit.takeDamage(this, 1);
                tileHit.takeDamage(this, 1, DamageType.Explosive);
                SpawnNewItem();
                itemGiven = true;
                if (tileWereHolding != null)
                {
                    break;
                }
            }
        }
    }

    void SpawnNewItem()
    {
        string[] contributorFolder = ContributorList.instance.activeContributorIDs;
        List<GameObject> possibleItem = new();

        for (int i = 0; i < contributorFolder.Length; i++)
        {
            Debug.Log(contributorFolder[i] + "/Prefabs");
            Object[] prefabs = Resources.LoadAll(contributorFolder[i] + "/Prefabs", typeof(GameObject));
            foreach (GameObject gameObject in prefabs)
            {
                if (gameObject != null)
                {
                    Tile tileType = gameObject.GetComponent<Tile>();
                    if (tileType != null && tileType.hasTag(TileTags.CanBeHeld))
                    {
                        possibleItem.Add(gameObject);
                    }
                }
            }

        }


        for (int i = 0;i < possibleItem.Count;i++)
        {
            Debug.Log(possibleItem[i] + " can be spawned");
        }

        int choice = Random.Range(0, possibleItem.Count);
        //Vector2 myGridCoord = Tile.toGridCoord(transform.position);
        //Instantiate(possibleItem[choice], new Vector3(transform.position.x, transform.position.y - 2, 0), Quaternion.identity);
        spawnTile(possibleItem[choice], this.transform.parent, 5, 5);

        speechBubble.GetComponent<SpriteRenderer>().sprite = thanks;
    }
}
