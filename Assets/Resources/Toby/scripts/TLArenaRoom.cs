using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An example of a simple room generator.
// Walls are spawned randomly (unless an exit is required)
// Also spawns a random number of normal rocks and basic enemies.
public class TLArenaRoom : Room {

	// The room needs references to the things it can spawn.
	public GameObject enemyPrefab1;
	public GameObject enemyPrefab2;
	public GameObject enemyPrefab3;
	public GameObject enchantfruitPrefab;

	public int minNumEnemy1 = 1, maxNumEnemy1 = 4;
	public int minNumEnemy2 = 1, maxNumEnemy2 = 4;
	public int minNumEnemy3 = 1, maxNumEnemy3 = 4;

	// Chance a wall
	public float borderWallProbability = 0.7f;


    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
		// It's very likely you'll want to do different generation methods depending on which required exits you receive
		// Here's an example of randomly choosing between two generation methods.

			roomGenerationVersionTwo(ourGenerator, requiredExits);

	}

	protected void roomGenerationVersionOne(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
		// In this version of room generation, I only generate the walls.
		generateWalls(ourGenerator, requiredExits);
	}

	protected void roomGenerationVersionTwo(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
		// In this version of room generation, I generate walls and then other stuff.
		generateWalls(ourGenerator, requiredExits);
		// Inside the borders I make some rocks and enemies.
		int numEnemy1 = Random.Range(minNumEnemy1, maxNumEnemy1+1);
		int numEnemy2 = Random.Range(minNumEnemy2, maxNumEnemy2+1);
		int numEnemy3 = Random.Range(minNumEnemy3, maxNumEnemy3+1);

		// First, let's make an array keeping track of where we've spawned objects already.
		bool[,] occupiedPositions = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
				if (x == 0 || x == LevelGenerator.ROOM_WIDTH-1
					|| y == 0 || y == LevelGenerator.ROOM_HEIGHT-1) {
					// All border zones are occupied.
					occupiedPositions[x, y] = true;
				}
				else {
					occupiedPositions[x, y] = false;
				}
			}
		}

		// Now we spawn rocks and enemies in random locations
		List<Vector2> possibleSpawnPositions = new List<Vector2>(LevelGenerator.ROOM_WIDTH*LevelGenerator.ROOM_HEIGHT);
		for (int i = 0; i < numEnemy1; i++) {
			possibleSpawnPositions.Clear();
			for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
				for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
					if (occupiedPositions[x, y]) {
						continue;
					}
					possibleSpawnPositions.Add(new Vector2(x, y));
				}
			}
			if (possibleSpawnPositions.Count > 0) {
				Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
				Tile.spawnTile(enemyPrefab1, transform, (int)spawnPos.x, (int)spawnPos.y);
				occupiedPositions[(int)spawnPos.x, (int)spawnPos.y] = true;
			}
		}
		for (int i = 0; i < numEnemy2; i++) {
			possibleSpawnPositions.Clear();
			for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
				for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
					if (occupiedPositions[x, y]) {
						continue;
					}
					possibleSpawnPositions.Add(new Vector2(x, y));
				}
			}
			if (possibleSpawnPositions.Count > 0) {
				Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
				Tile.spawnTile(enemyPrefab2, transform, (int)spawnPos.x, (int)spawnPos.y);
				occupiedPositions[(int)spawnPos.x, (int)spawnPos.y] = true;
			}
		}
		for (int i = 0; i < numEnemy3; i++) {
			possibleSpawnPositions.Clear();
			for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
				for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
					if (occupiedPositions[x, y]) {
						continue;
					}
					possibleSpawnPositions.Add(new Vector2(x, y));
				}
			}
			if (possibleSpawnPositions.Count > 0) {
				Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
				Tile.spawnTile(enemyPrefab3, transform, (int)spawnPos.x, (int)spawnPos.y);
				occupiedPositions[(int)spawnPos.x, (int)spawnPos.y] = true;
			}
		}
		possibleSpawnPositions.Clear();
		for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++) {
			for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++) {
				if (occupiedPositions[x, y]) {
					continue;
				}
				possibleSpawnPositions.Add(new Vector2(x, y));
			}
		}
		if (possibleSpawnPositions.Count > 0) {
			Vector2 spawnPos = GlobalFuncs.randElem(possibleSpawnPositions);
			Tile.spawnTile(enchantfruitPrefab, transform, (int)spawnPos.x, (int)spawnPos.y);
			occupiedPositions[(int)spawnPos.x, (int)spawnPos.y] = true;
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
