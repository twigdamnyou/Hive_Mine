using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PannelManager : MonoBehaviour
{
    #region Variables

    public static PannelManager Instance;

    private List<BasePannel> panelInstances = new List<BasePannel>();

    public PannelMapData mapData;

    #endregion

    #region Built In Methods

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        RegisterPreexistingPanels();
    }

    #endregion

    #region Custom Methods

    private void RegisterPreexistingPanels()
    {
        panelInstances = GetComponentsInChildren<BasePannel>().ToList();
    }

    public static BasePannel GetPanel(string panelID)
    {
        for (int i = 0; i < Instance.panelInstances.Count; i++)
        {
            BasePannel currentPanel = Instance.panelInstances[i];

            if (panelID == currentPanel.panelID)
            {
                return currentPanel;
            }
        }
        return null;
    }

    private static BasePannel CreatePanel(string panelID)
    {
        BasePannel targetPanel = Instance.mapData.GetPanelPrefab(panelID);
        if (targetPanel == null)
        {
            Debug.LogError("could not find " + panelID);
            return null;
        }
        BasePannel activePanel = Instantiate(targetPanel, Instance.transform);
        Instance.panelInstances.Add(activePanel);
        return activePanel;
    }

    public static void OpenPanel(string panelID)
    {
        BasePannel targetPanel = GetPanel(panelID);
        if (targetPanel == null)
        {
            targetPanel = CreatePanel(panelID);
        }
        targetPanel.Open();
    }

    //public void HideLastPanel(string panelId)
    //{
    //    //make sure there is a panel open
    //    if (AnyPanelShowing())
    //    {
    //        //get the last panel showing
    //        var lastPanel = listInstances[listInstances.Count - 1];

    //        listInstances.Remove(lastPanel);

    //        //destroy that panel
    //        objectPool.PoolObject(lastPanel.panelInstance);
    //    }
    //}

    public void ClosePanel(string panelID)
    {
        BasePannel targetPanel = GetPanel(panelID);
        if (targetPanel == null)
        {
            targetPanel = CreatePanel(panelID);
        }
        targetPanel.Close();
    }

    //public bool AnyPanelShowing()
    //{
    //    return GetAmountPanelsInStack() > 0;
    //}

    //public int GetAmountPanelsInStack()
    //{
    //    return listInstances.Count;
    //}

    #endregion





}
