using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using HoardIt.Core;

namespace HoardIt
{
    public enum EDungeonTile
    {
        Undefined = -1, Air, Floor, Wall
    }

    public struct DungeonData
    {
        int m_Width, m_Height;
        int m_Entrance, m_Exit;
        Rect[][] m_Rooms;
        Vector2[] m_Path;

        public int Width { get { return m_Width; } set { m_Width = value; } }
        public int Height { get { return m_Height; } set { m_Height = value; } }
        public int Entrance { get { return m_Entrance; } set { m_Entrance = value; } }
        public int Exit { get { return m_Exit; } set { m_Exit = value; } }

        public Rect[] RoomsUnclustered { get { return m_Rooms[0]; } set { m_Rooms[0] = value; } }
        public Rect[][] Rooms { get { return m_Rooms; } set { m_Rooms = value; } }
        public Vector2[] Path { get { return m_Path; } set { m_Path = value; } }

        public override string ToString()
        {
            return "Width: " + m_Width.ToString() + " Height: " + m_Height.ToString() + "Rooms: " + Rooms.Length;
        }
    }

    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Dungeon Settings")]
        public int m_DungeonSize = 25;
        public int m_MinRoomSize = 2, m_MaxRoomSize = 6;
        public int m_MaxRoomCount = 4;
        public int m_PathNodeCount = 5;

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
            Rect[] rooms = GenerateRoomsData(size, minRoomSize, maxRoomSize, roomCount);

