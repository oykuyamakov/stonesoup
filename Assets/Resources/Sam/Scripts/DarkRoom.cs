using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkRoom : Room
{
    public Sprite fogOfWarSprite;

    private const int FOG_NUMBER = 5;
    
    private bool [,] _fogSpawns = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    private Vector2Int _fogSpawn1 = Vector2Int.zero;
    private Vector2Int _fogSpawn2 = Vector2Int.zero;
    private Vector2Int _fogSpawn3 = Vector2Int.zero;

    [HideInInspector] public bool[,] darkCellBoundaries1 = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    [HideInInspector] public bool[,] darkCellBoundaries2 = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];
    [HideInInspector] public bool[,] darkCellBoundaries3 = new bool[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];

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
        // default room code for extracting data from text file
        string initialGridString = designedRoomFile.text;
        string[] rows = initialGridString.Trim().Split('\n');
        int width = rows[0].Trim().Split(',').Length;
        int height = rows.Length;
        if (height != LevelGenerator.ROOM_HEIGHT) {
            throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
        }
        if (width != LevelGenerator.ROOM_WIDTH) {
            throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_WIDTH, width));
        }
        int[,] indexGrid = new int[width, height];
        for (int r = 0; r < height; r++) {
            string row = rows[height-r-1];
            string[] cols = row.Trim().Split(',');
            for (int c = 0; c < width; c++) {
                indexGrid[c, r] = int.Parse(cols[c]);
            }
        }

        // default code, except if a tile is marked '5' we add it to the fog spawns array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileIndex = indexGrid[x, y];
                if (tileIndex == 0)
                    continue;
                if (tileIndex == FOG_NUMBER)
                {
                    _fogSpawns[x, y] = true;
                    continue;
                }      
                GameObject tileToSpawn;
                if (tileIndex < LevelGenerator.LOCAL_START_INDEX) {
                    tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex-1];
                }
                else {
                    tileToSpawn = localTilePrefabs[tileIndex-LevelGenerator.LOCAL_START_INDEX];
                }
                Tile.spawnTile(tileToSpawn, transform, x, y);
            }
        }

        // we're going to have three fog cells try to flood the room at once, so we pick the spawns for those cells
        while (_fogSpawn1 == Vector2Int.zero || _fogSpawn2 == Vector2Int.zero || _fogSpawn3 == Vector2Int.zero)
        {
            Vector2Int tryChooseFogSpawn = new Vector2Int(
                Random.Range(1, LevelGenerator.ROOM_WIDTH), 
                Random.Range(1, LevelGenerator.ROOM_HEIGHT));
            if (_fogSpawn1 == Vector2Int.zero && _fogSpawns[tryChooseFogSpawn.x, tryChooseFogSpawn.y] == true)
            {
                _fogSpawn1 = tryChooseFogSpawn;
            }

            if (_fogSpawn1 != Vector2Int.zero && _fogSpawns[tryChooseFogSpawn.x, tryChooseFogSpawn.y] == true)
            {
                _fogSpawn2 = tryChooseFogSpawn;
            }

            if (_fogSpawn1 != Vector2Int.zero && _fogSpawn2 != Vector2Int.zero &&
                _fogSpawns[tryChooseFogSpawn.x, tryChooseFogSpawn.y] == true)
            {
                _fogSpawn3 = tryChooseFogSpawn;
            }

        }

        // begin flooding from those points, adding the tiles they flood to their respective arrays
        List<Vector2Int> openSet1 = new();
        List<Vector2Int> openSet2 = new();
        List<Vector2Int> openSet3 = new();

        List<Vector2Int> closedSet1 = new();
        List<Vector2Int> closedSet2 = new();
        List<Vector2Int> closedSet3 = new();
        
        openSet1.Add(_fogSpawn1);
        openSet2.Add(_fogSpawn2);
        openSet3.Add(_fogSpawn3);

        // TODO: flood fill algo needs to check for the other floods
        while (openSet1.Count > 0 || openSet2.Count > 0 || openSet3.Count > 0)
        {
            if (openSet1.Count > 0)
            {
                Vector2Int currentPoint1 = openSet1[0];
                openSet1.Remove(currentPoint1);

                if (indexGrid[currentPoint1.x, currentPoint1.y] != FOG_NUMBER)
                    continue;
            
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 && y != 0)
                            continue;
                    
                        Vector2Int newPoint = new Vector2Int(currentPoint1.x + x, currentPoint1.y + y);
                        if (newPoint.x >= 0 && newPoint.x < LevelGenerator.ROOM_WIDTH &&
                            newPoint.y >= 0 && newPoint.y < LevelGenerator.ROOM_HEIGHT)
                        {
                            if (openSet1.Contains(newPoint) == false && closedSet1.Contains(newPoint) == false)
                            {
                                openSet1.Add(newPoint);
                            }
                        }
                    }
                }

                closedSet1.Add(currentPoint1);
            }
            
            if (openSet2.Count > 0)
            {
                Vector2Int currentPoint2 = openSet2[0];
                openSet1.Remove(currentPoint2);

                if (indexGrid[currentPoint2.x, currentPoint2.y] != FOG_NUMBER)
                    continue;
            
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 && y != 0)
                            continue;
                    
                        Vector2Int newPoint = new Vector2Int(currentPoint2.x + x, currentPoint2.y + y);
                        if (newPoint.x >= 0 && newPoint.x < LevelGenerator.ROOM_WIDTH &&
                            newPoint.y >= 0 && newPoint.y < LevelGenerator.ROOM_HEIGHT)
                        {
                            if (openSet2.Contains(newPoint) == false && closedSet2.Contains(newPoint) == false)
                            {
                                openSet2.Add(newPoint);
                            }
                        }
                    }
                }

                closedSet2.Add(currentPoint2);
            }
            
            if (openSet3.Count > 0)
            {
                Vector2Int currentPoint3 = openSet3[0];
                openSet3.Remove(currentPoint3);

                if (indexGrid[currentPoint3.x, currentPoint3.y] != FOG_NUMBER)
                    continue;
            
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 && y != 0)
                            continue;
                    
                        Vector2Int newPoint = new Vector2Int(currentPoint3.x + x, currentPoint3.y + y);
                        if (newPoint.x >= 0 && newPoint.x < LevelGenerator.ROOM_WIDTH &&
                            newPoint.y >= 0 && newPoint.y < LevelGenerator.ROOM_HEIGHT)
                        {
                            if (openSet3.Contains(newPoint) == false && closedSet3.Contains(newPoint) == false)
                            {
                                openSet3.Add(newPoint);
                            }
                        }
                    }
                }

                closedSet3.Add(currentPoint3);
            }
        }
    }
}
