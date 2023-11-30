//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class RectangularRoom
//{
//    //Room starting coordinates
//    public int x;
//    public int y;
//    //Room size
//    public int width;
//    public int height;

//    public RectangularRoom(int x, int y, int width, int height)
//    {
//        this.x = x;
//        this.y = y;
//        this.width = width;
//        this.height = height;
//    }

//    public Vector2Int Center() => new Vector2Int(x + width / 2, y + height / 2);

//    //return the area of this room as a Bounds
//    public Bounds GetBounds() => new Bounds(new Vector3(x, y, 0), new Vector3(width, height, 0));

//    //return the area of this room as a BoundsInt
//    public BoundsInt GetBoundsInt() => new BoundsInt(new Vector3Int(x, y, 0), new Vector3Int(width, height, 0));

//    //return true if this room overlaps with another room
//    public bool Overlap(List<RectangularRoom> otherRooms)
//    {
//        foreach (RectangularRoom otherRoom in otherRooms)
//        {
//            if (GetBounds().Intersects(otherRoom.GetBounds()))
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//}
