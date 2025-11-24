using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System;
using TMProOld;

[BepInPlugin("com.bellways.ventrica.silksong.alphabeticalitems", "Silksong Mod - Alphabetical Order Items", "1.0.0")]
public class SilksongAlphabeticalItems : BaseUnityPlugin
{
    private void Awake()
    {
        // apply all harmony patches
        Harmony.CreateAndPatchAll(typeof(SilksongAlphabeticalItems), null);
    }

    private void Update()
    {
        // detect F5 key press
        if (Input.GetKeyDown(KeyCode.F5))
        {
            // nothing happens, saving for debug
        }
    }

    [HarmonyPatch(typeof(UISelectionList), "Awake")]
    [HarmonyPostfix]
    private static void UISelectionList_Awake_Postfix(UISelectionList __instance)
    {
        try
        {
            // get private listItems field
            var listItemsField = AccessTools.Field(typeof(UISelectionList), "listItems");
            if (listItemsField == null)
                return;

            var listItems = (List<UISelectionListItem>)listItemsField.GetValue(__instance);
            if (listItems == null || listItems.Count == 0)
                return;

            // sort items alphabetically by display text or gameobject name
            listItems.Sort((a, b) =>
            {
                var textA = a.GetComponentInChildren<TMP_Text>();
                var textB = b.GetComponentInChildren<TMP_Text>();
                string displayA = textA != null ? textA.text : a.gameObject.name;
                string displayB = textB != null ? textB.text : b.gameObject.name;
                return string.Compare(displayA, displayB, StringComparison.Ordinal);
            });

            // update sibling index to match sorted order
            for (int i = 0; i < listItems.Count; i++)
            {
                listItems[i].transform.SetSiblingIndex(i);
            }
        }
        catch
        {
            // fail silently if an error occurs
        }
    }
}
