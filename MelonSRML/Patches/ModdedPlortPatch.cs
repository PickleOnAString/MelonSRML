using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI;
using MelonSRML.SR2.Idents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelonSRML.Patches
{
    [HarmonyPatch(typeof(MarketUI), "Start")]
    public static class PatchMarketUIStart
    {
        public static void Prefix(MarketUI __instance)
        {
            Debug.Log("Start Market Patching");
            foreach (ModdedIdent plort in ModdedIdent.moddedPlorts)
            {
                Debug.Log(plort.plortEntry);
               __instance.plorts = __instance.plorts.AddItem(plort.plortEntry).ToArray();
            }
        }
    }

    [HarmonyPatch(typeof(EconomyDirector), "InitModel")]
    public static class PatchEconomyDirectorInitModel
    {
        public static void Prefix(EconomyDirector __instance)
        {
            Debug.Log("Start Economy Patching");
            foreach (ModdedIdent plort in ModdedIdent.moddedPlorts)
            {
                Debug.Log(plort.valueMap);
                __instance.baseValueMap = __instance.baseValueMap.AddItem(plort.valueMap).ToArray();
            }
        }
    }
}