            // Generate path
            GenerateAndSortClusters(rooms, ref dungeon);
        }

        private Rect[] GenerateRoomsData(int size, int minRoomSize, int maxRoomSize, int roomCount)
        {
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

            return rooms.ToArray();
        }

        private void GenerateAndSortClusters(Rect[] rooms, ref DungeonData dungeon)
        {
            dungeon.Path = new Vector2[m_PathNodeCount];

            // Create some path nodes
            for (int i = 0; i < m_PathNodeCount; i++)
            {
                dungeon.Path[i] = new Vector2(Random.Range(m_MaxRoomSize, dungeon.Width - m_MaxRoomSize), Random.Range(m_MaxRoomSize, dungeon.Height - m_MaxRoomSize));
            }

            // Spread them out away from each other: Skipping
            List<Rect>[] roomNodePairing = new List<Rect>[dungeon.Path.Length];// node, room indices
            for (int i = 0; i < m_PathNodeCount; i++)
            {
                roomNodePairing[i] = new List<Rect>();
            }

            // for each room find it's closest node
            for (int i = 0; i < rooms.Length; i++)
            {
                int closest = 0;
                float distClosest = Mathf.Infinity;
                for (int j = 0; j < dungeon.Path.Length; j++)
                {
                    float dist = Vector2.Distance(rooms[i].center, dungeon.Path[j]);
                    if (dist < distClosest)
                    {
                        distClosest = dist;
                        closest = j;
                    }
                }
                roomNodePairing[closest].Add(rooms[i]);
            }

            Rect[][] sortedRects = new Rect[m_PathNodeCount][];
            for (int i = 0; i < m_PathNodeCount; i++)
            {
                sortedRects[i] = roomNodePairing[i].ToArray();
            }

            // move nodes to centre point of the cluster
            // to make it look nicer
            for (int i = 0; i < sortedRects.Length; i++)
            {
                Vector2 midpoint = new Vector2();
                var cluster = sortedRects[i];
                for (int j = 0; j < cluster.Length; j++)
                {
                    midpoint += cluster[j].center;
                }
                if (midpoint != Vector2.zero)
                    dungeon.Path[i] = midpoint / cluster.Length;

            }

            // link rooms in a node (sort rooms according to clusters)
            // Link clusters together (pace one cluster of rooms after the preveous
            dungeon.Rooms = sortedRects;
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

        private void ParseDungeonData(DungeonData dungeon, out EDungeonTile[,] tiles)
        {
            tiles = MakeEmptyTileMap(dungeon);

            // Tiles for Rooms
            for (int i = 0; i < dungeon.Rooms.Length; i++)//itterate clusters
            {
                for(int j = 0; j < dungeon.Rooms[i].Length; j++)//itterate rooms
                {
                    var currentRoom = dungeon.Rooms[i][j];
                    for (int y = (int)currentRoom.min.y; y < currentRoom.max.y; y++)
                    {
                        for (int x = (int)currentRoom.min.x; x < currentRoom.max.x; x++)
                        {
                            // The tiles on the edge or a room are walls
                            if (x == currentRoom.xMin || y == currentRoom.yMin || x == currentRoom.xMax - 1 || y == currentRoom.yMax - 1)
                            {
                                //tiles[x, y] = EDungeonTile.Wall;
                            }
                            else
                            {
                                //if (x < 0 || x >= dungeon.Width || y < 0 || y >= dungeon.Height)
                                //    Debug.LogError("Position " + new Vector2(x, y).ToString() + "Is out of bounds of the tile array. " + currentRoom.ToString());
                                tiles[x, y] = EDungeonTile.Floor;
                            }
                        }
                    }
                    JoinRoomsToCluster(dungeon, i, currentRoom, ref tiles);
                }
                if (i < dungeon.Rooms.Length - 1)
                {
                    JoinClusters(dungeon.Path[i + 1], dungeon.Path[i], ref tiles);
                }
            }


            Vector2 offset = new Vector2(-dungeon.Width / 2, -dungeon.Height / 2);
            // instantiate point gameobjects for debugging
            for (int i = 0; i < dungeon.Path.Length; i++)
            {
                GameObject point = new GameObject("Path node: " + i);
                point.transform.position = dungeon.Path[i] + offset;
            }

            // Tiles for paths
            Color debugColor = Color.HSVToRGB(0.0f, 1.0f, 1.0f);
            for (int i = 0; i < dungeon.Rooms.Length; i++)
            {
                debugColor = Color.HSVToRGB((float)i / dungeon.Rooms.Length, 1, 1);
                var cluster = dungeon.Rooms[i];
                for (int j = 1; j < cluster.Length; j++)
                {
                    // draw line from room i to room i-1
                    Debug.DrawLine(cluster[j - 1].center + offset, cluster[j].center + offset, debugColor, 100);
                }
                if (i < dungeon.Rooms.Length - 1)
                    Debug.DrawLine(dungeon.Path[i] + offset, dungeon.Path[i + 1] + offset, Color.white, 100);
            }
        }

        private void JoinClusters(Vector2 current, Vector2 prev, ref EDungeonTile[,] tiles)
        {

            int x, y;
            int start, finish;

            start = (int)Mathf.Min(current.y, prev.y);
            finish = (int)Mathf.Max(current.y, prev.y);
            x = (int)(current.x < prev.x ? current.x : prev.x);
            // do coridor to path node
            for (y = start; y < finish; y++)
            {
                tiles[x, y] = EDungeonTile.Floor;
            }

            start = (int)Mathf.Min(current.x, prev.x);
            finish = (int)Mathf.Max(current.x, prev.x);
            y = (int)(current.y > prev.x ? current.y : prev.y);
            for (x = start; x < finish; x++)
            {
                tiles[x, y] = EDungeonTile.Floor;
            }
        }

        private void JoinRoomsToCluster(DungeonData dungeon, int i, Rect currentRoom, ref EDungeonTile[,] tiles)
        {
            int x, y;
            int start, finish;

            start = (int)Mathf.Min(currentRoom.center.y, dungeon.Path[i].y);
            finish = (int)Mathf.Max(currentRoom.center.y, dungeon.Path[i].y);
            x = (int)(currentRoom.center.x < dungeon.Path[i].x ? currentRoom.center.x : dungeon.Path[i].x);
            // do coridor to path node
            for (y = start; y < finish; y++)
            {
                //if (y == start)
                //{
                //    tiles[x + 1, y - 1] = tiles[x + 1, y - 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x + 1, y - 1];
                //    tiles[x, y - 1] = tiles[x, y - 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x, y - 1];
                //    tiles[x - 1, y - 1] = tiles[x - 1, y - 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x - 1, y - 1];
                //}
                //tiles[x + 1, y] = tiles[x + 1, y] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x + 1, y];
                tiles[x, y] = EDungeonTile.Floor;
                //tiles[x - 1, y] = tiles[x - 1, y] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x - 1, y];
                //if (y == finish)
                //{
                //    tiles[x + 1, y + 1] = tiles[x + 1, y + 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x + 1, y + 1];
                //    tiles[x, y + 1] = tiles[x, y + 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x, y + 1];
                //    tiles[x - 1, y + 1] = tiles[x - 1, y + 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x - 1, y + 1];
                //}
            }

            start = (int)Mathf.Min(currentRoom.center.x, dungeon.Path[i].x);
            finish = (int)Mathf.Max(currentRoom.center.x, dungeon.Path[i].x);
            y = (int)(currentRoom.center.y < dungeon.Path[i].y ? currentRoom.center.y : dungeon.Path[i].y);
            for (x = start; x < finish; x++)
            {
                //if (x == start)
                //{
                //    tiles[x - 1, y + 1] = tiles[x - 1, y + 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x - 1, y + 1];
                //    tiles[x - 1, y] = tiles[x - 1, y] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x - 1, y];
                //    tiles[x - 1, y - 1] = tiles[x - 1, y - 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x - 1, y - 1];
                //}
                //tiles[x, y + 1] = tiles[x, y + 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x, y + 1];
                tiles[x, y] = EDungeonTile.Floor;
                //tiles[x, y - 1] = tiles[x, y - 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x, y - 1];
                //if (x == finish)
                //{
                //    tiles[x + 1, y + 1] = tiles[x + 1, y + 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x + 1, y + 1];
                //    tiles[x + 1, y] = tiles[x + 1, y] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x + 1, y];
                //    tiles[x + 1, y - 1] = tiles[x + 1, y - 1] != EDungeonTile.Floor ? EDungeonTile.Wall : tiles[x + 1, y - 1];
                //}
            }
        }

        internal EDungeonTile[,] MakeEmptyTileMap(DungeonData dungeon)
        {
            EDungeonTile[,] tiles = new EDungeonTile[dungeon.Width, dungeon.Height];
            for (int y = 0; y < dungeon.Height; y++)
            {
                for (int x = 0; x < dungeon.Width; x++)
                {
                    tiles[x, y] = EDungeonTile.Wall;
                }
            }

            return tiles;
        }

        private int Compare(Vector2 x, Vector2 y)
        {
            return (int)Vector2.Distance(x, y);
        }

        // Update is called once per frame
        void Update ()
        {
		
	    }

    }
}


