using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;

    public static bool ticDocked;
    public static bool minerDocked;

    public static Planet currentPlanet;
    public Planet testPlanet;

    public Vector2 regionSize = Vector2.one;

    public Player player;
    public Miner minerPrefab;

    public bool spawnPlanets = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start()
    {
        if (spawnPlanets == true)
            PlanetManager.instance.SpawnTest();
    }
            

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    PlanetManager.SpawnTest();
        //}
    }

    public static Vector2 GetTICPosition()
    {
        if (CheckPlayerStatus() == false)
        {
            Debug.LogWarning("Player null or inactive when getting position");
            return Vector2.zero;
        }
        return instance.player.transform.position;
    }

    public static Vector2 GetMinerPosition()
    {
        if (CheckPlayerStatus() == false)
        {
            Debug.LogWarning("Miner null or inactive when getting position");
            return Vector2.zero;
        }
        return instance.minerPrefab.transform.position;
    }

    public static bool CheckPlayerStatus()
    {
        if (instance.player == null)
        {
            return false;
        }
        bool active = instance.player.gameObject.activeInHierarchy;

        return active;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(regionSize / 2, regionSize);
    }

    private void StartGame()
    {
        //TODO: boot order of all the systems
    }

        //method for difficulty incrementor
        //will increment difficulty based on time ala risk of rain

        //method for timer

        //method to fire off spawner based on time and difficutly

        //super tech creep idea, enemies have a difficulty point value on them
        //difficulty is a point value that the spawn manger fills with enemies based on point value
        //means the game could theoretically be infinite

    }
