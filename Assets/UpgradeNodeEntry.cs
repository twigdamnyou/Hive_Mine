using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// Should be attached to the actual ui node for an upgrade
/// will dictate all the ui related things for actual upgrades
/// </summary>
public class UpgradeNodeEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    //TODO: Does the user have enough resources
    //TODO: how does it now what 'item' it associated with
    //TODO: what are the upgrade costs

    public Upgrade upgrade;
    public bool selected;


    #region UI Callbacks
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
    #endregion
}
