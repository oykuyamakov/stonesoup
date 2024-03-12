using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class FogOfWar : Tile
{
    private List<Vector2Int> _cellBoundaryWallIndices = new();
    private List<GameObject> _cellBoundaryWalls = new();
    private List<Vector2Int> _indicesToCheck = new();
    private List<GameObject> _wallsToCheck = new();
    private Vector2Int _startPoint;
    private Room _myRoom;
    private Vector2Int _gridPos;
    
    private const int FOG_NUMBER = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        var localPos = transform.localPosition;
        _gridPos = new Vector2Int((int)toGridCoord(localPos).x, (int)toGridCoord(localPos).y);
        
        _myRoom = transform.parent.GetComponent<Room>();
        int[,] indexGrid = loadIndexGrid();

        for (int x = 0; x < LevelGenerator.ROOM_WIDTH; x++)
        {
            for (int y = 0; y < LevelGenerator.ROOM_HEIGHT; y++)
            {
                if (indexGrid[x, y] == 1)
                {
                    _indicesToCheck.Add(new Vector2Int(x,y));
                }
            }
        }

        /*
        Vector2 position = transform.localPosition;
        //_startPoint = new Vector2Int((int)toGridCoord(globalX, globalY).x, (int)toGridCoord(globalX, globalY).y);
        _startPoint = new Vector2Int((int)toGridCoord(position).x, (int)toGridCoord(position).y);
        Debug.Log(_startPoint);
        List<Vector2Int> openSet = new();
        List<Vector2Int> closedSet = new();

        openSet.Add(_startPoint);

        while (openSet.Count > 0)
        {
            Vector2Int currentPoint = openSet[0];
            openSet.Remove(currentPoint);
            //Debug.Log("currentPoint: " + currentPoint.x + ", " + currentPoint.y);
            
            if (indexGrid[currentPoint.x, currentPoint.y] == 1)
            {
                Debug.Log("added currentPoint: " + currentPoint.x + ", " + currentPoint.y + "to _cellBoundaryWallIndices");
                _cellBoundaryWallIndices.Add(currentPoint);
            }
            
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x != 0 && y != 0)
                        continue;

                    Vector2Int newPoint = new Vector2Int(currentPoint.x + x, currentPoint.y + y);
                    if (newPoint.x >= 0 && newPoint.x < LevelGenerator.ROOM_WIDTH &&
                        newPoint.y >= 0 && newPoint.y < LevelGenerator.ROOM_HEIGHT)
                    {
                        if (openSet.Contains(newPoint) == false && closedSet.Contains(newPoint) == false)
                        {
                            openSet.Add(newPoint);
                        }
                    }
                }
            }

            closedSet.Add(currentPoint);
        }
        */

        List<GameObject> objectsInRoom = new();
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            //Debug.Log($"Added {transform.parent.GetChild(i).name} to objectsInRoom");
            objectsInRoom.Add(transform.parent.GetChild(i).gameObject);
        }

        foreach (GameObject g in objectsInRoom)
        {
            var myPosition = g.transform.localPosition;
            Vector2Int myCoord = new Vector2Int((int)toGridCoord(myPosition).x, (int)toGridCoord(myPosition).y);

            //Debug.Log($"Checking {g.name} ({myCoord})");
            foreach (var v in _indicesToCheck)
            {
                //Debug.Log($"    against {v}");
                    
                if (v == myCoord)
                {
                    if (indexGrid[v.x, v.y] == 1)
                    {
                        //Debug.Log($"Added {g.name}");
                        _cellBoundaryWalls.Add(g.transform.gameObject);
                    }
                }
            }
        }
        
        /*Debug.Log("DONE WITH VOID START\nOBJECTS TO BE CHECKED ARE:");
        foreach( var x in _cellBoundaryWalls) {
            Debug.Log( x.ToString());
        }*/
    }

    private void Update()
    {
        foreach (var g in _cellBoundaryWalls)
        {
            //Debug.Log($"Checking {g.name}");
            if (g == null)
            {
                if (isActiveAndEnabled)
                {
                    //Debug.Log("Shutting fog off");
                    gameObject.SetActive(false);

                    maybeSpawnTile();
                }
            }
        }
    }

    void maybeSpawnTile()
    {
        float roll = Random.value;
        TileTags tileToCheck;

        if (roll < .4f)
        {
            if (roll < .15f)
            {
                tileToCheck = TileTags.CanBeHeld;
            }
            else
            {
                tileToCheck = TileTags.Enemy;
            }
            
            string[] contributorFolder = ContributorList.instance.activeContributorIDs;
            List<GameObject> possibleItem = new();

            for (int i = 0; i < contributorFolder.Length; i++)
            {
                Object[] prefabs = Resources.LoadAll(contributorFolder[i] + "/Prefabs", typeof(GameObject));
                foreach (GameObject gameObject in prefabs)
                {
                    if (gameObject != null)
                    {
                        Tile tileType = gameObject.GetComponent<Tile>();
                        if (tileType != null && tileType.hasTag(tileToCheck))
                        {
                            possibleItem.Add(gameObject);
                        }
                    }
                }

            }

            int choice = Random.Range(0, possibleItem.Count);
        
            spawnTile(possibleItem[choice], this.transform.parent, _gridPos.x, _gridPos.y);
        }
    }

    public int[,] loadIndexGrid()
    {
        string initialGridString = _myRoom.designedRoomFile.text;
        string[] rows = initialGridString.Trim().Split('\n');
        int width = rows[0].Trim().Split(',').Length;
        int height = rows.Length;
        if (height != LevelGenerator.ROOM_HEIGHT) {
            throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}", _myRoom.roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
        }
        if (width != LevelGenerator.ROOM_WIDTH) {
            throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}", _myRoom.roomAuthor, LevelGenerator.ROOM_WIDTH, width));
        }
        int[,] indexGrid = new int[width, height];
        for (int r = 0; r < height; r++) {
            string row = rows[height-r-1];
            string[] cols = row.Trim().Split(',');
            for (int c = 0; c < width; c++) {
                indexGrid[c, r] = int.Parse(cols[c]);
            }
        }

        return indexGrid;
    }
}
