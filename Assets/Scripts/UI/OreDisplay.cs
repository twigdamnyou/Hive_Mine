using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OreDisplay : MonoBehaviour
{
    public OreClass.OreType oreType;
    public TextMeshProUGUI ammountText;
    public Image oreIcon;

    public void Setup(OreClass.OreType oreType, Sprite icon, float initialAmmount)
    {
        this.oreType = oreType;
        this.oreIcon.sprite = icon;
        this.ammountText.text = initialAmmount.ToString();

        //Debug.Log("recieved " + initialAmmount);
    }

    public void UpdateOreValue(float newTotal)
    {
        ammountText.text = newTotal.ToString();

        //Debug.Log("updated " + newTotal);
    }

}
