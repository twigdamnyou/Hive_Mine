using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;



    //public void SetBarMax(float value)
    //{
    //    slider.maxValue = value;
    //    slider.value = value;
    //}

    public void SetBarMaxWithGradient(float value)
    {
        slider.maxValue = value;
        slider.value = value;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetBarCurrent(float vlaue)
    {
        slider.value = vlaue;
    }

    public void SetBarCurrentWithGradient(float vlaue)
    {
        slider.value = vlaue;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}
