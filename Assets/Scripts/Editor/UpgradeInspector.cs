using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Upgrade))]
public class UpgradeInspector : Editor
{
    Upgrade upgrade;

    private void OnEnable()
    {
        upgrade = (Upgrade)target;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        upgrade.upgradeType = (UpgradeType)EditorGUILayout.EnumPopup("Upgrade Type", upgrade.upgradeType);
        upgrade.upgradeName = EditorGUILayout.TextField("Upgrade Name", upgrade.upgradeName);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Cost", EditorStyles.miniButtonLeft))
        {
            AddOreCost(upgrade);
        }

        if (upgrade.upgradeCostData.Count > 0)
        {
            if (GUILayout.Button("Remove Oldest Cost", EditorStyles.miniButtonRight))
            {
                RemoveOreCost(upgrade);
            }
        }
        GUILayout.EndHorizontal();

        DrawUpgradeCostData(upgrade);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Prereq", EditorStyles.miniButtonLeft))
        {
            AddPrereq(upgrade);
        }
        if (upgrade.prerequisiteUpgrades.Count > 0)
        {
            if (GUILayout.Button("Remove Oldest Prereq", EditorStyles.miniButtonRight))
            {
                RemovePrereq(upgrade);
            }
        }
        GUILayout.EndHorizontal();

        DrawPrereqData(upgrade);

        switch (upgrade.upgradeType)
        {
            case UpgradeType.WeaponDamage:
                upgrade.weaponDamageModifier = EditorGUILayout.FloatField("Weapon Damage", upgrade.weaponDamageModifier);
                break;
            case UpgradeType.WeaponCooldown:
                upgrade.weaponCooldownModifier = EditorGUILayout.FloatField("Weapon Cooldown", upgrade.weaponCooldownModifier);
                break;
            case UpgradeType.WeaponRamge:
                upgrade.weaponRangeModifier = EditorGUILayout.FloatField("Weapon Range", upgrade.weaponRangeModifier);
                break;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    #region DrawCost
    private void DrawUpgradeCostData(Upgrade upgrade)
    {
        foreach (Upgrade.UpgradeCostData entry in upgrade.upgradeCostData)
        {
            entry.oreType = (OreClass.OreType)EditorGUILayout.EnumPopup("Ore Type", entry.oreType);
            entry.cost = EditorGUILayout.FloatField("Ore Cost", entry.cost);
        }
    }

    private void AddOreCost(Upgrade upgrade)
    {
        upgrade.upgradeCostData.Add(new Upgrade.UpgradeCostData());
    }

    private void RemoveOreCost(Upgrade upgrade)
    {
        if (upgrade.upgradeCostData.Count > 0)
            upgrade.upgradeCostData.Remove(upgrade.upgradeCostData[0]);

    }
    #endregion

    #region Draw Prereq
    private void DrawPrereqData(Upgrade upgrade)
    {
        for (int i = 0; i < upgrade.prerequisiteUpgrades.Count; i++)
        {
            upgrade.prerequisiteUpgrades[i] = (Upgrade)EditorGUILayout.ObjectField("Prereq", upgrade.prerequisiteUpgrades[i], typeof(Upgrade),false);
        }
    }

    private void AddPrereq(Upgrade upgrade)
    {
        upgrade.prerequisiteUpgrades.Add(null);
    }

    private void RemovePrereq(Upgrade upgrade)
    {
        if (upgrade.prerequisiteUpgrades.Count > 0)
            upgrade.prerequisiteUpgrades.Remove(upgrade.prerequisiteUpgrades[0]);

    }
    #endregion
}
