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

    [System.Serializable]
    public class EnemyData
    {
        public GameObject prefab;
        public float threatLevel;
        public EnemyType enemyType;

    }
}

