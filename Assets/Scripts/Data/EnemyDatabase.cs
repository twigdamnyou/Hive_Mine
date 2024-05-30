using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    public enum EnemyType
    {
        Space,
        Ground,
    }
    
    public List<EnemyData> enemyData = new List<EnemyData>();

    public List<EnemyData> GetEnemiesByType(EnemyType type)
    {
        List<EnemyData> validEnemies = new List<EnemyData>();

        for (int i = 0; i < enemyData.Count; i++)
        {
            if (enemyData[i].enemyType == type)
            {
                validEnemies.Add(enemyData[i]);
            }
        }

        return validEnemies;
    }

    public List<EnemyData> GetEnemiesByThreat(float threatLevel)
    {
        if (threatLevel < 1)
        {
            Debug.LogError("Tried to find a threat level less then 1");
            return null;
        }

        List<EnemyData> validEnemies = new List<EnemyData>();

        for (int i = 0; i < enemyData.Count; i++)
        {
            if (enemyData[i].threatLevel == threatLevel)
            {
                validEnemies.Add(enemyData[i]);
            }
        }

        if (validEnemies.Count == 0)
        {
            Debug.LogWarning("No Enemies at threat level: " + threatLevel + " found, findning closest match");
            GetEnemiesByThreat(threatLevel - 1);
        }
        return validEnemies;
    }

    public List<EnemyData> GetEnemies(EnemyType type, float threatLevel = 1f)
    {
        List<EnemyData> validEnemies = new List<EnemyData>();

        for (int i = 0; i < enemyData.Count; i++)
        {
            if (enemyData[i].enemyType == type && enemyData[i].threatLevel == threatLevel)
            {
                validEnemies.Add(enemyData[i]);
            }
        }

        return validEnemies;
    }

    [System.Serializable]
    public class EnemyData
    {
        public GameObject prefab;
        public float threatLevel;
        public EnemyType enemyType;

    }
}

