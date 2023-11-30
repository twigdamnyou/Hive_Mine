//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//sealed class ProcGen : MonoBehaviour
//{

//    public void GenerateMine(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, List<RectangularRoom> rooms)
//    {
//        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
//        {
//            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
//            int roomHeight = Random.Range(roomMinSize, roomMaxSize);

//            int roomx = Random.Range(0, mapWidth - roomWidth - 1);
//            int roomy = Random.Range(0, mapHeight - roomHeight - 1);

//            RectangularRoom newRoom = new RectangularRoom(roomx, roomy, roomWidth, roomHeight);

//            //check if this room intersects with any other rooms
//            if (newRoom.Overlap(rooms))
//            {
//                continue;
//            }
//            //if there are no intersections then the room is valid

//            //dig out this room inner area and builds the walls
//            for (int x = roomx; x < roomx + roomWidth; x++)
//            {
//                for (int y = roomy; y < roomy + roomHeight; y++)
//                {
//                    if (x == roomx || x == roomx + roomWidth - 1 || y == roomy || y == roomy + roomHeight - 1)
//                    {
//                        if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
//                        {
//                            continue;
//                        }
//                    }
//                    else
//                    {
//                        if (MapManager.instance.ObsticleMap.GetTile(new Vector3Int(x, y, 0)))
//                        {
//                            MapManager.instance.ObsticleMap.SetTile(new Vector3Int(x, y, 0), null);
//                        }
//                        MapManager.instance.FloorMap.SetTile(new Vector3Int(x, y, 0), MapManager.instance.FloorTile);
//                    }
//                }
//            }

//            if (MapManager.instance.Rooms.Count == 0)
//            {
//                //the first room, where the player starts
//                MapManager.instance.CreatePlayer(newRoom.Center());
//            }
//            else
//            {
//                //dig out a tunnel between this room and the previous one
//                TunnelBetween(MapManager.instance.Rooms[MapManager.instance.Rooms.Count - 1], newRoom);
//            }

//            rooms.Add(newRoom);
//        }
//    }

//    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
//    {
//        Vector2Int oldRoomCenter = oldRoom.Center();
//        Vector2Int newRoomCenter = newRoom.Center();
//        Vector2Int tunnelCorner;

//        if (Random.value < 0.5)
//        {
//            //move horizontally, the vertically
//            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
//        }
//        else
//        {
//            //move vertically, the horizontally
//            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
//        }

//        //generate the coordinatess for this tunnel
//        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
//        BresenhamLine(oldRoomCenter, tunnelCorner, tunnelCoords);
//        BresenhamLine(tunnelCorner, newRoomCenter, tunnelCoords);

//        for (int i = 0; i < tunnelCoords.Count; i++)
//        {
//            if (MapManager.instance.ObsticleMap.HasTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0)))
//            {
//                MapManager.instance.ObsticleMap.SetTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0), null);
//            }

//            MapManager.instance.FloorMap.SetTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0), MapManager.instance.FloorTile);

//            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
//            {
//                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
//                {
//                    if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
//                    {
//                        continue;
//                    }
//                }
//            }
//        }
//    }

//    private bool SetWallTileIfEmpty(Vector3Int pos)
//    {
//        if (MapManager.instance.FloorMap.GetTile(new Vector3Int(pos.x, pos.y, 0)))
//        {
//            return true;
//        }
//        else
//        {
//            MapManager.instance.ObsticleMap.SetTile(new Vector3Int(pos.x, pos.y, 0), MapManager.instance.WallTile);
//        }
//    }

//    private void BresenhamLine(Vector2Int roomCenter, Vector2Int tunnelCorner, List<Vector2Int> tunnelCoords)
//    {
//        int x = roomCenter.x, y = roomCenter.y;
//        int dx = Mathf.Abs(tunnelCorner.x - roomCenter.x), dy = Mathf.Abs(tunnelCorner.y - roomCenter.y);
//        int sx = roomCenter.x < tunnelCorner.x ? 1 : -1, sy = roomCenter.y < tunnelCorner.y ? 1 : -1;
//        int err = dx - dy;
//        while (true)
//        {
//            tunnelCoords.Add(new Vector2Int(x, y));
//            if (x == tunnelCorner.x && y == tunnelCorner.y)
//            {
//                break;
//            }
//            int e2 = 2 * err;
//            if (e2 > -dy)
//            {
//                err -= dy;
//                x += sx;
//            }
//            if (e2 < dx)
//            {
//                err += dx;
//                y += sy;
//            }
//        }
//    }

//}
