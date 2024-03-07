using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mike : apt283BasicEnemy
{
    private SpriteRenderer _sr;
    private float _flipTimer = 0;
    public override void init()
    {
        base.init();
        _sr = GetComponent<SpriteRenderer>();
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
            _flipTimer = 0;
        }
    }
}
