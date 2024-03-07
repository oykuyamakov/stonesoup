using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuniksGhost : Tile{
    protected float lifetime = 1f;
    protected float speed = 1f;
    protected Vector2 dir = Vector2.zero;
    protected float timer = 0;

    void Update(){
        timer+=Time.deltaTime;
        if(timer>=lifetime) die();

        moveViaVelocity(dir, 10, speed);
    }

    public void Initialize(float _lifetime, float _speed, Vector2 _dir){
        lifetime = _lifetime;
        speed = _speed;
        dir = _dir;
        addTag(TileTags.Player);
        addTag(TileTags.Friendly);
    }

}
