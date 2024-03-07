using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLFightForRewardsRoom : apt283RandomDFSRoom {

    public GameObject rewardPrefab;
    public GameObject enemyPrefab;
    public GameObject buffitupPrefab;


    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        base.fillRoom(ourGenerator, requiredExits);
        foreach (SearchVertex vertex in _closed) {
            if (Random.value <= 0.3f)
            {
                Tile.spawnTile(buffitupPrefab, transform, (int)vertex.gridPos.x, (int)vertex.gridPos.y);
                break;
            }
        }
        foreach (SearchVertex vertex in _closed) {
            // Only look at vertices that were dead ends and weren't neighboring the exits.
            if (!vertex.isDeadEnd) {
                continue;
            }

            bool closeToExit = false;
            foreach (Vector2Int exitPoint in requiredExits.requiredExitLocations()) {
                int manDistanceToExit = (int)Mathf.Abs(exitPoint.x-vertex.gridPos.x)+(int)Mathf.Abs(exitPoint.y-vertex.gridPos.y);
                if (manDistanceToExit <= 1) {
                    closeToExit = true;
                    break;
                }
            }
            if (closeToExit) {
                continue;
            }
            Tile.spawnTile(rewardPrefab, transform, (int)vertex.gridPos.x, (int)vertex.gridPos.y);
            // Spawn the arrow traps depending on if we're open to the left or the right.
            
            
            if (vertex.parent.gridPos.x < vertex.gridPos.x) {
                Tile.spawnTile(enemyPrefab, transform, (int)vertex.gridPos.x-1, (int)vertex.gridPos.y);
            }
            else if (vertex.parent.gridPos.x > vertex.gridPos.x) {
                Tile.spawnTile(enemyPrefab, transform, (int)vertex.gridPos.x+1, (int)vertex.gridPos.y);
            }
            else if (vertex.parent.gridPos.y < vertex.gridPos.y) {
                Tile.spawnTile(enemyPrefab, transform, (int)vertex.gridPos.x, (int)vertex.gridPos.y-1);
            }
            else if (vertex.parent.gridPos.y > vertex.gridPos.y) {
                Tile.spawnTile(enemyPrefab, transform, (int)vertex.gridPos.x, (int)vertex.gridPos.y+1);
            }

        }
    }

}