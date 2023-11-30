using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<Wave> waves = new List<Wave>();

    public float nextWaveTimer = 180f;
    public List<Vector2> spaceSpawnPoints = new List<Vector2>();
    public List<Vector2> planetSpawnPoints = new List<Vector2>();
    //public List<Transform> tSpawnPoints = new List<Transform>();
    public int enemiesPerWave = 3;

    [Header("Spawn Cords Bounds")]
    public float lowerMin = -0.1f;
    public float upperMin = -0.1f;
    public float lowerMax = 1.1f;
    public float upperMax = 1.9f;
    public LayerMask spawnMask;

    [Header("Wave Timer")]
    public float waveTimerDuration = 300f;
    public Timer waveTimer;

    public EnemyDatabase enemyDatabase;
    public static SpawnManager instance;

    [Header("Testing")]
    public NPC testEnemy;
    public Transform spawnPoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        SetUpTimer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnWave();
        }

        UpdateTimer(waveTimer, true);
    }

    public void SpawnWave()
    {
        int groundEnemiesCount = Random.Range(enemiesPerWave / 4, enemiesPerWave / 2);
        int spaceEnemiesCount = enemiesPerWave - groundEnemiesCount;

        if (GameManager.currentPlanet != null)
        {
            Debug.Log("spawning both Space and Ground enemies");

            SpawnSpaceWave(spaceEnemiesCount);
            SpawnGroundWave(groundEnemiesCount);
        }
        else
        {
            Debug.Log("spawning only Space enemies");

            SpawnSpaceWave(enemiesPerWave);
        }
    }

    public void SpawnSpaceWave(int spawncount)
    {
        Debug.Log("Spawning " + enemiesPerWave + "Space enemies Per Wave.");

        StartCoroutine(SpawnEnemiesOnDelay(spawncount, EnemyDatabase.EnemyType.Space, 5f));
    }

    public void SpawnGroundWave(int spawncount)
    {
        if (GameManager.currentPlanet != null)
        {
            StartCoroutine(SpawnEnemiesOnDelay(spawncount, EnemyDatabase.EnemyType.Ground, 5f));
        }

        Debug.Log("Spawning " + enemiesPerWave + "Ground enemies Per Wave.");
    }

    private IEnumerator SpawnEnemiesOnDelay(List<Vector2> spawnPoints, EnemyDatabase.EnemyType enemyType)
    {
        WaitForSeconds waiter = new WaitForSeconds(0.2f);

        for (int i = 0; i < enemiesPerWave; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);

            Vector2 targetPoint = spawnPoints[randomIndex];

            InstantiateEnemy(targetPoint, enemyType, 1f);

            yield return waiter;
        }

        spawnPoints.Clear();
    }

    private IEnumerator SpawnEnemiesOnDelay(int spawncount, EnemyDatabase.EnemyType enemyType, float threatLevel)
    {
        WaitForSeconds waiter = new WaitForSeconds(0.2f);

        for (int i = 0; i < spawncount; i++)
        {
            switch (enemyType)
            {
                case EnemyDatabase.EnemyType.Space:
                    CreateSpaceEnemy(threatLevel);
                    break;
                case EnemyDatabase.EnemyType.Ground:
                    CreateGroundEnemy(threatLevel);
                    break;
            }

            yield return waiter;
        }
    }


    public void InstantiateEnemy(Vector2 spawnLocation, EnemyDatabase.EnemyType type, float threatLevel)
    {
        List<EnemyDatabase.EnemyData> validEnemiesByType = enemyDatabase.GetEnemiesByType(type);
        List<GameObject> validEnemiesByThreat = new List<GameObject>();

        for (int i = 0; i < validEnemiesByType.Count; i++)
        {
            if (validEnemiesByType[i].threatLevel <= threatLevel)
            {
                validEnemiesByThreat.Add(validEnemiesByType[i].prefab);
            }
        }

        int randomIndex = Random.Range(0, validEnemiesByThreat.Count);
        GameObject enemyPrefab = validEnemiesByThreat[randomIndex];

        GameObject newEnemy = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
    }

    public void CreateSpaceEnemy(float threatLevel)
    {
        List<Vector2> currentCordList = GetRandomPos();

        List<Vector2> validPoints = new List<Vector2>();

        for (int i = 0; i < currentCordList.Count; i++)
        {
            bool result = DetectBlockersforSpawns(currentCordList[i]);
            if (result == false)
            {
                validPoints.Add(currentCordList[i]);
            }
        }

        //Debug.Log(validPoints.Count + " Number of points in valid spawn list");
        //for (int i = 0; i < validPoints.Count; i++)
        //{
        //    Debug.Log(validPoints[i] + " is a Valid point");
        //}

        int randomIndex = Random.Range(0, validPoints.Count);

        Vector2 worldPosition = Camera.main.ViewportToWorldPoint(validPoints[randomIndex]);

        InstantiateEnemy(worldPosition, EnemyDatabase.EnemyType.Space, threatLevel);
    }

    public void CreateGroundEnemy(float threatLevel)
    {
        Vector2 spawnPoint = GetRandomPlanetSpawnPoint();

        InstantiateEnemy(spawnPoint, EnemyDatabase.EnemyType.Ground, threatLevel);
    }

    public List<Vector2> GetRandomPos()
    {
        //finding screen edge corners
        Vector2 bottomLeft = new Vector2(Random.Range(-0.1f, 0f), Random.Range(-0.1f, 0f));
        Vector2 topLeft = new Vector2(Random.Range(-0.1f, 0f), Random.Range(1f, 1.1f));
        Vector2 bottomRight = new Vector2(Random.Range(1f, 1.1f), Random.Range(-0.1f, 0f));
        Vector2 topRight = new Vector2(Random.Range(1f, 1.1f), Random.Range(1f, 1.1f));

        List<Vector2> results = new List<Vector2>();

        if (GameManager.currentPlanet != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 randomTop = GetRandomPosBetweenPoints(topLeft, topRight);

                results.Add(randomTop);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 randomLeft = GetRandomPosBetweenPoints(bottomLeft, topLeft);
                Vector2 randomRight = GetRandomPosBetweenPoints(bottomRight, topRight);
                Vector2 randomTop = GetRandomPosBetweenPoints(topLeft, topRight);
                Vector2 randomBottom = GetRandomPosBetweenPoints(bottomLeft, bottomRight);

                results.AddRange(new List<Vector2> { randomLeft, randomRight, randomTop, randomBottom });
            }
        }

        return results;
    }

    public void CreateSpawnPoints()
    {
        List<Vector2> currentCordList = GetRandomPos();

        List<Vector2> validPoints = new List<Vector2>();

        for (int i = 0; i < currentCordList.Count; i++)
        {
            bool result = DetectBlockersforSpawns(currentCordList[i]);
            if (result == false)
            {
                validPoints.Add(currentCordList[i]);
            }
        }

        for (int i = 0; i < enemiesPerWave; i++)
        {
            int randomInt = Random.Range(0, validPoints.Count);

            Vector3 spawnPoint = Camera.main.ViewportToWorldPoint(validPoints[randomInt]);
            spaceSpawnPoints.Add(spawnPoint);
        }
    }

    public Vector2 GetRandomPosBetweenPoints(Vector2 pointA, Vector2 pointB)
    {
        float randomNum = Random.Range(0f, 1f);

        Vector2 result = Vector2.Lerp(pointA, pointB, randomNum);

        return result;
    }

    public void CreateSpawnPointsOnPlanet()
    {
        List<Vector2> offScreenSpawnPoints = new List<Vector2>();
        List<Vector2> validSpawnPoints = new List<Vector2>();

        Vector2 screenCenter = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));

        foreach (Transform spawnPoint in GameManager.currentPlanet.enemySpawnPoints)
        {
            bool onScreen = TargetUtilities.IsTransformOnScreen(spawnPoint);

            if (onScreen == false)
            {
                offScreenSpawnPoints.Add(spawnPoint.position);
            }
        }

        Vector2 closest = TargetUtilities.FindNearestVector(offScreenSpawnPoints, screenCenter);
        offScreenSpawnPoints.Remove(closest);
        validSpawnPoints.Add(closest);

        Vector2 secondClosest = TargetUtilities.FindNearestVector(offScreenSpawnPoints, screenCenter);
        validSpawnPoints.Add(secondClosest);

        planetSpawnPoints.AddRange(validSpawnPoints);
    }

    public Vector2 GetRandomPlanetSpawnPoint()
    {
        List<Vector2> offScreenSpawnPoints = new List<Vector2>();
        List<Vector2> validSpawnPoints = new List<Vector2>();

        Vector2 screenCenter = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));

        foreach (Transform spawnPoint in GameManager.currentPlanet.enemySpawnPoints)
        {
            bool onScreen = TargetUtilities.IsTransformOnScreen(spawnPoint);

            if (onScreen == false)
            {
                offScreenSpawnPoints.Add(spawnPoint.position);
            }
        }

        Vector2 closest = TargetUtilities.FindNearestVector(offScreenSpawnPoints, screenCenter);
        offScreenSpawnPoints.Remove(closest);
        validSpawnPoints.Add(closest);

        Vector2 secondClosest = TargetUtilities.FindNearestVector(offScreenSpawnPoints, screenCenter);
        validSpawnPoints.Add(secondClosest);

        int randomIndex = validSpawnPoints.GetRandomIndex();

        return validSpawnPoints[randomIndex];
    }

    public bool DetectBlockersforSpawns(Vector2 testPoint)
    {
        //Debug.Log("Testing: " + testPoint);

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ViewportToWorldPoint(testPoint), Vector3.zero, Mathf.Infinity, spawnMask);

        if (hit.collider != null)
        {
            //Debug.Log("we hit " + hit.collider.gameObject.name);
            return true;
        }
        else
        {
            //Debug.Log("Hit nothing");
            return false;
        }
    }

    private void UpdateTimer(Timer timer, bool shouldUpdate)
    {
        if (timer != null && shouldUpdate == true)
        {
            timer.UpdateClock();
        }


    }

    public void SetUpTimer()
    {
        waveTimer = new Timer(waveTimerDuration, SpawnWave, true);
        //Debug.Log("currrent wave timer ratio at setup: " + waveTimer.RatioOfRemaining);
    }

}

[System.Serializable]
public class Wave
{
    public float waveDifficulty;
    public List<WaveEnemies> enemies = new List<WaveEnemies>();


    [System.Serializable]
    public class WaveEnemies
    {
        public Entity enemy;
        public int enemyCount = 1;
    }


}
