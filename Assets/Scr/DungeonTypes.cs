//#define Debug
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using HoardIt.Assets;
using HoardIt.Core;
using System;

namespace HoardIt.Dungeon
{
    public enum EDungeonTile
    {
        Undefined = -1, Air, Floor, Wall, DownStairs
    }

    public class DungeonPopulation
    {
        private Item[] m_Items;
        private Point[] m_TargetRooms;
        private int m_NumItems;

        public DungeonPopulation(Item[] items, Point[] targetRooms)
        {
            Items = items;
            TargetRooms = targetRooms;
            m_NumItems = m_Items.Length;
        }

        public Item[] Items { get { return m_Items; } set { m_Items = value; } }

        public Point[] TargetRooms { get { return m_TargetRooms; } set { m_TargetRooms = value; } }

        public int NumItems { get { return m_NumItems; } set { m_NumItems = value; } }
        // Traps (though maybe this should be within a Room struct, replacing Rect)

    }
    
    public class RawDungeonData
    {
        int m_Width, m_Height;
        int[] m_Entrance, m_Exit;
        Rect[][] m_Rooms;// array of sets of rooms close to each other/"clusters"
        Vector2[] m_Path;
        DungeonPopulation m_DungeonPopulation;

        public RawDungeonData()
        {

        }

        public int Width { get { return m_Width; } set { m_Width = value; } }
        public int Height { get { return m_Height; } set { m_Height = value; } }
        public int[] Entrance { get { return m_Entrance; } set { m_Entrance = value; } }
        public int[] Exit { get { return m_Exit; } set { m_Exit = value; } }

        public Rect[][] Rooms { get { return m_Rooms; } set { m_Rooms = value; } }
        public Vector2[] Path { get { return m_Path; } set { m_Path = value; } }

        public DungeonPopulation DungeonPopulation { get { return m_DungeonPopulation; } set { m_DungeonPopulation = value; } }

        public void GenerateRoomsData(int minRoomSize, int maxRoomSize, int roomCount)
        {
            List<Rect> rooms = new List<Rect>();
            Rect tryRoom;
            int trys = 0;
            int maxTrys = 100;
            while (trys < maxTrys && rooms.Count < roomCount)
            {
                Vector2 roomSize = new Vector2(
                    Random.Range(minRoomSize, maxRoomSize) + 2,
                    Random.Range(minRoomSize, maxRoomSize) + 2);
                tryRoom = new Rect(new Vector2(
                    (int)Random.Range(1, m_Width - 2 - roomSize.x),
                    (int)Random.Range(1, m_Height - 2 - roomSize.y)),
                    roomSize);

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
                    trys++;
                else
                {
                    rooms.Add(tryRoom);
                    trys = 0;
                }
            }
            //place all rooms into the first element of Rooms for using later
            m_Rooms = new Rect[1][];
            m_Rooms[0] = rooms.ToArray();
        }

        public DungeonPopulation GetPopulation()
        {
            return m_DungeonPopulation;
        }

        public void GenerateAndSortClusters(int pathLength, int maxRoomSize)
        {
            m_Path = new Vector2[pathLength];

            // Create some path nodes
            for (int i = 0; i < pathLength; i++)
            {
                m_Path[i] = new Vector2(
                    Random.Range(maxRoomSize, m_Width - maxRoomSize), 
                    Random.Range(maxRoomSize, m_Height - maxRoomSize));
            }

            List<Rect>[] clusters = new List<Rect>[m_Path.Length];// node, room indices
            for (int i = 0; i < pathLength; i++)
            {
                clusters[i] = new List<Rect>();
            }

            // for each room find it's closest node
            for (int i = 0; i < m_Rooms[0].Length; i++)
            {
                int closest = 0;
                float distClosest = Mathf.Infinity;
                for (int j = 0; j < m_Path.Length; j++)
                {
                    float dist = Vector2.Distance(m_Rooms[0][i].center, m_Path[j]);
                    if (dist < distClosest)
                    {
                        distClosest = dist;
                        closest = j;
                    }
                }
                clusters[closest].Add(m_Rooms[0][i]);
            }

            Rect[][] sortedRects = new Rect[pathLength][];
            for (int i = 0; i < pathLength; i++)
            {
                sortedRects[i] = clusters[i].ToArray();
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
                    m_Path[i] = midpoint / cluster.Length;

            }

            // link rooms in a node (sort rooms according to clusters)
            // Link clusters together (pace one cluster of rooms after the preveous
            m_Rooms = sortedRects;
            m_Entrance = new int[2] { 0, 0 };
            int numRooms = m_Rooms.Length;
            m_Exit = new int[2] { numRooms - 1, m_Rooms[numRooms - 1].Length - 1 };
        }

