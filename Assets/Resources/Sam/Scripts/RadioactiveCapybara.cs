using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioactiveCapybara : Tile
{
    private AudioClip _capybaraSounds;
    private float _clipTimer = 9;
    private int _clipCount = 0;
    private List<SpriteRenderer> tilesInRoom = new();

    public override void init()
    {
        base.init();
        _capybaraSounds = Resources.Load<AudioClip>("Sam/Audio/capybara sounds");
        
        // get a list of every tile in the room
        /*for (int i = 0; i < transform.parent.transform.childCount; i++)
        {
            tilesInRoom.Add(transform.parent.GetChild(i).GetComponent<SpriteRenderer>());
            Debug.Log(tilesInRoom[i]);
        }*/

    }
    
    // Update is called once per frame
    void Update()
    {
        _clipTimer += Time.deltaTime;

        if (_clipTimer >= 10)
        {
            Debug.Log("play it again sam");
            _clipTimer = 0;
            AudioSource playItAgainSam = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            playItAgainSam.clip = _capybaraSounds;
            playItAgainSam.Play();
            _clipCount++;

            if (tilesInRoom.Count == 0)
            {
                for (int i = 0; i < transform.parent.transform.childCount; i++)
                {
                    tilesInRoom.Add(transform.parent.GetChild(i).GetComponent<SpriteRenderer>());
                    //Debug.Log(tilesInRoom[i]);
                }
            }

        }

        if (_clipCount > 2)
        {
            // TODO: delete some of the AudioSources lol it's making the game run like ass
        }
        
        // GREEN
        foreach (SpriteRenderer sr in tilesInRoom)
        {
            if (sr != null)
            {
                Color c = sr.color;
                c.r -= .0003f;
                c.b -= .0003f;
                sr.color = c;
            
            
                if (c.r <= 0.3f)
                {
                    sr.gameObject.GetComponent<Tile>().takeDamage(this, 1, DamageType.Explosive);
                    sr.gameObject.GetComponent<Tile>().takeDamage(this, 1);
                }
            }
        }
    }
}
