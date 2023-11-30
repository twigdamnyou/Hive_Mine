using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Panel Map Data")]
public class PannelMapData : ScriptableObject
{
    public List<PanelMapEntry> panelPrefabs = new List<PanelMapEntry>();

    public BasePannel GetPanelPrefab(string panelID)
    {
        for (int i = 0; i < panelPrefabs.Count; i++)
        {
            //Debug.Log("checking : " + panelID + " : " + panelPrefabs[i].panelID);

            if (panelID == panelPrefabs[i].panelID)
            {
                return panelPrefabs[i].panelPrefab;
            }
        }
        return null;
    }



    [System.Serializable]
    public class PanelMapEntry
    {
        public string panelID;
        public BasePannel panelPrefab;

    }
}
