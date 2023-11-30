//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class MapManager : MonoBehaviour
//{
//    public static MapManager instance;

//    [Header("Map Settings")]
//    [SerializeField] private int width = 80;
//    [SerializeField] private int height = 45;
//    [SerializeField] private int roomMaxSize = 10;
//    [SerializeField] private int roomMinSize = 6;
//    [SerializeField] private int maxRooms = 30;

//    [Header("Sprites / Colors")]
//    [SerializeField] private Color32 darkColor = new Color32(0,0, 0, 0);
//    [SerializeField] private Color32 lightColor = new Color32(255, 255, 255, 255);

//    [Header("Tiles")]
//    [SerializeField] private Tile floorTile;
//    [SerializeField] private Tile wallTile;

//    [Header("TileMaps")]
//    [SerializeField] private Tilemap floorMap;
//    [SerializeField] private Tilemap obstacleMap;

//    [Header("Features")]
//    [SerializeField] private List<RectangularRoom> rooms = new List<RectangularRoom>();

//    public Tile FloorTile { get => floorTile; }
//    public Tile WallTile { get => wallTile; }
//    public Tilemap FloorMap { get => floorMap; }
//    public Tilemap ObstacleMap { get => obstacleMap; }
//    public List<RectangularRoom> Rooms { get => rooms; }

//    private void Awake()
//    {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(gameObject);
//    }

//    private void Start()
//    {
//        ProcGen procGen = new ProcGen();
//        procGen.GenerateMine(width, height, roomMaxSize, roomMinSize, maxRooms, rooms);


//    }

//    //return true if x and y are intise of the counds of this map
//    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;


//}
