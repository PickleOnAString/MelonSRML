using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonSRML.SR2;
using MelonSRML.SR2.Idents;
using MelonSRML.SR2.Pedia;

namespace MelonSRML.Patches
{
    [HarmonyPatch(typeof(PediaDirector), nameof(PediaDirector.Awake))]
    internal static class PediaDirectorAwakePatch
    {
        public static void Prefix(PediaDirector __instance)
        {
            foreach (var pediaEntry in PediaRegistry.addedPedias)
            {
                var identPediaEntry = pediaEntry.TryCast<IdentifiablePediaEntry>();
                if (identPediaEntry && !__instance.identDict.ContainsKey(identPediaEntry.identifiableType))
                    __instance.identDict.Add(identPediaEntry.identifiableType, pediaEntry);

                if (pediaEntry.IsUnlockedInitially)
                    __instance.initUnlocked = __instance.initUnlocked.AddItem(pediaEntry).ToArray();
            }

            Type[] moddedPedias = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(ModdedPedia))).ToArray();
            foreach (var moddedPedia in moddedPedias)
            {
                ModdedPedia moddedPediaIntance = (ModdedPedia)Activator.CreateInstance(moddedPedia);
                __instance.identDict.Add(moddedPediaIntance.PediaIdent, moddedPediaIntance.GetPedia());
            }
        }
    }
}