        public Vector2 GetWorldOffset()
        {
            return new Vector2(-m_Width / 2, -m_Height / 2);
        }

        public Vector2 GetExitPosition()
        {
            return m_Rooms[m_Exit[0]][m_Exit[1]].center;
        }

        public override string ToString()
        {
            return "Width: " + m_Width.ToString() + " Height: " + m_Height.ToString() + "Rooms: " + Rooms.Length;
        }
    }

    public struct TileDungeonData
    {
        private EDungeonTile[,] m_Tiles;

        public EDungeonTile[,] Tiles { get { return m_Tiles; } set { m_Tiles = value; } }

        // Fill out a TileDungeon using RawDungeon data
        public void ParseDungeonData(RawDungeonData rawDungeon)
        {

            m_Tiles = MakeEmptyTileMap(rawDungeon);

            // Tiles for Rooms
            for (int i = 0; i < rawDungeon.Rooms.Length; i++)//itterate clusters
            {
                for (int j = 0; j < rawDungeon.Rooms[i].Length; j++)//itterate rooms
                {
                    for (int y = (int)rawDungeon.Rooms[i][j].min.y; y < rawDungeon.Rooms[i][j].max.y; y++)
                    {
                        for (int x = (int)rawDungeon.Rooms[i][j].min.x; x < rawDungeon.Rooms[i][j].max.x; x++)
                        {
                            // The tiles on the edge or a room are walls
                            if (x == rawDungeon.Rooms[i][j].xMin ||
                                y == rawDungeon.Rooms[i][j].yMin ||
                                x == rawDungeon.Rooms[i][j].xMax - 1 ||
                                y == rawDungeon.Rooms[i][j].yMax - 1)
                            {
                                //tiles[x, y] = EDungeonTile.Wall;
                            }
                            else
                            {
                                m_Tiles[x, y] = EDungeonTile.Floor;
                            }
                        }
                    }
                    JoinPoints(rawDungeon.Rooms[i][j].center, rawDungeon.Path[i]);
                }
                if (i < rawDungeon.Rooms.Length - 1)
                {
                    JoinPoints(rawDungeon.Path[i + 1], rawDungeon.Path[i]);
                }
            }

            m_Tiles[(int)rawDungeon.GetExitPosition().x, (int)rawDungeon.GetExitPosition().y] = EDungeonTile.DownStairs;

#if Debug
            // Tiles for paths
            for (int i = 0; i < rawDungeon.Rooms.Length; i++)
            {
                var cluster = rawDungeon.Rooms[i];
                Color debugColor = Color.HSVToRGB((float)i / rawDungeon.Rooms.Length, 1, 1);
                for (int j = 1; j < cluster.Length; j++)
                    Debug.DrawLine(cluster[j - 1].center + rawDungeon.GetWorldOffset(),
                        cluster[j].center + rawDungeon.GetWorldOffset(),
                        debugColor, 100);

                if (i < rawDungeon.Rooms.Length - 1)
                    Debug.DrawLine(rawDungeon.Path[i] + rawDungeon.GetWorldOffset(),
                        rawDungeon.Path[i + 1] + rawDungeon.GetWorldOffset(),
                        Color.white, 100);
            }
#endif
        }

        private EDungeonTile[,] MakeEmptyTileMap(RawDungeonData dungeon)
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
        
        // Joint 2 points on a tile grid with floor Tiles
        private void JoinPoints(Vector2 room, Vector2 node)
        {
            int x, y;
            int start, finish;

            start = (int)Mathf.Min(room.y, node.y);
            finish = (int)Mathf.Max(room.y, node.y);
            x = (int)room.x;
            // do coridor to path node
            for (y = start; y <= finish; y++)
            {
                m_Tiles[x, y] = EDungeonTile.Floor;
            }

            start = (int)Mathf.Min(room.x, node.x);
            finish = (int)Mathf.Max(room.x, node.x);
            y = (int)node.y;
            for (x = start; x <= finish; x++)
            {
                m_Tiles[x, y] = EDungeonTile.Floor;
            }
        }

    }
}