using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static bool IsSimilar(this float value, float value2, float acceptableDif)
    {
        return (Mathf.Abs(value - value2) <= acceptableDif);
    }
}
