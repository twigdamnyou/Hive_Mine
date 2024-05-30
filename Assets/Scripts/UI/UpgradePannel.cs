using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UpgradePannel : BasePannel
{
    [Header("Ore Text")]
    public TextMeshProUGUI ichorText;
    public TextMeshProUGUI compositeWaxText;
    public TextMeshProUGUI chitenWeaveText;
    public TextMeshProUGUI sclerimiteText;
    public TextMeshProUGUI neurotiteText;

    [Header("Scrolling")]
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public RectTransform entryTransform;
    public HorizontalLayoutGroup hlg;

    private bool isSnapped;
    public float snapForce;
    private float snapSpeed;

    public List<UpgradeEntry> upgradeEntries = new List<UpgradeEntry>();
    private int currrentIndex;


    [Header("Ore Display")]
    public OreDisplay oreDisplayTemplate;
    public Transform oreDisplayHolder;
    private List<OreDisplay> oreDisplayList = new List<OreDisplay>();

    private void Update()
    {
        int currentElement = Mathf.RoundToInt(0 - contentPanel.localPosition.x / (entryTransform.rect.width + hlg.spacing));
        if (currentElement < 2)
            currentElement = 2;

        if (currentElement > upgradeEntries.Count - 1)
            currentElement = upgradeEntries.Count - 1;

        PerformSnap(currentElement);
    }

    protected override void Awake()
    {
        base.Awake();
        oreDisplayTemplate.gameObject.SetActive(false);
        SetUpOreDisplay();


        contentPanel = scrollRect.content;
        RegisterPreexistingElements();
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

    #region OreTracker

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
    #endregion

    private void RegisterPreexistingElements()
    {
        upgradeEntries = GetComponentsInChildren<UpgradeEntry>().ToList();
        //Debug.Log("Number of registered upgrade entries: " + upgradeEntries.Count);
    }



    #region Upgrade Element UI

    public void OnPageRightClicked()
    {
        int currentElement = Mathf.RoundToInt(0 - contentPanel.localPosition.x / (entryTransform.rect.width + hlg.spacing));
        PerformSnap(currentElement + 1);

    }

    public void OnPageLeftClicked()
    {
        int currentElement = Mathf.RoundToInt(0 - contentPanel.localPosition.x / (entryTransform.rect.width + hlg.spacing));
        PerformSnap(currentElement - 1);
    }

    private void ScrollToItem(RectTransform target)
    {
        StartCoroutine(ScrollViewFocusFunctions.FocusOnItemCoroutine(scrollRect, target, 1f));
    }

    private void PerformSnap(int currentItem)
    {
        //isSnapped = false;
        scrollRect.velocity = Vector2.zero;
        snapSpeed += snapForce * Time.deltaTime;
        contentPanel.localPosition = new Vector3(
            Mathf.MoveTowards(contentPanel.localPosition.x, 0 - (currentItem * (entryTransform.rect.width + hlg.spacing)), snapSpeed),
            contentPanel.localPosition.y,
            contentPanel.localPosition.z);
        //if (contentPanel.localPosition.x == 0 - (currentItem * (entryTransform.rect.width + hlg.spacing)))
        //{
        //    isSnapped = true;
        //}

        //if (scrollRect.velocity.magnitude < 200 && !isSnapped)
        //{
        //    scrollRect.velocity = Vector2.zero;
        //    snapSpeed += snapForce * Time.deltaTime;
        //    contentPanel.localPosition = new Vector3(
        //        Mathf.MoveTowards(contentPanel.localPosition.x, 0 - (currentItem * (entryTransform.rect.width + hlg.spacing)), snapSpeed),
        //        contentPanel.localPosition.y,
        //        contentPanel.localPosition.z);
        //    if (contentPanel.localPosition.x == 0 - (currentItem * (entryTransform.rect.width + hlg.spacing)))
        //    {
        //        isSnapped = true;
        //    }
        //}
        //if (scrollRect.velocity.magnitude > 200)
        //{
        //    isSnapped = false;
        //    snapSpeed = 0;
        //}
    }

    #endregion
}


