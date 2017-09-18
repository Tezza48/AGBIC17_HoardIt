using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using HoardIt.Core;
using HoardIt.Dungeon;
using HoardIt.Assets;

namespace HoardIt
{
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Dungeon Settings")]
        public int m_DungeonSize = 25;
        public int m_MinRoomSize = 2, m_MaxRoomSize = 6;
        public int m_MaxRoomCount = 4;
        public int m_PathNodeCount = 5;
        [Range(0, 10)]
        public int m_MinItemTarget = 3;
        [Range(5, 20)]
        public int m_MaxItemTarget = 7;

        [Header("Prefabs")]
        public GameObject Prefab_WallTile;
        public GameObject Prefab_FloorTile;
        public GameObject Prefab_DownStairs;

        public TextMesh Prefab_TextMesh;

        [Header("Scene Referances")]
        public GameObject Player;

        [Header("Private Fields")]
        private RawDungeonData m_RawDungeonData;
        private TileDungeonData m_DungeonData;

        private List<GameObject> m_SpawnedTiles;
        
	    // Use this for initialization
	    void Start ()
        {
            m_SpawnedTiles = new List<GameObject>();
            GenerateDungeon();
	    }

        private void GenerateDungeon()
        {
            GenerateDungeonData(m_DungeonSize, m_MinRoomSize, m_MaxRoomSize, m_MaxRoomCount, out m_RawDungeonData);
            InstantiateDungeon();
        }

        private void GenerateDungeonData(int size, int minRoomSize, int maxRoomSize, int roomCount, out RawDungeonData dungeon)
        {
            dungeon = new RawDungeonData { Width = size, Height = size };
            
            m_RawDungeonData.GenerateRoomsData(minRoomSize, maxRoomSize, roomCount);

            m_RawDungeonData.GenerateAndSortClusters(m_PathNodeCount, m_MaxRoomSize);
        }

        private void InstantiateDungeon()
        {
            PopulateDungeon(ref m_RawDungeonData);

            m_DungeonData.ParseDungeonData(m_RawDungeonData);

            InstantiateTiles(m_DungeonData.Tiles, m_RawDungeonData.Width, m_RawDungeonData.Height);

            InstantiatePopulation(m_RawDungeonData.GetPopulation());
        }

        private void InstantiatePopulation(DungeonPopulation population)
        {
            for (int i = 0; i < population.NumItems; i++)
            {
                var newObj = new GameObject();

                ItemEntity entity = newObj.AddComponent<ItemEntity>();

                entity.Init(population.Items[i]);

                // set it's position
                Rect roomPos = m_RawDungeonData.Rooms[population.TargetRooms[i].X][population.TargetRooms[i].Y];
                roomPos.position += m_RawDungeonData.GetWorldOffset();
                Vector2 position = new Vector2(Random.Range(roomPos.xMin + 0.5f, roomPos.xMax + 0.5f), Random.Range(roomPos.yMin + 0.5f, roomPos.yMax + 0.5f));
                entity.transform.position = position;
            }
        }

        private void GenerateAndSortClusters(Rect[] rooms, ref RawDungeonData dungeon)
        {
            dungeon.Path = new Vector2[m_PathNodeCount];

            // Create some path nodes
            for (int i = 0; i < m_PathNodeCount; i++)
            {
                dungeon.Path[i] = new Vector2(Random.Range(m_MaxRoomSize, dungeon.Width - m_MaxRoomSize), Random.Range(m_MaxRoomSize, dungeon.Height - m_MaxRoomSize));
            }
            
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
            dungeon.Entrance = new int[2] { 0, 0};
            int numRooms = dungeon.Rooms.Length;
            dungeon.Exit = new int[2] { numRooms - 1, dungeon.Rooms[numRooms - 1].Length - 1 };
        }

        private void PopulateDungeon(ref RawDungeonData dungeon)
        {
            Player.transform.position = dungeon.Rooms[dungeon.Entrance[0]][dungeon.Entrance[1]].center + dungeon.GetWorldOffset();
            GenerateLoot(ref dungeon);
        }

        private void GenerateLoot(ref RawDungeonData dungeon)
        {
            List<Item> items = new List<Item>();
            List<Point> targetRooms = new List<Point>();

            int numItemsToSpawn = Random.Range(m_MinItemTarget, m_MaxItemTarget + 1);
            int numTypes = ItemBase.singleton.baseTypes.Length;
            for (int i = 0; i < numItemsToSpawn; i++)
            {
                TypeSizeSprite link = ItemBase.singleton.baseTypes[
                    Random.Range(0, numTypes)];

                // name is by default the string version of the type name
                Item newItem = new Item(link.m_Type.ToString(), link.m_Type, link.m_Size, link.m_Sprite);
                
                // Pick the room for it to go in
                int cluster = Random.Range(0, dungeon.Rooms.Length);
                int room = Random.Range(0, dungeon.Rooms[cluster].Length);

                items.Add(newItem);
                targetRooms.Add(new Point(cluster, room));
            }

            dungeon.DungeonPopulation = new DungeonPopulation(items.ToArray(), targetRooms.ToArray());
        }

        // Instantiate all tiles in a TileDungeon
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
                        case EDungeonTile.DownStairs:
                            m_SpawnedTiles.Add(Instantiate(Prefab_DownStairs));
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

        // Update is called once per frame
        void Update ()
        {
		
	    }

    }
}


