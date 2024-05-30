using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static OreClass;
using static UnityEngine.EventSystems.EventTrigger;

public class ListAndDictionaryCheatSheet : MonoBehaviour
{

    public List<string> list = new List<string>();
    public List<string> list2 = new List<string>();

    public Dictionary<string, int> dictionary = new Dictionary<string, int>();

    public string ForEachExamples()
    {
        #region Loop Through Dictionary Examples

        //loop through the dictionary and perform actions to each entry
        foreach (var entry in dictionary)
        {
            PerformExample(entry.Key, entry.Value);
        }

        #endregion

        #region Return Keys or values from a dictionary

        //if the dictionary contains the correct key, return its value
        if (dictionary.ContainsKey("Example") == true)
        {
            int blah = dictionary["Example"];
            //return blah;
        }

        if (dictionary.ContainsValue(1) == true)
        {
            
        }

        #endregion

        #region Add or Remove from dictionaries


        #endregion

        return null;

    }

    public string ForLoopExamples()
    {
        //returns an item in the list based on if it matches the given item
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "Example")
            {
                return list[i];
            }
        }

        
        return null;

    }

    public void PerformExample(string exmple1, int exmple2)
    {

    }

}
