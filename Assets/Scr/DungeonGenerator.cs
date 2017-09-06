using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HoardIt
{
    public enum EDungeonTile
    {
        Undefined = -1, Air, Floor, Wall
    }

    public struct DungeonData
    {
        int m_Width, m_Height;
        Rect[] m_Rooms;
        int m_Entrance, m_Exit;

        public int Width { get { return m_Width; } set { m_Width = value; } }
        public int Height { get { return m_Height; } set { m_Height = value; } }

        public Rect[] Rooms { get { return m_Rooms; } set { m_Rooms = value; } }

        public int Entrance { get { return m_Entrance; } set { m_Entrance = value; } }
        public int Exit { get { return m_Exit; } set { m_Exit = value; } }

        public override string ToString()
        {
            return m_Width.ToString() + " " + m_Height.ToString();
        }
    }

    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Dungeon Settings")]
        public int m_DungeonSize = 25;
        public int m_MinRoomSize = 2, m_MaxRoomSize = 6;
        public int m_MaxRoomCount = 4;

        [Header("Prefabs")]
        public GameObject Prefab_WallTile;
        public GameObject Prefab_FloorTile;

        [Header("Scene Referances")]
        public GameObject Player;

        [Header("Private Fields")]
        private DungeonData m_DungeonData;

        private List<GameObject> m_SpawnedTiles;
        
	    // Use this for initialization
	    void Start ()
        {
            m_SpawnedTiles = new List<GameObject>();
            GenerateDungeon();
	    }

        private void GenerateDungeon()
        {
            GenerateDungeonData(m_DungeonSize, m_MinRoomSize, m_MaxRoomSize, m_MaxRoomCount, out m_DungeonData);
            InstantiateDungeon(m_DungeonData);
        }

        private void GenerateDungeonData(int size, int minRoomSize, int maxRoomSize, int roomCount, out DungeonData dungeon)
        {
            dungeon = new DungeonData
            {
                Width = size,
                Height = size
            };

            // generate room
            // check against other rooms so see if colliding
            // repeat till max reached or out of trys
            List<Rect> rooms = new List<Rect>();
            Rect tryRoom;
            int trys = 0;
            int maxTrys = 100;
            while (trys < maxTrys && rooms.Count < roomCount)
            {
                Vector2 roomSize = new Vector2(Random.Range(minRoomSize, maxRoomSize) + 2, Random.Range(minRoomSize, maxRoomSize) + 2);

                Vector2 roomPosition = new Vector2(
                    (int)Random.Range(1, size - 2 - roomSize.x),
                    (int)Random.Range(1, size - 2 - roomSize.y));

                tryRoom = new Rect(roomPosition, roomSize);

                bool failed = false;

                foreach (var room in rooms)
                {
                    if (tryRoom.Overlaps(room))
                    {
                        failed = true;
                        break;
                    }
                }
                if (failed)
                {
                    trys++;
                }
                else
                {
                    rooms.Add(tryRoom);
                    trys = 0;
                }
            }
            dungeon.Rooms = rooms.ToArray();
        }

        private void InstantiateDungeon(DungeonData dungeon)
        {
            EDungeonTile[,] tiles;

            ///TODO:
            ///Generate Pathways
            ///Generate Walls on outline of floor tiles

            ParseDungeonData(dungeon, out tiles);

            InstantiateTiles(tiles, dungeon.Width, dungeon.Height);

        }

        private void InstantiateTiles(EDungeonTile[,] tiles, int width, int height)
        {
            Vector2 spawnPos;
            Vector2 spawnOffset = new Vector2(-width / 2, -height / 2);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    spawnPos = new Vector2(x, y);
                    switch (tiles[x, y])
                    {
                        case EDungeonTile.Undefined:
                        case EDungeonTile.Air:
                            continue;
                        case EDungeonTile.Floor:
                            m_SpawnedTiles.Add(Instantiate(Prefab_FloorTile));
                            break;
                        case EDungeonTile.Wall:
                            m_SpawnedTiles.Add(Instantiate(Prefab_WallTile));
                            break;
                        default:
                            break;
                    }

                    var thisTile = m_SpawnedTiles[m_SpawnedTiles.Count - 1];

                    thisTile.transform.parent = transform;
                    thisTile.name = "Tile: " + x + ", " + y;
                    thisTile.transform.position = spawnPos + spawnOffset;

                }
            }
        }

        private static void ParseDungeonData(DungeonData dungeon, out EDungeonTile[,] tiles)
        {
            tiles = MakeEmptyTileMap(dungeon);
            
            foreach (var currentRoom in dungeon.Rooms)
            { 
                for (int y = (int)currentRoom.min.y; y < currentRoom.max.y; y++)
                {
                    for (int x = (int)currentRoom.min.x; x < currentRoom.max.x; x++)
                    {
                        // The tiles on the edge or a room are walls
                        if (x == currentRoom.xMin || y == currentRoom.yMin || x == currentRoom.xMax - 1 || y == currentRoom.yMax - 1)
                        {
                            tiles[x, y] = EDungeonTile.Wall;
                        }
                        else
                        {
                            //if (x < 0 || x >= dungeon.Width || y < 0 || y >= dungeon.Height)
                            //    Debug.LogError("Position " + new Vector2(x, y).ToString() + "Is out of bounds of the tile array. " + currentRoom.ToString());
                            tiles[x, y] = EDungeonTile.Floor;
                        }
                    }
                }
            }
        }

        internal static EDungeonTile[,] MakeEmptyTileMap(DungeonData dungeon)
        {
            EDungeonTile[,] tiles = new EDungeonTile[dungeon.Width, dungeon.Height];
            for (int y = 0; y < dungeon.Height; y++)
            {
                for (int x = 0; x < dungeon.Width; x++)
                {
                    tiles[x, y] = EDungeonTile.Air;
                }
            }

            return tiles;
        }

        // Update is called once per frame
        void Update ()
        {
		
	    }

    }
}


