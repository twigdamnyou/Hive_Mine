using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyDatabase;

public class SpawnManager : MonoBehaviour
{
    public List<Wave> waves = new List<Wave>();
    public bool waveActivelySpawning = false;

    public List<Vector2> spaceSpawnPoints = new List<Vector2>();
    public List<Vector2> planetSpawnPoints = new List<Vector2>();

    [Header("Wave parameters")]
    public int currentWave = 1;
    public int maxActiveEnemies = 20;
    public float totalThreatPerWave = 20f;
    public float maxSingleEnemyThreat = 1f;
    private List<EnemyData> currentWaveValidEnemies = new List<EnemyData>();

    [Header("Spawn Cords Bounds")]
    public float lowerMin = -0.1f;
    public float upperMin = -0.1f;
    public float lowerMax = 1.1f;
    public float upperMax = 1.9f;
    public LayerMask spawnMask;

    [Header("Wave Timer")]
    public float waveTimerDuration = 300f;
    public Timer waveTimer;
    public bool updateTimer = true;

    public EnemyDatabase enemyDatabase;
    public static SpawnManager instance;

    [Header("Testing")]
    public NPC testEnemy;
    public Transform spawnPoint;


    //threat level allacation ammount
    //singal enemy threat level max
    //max number of enemies allowed active
    //list all enemies being spawned during wave
    //list should spread out threat level so i dont overwhelm player with high level enemies
    //need to know when combat is on and off and include edge cases

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

        if (Input.GetKeyDown(KeyCode.K))
        {
            EntityManager.KillAllEnemies();
        }

