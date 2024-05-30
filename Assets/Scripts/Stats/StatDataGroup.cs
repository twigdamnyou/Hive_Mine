using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stats", fileName = "Stat Data Group")]
public class StatDataGroup : ScriptableObject 
{
    public List<StatData> data = new List<StatData>();

    [System.Serializable]
    public class StatData
    {
        public Stat statName;
        public float startingValue;
    }
}
