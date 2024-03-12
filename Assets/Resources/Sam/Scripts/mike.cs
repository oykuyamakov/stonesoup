using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mike : apt283BasicEnemy
{
    protected TileText _tileText;
    private SpriteRenderer _sr;
    private float _flipTimer = 0;
    public override void init()
    {
        base.init();
        _sr = GetComponent<SpriteRenderer>();
        _tileText = GetComponent<TileText>();
    }
    
    // Update is called once per frame
    void Update()
    {
        _flipTimer += Time.deltaTime;
        if (_flipTimer >= .5f)
        {
            takeStep();
            if (_sr.flipX == true)
            {
                _sr.flipX = false;
            }
            else
            {
                _sr.flipX = true;
            }
            _flipTimer = Random.value/4;
            
            _tileText.DisplayText("bitch", 1f);
        }
    }
}
