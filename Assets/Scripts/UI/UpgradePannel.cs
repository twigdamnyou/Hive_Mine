using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePannel : BasePannel
{

    public TextMeshProUGUI ichorText;
    public TextMeshProUGUI compositeWaxText;
    public TextMeshProUGUI chitenWeaveText;
    public TextMeshProUGUI sclerimiteText;
    public TextMeshProUGUI neurotiteText;

    [Header("Ore Display")]
    public OreDisplay oreDisplayTemplate;
    public Transform oreDisplayHolder;
    private List<OreDisplay> oreDisplayList = new List<OreDisplay>();

    protected override void Awake()
    {
        base.Awake();
        oreDisplayTemplate.gameObject.SetActive(false);
        SetUpOreDisplay();
    }

    public override void Open()
    {
        base.Open();

    }

    private void OnEnable()
    {
        EventManager.AddListener(EventManager.GameEvent.OreChanged, OnOreAmmountChanged);
    }

    private void OnDisable()
    {
        //EventManager.RemoveListener(EventManager.GameEvent.OreChanged, OnOreAmmountChanged);
        EventManager.RemoveMyListeners(this);
    }

    public void OnOreAmmountChanged(EventData eventData)
    {
        int oreType = eventData.GetInt("OreType");
        OreClass.OreType targetOre = (OreClass.OreType)oreType;

        float targetOreTotal = eventData.GetFloat("CurrentTotal");

        OreDisplay targetDisplay = GetOreDisplay(targetOre);
        targetDisplay.UpdateOreValue(targetOreTotal);
    }

    public void SetUpOreDisplay()
    {
        for (int i = 0; i < oreDisplayList.Count; i++)
        {
            Destroy(oreDisplayList[i].gameObject);
        }
        oreDisplayList.Clear();

        OreClass.OreType[] allOres = System.Enum.GetValues(typeof(OreClass.OreType)) as OreClass.OreType[];

        foreach (OreClass.OreType oreType in allOres)
        {
            OreDisplay newDisplay = Instantiate(oreDisplayTemplate, oreDisplayHolder);
            newDisplay.gameObject.SetActive(true);
            newDisplay.Setup(oreType, InventoryManager.instance.GetOreSprite(oreType), InventoryManager.instance.GetOreAmmount(oreType));
            oreDisplayList.Add(newDisplay);
        }
    }

    private OreDisplay GetOreDisplay(OreClass.OreType oreType)
    {
        foreach (OreDisplay display in oreDisplayList)
        {
            if (display.oreType == oreType)
            {
                return display;
            }
        }

        return null;
    }

    public void DisplayOreAmmount()
    {

    }

}
