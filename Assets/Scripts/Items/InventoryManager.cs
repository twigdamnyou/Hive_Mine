using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using AYellowpaper.SerializedCollections;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private ItemClass item;

    [Header("Ore Database")]
    public List<OreClass> oreData = new List<OreClass>();

    [Header("Ores")]
    public Dictionary<OreClass.OreType, int> oreDictionary = new Dictionary<OreClass.OreType, int>();

    [Header("Miners Backpack")]
    public int maxBackPackSize = 15;
    public int currentBackPackAmmount;
    public Dictionary<OreClass.OreType, int> minersOreDictionary = new Dictionary<OreClass.OreType, int>();
    public float emptyDelay = 0.5f;

    [Header("Upgrades")]
    public EarthquakeMachine earthquakeMachine;
    public Shield shieldUpgrade;
    public TICTractorBeam ticTractorBeam;
    public DomeShield domeShieldUpgade;

    [SerializedDictionary("Equipment Name" , "Stats")]
    public SerializedDictionary<string, StatDataGroup> equipmentStats = new SerializedDictionary<string, StatDataGroup>();

    public LayerMask earthquakeDigMask;

    [Header("Has Upgrade")]
    public bool hasShield;
    public bool hasEarthquake;
    public bool hasTICTractorBeam;
    public bool hasMinerRepulser;
    public bool hasDrones;
    public bool hasDomeShield;


    public static InventoryManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (domeShieldUpgade.IsActive == true)
                DeactivateDomeShield();
            else
                ActivateDomeShield();
        }
    }

    private void Start()
    {
        //TODO: loop through all meta equipment and create each one
        CreateTICTractorBeam();
        CreateShield();
        CreateDomeShield();
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventManager.GameEvent.TICDocked, OnTICDocked);
        EventManager.AddListener(EventManager.GameEvent.TICUndocked, OnTICUndocked);
        EventManager.AddListener(EventManager.GameEvent.WaveEnded, ResetShield);
    }

    private void OnDisable()
    {
        //EventManager.RemoveListener(EventManager.GameEvent.TICDocked, OnTICDocked);
        //EventManager.RemoveListener(EventManager.GameEvent.TICUndocked, OnTICUndocked);
        EventManager.RemoveMyListeners(this);
    }

    #region Events

    private void OnTICDocked(EventData data)
    {
        ActivateEarthquakeMachine();
        ActivateTICTractorBeam();
    }

    private void OnTICUndocked(EventData data)
    {
        DeactivateEarthquakeMachine();
        DeactivateTICTractorBeam();
    }

    public void SendCreateEquipmentEvent(string name)
    {
        EventData data = new EventData();
        data.AddString("Name", name);

        EventManager.SendEvent(EventManager.GameEvent.EquipmentCreated, data);
    }

    #endregion

    #region Ore

    public void AddOre(OreClass.OreType oreType, int quantity)
    {
        if (oreDictionary.ContainsKey(oreType) == true)
        {
            oreDictionary[oreType] += quantity;
            //Debug.Log(quantity + " added to " + oreType);
        }
        else
        {
            oreDictionary.Add(oreType, quantity);
            //Debug.Log(oreType + " created with: " + quantity);
        }

        SendOreChangeEvent(oreType, quantity);
    }

    public void SubtractOre(OreClass.OreType oreType, int quantity)
    {
        if (oreDictionary.ContainsKey(oreType) == true && oreDictionary[oreType] >= 1)
        {
            oreDictionary[oreType] -= quantity;
        }
        else
        {
            Debug.LogWarning(oreType + " is not in the inventory");
            return;
        }
    }

    public bool SpendOre(OreClass.OreType oreType, int quantity)
    {
        int oreAmmount = GetOreAmmount(oreType);
        if (oreAmmount >= quantity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetOreAmmount(OreClass.OreType oreType)
    {
        if (oreDictionary.ContainsKey(oreType) == true)
        {
            return oreDictionary[oreType];
        }

        return 0;
    }

    public void SendOreChangeEvent(OreClass.OreType oreType, float changeValue)
    {
        EventData eventData = new EventData();

        eventData.AddFloat("ChangeValue", changeValue);
        eventData.AddFloat("CurrentTotal", GetOreAmmount(oreType));
        eventData.AddInt("OreType", (int)oreType);

        EventManager.SendEvent(EventManager.GameEvent.OreChanged, eventData);
    }

    public Sprite GetOreSprite(OreClass.OreType oreType)
    {
        for (int i = 0; i < oreData.Count; i++)
        {
            if (oreData[i].oreType == oreType)
            {
                return oreData[i].itemIcon;
            }
        }

        return null;
    }

    public GameObject GetOrePrefab(OreClass.OreType oreType)
    {
        for (int i = 0; i < oreData.Count; i++)
        {
            if (oreData[i].oreType == oreType)
            {
                return oreData[i].prefab;
            }
        }

        return null;
    }

    #endregion

    #region Miners Backpack

    public void MinerAddOre(OreClass.OreType oreType, int quantity)
    {
        if (minersOreDictionary.ContainsKey(oreType) == true)
        {
            minersOreDictionary[oreType] += quantity;
        }
        else
        {
            minersOreDictionary.Add(oreType, quantity);
        }

        SendMinorOreChangeEvent(oreType, quantity);
    }

    public int MinerRemoveOre(OreClass.OreType oreType, int quantity)
    {
        int currentAmmount = GetMinersOreAmmount(oreType);
        int ammountRemoved = 0;

        if (minersOreDictionary.ContainsKey(oreType) == true && currentAmmount > 0)
        {
            if (quantity > currentAmmount)
            {
                int difference = Mathf.Abs(currentAmmount - quantity);
                minersOreDictionary[oreType] -= difference;

                ammountRemoved = difference;
            }
            else
            {
                minersOreDictionary[oreType] -= quantity;
                ammountRemoved = quantity;
            }

            currentBackPackAmmount -= ammountRemoved;
            SendMinorOreChangeEvent(oreType, quantity);
        }

        return ammountRemoved;
    }

    public int GetMinersOreAmmount(OreClass.OreType oreType)
    {
        if (minersOreDictionary.ContainsKey(oreType) == true)
        {
            //Debug.Log("Ammount: " + minersOreDictionary[oreType]);
            return minersOreDictionary[oreType];
        }

        return 0;
    }

    public void EmptyMinerBackpack()
    {
        //Debug.Log("clearing miner inventory");
        minersOreDictionary.Clear();
        currentBackPackAmmount = 0;
    }

    public void EmptyMinerBackpackSlowly()
    {
        Tuple<OreClass.OreType, int> contents = CheckPackpackContents();
        if (contents == null)
            return;

        MinerRemoveOre(contents.Item1, contents.Item2);
        Instantiate(GetOrePrefab(contents.Item1), GameManager.instance.minerPrefab.transform.position, Quaternion.identity);
    }

    private Tuple<OreClass.OreType, int> CheckPackpackContents()
    {
        var firstElement = minersOreDictionary.FirstOrDefault();
        var lastElement = minersOreDictionary.LastOrDefault();

        if (firstElement.Value <= 0)
        {
            if (lastElement.Value <= 0)
            {
                return null;
            }

            return new Tuple<OreClass.OreType, int>(lastElement.Key, 1);
        }

        return new Tuple<OreClass.OreType, int>(firstElement.Key, 1);
    }

    public IEnumerator EmptyMinerBackpackSlowly(int quantityToRemove)
    {
        foreach (var entry in minersOreDictionary)
        {
            int startingOreAmmount = GetMinersOreAmmount(entry.Key);

            while (GetOreAmmount(entry.Key) > 0)
            {
                int oreAmmountRemoved = MinerRemoveOre(entry.Key, quantityToRemove);
                yield return emptyDelay;

                for (int i = 0; i < oreAmmountRemoved; i++)
                {
                    Instantiate(GetOrePrefab(entry.Key), GameManager.instance.minerPrefab.transform.position, Quaternion.identity);
                }
            }
        }
    }

    public void SendMinorOreChangeEvent(OreClass.OreType oreType, float changeValue)
    {
        EventData eventData = new EventData();

        eventData.AddFloat("ChangeValue", changeValue);
        eventData.AddFloat("CurrentTotal", currentBackPackAmmount);
        eventData.AddInt("OreType", (int)oreType);


        EventManager.SendEvent(EventManager.GameEvent.BackpackOreChanged, eventData);
    }

    #endregion

    #region Activate Equipment



    #endregion

    #region EarthquakeMachine
    public void CreateEarthquakeMachine()
    {
        earthquakeMachine = new EarthquakeMachine("Earthquake Machine", "", GameManager.instance.player, 5f, 15f, earthquakeDigMask, equipmentStats["EarthquakeMachine"]);
        SendCreateEquipmentEvent("Earthquake Machine");
    }

    public void ActivateEarthquakeMachine()
    {
        if (earthquakeMachine != null)
        {
            earthquakeMachine.Activate();
        }
    }

    public void DeactivateEarthquakeMachine()
    {
        if (earthquakeMachine != null)
        {
            earthquakeMachine.Deactivate();
        }
    }

    #endregion

    #region ShieldUpgrade

    public void CreateShield()
    {
        shieldUpgrade = new Shield("Shield", "", GameManager.instance.player, equipmentStats["Shield"]);
        SendCreateEquipmentEvent("Shield");
        ActivateShield();
    }

    public void ActivateShield()
    {
        if (shieldUpgrade != null)
        {
            shieldUpgrade.Activate();
        }
    }

    public void DeactivateShield()
    {
        if (shieldUpgrade != null)
        {
            shieldUpgrade.Deactivate();
        }
    }

    public void ResetShield(EventData data)
    {
        shieldUpgrade.ResetShieldToMax("Shield");
        Debug.Log("attempting to reset shields");
    }

    #endregion

    #region TICTractorBeam

    public void CreateTICTractorBeam()
    {
        ticTractorBeam = new TICTractorBeam("Tractor Beam", "", GameManager.instance.player, equipmentStats["TICTractorBeam"]);
        SendCreateEquipmentEvent("Tractor Beam");
    }

    public void ActivateTICTractorBeam()
    {
        if (ticTractorBeam != null)
        {
            ticTractorBeam.Activate();
        }
    }

    public void DeactivateTICTractorBeam()
    {
        if (ticTractorBeam != null)
        {
            ticTractorBeam.Deactivate();
        }
    }

    #endregion

    #region MinerRepulser




    #endregion

    #region Drones

    #endregion

    #region DomeShield

    public void CreateDomeShield()
    {
        domeShieldUpgade = new DomeShield("Dome Shield", "", GameManager.instance.player, equipmentStats["DomeShield"]);
        SendCreateEquipmentEvent("Dome Shield");
    }

    public void ActivateDomeShield()
    {
        if (domeShieldUpgade != null)
        {
            domeShieldUpgade.Activate();
        }
    }

    public void DeactivateDomeShield()
    {
        if (domeShieldUpgade != null)
        {
            domeShieldUpgade.Deactivate();
        }
    }


    #endregion
}
