using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : BasePannel
{
    [Header("Health Bar")]
    public BarScript healthBar;

    [Header("Shield Bar")]
    public BarScript shieldBar;
    public BarScript domeShieldBar;

    [Header("Timer Bar")]
    public BarScript waveBar;

    public Player player;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetupBars()
    {
        SetInitialHealthStats();

    }

    private void OnEnable()
    {
        EventManager.AddListener(EventManager.GameEvent.StatChanged, OnHealthChanged);
        EventManager.AddListener(EventManager.GameEvent.StatChanged, OnShieldChanged);
        EventManager.AddListener(EventManager.GameEvent.EquipmentCreated, OnEquipmentCreated);
    }

    private void OnEquipmentCreated(EventData eventdata)
    {
        switch (eventdata.GetString("Name"))
        {
            case "Shield":
                SetInitialShieldStats();
                break;
            case "Dome Shield":
                //Debug.Log("Creating Domeshield UI");
                SetInitialDomeShieldStats();
                break;
        }
    }

    private void OnDisable()
    {
        //EventManager.RemoveListener(EventManager.GameEvent.StatChanged, OnHealthChanged);
        EventManager.RemoveMyListeners(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) && GameManager.minerDocked == true)
        {
            BasePannel upgradePannel = PannelManager.GetPanel("UpgradePannel");
            if (upgradePannel == null)
            {
                PannelManager.OpenPanel("UpgradePannel");
            }
            else
            {
                upgradePannel.Toggle();
            }
        }

        UpdateWaveBar();
    }

    #region Bars

    private void SetInitialHealthStats()
    {
        //currentHealthText.text = "" + player.currentHealth;
        //maxHealthText.text = "" + player.maxHealth;

        //healthBar.SetBarMax(player.maxHealth);
        //Debug.Log("setting up health");
        healthBar.SetBarCurrent(1f);
    }

    private void OnHealthChanged(EventData eventData)
    {
        Entity target = eventData.GetEntity("Target");
        if (target != player)
            return;

        string stat = eventData.GetString("StatType");
        if (stat != "Health")
            return;
        float valueChanged = eventData.GetFloat("ChangeValue");
        float newCurrentHealth = eventData.GetFloat("CurrentTotal") - valueChanged;

        //currentHealthText.text = "" + newCurrentHealth;

        healthBar.SetBarCurrent(newCurrentHealth / player.MyStats[Stat.Health]);
    }

    private void SetInitialShieldStats()
    {
        //Debug.Log("setting up shields");
        if (InventoryManager.instance.shieldUpgrade != null)
        {
            //Debug.Log("shield found continuing set up");
            shieldBar.gameObject.SetActive(true);
            shieldBar.SetBarCurrent(1f);
        }
        else
        {
            // Debug.Log("Shield not found");
        }
    }

    private void SetInitialDomeShieldStats()
    {
        if (InventoryManager.instance.domeShieldUpgade != null)
        {
            //Debug.Log("shield found continuing set up");
            domeShieldBar.SetBarCurrent(1f);
        }
    }

    public void ToggleDomeshieldUI(bool toggle)
    {
        domeShieldBar.gameObject.SetActive(toggle);
    }

    private void OnShieldChanged(EventData eventData)
    {
        Entity target = eventData.GetEntity("Target");
        if (target != player)
            return;
        float valueChanged = eventData.GetFloat("ChangeValue");
        float newCurrentShield = eventData.GetFloat("CurrentTotal");
        string stat = eventData.GetString("StatType");
        switch (stat)
        {
            case "Shield":
                shieldBar.SetBarCurrent(newCurrentShield / InventoryManager.instance.shieldUpgrade.EquipmentStats.GetStat(Stat.MaxShield));
                break;
            case "Dome Shield":
                domeShieldBar.SetBarCurrent(newCurrentShield / InventoryManager.instance.domeShieldUpgade.EquipmentStats.GetStat(Stat.MaxShield));
                break;
        }


        //currentShieldText.text = "" + newCurrentShield;

    }

    private void UpdateWaveBar()
    {

        waveBar.SetBarCurrent(SpawnManager.instance.waveTimer.RatioOfRemaining);
    }

    #endregion

}
