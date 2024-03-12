using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dummy : Tile
{
    protected TileText _tileText;
    private int _lastHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        _lastHealth = health;
        _tileText = GetComponent<TileText>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health < _lastHealth)
        {
            _lastHealth = health;

            float roll = Random.value;
            if (roll < .5f)
            {
                _tileText.DisplayText("Ouch!");
            }
            else
            {
                _tileText.DisplayText("Pain makes me feel gooooooood");
            }

        }
    }
}
