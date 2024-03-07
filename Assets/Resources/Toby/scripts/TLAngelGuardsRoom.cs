using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLAngelGuardsRoom : Room
{
    // Start is called before the first frame update
    public GameObject cryingAngelPrefab;
    public float borderWallProbability = 0.7f;
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        // It's very likely you'll want to do different generation methods depending on which required exits you receive
        // Here's an example of randomly choosing between two generation methods.
        if (Random.value <= 0.5f) {
            roomGenerationVersionOne(ourGenerator, requiredExits);
        }		
        else {
            roomGenerationVersionTwo(ourGenerator, requiredExits);
        }

    }
    
    protected void roomGenerationVersionOne(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        generateWalls(ourGenerator, requiredExits);

        // Spawn enemies on the diagonal from top-left to bottom-right
        for (int i = 0; i < 6; i++) {
            int spawnX = Random.Range(1, LevelGenerator.ROOM_WIDTH - 1);
            int spawnY = Random.Range(1, LevelGenerator.ROOM_HEIGHT - 1);
            Tile.spawnTile(cryingAngelPrefab, transform, spawnX, spawnY);
        }
    }

    protected void roomGenerationVersionTwo(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        // In this version of room generation, I generate walls and then other stuff.
        generateWalls(ourGenerator, requiredExits);

        // Spawn enemies on the diagonal from top-right to bottom-left
        for (int i = 0; i < 6; i++) {
            int spawnX = Random.Range(1, LevelGenerator.ROOM_WIDTH - 1);
            int spawnY = Random.Range(1, LevelGenerator.ROOM_HEIGHT - 1);
            Tile.spawnTile(cryingAngelPrefab, transform, spawnX, LevelGenerator.ROOM_HEIGHT - 1 - spawnY);
        }
    }
    
    protected void generateWalls(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
    		// Basically we go over the border and determining where to spawn walls.
    		bool[,] wallMap = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
    			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
    				if (x == 0 || x == LevelGenerator.ROOM_WIDTH-1
    					|| y == 0 || y == LevelGenerator.ROOM_HEIGHT-1) {
    					
    					if (x == LevelGenerator.ROOM_WIDTH/2 
    						&& y == LevelGenerator.ROOM_HEIGHT-1
                            && requiredExits.upExitRequired) {
    						wallMap[x, y] = false;
    					}
    					else if (x == LevelGenerator.ROOM_WIDTH-1
    						     && y == LevelGenerator.ROOM_HEIGHT/2
                                 && requiredExits.rightExitRequired) {
    						wallMap[x, y] = false;
    					}
    					else if (x == LevelGenerator.ROOM_WIDTH/2
    						     && y == 0
                                 && requiredExits.downExitRequired) {
    						wallMap[x, y] = false;
    					}
    					else if (x == 0 
    						     && y == LevelGenerator.ROOM_HEIGHT/2 
                                 && requiredExits.leftExitRequired) {
    						wallMap[x, y] = false;
    					}
    					else {
    						wallMap[x, y] = Random.value <= borderWallProbability;
    					}
    					continue;
    				}
    				wallMap[x, y] = false;
    			}
    		}
    
    		// Now actually spawn all the walls.
    		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
    			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
    				if (wallMap[x, y]) {
    					Tile.spawnTile(ourGenerator.normalWallPrefab, transform, x, y);
    				}
    			}
    		}
    	}
    
    
    
    	// Simple utility function because I didn't bother looking up a more general Contains function for arrays.
    	// Whoops.
    	protected bool containsDir (Dir[] dirArray, Dir dirToCheck) {
    		foreach (Dir dir in dirArray) {
    			if (dirToCheck == dir) {
    				return true;
    			}
    		}
    		return false;
    	}
}
