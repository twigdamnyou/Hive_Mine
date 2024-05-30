using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mine Database")]
public class MineDatabase : ScriptableObject
{
    public enum MineSize
    {
        Small,
        Medium,
        Large,
        Huge
    }

    public enum FeatureType
    {
        None,
        Upgrade,
        Store
    }

    public List<MineData> mineData = new List<MineData>();

    public List<MineData> GetMinesBySize(MineSize type)
    {
        List<MineData> validMines = new List<MineData>();

        for (int i = 0; i < mineData.Count; i++)
        {
            if (mineData[i].mineSize == type)
            {
                validMines.Add(mineData[i]);
            }
        }

        return validMines;
    }

    public List<MineData> GetMinesBySecret(FeatureType featureType)
    {
        List<MineData> validMines = new List<MineData>();

        for (int i = 0; i < mineData.Count; i++)
        {
            if (mineData[i].featureType == featureType)
            {
                validMines.Add(mineData[i]);
            }
        }

        return validMines;
    }

    public List<MineData> GetMines(MineSize type, FeatureType featureType)
    {
        List<MineData> validEnemies = new List<MineData>();

        for (int i = 0; i < mineData.Count; i++)
        {
            if (mineData[i].mineSize == type && mineData[i].featureType == featureType)
            {
                validEnemies.Add(mineData[i]);
            }
        }

        return validEnemies;
    }

    [System.Serializable]
    public class MineData
    {
        public GameObject prefab;
        public MineSize mineSize;
        public FeatureType featureType;

    }
}