        if (updateTimer == true)
        {
            UpdateTimer(waveTimer, true);
        }
        else
        {
            UpdateTimer(waveTimer, false);
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventManager.GameEvent.WaveEnded, UpdateTimer);
    }

    public void SpawnWave()
    {
        PopulateCurrentWaveEnemies();

        //int groundEnemiesCount = Random.Range(maxActiveEnemies / 4, maxActiveEnemies / 2);
        //int spaceEnemiesCount = maxActiveEnemies - groundEnemiesCount;

        StartCoroutine(SpawnEnemiesOnDelay());

        //after spawning everything increase difficulty
        totalThreatPerWave *= 1.25f;
        if (maxSingleEnemyThreat == 5f)
            maxSingleEnemyThreat = 5f;
        else
            maxSingleEnemyThreat += 1f;
    }

    public void PopulateCurrentWaveEnemies()
    {
        //clear Lists
        currentWaveValidEnemies.Clear();

        //determine number of enemies per threat catagory
        float threatPartition = Mathf.Ceil(totalThreatPerWave / 3);
        float topThreat = maxSingleEnemyThreat;
        float midThreat = maxSingleEnemyThreat - 1f <= 0 ? 1f : maxSingleEnemyThreat - 1f;
        float lowThreat = maxSingleEnemyThreat - 2f <= 0 ? 1f : maxSingleEnemyThreat - 4f;

        int topPartitionCount = (int)Mathf.Ceil(threatPartition / topThreat);
        int midPartitionCount = (int)Mathf.Ceil(threatPartition / midThreat);
        int lowPartitionCount = (int)Mathf.Ceil(threatPartition / lowThreat);

        if (GameManager.currentPlanet != null)
        {
            //populate list with both space enemies and ground enemies
            List<EnemyData> allTopThreatEnemies = enemyDatabase.GetEnemiesByThreat(topThreat);
            List<EnemyData> allMidThreatEnemies = enemyDatabase.GetEnemiesByThreat(midThreat);
            List<EnemyData> allLoWThreatEnemies = enemyDatabase.GetEnemiesByThreat(lowThreat);

            currentWaveValidEnemies.AddRange(PickEnemies(topPartitionCount, allTopThreatEnemies));
            currentWaveValidEnemies.AddRange(PickEnemies(midPartitionCount, allMidThreatEnemies));
            currentWaveValidEnemies.AddRange(PickEnemies(lowPartitionCount, allLoWThreatEnemies));
        }
        else
        {
            //populate list with space enemies
            List<EnemyData> spaceTopThreatEnemies = enemyDatabase.GetEnemies(EnemyType.Space, topThreat);
            List<EnemyData> spaceMidThreatEnemies = enemyDatabase.GetEnemies(EnemyType.Space, midThreat);
            List<EnemyData> spaceLoWThreatEnemies = enemyDatabase.GetEnemies(EnemyType.Space, lowThreat);

            currentWaveValidEnemies.AddRange(PickEnemies(topPartitionCount, spaceTopThreatEnemies));
            currentWaveValidEnemies.AddRange(PickEnemies(midPartitionCount, spaceMidThreatEnemies));
            currentWaveValidEnemies.AddRange(PickEnemies(lowPartitionCount, spaceLoWThreatEnemies));
        }
    }

    private List<EnemyData> PickEnemies(int partitionCount, List<EnemyData> enemyList)
    {
        List<EnemyData> results = new List<EnemyData>();
        if (enemyList.Count == 0)
        {
            Debug.LogWarning("List of enemies to spawn this wave is empty");
            return results;
        }
            

        for (int i = 0; i < partitionCount; i++)
        {
            int randomIndex = Random.Range(0, enemyList.Count);
            EnemyData enemyData = enemyList[randomIndex];
            results.Add(enemyData);
        }
        return results;
    }

    private IEnumerator SpawnEnemiesOnDelay()
    {
        WaitForSeconds waiter = new WaitForSeconds(0.2f);
        WaitForEndOfFrame frameWaiter = new WaitForEndOfFrame();

        waveActivelySpawning = true;
        updateTimer = false;

        EventData eventData = new EventData();
        eventData.AddBool("WaveStatus", true);
        EventManager.SendEvent(EventManager.GameEvent.WaveStarted, eventData);

        for (int i = 0; i < currentWaveValidEnemies.Count; i++)
        {
            while (EntityManager.GetActiveEnemyCount() >= maxActiveEnemies)
            {
                yield return frameWaiter;
            }

            if (currentWaveValidEnemies[i].enemyType == EnemyType.Space)
                CreateSpaceEnemy(currentWaveValidEnemies[i]);
            else
                CreateGroundEnemy(currentWaveValidEnemies[i]);

            yield return waiter;
        }

        waveActivelySpawning = false;
    }

    public void CreateSpaceEnemy(EnemyData enemyData)
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

        int randomIndex = Random.Range(0, validPoints.Count);

        Vector2 worldPosition = Camera.main.ViewportToWorldPoint(validPoints[randomIndex]);

        Instantiate(enemyData.prefab, worldPosition, Quaternion.identity);
    }

    public void CreateGroundEnemy(EnemyData enemyData)
    {
        Vector2 spawnPoint = GetRandomPlanetSpawnPoint();

        Instantiate(enemyData.prefab, spawnPoint, Quaternion.identity);
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

        for (int i = 0; i < maxActiveEnemies; i++)
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

    public void UpdateTimer(EventData data)
    {
        waveTimer.UpdateClock();
        updateTimer = true;
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


    #region Events


    #endregion

    #region Depricated Code

    //public void SpawnSpaceWave(int spawncount)
    //{
    //    Debug.Log("Spawning " + maxActiveEnemies + "Space enemies Per Wave.");

    //    StartCoroutine(SpawnEnemiesOnDelay(spawncount, EnemyDatabase.EnemyType.Space, 5f));
    //}

    //public void SpawnGroundWave(int spawncount)
    //{
    //    if (GameManager.currentPlanet != null)
    //    {
    //        StartCoroutine(SpawnEnemiesOnDelay(spawncount, EnemyDatabase.EnemyType.Ground, 5f));
    //    }

    //    Debug.Log("Spawning " + maxActiveEnemies + "Ground enemies Per Wave.");
    //}

    ////private IEnumerator SpawnEnemiesOnDelay(List<Vector2> spawnPoints, EnemyDatabase.EnemyType enemyType)
    ////{
    ////    WaitForSeconds waiter = new WaitForSeconds(0.2f);

    ////    for (int i = 0; i < enemiesPerWave; i++)
    ////    {
    ////        int randomIndex = Random.Range(0, spawnPoints.Count);

    ////        Vector2 targetPoint = spawnPoints[randomIndex];

    ////        InstantiateEnemy(targetPoint, enemyType, 1f);

    ////        yield return waiter;
    ////    }

    ////    spawnPoints.Clear();
    ////}

    //private IEnumerator SpawnEnemiesOnDelay(int spawncount, EnemyDatabase.EnemyType enemyType, float threatLevel)
    //{
    //    WaitForSeconds waiter = new WaitForSeconds(0.2f);

    //    for (int i = 0; i < spawncount; i++)
    //    {
    //        switch (enemyType)
    //        {
    //            case EnemyDatabase.EnemyType.Space:
    //                CreateSpaceEnemy(threatLevel);
    //                break;
    //            case EnemyDatabase.EnemyType.Ground:
    //                CreateGroundEnemy(threatLevel);
    //                break;
    //        }

    //        yield return waiter;
    //    }
    //}


    //public void InstantiateEnemy(Vector2 spawnLocation, EnemyDatabase.EnemyType type, float threatLevel)
    //{
    //    List<EnemyDatabase.EnemyData> validEnemiesByType = enemyDatabase.GetEnemiesByType(type);
    //    List<GameObject> validEnemiesByThreat = new List<GameObject>();

    //    for (int i = 0; i < validEnemiesByType.Count; i++)
    //    {
    //        if (validEnemiesByType[i].threatLevel <= threatLevel)
    //        {
    //            validEnemiesByThreat.Add(validEnemiesByType[i].prefab);
    //        }
    //    }

    //    int randomIndex = Random.Range(0, validEnemiesByThreat.Count);
    //    GameObject enemyPrefab = validEnemiesByThreat[randomIndex];

    //    GameObject newEnemy = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
    //}

    //public void CreateSpaceEnemy(float threatLevel)
    //{
    //    List<Vector2> currentCordList = GetRandomPos();

    //    List<Vector2> validPoints = new List<Vector2>();

    //    for (int i = 0; i < currentCordList.Count; i++)
    //    {
    //        bool result = DetectBlockersforSpawns(currentCordList[i]);
    //        if (result == false)
    //        {
    //            validPoints.Add(currentCordList[i]);
    //        }
    //    }

    //    //Debug.Log(validPoints.Count + " Number of points in valid spawn list");
    //    //for (int i = 0; i < validPoints.Count; i++)
    //    //{
    //    //    Debug.Log(validPoints[i] + " is a Valid point");
    //    //}

    //    int randomIndex = Random.Range(0, validPoints.Count);

    //    Vector2 worldPosition = Camera.main.ViewportToWorldPoint(validPoints[randomIndex]);

    //    InstantiateEnemy(worldPosition, EnemyDatabase.EnemyType.Space, threatLevel);
    //}

    //public void CreateGroundEnemy(float threatLevel)
    //{
    //    Vector2 spawnPoint = GetRandomPlanetSpawnPoint();

    //    InstantiateEnemy(spawnPoint, EnemyDatabase.EnemyType.Ground, threatLevel);
    //}



    #endregion
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
