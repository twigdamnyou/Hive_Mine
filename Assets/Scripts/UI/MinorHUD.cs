using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinorHUD : BasePannel
{
    [Header("Backpack Bar")]
    public BarScript backpackBar;

    public Miner miner;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        ResetBackpackBar();
        Close();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventManager.GameEvent.BackpackOreChanged, OnBackpackChanged);
    }

    private void OnDisable()
    {
        //EventManager.RemoveListener(EventManager.GameEvent.BackpackOreChanged, OnBackpackChanged);
        EventManager.RemoveMyListeners(this);
    }

    public void ResetBackpackBar()
    {
            backpackBar.SetBarCurrent(0f);
    }

    private void OnBackpackChanged(EventData eventData)
    {
        float newCurrentBackpack = eventData.GetFloat("CurrentTotal");

        backpackBar.SetBarCurrent(newCurrentBackpack / InventoryManager.instance.maxBackPackSize);

        if (InventoryManager.instance.currentBackPackAmmount == 0f)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

}
