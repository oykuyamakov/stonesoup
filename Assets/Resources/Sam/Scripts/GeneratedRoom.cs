using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedRoom : Room
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0 ; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                bool fill = false;
                if (x == 0 && !requiredExits.leftExitRequired)
                {
                    fill = true;
                }
                if (x == LevelGenerator.ROOM_WIDTH - 1 && !requiredExits.rightExitRequired)
                {
                    fill = true;
                }
                if (y == 0 && !requiredExits.downExitRequired)
                {
                    fill = true;
                }
                if (y == LevelGenerator.ROOM_HEIGHT - 1 && !requiredExits.upExitRequired)
                {
                    fill = true;
                }
                if (Random.Range(0, 100) < 50)
                {
                    fill = false;
                }
                if (fill)
                {
                    Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
                }
            }
        }

        //base.fillRoom(ourGenerator, requiredExits);
    }
}
